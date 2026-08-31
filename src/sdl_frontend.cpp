#include "wa2/sdl_frontend.hpp"

#include "wa2/error.hpp"

#include <SDL.h>
#include <SDL_image.h>
#include <SDL_mixer.h>
#include <SDL_ttf.h>

#include <algorithm>
#include <array>
#include <cmath>
#include <cstdio>
#include <iostream>
#include <map>
#include <memory>
#include <string>
#include <unordered_map>
#include <unordered_set>
#include <utility>
#include <vector>

namespace wa2 {
namespace {

constexpr int kWidth = 1280;
constexpr int kHeight = 720;
constexpr int kVoiceChannelBase = 0;
constexpr int kSeChannelBase = 8;

struct Texture {
    SDL_Texture* value = nullptr;
    int width = 0;
    int height = 0;
};

int mixer_volume(int value) {
    return std::clamp(value, 0, 255) * MIX_MAX_VOLUME / 255;
}

std::string strip_tags(const std::string& input) {
    std::string output;
    output.reserve(input.size());
    bool tag = false;
    for (std::size_t i = 0; i < input.size(); ++i) {
        const char ch = input[i];
        if (ch == '<') { tag = true; continue; }
        if (tag) {
            if (ch == '>') tag = false;
            continue;
        }
        if (ch == '\\' && i + 1 < input.size()) {
            const char command = input[i + 1];
            if (command == 'k' || command == 'K') { ++i; continue; }
            if (command == 'n' || command == 'N') { output.push_back('\n'); ++i; continue; }
        }
        output.push_back(ch);
    }
    return output;
}

std::string format_name(const char* format, int a, int b = 0, int c = 0) {
    std::array<char, 64> buffer{};
    std::snprintf(buffer.data(), buffer.size(), format, a, b, c);
    return buffer.data();
}

double chunk_seconds(const Mix_Chunk* chunk) {
    if (chunk == nullptr) return 0.0;
    int frequency = 0;
    std::uint16_t format = 0;
    int channels = 0;
    if (Mix_QuerySpec(&frequency, &format, &channels) == 0 || frequency <= 0 || channels <= 0) return 0.0;
    const int bits = SDL_AUDIO_BITSIZE(format);
    if (bits <= 0) return 0.0;
    return static_cast<double>(chunk->alen) / (frequency * channels * (bits / 8.0));
}

std::uint16_t little_u16(const std::vector<std::uint8_t>& bytes, std::size_t offset) {
    return static_cast<std::uint16_t>(bytes[offset]) |
           static_cast<std::uint16_t>(static_cast<std::uint16_t>(bytes[offset + 1]) << 8U);
}

SDL_Surface* load_tga_surface(const std::vector<std::uint8_t>& bytes, std::string* error) {
    constexpr std::size_t header_size = 18;
    if (bytes.size() < header_size) {
        if (error != nullptr) *error = "TGA header is truncated";
        return nullptr;
    }
    const std::uint8_t id_size = bytes[0];
    const std::uint8_t color_map_type = bytes[1];
    const std::uint8_t image_type = bytes[2];
    const std::uint16_t color_map_length = little_u16(bytes, 5);
    const std::uint8_t color_map_depth = bytes[7];
    const std::uint16_t width = little_u16(bytes, 12);
    const std::uint16_t height = little_u16(bytes, 14);
    const std::uint8_t depth = bytes[16];
    const std::uint8_t descriptor = bytes[17];
    const bool true_color = image_type == 2 || image_type == 10;
    const bool grayscale = image_type == 3 || image_type == 11;
    const bool rle = image_type == 10 || image_type == 11;
    if ((!true_color && !grayscale) || (true_color && depth != 24 && depth != 32) ||
        (grayscale && depth != 8) || width == 0 || height == 0) {
        if (error != nullptr) *error = "unsupported TGA type or pixel depth";
        return nullptr;
    }
    if (color_map_type > 1) {
        if (error != nullptr) *error = "invalid TGA color-map type";
        return nullptr;
    }

    const std::size_t color_map_bytes = color_map_type == 0 ? 0U :
        static_cast<std::size_t>(color_map_length) * ((color_map_depth + 7U) / 8U);
    const std::size_t data_offset = header_size + id_size + color_map_bytes;
    if (data_offset > bytes.size()) {
        if (error != nullptr) *error = "TGA image data offset is outside the file";
        return nullptr;
    }
    const std::size_t pixel_count = static_cast<std::size_t>(width) * height;
    if (pixel_count > 64U * 1024U * 1024U) {
        if (error != nullptr) *error = "TGA dimensions exceed safety limit";
        return nullptr;
    }

    SDL_Surface* surface = SDL_CreateRGBSurfaceWithFormat(0, width, height, 32, SDL_PIXELFORMAT_RGBA32);
    if (surface == nullptr) {
        if (error != nullptr) *error = SDL_GetError();
        return nullptr;
    }
    std::size_t cursor = data_offset;
    std::size_t output_index = 0;
    const std::size_t source_pixel_size = depth / 8U;
    const bool top_origin = (descriptor & 0x20U) != 0;
    const bool right_origin = (descriptor & 0x10U) != 0;

    auto read_pixel = [&](std::array<std::uint8_t, 4>* rgba) -> bool {
        if (cursor > bytes.size() || source_pixel_size > bytes.size() - cursor) return false;
        if (grayscale) {
            (*rgba)[0] = bytes[cursor];
            (*rgba)[1] = bytes[cursor];
            (*rgba)[2] = bytes[cursor];
            (*rgba)[3] = 255;
        } else {
            (*rgba)[0] = bytes[cursor + 2];
            (*rgba)[1] = bytes[cursor + 1];
            (*rgba)[2] = bytes[cursor];
            (*rgba)[3] = source_pixel_size == 4 ? bytes[cursor + 3] : 255;
        }
        cursor += source_pixel_size;
        return true;
    };
    auto write_pixel = [&](const std::array<std::uint8_t, 4>& rgba) {
        const std::size_t source_y = output_index / width;
        const std::size_t source_x = output_index % width;
        const std::size_t target_y = top_origin ? source_y : height - 1U - source_y;
        const std::size_t target_x = right_origin ? width - 1U - source_x : source_x;
        auto* target = static_cast<std::uint8_t*>(surface->pixels) + target_y * surface->pitch + target_x * 4U;
        target[0] = rgba[0];
        target[1] = rgba[1];
        target[2] = rgba[2];
        target[3] = rgba[3];
        ++output_index;
    };

    bool valid = true;
    while (output_index < pixel_count && valid) {
        std::size_t packet_count = 1;
        bool repeated = false;
        if (rle) {
            if (cursor >= bytes.size()) { valid = false; break; }
            const std::uint8_t packet = bytes[cursor++];
            packet_count = (packet & 0x7FU) + 1U;
            repeated = (packet & 0x80U) != 0;
            if (packet_count > pixel_count - output_index) { valid = false; break; }
        }
        std::array<std::uint8_t, 4> pixel{};
        if (repeated) {
            if (!read_pixel(&pixel)) { valid = false; break; }
            for (std::size_t i = 0; i < packet_count; ++i) write_pixel(pixel);
        } else {
            for (std::size_t i = 0; i < packet_count; ++i) {
                if (!read_pixel(&pixel)) { valid = false; break; }
                write_pixel(pixel);
            }
        }
    }
    if (!valid || output_index != pixel_count) {
        SDL_FreeSurface(surface);
        if (error != nullptr) *error = "TGA pixel data is truncated or malformed";
        return nullptr;
    }
    return surface;
}

} // namespace

struct SdlFrontend::Impl {
    SDL_Window* window = nullptr;
    SDL_Renderer* renderer = nullptr;
    TTF_Font* font = nullptr;
    TTF_Font* small_font = nullptr;
    Runtime* runtime = nullptr;
    std::unordered_map<std::string, Texture> textures;
    std::unordered_map<std::string, Texture> effect_textures;
    std::unordered_set<std::string> failed_textures;
    Mix_Music* music = nullptr;
    std::vector<std::uint8_t> music_bytes;
    std::array<Mix_Chunk*, 24> chunks{};
    std::array<double, 24> channel_ends{};
    std::array<std::vector<std::uint8_t>, 24> chunk_bytes;
    VideoPlayer video;
    bool video_was_playing = false;
    SDL_Texture* transition_target = nullptr;
    SDL_Texture* transition_mask = nullptr;
    SDL_BlendMode mask_blend_mode = SDL_BLENDMODE_INVALID;
    std::string loaded_mask_path;
    int loaded_mask_width = 0;
    int loaded_mask_height = 0;
    std::vector<std::uint8_t> loaded_mask_values;
    bool transition_warning_printed = false;
    bool transition_textures_initialized = false;
    bool mask_blend_supported = false;

    SDL_Surface* decoded_surface(const std::string& path, std::string* error) {
        try {
            const auto bytes = runtime->archives().read(path);
            if (bytes.empty()) {
                if (error != nullptr) *error = "not found in loaded archives";
                return nullptr;
            }
            SDL_RWops* rw = SDL_RWFromConstMem(bytes.data(), static_cast<int>(bytes.size()));
            SDL_Surface* surface = rw == nullptr ? nullptr : IMG_Load_RW(rw, 1);
            const std::string normalized = ArchiveIndex::normalized_name(path);
            if (surface == nullptr && normalized.size() >= 4 && normalized.substr(normalized.size() - 4) == ".tga") {
                surface = load_tga_surface(bytes, error);
            }
            if (surface == nullptr && error != nullptr && error->empty()) *error = IMG_GetError();
            return surface;
        } catch (const std::exception& ex) {
            if (error != nullptr) *error = ex.what();
            return nullptr;
        }
    }

    Texture* texture(const std::string& path) {
        if (path.empty() || runtime == nullptr) return nullptr;
        const auto found = textures.find(path);
        if (found != textures.end()) return &found->second;
        if (failed_textures.find(path) != failed_textures.end()) return nullptr;
        const auto fail = [this, &path](const std::string& reason) -> Texture* {
            failed_textures.insert(path);
            std::cerr << "wa2-cpp: texture '" << path << "': " << reason << '\n';
            return nullptr;
        };
        try {
            std::string decode_error;
            SDL_Surface* surface = decoded_surface(path, &decode_error);
            if (surface == nullptr) return fail(decode_error);
            Texture loaded;
            loaded.width = surface->w;
            loaded.height = surface->h;
            loaded.value = SDL_CreateTextureFromSurface(renderer, surface);
            SDL_FreeSurface(surface);
            if (loaded.value == nullptr) return fail(SDL_GetError());
            return &textures.emplace(path, loaded).first->second;
        } catch (const std::exception& ex) {
            return fail(ex.what());
        }
    }

    Texture* effect_texture(const std::string& path, int mode) {
        if (mode != 4 && mode != 6) return texture(path);
        const std::string key = path + "#mode" + std::to_string(mode);
        const auto found = effect_textures.find(key);
        if (found != effect_textures.end()) return &found->second;
        if (failed_textures.find(key) != failed_textures.end()) return nullptr;
        const auto fail = [this, &key, &path, mode](const std::string& reason) -> Texture* {
            failed_textures.insert(key);
            std::cerr << "wa2-cpp: texture effect " << mode << " for '" << path << "': " << reason << '\n';
            return nullptr;
        };
        std::string decode_error;
        SDL_Surface* source = decoded_surface(path, &decode_error);
        if (source == nullptr) return fail(decode_error);
        SDL_Surface* rgba = SDL_ConvertSurfaceFormat(source, SDL_PIXELFORMAT_RGBA32, 0);
        SDL_FreeSurface(source);
        if (rgba == nullptr) return fail(SDL_GetError());
        for (int y = 0; y < rgba->h; ++y) {
            auto* row = static_cast<std::uint8_t*>(rgba->pixels) + y * rgba->pitch;
            for (int x = 0; x < rgba->w; ++x) {
                auto* pixel = row + x * 4;
                const std::uint8_t red = pixel[0];
                if (mode == 4) {
                    pixel[3] = red;
                } else {
                    pixel[0] = 0;
                    pixel[1] = 0;
                    pixel[2] = 0;
                    pixel[3] = static_cast<std::uint8_t>(255 - red);
                }
            }
        }
        Texture loaded;
        loaded.width = rgba->w;
        loaded.height = rgba->h;
        loaded.value = SDL_CreateTextureFromSurface(renderer, rgba);
        SDL_FreeSurface(rgba);
        if (loaded.value == nullptr) return fail(SDL_GetError());
        return &effect_textures.emplace(key, loaded).first->second;
    }

    void draw_texture(const std::string& path, Vec2 position, Vec2 scale,
                      float alpha = 1.0F, int mode = 1) {
        Texture* image = effect_texture(path, mode);
        if (image == nullptr) return;
        SDL_SetTextureAlphaMod(image->value, static_cast<std::uint8_t>(std::clamp(alpha, 0.0F, 1.0F) * 255));
        SDL_SetTextureBlendMode(image->value, mode == 3 ? SDL_BLENDMODE_ADD : SDL_BLENDMODE_BLEND);
        SDL_Rect destination{
            static_cast<int>(std::lround(position.x)),
            static_cast<int>(std::lround(position.y)),
            static_cast<int>(std::lround(image->width * scale.x)),
            static_cast<int>(std::lround(image->height * scale.y)),
        };
        SDL_RenderCopy(renderer, image->value, nullptr, &destination);
    }

    float sprite_alpha_at(const SpriteState& sprite, double now_seconds) const {
        if (!sprite.alpha_transition_active || sprite.alpha_duration_seconds <= 0.0F) return sprite.alpha;
        const float progress = std::clamp(
            static_cast<float>((now_seconds - sprite.alpha_start_seconds) / sprite.alpha_duration_seconds),
            0.0F, 1.0F);
        return sprite.alpha_from + (sprite.alpha_to - sprite.alpha_from) * progress;
    }

    void draw_layers(const BackgroundState& background,
                     const std::vector<CharacterState>& characters,
                     const std::unordered_map<std::int32_t, SpriteState>& scene_sprites,
                     double now_seconds,
                     float alpha = 1.0F) {
        draw_texture(background.path, background.offset, background.scale, alpha);

        static const std::array<int, 11> positions{-288, 0, 288, -384, 384, -480, 480, -480, -160, 160, 480};
        for (const auto& character : characters) {
            const int offset = character.position >= 0 && character.position < static_cast<int>(positions.size())
                                   ? positions[static_cast<std::size_t>(character.position)] : 0;
            draw_texture(character.path, Vec2{static_cast<float>(-offset), 0.0F}, Vec2{1.0F, 1.0F}, alpha);
        }

        std::vector<const SpriteState*> ordered;
        ordered.reserve(scene_sprites.size());
        for (const auto& pair : scene_sprites) ordered.push_back(&pair.second);
        std::sort(ordered.begin(), ordered.end(), [](const SpriteState* a, const SpriteState* b) {
            return a->layer < b->layer;
        });
        for (const SpriteState* sprite : ordered) {
            draw_texture(sprite->path,
                         Vec2{sprite->position.x + sprite->offset.x, sprite->position.y + sprite->offset.y},
                         sprite->scale, sprite_alpha_at(*sprite, now_seconds) * alpha, sprite->mode);
        }
    }

    bool ensure_transition_textures() {
        if (transition_textures_initialized) return transition_target != nullptr && transition_mask != nullptr;
        transition_textures_initialized = true;
        if (SDL_RenderTargetSupported(renderer) == SDL_FALSE) return false;
        transition_target = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGBA8888,
                                              SDL_TEXTUREACCESS_TARGET, kWidth, kHeight);
        transition_mask = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGBA32,
                                            SDL_TEXTUREACCESS_STREAMING, kWidth, kHeight);
        if (transition_target == nullptr || transition_mask == nullptr) {
            if (transition_target != nullptr) SDL_DestroyTexture(transition_target);
            if (transition_mask != nullptr) SDL_DestroyTexture(transition_mask);
            transition_target = nullptr;
            transition_mask = nullptr;
            return false;
        }
        SDL_SetTextureBlendMode(transition_target, SDL_BLENDMODE_BLEND);
        mask_blend_mode = SDL_ComposeCustomBlendMode(
            SDL_BLENDFACTOR_ZERO, SDL_BLENDFACTOR_ONE, SDL_BLENDOPERATION_ADD,
            SDL_BLENDFACTOR_ZERO, SDL_BLENDFACTOR_SRC_ALPHA, SDL_BLENDOPERATION_ADD);
        mask_blend_supported = mask_blend_mode != SDL_BLENDMODE_INVALID &&
                               SDL_SetTextureBlendMode(transition_mask, mask_blend_mode) == 0;
        return true;
    }

    bool load_mask(const std::string& path) {
        if (path == loaded_mask_path) return !loaded_mask_values.empty();
        loaded_mask_path = path;
        loaded_mask_values.clear();
        loaded_mask_width = 0;
        loaded_mask_height = 0;
        std::string decode_error;
        SDL_Surface* source = decoded_surface(path, &decode_error);
        if (source == nullptr) {
            std::cerr << "wa2-cpp: transition mask '" << path << "': " << decode_error << '\n';
            return false;
        }
        SDL_Surface* rgba = SDL_ConvertSurfaceFormat(source, SDL_PIXELFORMAT_RGBA32, 0);
        SDL_FreeSurface(source);
        if (rgba == nullptr) {
            std::cerr << "wa2-cpp: transition mask '" << path << "': " << SDL_GetError() << '\n';
            return false;
        }
        loaded_mask_width = rgba->w;
        loaded_mask_height = rgba->h;
        loaded_mask_values.resize(static_cast<std::size_t>(rgba->w) * rgba->h);
        for (int y = 0; y < rgba->h; ++y) {
            const auto* row = static_cast<const std::uint8_t*>(rgba->pixels) + y * rgba->pitch;
            for (int x = 0; x < rgba->w; ++x) {
                loaded_mask_values[static_cast<std::size_t>(y) * rgba->w + x] = row[x * 4];
            }
        }
        SDL_FreeSurface(rgba);
        return true;
    }

    bool update_transition_mask(const std::string& path, float progress) {
        if (!ensure_transition_textures() || !mask_blend_supported || !load_mask(path)) return false;
        void* pixels = nullptr;
        int pitch = 0;
        if (SDL_LockTexture(transition_mask, nullptr, &pixels, &pitch) != 0) return false;
        for (int y = 0; y < kHeight; ++y) {
            auto* row = static_cast<std::uint8_t*>(pixels) + y * pitch;
            const int source_y = std::min(loaded_mask_height - 1, y * loaded_mask_height / kHeight);
            for (int x = 0; x < kWidth; ++x) {
                const int source_x = std::min(loaded_mask_width - 1, x * loaded_mask_width / kWidth);
                const float mask = loaded_mask_values[static_cast<std::size_t>(source_y) * loaded_mask_width + source_x] / 255.0F;
                const float reveal = std::clamp(progress * 2.0F - (1.0F - mask), 0.0F, 1.0F);
                row[x * 4] = 255;
                row[x * 4 + 1] = 255;
                row[x * 4 + 2] = 255;
                row[x * 4 + 3] = static_cast<std::uint8_t>(std::lround(reveal * 255.0F));
            }
        }
        SDL_UnlockTexture(transition_mask);
        return true;
    }

    void draw_scene(const SceneState& scene, double now_seconds) {
        const VisualTransitionState& transition = scene.visual_transition;
        if (!transition.active || transition.duration_seconds <= 0.0F) {
            draw_layers(scene.background, scene.characters, scene.sprites, now_seconds);
            return;
        }
        const float progress = std::clamp(
            static_cast<float>((now_seconds - transition.start_seconds) / transition.duration_seconds),
            0.0F, 1.0F);
        draw_layers(transition.previous.background, transition.previous.characters,
                    transition.previous.sprites, now_seconds);

        bool rendered_target = false;
        if (ensure_transition_textures() && SDL_SetRenderTarget(renderer, transition_target) == 0) {
            SDL_SetRenderDrawBlendMode(renderer, SDL_BLENDMODE_NONE);
            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
            SDL_RenderClear(renderer);
            SDL_SetRenderDrawBlendMode(renderer, SDL_BLENDMODE_BLEND);
            draw_layers(scene.background, scene.characters, scene.sprites, now_seconds);
            bool masked = false;
            if (!transition.mask_path.empty() && update_transition_mask(transition.mask_path, progress)) {
                masked = SDL_RenderCopy(renderer, transition_mask, nullptr, nullptr) == 0;
            }
            SDL_SetRenderTarget(renderer, nullptr);
            SDL_SetTextureAlphaMod(transition_target,
                                   masked ? 255 : static_cast<std::uint8_t>(std::lround(progress * 255.0F)));
            SDL_RenderCopy(renderer, transition_target, nullptr, nullptr);
            SDL_SetTextureAlphaMod(transition_target, 255);
            rendered_target = true;
        }
        if (!rendered_target) {
            if (!transition_warning_printed) {
                std::cerr << "wa2-cpp: render-target blending unavailable; using alpha transition fallback\n";
                transition_warning_printed = true;
            }
            SDL_SetRenderTarget(renderer, nullptr);
            draw_layers(scene.background, scene.characters, scene.sprites, now_seconds, progress);
        }
    }

    Color fade_color_at(const SceneState& scene, double now_seconds) const {
        if (!scene.color_transition.active || scene.color_transition.duration_seconds <= 0.0F) {
            return scene.fade_color;
        }
        const float progress = std::clamp(
            static_cast<float>((now_seconds - scene.color_transition.start_seconds) /
                               scene.color_transition.duration_seconds), 0.0F, 1.0F);
        const Color& from = scene.color_transition.from;
        const Color& to = scene.color_transition.to;
        return {from.r + (to.r - from.r) * progress,
                from.g + (to.g - from.g) * progress,
                from.b + (to.b - from.b) * progress,
                1.0F};
    }

    void draw_color_fade(const SceneState& scene, double now_seconds) {
        const Color color = fade_color_at(scene, now_seconds);
        const float distance = std::sqrt((color.r - 0.5F) * (color.r - 0.5F) +
                                         (color.g - 0.5F) * (color.g - 0.5F) +
                                         (color.b - 0.5F) * (color.b - 0.5F));
        const float weight = std::clamp(distance * 2.0F * 0.6F, 0.0F, 1.0F);
        if (weight <= 0.0F) return;
        SDL_SetRenderDrawBlendMode(renderer, SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(renderer,
                               color.r >= 0.5F ? 255 : 0,
                               color.g >= 0.5F ? 255 : 0,
                               color.b >= 0.5F ? 255 : 0,
                               static_cast<std::uint8_t>(std::lround(weight * 255.0F)));
        SDL_Rect screen{0, 0, kWidth, kHeight};
        SDL_RenderFillRect(renderer, &screen);
    }

    void draw_text(const std::string& value, int x, int y, int width, SDL_Color color, bool small = false) {
        TTF_Font* selected = small ? small_font : font;
        if (selected == nullptr || value.empty()) return;
        SDL_Surface* surface = TTF_RenderUTF8_Blended_Wrapped(selected, strip_tags(value).c_str(), color,
                                                              static_cast<std::uint32_t>(std::max(1, width)));
        if (surface == nullptr) return;
        SDL_Texture* text = SDL_CreateTextureFromSurface(renderer, surface);
        if (text != nullptr) {
            SDL_Rect target{x, y, surface->w, surface->h};
            SDL_RenderCopy(renderer, text, nullptr, &target);
            SDL_DestroyTexture(text);
        }
        SDL_FreeSurface(surface);
    }

    Mix_Chunk* load_chunk(const std::string& path, int slot) {
        if (runtime == nullptr || slot < 0 || slot >= static_cast<int>(chunks.size())) return nullptr;
        Mix_HaltChannel(slot);
        if (chunks[slot] != nullptr) {
            Mix_FreeChunk(chunks[slot]);
            chunks[slot] = nullptr;
        }
        chunk_bytes[slot] = runtime->archives().read(path);
        if (chunk_bytes[slot].empty()) return nullptr;
        SDL_RWops* rw = SDL_RWFromConstMem(chunk_bytes[slot].data(), static_cast<int>(chunk_bytes[slot].size()));
        chunks[slot] = rw == nullptr ? nullptr : Mix_LoadWAV_RW(rw, 1);
        return chunks[slot];
    }
};

SdlFrontend::SdlFrontend() : impl_(std::make_unique<Impl>()) {}

SdlFrontend::~SdlFrontend() {
    stop_all();
    if (impl_->transition_mask != nullptr) SDL_DestroyTexture(impl_->transition_mask);
    if (impl_->transition_target != nullptr) SDL_DestroyTexture(impl_->transition_target);
    for (auto& pair : impl_->effect_textures) SDL_DestroyTexture(pair.second.value);
    for (auto& pair : impl_->textures) SDL_DestroyTexture(pair.second.value);
    for (auto*& chunk : impl_->chunks) {
        if (chunk != nullptr) Mix_FreeChunk(chunk);
        chunk = nullptr;
    }
    if (impl_->small_font != nullptr) TTF_CloseFont(impl_->small_font);
    if (impl_->font != nullptr) TTF_CloseFont(impl_->font);
    if (impl_->renderer != nullptr) SDL_DestroyRenderer(impl_->renderer);
    if (impl_->window != nullptr) SDL_DestroyWindow(impl_->window);
    Mix_CloseAudio();
    TTF_Quit();
    IMG_Quit();
    SDL_Quit();
}

bool SdlFrontend::initialize(const std::string& title, const std::string& font_path, std::string* error) {
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO | SDL_INIT_GAMECONTROLLER) != 0) {
        if (error != nullptr) *error = SDL_GetError();
        return false;
    }
    IMG_Init(IMG_INIT_PNG | IMG_INIT_JPG);
    if (TTF_Init() != 0) {
        if (error != nullptr) *error = TTF_GetError();
        return false;
    }
    impl_->window = SDL_CreateWindow(title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                                     kWidth, kHeight, SDL_WINDOW_SHOWN | SDL_WINDOW_RESIZABLE);
    impl_->renderer = SDL_CreateRenderer(impl_->window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
    if (impl_->window == nullptr || impl_->renderer == nullptr) {
        if (error != nullptr) *error = SDL_GetError();
        return false;
    }
    SDL_RenderSetLogicalSize(impl_->renderer, kWidth, kHeight);
    SDL_SetRenderDrawBlendMode(impl_->renderer, SDL_BLENDMODE_BLEND);
    if (font_path.empty()) {
        if (error != nullptr) *error = "no UI font was found; pass --font PATH";
        return false;
    }
    impl_->font = TTF_OpenFont(font_path.c_str(), 30);
    impl_->small_font = TTF_OpenFont(font_path.c_str(), 21);
    if (impl_->font == nullptr || impl_->small_font == nullptr) {
        if (error != nullptr) *error = std::string("cannot open UI font: ") + TTF_GetError();
        return false;
    }
    if (Mix_OpenAudio(48000, AUDIO_S16SYS, 2, 2048) < 0) {
        if (error != nullptr) *error = Mix_GetError();
        return false;
    }
    Mix_AllocateChannels(24);
    return true;
}

void SdlFrontend::attach(Runtime& runtime) { impl_->runtime = &runtime; }

void SdlFrontend::render(double now_seconds, std::size_t selected_choice) {
    if (impl_->runtime == nullptr) return;
    const SceneState& scene = impl_->runtime->scene();
    SDL_SetRenderDrawColor(impl_->renderer, 0, 0, 0, 255);
    SDL_RenderClear(impl_->renderer);

    impl_->draw_scene(scene, now_seconds);
    impl_->draw_color_fade(scene, now_seconds);

    if (impl_->video.playing() || impl_->video_was_playing) {
        impl_->video.update(now_seconds);
        impl_->video.render(impl_->renderer, kWidth, kHeight);
        if (impl_->video_was_playing && impl_->video.finished()) {
            impl_->video_was_playing = false;
            impl_->runtime->movie_finished();
        }
    }

    if (scene.message_visible) {
        SDL_SetRenderDrawColor(impl_->renderer, 8, 12, 18, scene.novel_mode ? 195 : 225);
        SDL_Rect panel{0, scene.novel_mode ? 0 : 470, kWidth, scene.novel_mode ? kHeight : 250};
        SDL_RenderFillRect(impl_->renderer, &panel);
        const int base_y = scene.novel_mode ? 42 : 492;
        impl_->draw_text(scene.speaker, 80, base_y, 1120, SDL_Color{116, 205, 255, 255}, true);
        impl_->draw_text(scene.message, 80, base_y + 38, 1120, SDL_Color{245, 247, 250, 255});
    }

    if (impl_->runtime->at_title()) {
        SDL_SetRenderDrawColor(impl_->renderer, 5, 10, 15, 235);
        SDL_Rect title{0, 0, kWidth, kHeight};
        SDL_RenderFillRect(impl_->renderer, &title);
        impl_->draw_text("WHITE ALBUM2 C++ Runtime", 350, 95, 620, SDL_Color{245,247,250,255});
    }

    if (!scene.choices.empty()) {
        const int item_height = 64;
        const int top = impl_->runtime->at_title() ? 190 : 250 - static_cast<int>(scene.choices.size()) * 5;
        for (std::size_t i = 0; i < scene.choices.size(); ++i) {
            const bool selected = i == selected_choice;
            const bool enabled = scene.choices[i].enabled;
            SDL_SetRenderDrawColor(impl_->renderer, selected ? 36 : 10, selected ? 118 : 30,
                                   selected ? 166 : 48, enabled ? 235 : 145);
            SDL_Rect item{220, top + static_cast<int>(i) * (item_height + 12), 840, item_height};
            SDL_RenderFillRect(impl_->renderer, &item);
            impl_->draw_text(scene.choices[i].text, 250, item.y + 15, 780,
                             enabled ? SDL_Color{255,255,255,255} : SDL_Color{145,150,156,255}, true);
        }
    }

    if (scene.calendar.visible) {
        SDL_SetRenderDrawColor(impl_->renderer, 12, 17, 23, 235);
        SDL_Rect calendar{430, 235, 420, 200};
        SDL_RenderFillRect(impl_->renderer, &calendar);
        const std::string date = std::to_string(scene.calendar.year) + " / " +
                                 std::to_string(scene.calendar.month) + " / " +
                                 std::to_string(scene.calendar.day);
        impl_->draw_text(date, 490, 295, 320, SDL_Color{255,255,255,255});
    }

    SDL_RenderPresent(impl_->renderer);
}

SDL_Renderer* SdlFrontend::renderer() noexcept { return impl_->renderer; }
SDL_Window* SdlFrontend::window() noexcept { return impl_->window; }

void SdlFrontend::play_bgm(std::int32_t id, bool loop, std::int32_t volume) {
    if (impl_->runtime == nullptr) return;
    stop_bgm(0.0F);
    std::string path = format_name("BGM_%03d.OGG", id);
    if (!impl_->runtime->archives().contains(path)) path = format_name("BGM_%03d_A.OGG", id);
    impl_->music_bytes = impl_->runtime->archives().read(path);
    if (impl_->music_bytes.empty()) return;
    SDL_RWops* rw = SDL_RWFromConstMem(impl_->music_bytes.data(), static_cast<int>(impl_->music_bytes.size()));
    impl_->music = rw == nullptr ? nullptr : Mix_LoadMUS_RW(rw, 1);
    if (impl_->music != nullptr) {
        Mix_VolumeMusic(mixer_volume(volume));
        Mix_PlayMusic(impl_->music, loop ? -1 : 0);
    }
}

void SdlFrontend::stop_bgm(float fade_seconds) {
    if (fade_seconds > 0.0F) {
        Mix_FadeOutMusic(static_cast<int>(fade_seconds * 1000));
        return;
    }
    Mix_HaltMusic();
    if (impl_->music != nullptr) {
        Mix_FreeMusic(impl_->music);
        impl_->music = nullptr;
    }
    impl_->music_bytes.clear();
}

void SdlFrontend::play_voice(std::int32_t label, std::int32_t id, std::int32_t character,
                             std::int32_t volume, bool loop, std::int32_t channel) {
    const int slot = std::clamp(channel, 0, 7) + kVoiceChannelBase;
    const std::string path = format_name("%04d_%04d_%02d.ogg", label, id, character);
    Mix_Chunk* chunk = impl_->load_chunk(path, slot);
    if (chunk == nullptr) return;
    Mix_Volume(slot, mixer_volume(volume));
    Mix_PlayChannel(slot, chunk, loop ? -1 : 0);
    impl_->channel_ends[slot] = SDL_GetTicks64() / 1000.0 + (loop ? 86400.0 : chunk_seconds(chunk));
}

void SdlFrontend::stop_voice(std::int32_t channel, float fade_seconds) {
    const int slot = std::clamp(channel, 0, 7) + kVoiceChannelBase;
    fade_seconds > 0.0F ? Mix_FadeOutChannel(slot, static_cast<int>(fade_seconds * 1000)) : Mix_HaltChannel(slot);
    impl_->channel_ends[slot] = 0.0;
}

double SdlFrontend::voice_remaining(std::int32_t channel) const {
    const int slot = std::clamp(channel, 0, 7) + kVoiceChannelBase;
    return Mix_Playing(slot) ? std::max(0.0, impl_->channel_ends[slot] - SDL_GetTicks64() / 1000.0) : 0.0;
}

void SdlFrontend::play_se(std::int32_t channel, std::int32_t id, bool loop,
                          float fade_seconds, std::int32_t volume) {
    int logical = channel;
    if (logical < 0) {
        logical = 0;
        while (logical < 8 && Mix_Playing(kSeChannelBase + logical)) ++logical;
        if (logical >= 8) logical = 0;
    }
    const int slot = kSeChannelBase + std::clamp(logical, 0, 7);
    std::string path = format_name("se_%04d.wav", id);
    if (impl_->runtime != nullptr && !impl_->runtime->archives().contains(path)) path = format_name("se_%04d.ogg", id);
    Mix_Chunk* chunk = impl_->load_chunk(path, slot);
    if (chunk == nullptr) return;
    Mix_Volume(slot, mixer_volume(volume));
    if (fade_seconds > 0.0F) Mix_FadeInChannel(slot, chunk, loop ? -1 : 0, static_cast<int>(fade_seconds * 1000));
    else Mix_PlayChannel(slot, chunk, loop ? -1 : 0);
    impl_->channel_ends[slot] = SDL_GetTicks64() / 1000.0 + (loop ? 86400.0 : chunk_seconds(chunk));
}

void SdlFrontend::stop_se(std::int32_t channel, float fade_seconds) {
    const int slot = kSeChannelBase + std::clamp(channel, 0, 7);
    fade_seconds > 0.0F ? Mix_FadeOutChannel(slot, static_cast<int>(fade_seconds * 1000)) : Mix_HaltChannel(slot);
    impl_->channel_ends[slot] = 0.0;
}

double SdlFrontend::se_remaining(std::int32_t channel) const {
    const int slot = kSeChannelBase + std::clamp(channel, 0, 7);
    return Mix_Playing(slot) ? std::max(0.0, impl_->channel_ends[slot] - SDL_GetTicks64() / 1000.0) : 0.0;
}

void SdlFrontend::stop_all() {
    Mix_HaltChannel(-1);
    stop_bgm(0.0F);
    stop_movie();
}

bool SdlFrontend::play_movie(const std::string& path) {
    std::string error;
    const bool opened = impl_->video.open(impl_->renderer, path, &error);
    impl_->video_was_playing = opened;
    if (!opened && impl_->runtime != nullptr) impl_->runtime->scene().message = error;
    return opened;
}

void SdlFrontend::stop_movie() {
    impl_->video.stop();
    impl_->video_was_playing = false;
}

} // namespace wa2

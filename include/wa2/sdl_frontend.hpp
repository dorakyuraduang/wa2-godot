#pragma once

#include "wa2/runtime.hpp"
#include "wa2/video.hpp"

#include <cstdint>
#include <memory>
#include <string>

struct SDL_Renderer;
struct SDL_Window;

namespace wa2 {

class SdlFrontend final : public MediaSink {
public:
    SdlFrontend();
    ~SdlFrontend();
    SdlFrontend(const SdlFrontend&) = delete;
    SdlFrontend& operator=(const SdlFrontend&) = delete;

    bool initialize(const std::string& title, const std::string& font_path, std::string* error = nullptr);
    void attach(Runtime& runtime);
    void render(double now_seconds, std::size_t selected_choice);
    SDL_Renderer* renderer() noexcept;
    SDL_Window* window() noexcept;

    void play_bgm(std::int32_t id, bool loop, std::int32_t volume) override;
    void stop_bgm(float fade_seconds) override;
    void play_voice(std::int32_t label, std::int32_t id, std::int32_t character,
                    std::int32_t volume, bool loop, std::int32_t channel) override;
    void stop_voice(std::int32_t channel, float fade_seconds) override;
    double voice_remaining(std::int32_t channel) const override;
    void play_se(std::int32_t channel, std::int32_t id, bool loop,
                 float fade_seconds, std::int32_t volume) override;
    void stop_se(std::int32_t channel, float fade_seconds) override;
    double se_remaining(std::int32_t channel) const override;
    void stop_all() override;
    bool play_movie(const std::string& path) override;
    void stop_movie() override;

private:
    struct Impl;
    std::unique_ptr<Impl> impl_;
};

} // namespace wa2

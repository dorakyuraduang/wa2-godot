#include "wa2/runtime.hpp"

#include "wa2/error.hpp"

#include <algorithm>
#include <array>
#include <cmath>
#include <cstdio>
#include <cstdlib>
#include <sstream>

namespace wa2 {
namespace {

constexpr float kFrameTime = 1.0F / 60.0F;

std::string format_image(const char* format, int a, int b = 0, int c = 0) {
    std::array<char, 64> buffer{};
    std::snprintf(buffer.data(), buffer.size(), format, a, b, c);
    return buffer.data();
}

const char* character_prefix(std::int32_t id) {
    static const std::unordered_map<std::int32_t, const char*> prefixes{
        {0,"har"},{1,"kaz"},{2,"set"},{3,"koh"},{4,"izu"},{5,"mar"},
        {10,"tak"},{11,"ioo"},{12,"chi"},{13,"pap"},{14,"mam"},{15,"oto"},
        {16,"you"},{17,"tan"},{18,"shi"},{19,"tom"},{20,"sat"},{21,"hon"},
        {22,"nak"},{23,"say"},{24,"aco"},{25,"mih"},{26,"mhh"},{27,"ueh"},
        {28,"yos"},{29,"tan"},{30,"ham"},{31,"mat"},{32,"kiz"},{33,"suz"},
        {34,"saw"},{35,"miy"},{36,"yan"},{37,"mas"},
    };
    const auto found = prefixes.find(id);
    return found == prefixes.end() ? "unk" : found->second;
}

std::string movie_relative_path(std::string name) {
    name = ArchiveIndex::normalized_name(std::move(name));
    static const std::unordered_map<std::string, std::string> paths{
        {"mv00","mv000.pak"},{"mv10","mv100.pak"},{"mv11","mv110.pak"},
        {"mv12","mv120.pak"},{"mv13","mv130.pak"},{"mv14","mv140.pak"},
        {"mv20","mv200.pak"},{"mv21","mv210.pak"},{"mv22","mv220.pak"},
        {"mv23","mv230.pak"},{"mv24","mv240.pak"},{"mv01","IC/mv010.pak"},
        {"mv02","IC/mv020.pak"},{"mv07","IC/MV070.pak"},{"mv08","IC/MV080.pak"},
        {"mv09","IC/MV090.pak"},
    };
    const auto found = paths.find(name);
    return found == paths.end() ? std::string{} : found->second;
}

bool is_false_yield(std::uint32_t function) {
    switch (function) {
    case 0x3: case 0x5: case 0x87: case 0x8E: case 0x8F: case 0x91:
    case 0x95: case 0x97: case 0xA0: case 0xA2: case 0xA3: case 0xA9:
    case 0xB2: case 0xB3: case 0xB5: case 0xCF: case 0xD3: case 0xD4:
    case 0xD6: case 0xD7: case 0xD8: case 0xD9: case 0xDA: case 0xE0:
    case 0xE4: case 0xE7: case 0xE8:
        return true;
    default:
        return false;
    }
}

} // namespace

Runtime::Runtime(std::string resource_root, std::string save_root, MediaSink& media)
    : archives_(std::move(resource_root)), save_root_(std::move(save_root)), media_(media) {}

bool Runtime::initialize(Language language, std::string* error) {
    language_ = language;
    warnings_.clear();
    archives_.clear();

    const std::array<const char*, 8> base_archives{
        "BGM.PAK", "IC/BGM.PAK", "IC/bak.pak", "IC/grp.pak", "IC/char.pak",
        "IC/VOICE.PAK", "IC/SE.PAK", "bak.pak",
    };
    for (const char* archive : base_archives) {
        std::string archive_error;
        if (!archives_.load_archive(archive, &archive_error)) warnings_.push_back(archive_error);
    }

    const char* script_archive = language == Language::japanese ? "script.pak" : "ck-gal.pak";
    std::string script_error;
    if (!archives_.load_archive(script_archive, &script_error)) {
        if (error != nullptr) *error = script_error;
        return false;
    }

    // Root resources override the IC and script archives in the original runtime.
    const std::array<const char*, 4> root_archives{"grp.pak", "char.pak", "VOICE.PAK", "SE.PAK"};
    for (const char* archive : root_archives) {
        std::string archive_error;
        if (!archives_.load_archive(archive, &archive_error)) warnings_.push_back(archive_error);
    }

    std::string store_error;
    if (!system_store_.load_or_create(join_path(save_root_, "sys.sav"), &store_error)) {
        if (error != nullptr) *error = store_error;
        return false;
    }
    at_title_ = true;
    return true;
}

bool Runtime::start_script(const std::string& name, std::int32_t point, std::string* error) {
    try {
        script_stack_.clear();
        script_stack_.push_back(std::make_unique<ScriptVm>(archives_, *this, name, point));
        pending_script_ = {};
        pending_finished_ = nullptr;
        wait_mode_ = WaitMode::none;
        running_ = true;
        at_title_ = false;
        scene_.choices.clear();
        return true;
    } catch (const std::exception& ex) {
        if (error != nullptr) *error = ex.what();
        return false;
    }
}

void Runtime::tick(double now_seconds) {
    now_ = now_seconds;
    if (scene_.visual_transition.active &&
        now_ >= scene_.visual_transition.start_seconds + scene_.visual_transition.duration_seconds) {
        scene_.visual_transition.active = false;
    }
    if (scene_.color_transition.active &&
        now_ >= scene_.color_transition.start_seconds + scene_.color_transition.duration_seconds) {
        scene_.fade_color = scene_.color_transition.to;
        scene_.color_transition.active = false;
    }
    for (auto& pair : scene_.sprites) {
        SpriteState& sprite = pair.second;
        if (sprite.alpha_transition_active &&
            now_ >= sprite.alpha_start_seconds + sprite.alpha_duration_seconds) {
            sprite.alpha = sprite.alpha_to;
            sprite.alpha_transition_active = false;
        }
    }
    apply_script_finish();
    apply_pending_script();
    if (!running_ || script_stack_.empty()) return;

    if (wait_mode_ == WaitMode::timer && now_ >= wait_until_) wait_mode_ = WaitMode::none;
    if (wait_mode_ != WaitMode::none) return;

    ScriptVm* script = active_script();
    if (script != nullptr) script->run();
    apply_script_finish();
    apply_pending_script();
}

void Runtime::advance() {
    if (wait_mode_ == WaitMode::click) {
        wait_mode_ = WaitMode::none;
        scene_.calendar.visible = false;
        ++current_message_;
    } else if (wait_mode_ == WaitMode::timer && skip()) {
        wait_mode_ = WaitMode::none;
        finish_transitions();
    } else if (wait_mode_ == WaitMode::movie && skip()) {
        media_.stop_movie();
        movie_finished();
    }
}

bool Runtime::choose(std::size_t index) {
    if (wait_mode_ != WaitMode::choice || index >= scene_.choices.size() || !scene_.choices[index].enabled) {
        return false;
    }
    ScriptVm* script = active_script();
    if (script == nullptr || !choice_has_target_ || choice_target_ >= script->arguments().size()) return false;
    script->assign(script->arguments()[choice_target_], static_cast<std::int32_t>(index));
    scene_.choices.clear();
    choice_has_target_ = false;
    wait_mode_ = WaitMode::none;
    return true;
}

void Runtime::movie_finished() {
    if (wait_mode_ == WaitMode::movie) wait_mode_ = WaitMode::none;
}

ScriptVm* Runtime::active_script() noexcept {
    return script_stack_.empty() ? nullptr : script_stack_.back().get();
}

std::int32_t Runtime::game_flag(std::size_t index) const {
    return index < game_flags_.size() ? game_flags_[index] : 0;
}

void Runtime::set_game_flag(std::size_t index, std::int32_t value) {
    if (index >= game_flags_.size()) game_flags_.resize(index + 1, 0);
    game_flags_[index] = value;
}

bool Runtime::call_function(std::uint32_t function, std::vector<Value>& args, ScriptVm& vm) {
    switch (function) {
    case 0x0: // SLoad
        pending_script_ = {ScriptAction::replace, arg_s(args, 0, vm), arg_i(args, 1, vm)};
        return false;
    case 0x1: // SCall
        pending_script_ = {ScriptAction::push, arg_s(args, 0, vm), arg_i(args, 1, vm)};
        return false;
    case 0x2: // call
        pending_script_ = {ScriptAction::push, vm.name(), arg_i(args, 0, vm)};
        return false;
    case 0x3:
    case 0x4:
    case 0x5:
        return function == 0x4;
    case 0x6: {
        const auto value = arg_i(args, args.empty() ? 0 : args.size() - 1, vm);
        if (!args.empty()) args.pop_back();
        vm.push_integer(value);
        return true;
    }
    case 0x7: {
        const float value = arg_f(args, args.empty() ? 0 : args.size() - 1, vm);
        if (!args.empty()) args.pop_back();
        vm.push_float(value);
        return true;
    }
    case 0x8:
        vm.push_integer(std::rand());
        return true;
    case 0x9: case 0xA: case 0xB: case 0xC: case 0xD: case 0xE: {
        const float value = arg_f(args, args.empty() ? 0 : args.size() - 1, vm);
        if (!args.empty()) args.pop_back();
        float result = 0.0F;
        if (function == 0x9) result = std::sin(value * 3.1415926F / 180.0F);
        if (function == 0xA) result = std::cos(value);
        if (function == 0xB) result = std::tan(value);
        if (function == 0xC) result = std::asin(value);
        if (function == 0xD) result = std::acos(value);
        if (function == 0xE) result = std::atan(value);
        vm.push_float(result);
        return true;
    }
    case 0xF:
        if (args.size() >= 2) {
            const float y = arg_f(args, args.size() - 2, vm);
            const float x = arg_f(args, args.size() - 1, vm);
            args.resize(args.size() - 2);
            vm.push_float(std::atan2(y, x));
        }
        return true;
    case 0x10:
        if (args.size() >= 2) {
            const float base = arg_f(args, args.size() - 2, vm);
            const float exponent = arg_f(args, args.size() - 1, vm);
            args.resize(args.size() - 2);
            vm.push_float(std::pow(base, exponent));
        }
        return true;
    case 0x11: {
        const float value = arg_f(args, args.empty() ? 0 : args.size() - 1, vm);
        if (!args.empty()) args.pop_back();
        vm.push_float(std::sqrt(std::max(0.0F, value)));
        return true;
    }
    case 0x12:
        vm.push_integer(static_cast<std::int32_t>(now_ * 1000.0));
        return true;
    case 0x80:
    case 0x81:
        return true;
    case 0x82:
    case 0x83:
    case 0x85: {
        const std::string text = arg_s(args, 0, vm);
        const bool append = arg_i(args, 2, vm) == 0;
        if (append) scene_.message += "\\k" + text;
        else scene_.message = text;
        scene_.message_visible = true;
        current_message_ = arg_i(args, 1, vm);
        skip_disabled_ = function == 0x85;
        wait_mode_ = WaitMode::click;
        return false;
    }
    case 0x84:
        scene_.speaker.clear();
        return true;
    case 0x86:
        skip_disabled_ = false;
        return true;
    case 0x87:
        wait_mode_ = WaitMode::click;
        return false;
    case 0x88:
        demo_mode_ = arg_i(args, 0, vm) > 0;
        if (demo_mode_) auto_mode_ = false;
        return true;
    case 0x89:
        if (arg_i(args, 1, vm, -1) != -1) label_ = arg_i(args, 1, vm);
        return true;
    case 0x8A:
        media_.play_voice(label_, arg_i(args, 4, vm), arg_i(args, 0, vm), arg_i(args, 1, vm),
                          arg_i(args, 2, vm) == 1, arg_i(args, 3, vm));
        return true;
    case 0x8B:
        media_.play_voice(arg_i(args, 2, vm), arg_i(args, 1, vm), arg_i(args, 0, vm),
                          arg_i(args, 3, vm), arg_i(args, 4, vm) == 1, arg_i(args, 5, vm));
        return true;
    case 0x8C:
        wait_for(media_.voice_remaining(arg_i(args, 1, vm)), WaitMode::timer);
        args.clear();
        return false;
    case 0x8D:
        media_.stop_voice(arg_i(args, 1, vm), arg_i(args, 0, vm) * kFrameTime);
        return true;
    case 0x8E:
    case 0x8F:
        return false;
    case 0x90:
    case 0xDF:
        scene_.speaker = !args.empty() && args[0].command == CommandType::string_variable ? arg_s(args, 0, vm) : "";
        return true;
    case 0x91:
    case 0xE0:
        return false;
    case 0x92:
    case 0x93:
    case 0x94:
    case 0xE1:
    case 0xE2:
    case 0xE3: {
        const std::int32_t id = arg_i(args, 2, vm) + 10 * arg_i(args, 1, vm);
        const bool preserve = function == 0x93 || function == 0xE2;
        const int type = function == 0x94 || function == 0xE3 ? 1 : 0;
        float scale_x = arg_f(args, 7, vm, 1.0F);
        float scale_y = arg_f(args, 8, vm, 1.0F);
        if (function < 0xE1) {
            scale_x /= 1280.0F;
            scale_y /= 720.0F;
        }
        render_image(id, arg_i(args, 0, vm), preserve, type, arg_i(args, 3, vm),
                     arg_i(args, 4, vm), arg_i(args, 5, vm), arg_i(args, 6, vm), scale_x, scale_y);
        return false;
    }
    case 0x95:
        return false;
    case 0x96:
    case 0xE7:
        wait_for(std::max(1, arg_i(args, 2, vm)) * kFrameTime);
        return false;
    case 0x97:
        return false;
    case 0x98: {
        const float seconds = arg_i(args, 1, vm) * kFrameTime;
        const Color target{arg_i(args, 2, vm) / 255.0F, arg_i(args, 3, vm) / 255.0F,
                           arg_i(args, 4, vm) / 255.0F, 1.0F};
        begin_color_transition(current_fade_color(), target, seconds);
        wait_for(seconds);
        return false;
    }
    case 0x99: {
        const float seconds = arg_i(args, 0, vm) * kFrameTime;
        const Color source{arg_i(args, 1, vm) / 255.0F, arg_i(args, 2, vm) / 255.0F,
                           arg_i(args, 3, vm) / 255.0F, 1.0F};
        begin_color_transition(source, Color{0.5F, 0.5F, 0.5F, 1.0F}, seconds);
        wait_for(seconds);
        return false;
    }
    case 0x9A:
    case 0x9B: {
        const float seconds = function == 0x9A ? arg_i(args, 5, vm) * kFrameTime : 0.0F;
        begin_visual_transition(seconds);
        add_character(arg_i(args, 0, vm), arg_i(args, 1, vm), arg_i(args, 2, vm));
        if (function == 0x9A) wait_for(seconds);
        return function == 0x9B;
    }
    case 0x9C:
    case 0x9D: {
        const float seconds = function == 0x9C ? arg_i(args, 2, vm) * kFrameTime : 0.0F;
        begin_visual_transition(seconds);
        remove_character(arg_i(args, 0, vm));
        if (function == 0x9C) wait_for(seconds);
        return function == 0x9D;
    }
    case 0x9E:
        media_.play_bgm(arg_i(args, 0, vm), arg_i(args, 2, vm) != 0, arg_i(args, 3, vm, 255));
        return true;
    case 0x9F:
        media_.stop_bgm(arg_i(args, 0, vm) * kFrameTime);
        return true;
    case 0xA0:
    case 0xA1:
    case 0xA2:
    case 0xA3:
        return function == 0xA1;
    case 0xA4:
        media_.play_se(-1, arg_i(args, 0, vm), false, 0.0F, arg_i(args, 1, vm, 255));
        args.clear();
        return true;
    case 0xA5:
        media_.play_se(arg_i(args, 0, vm), arg_i(args, 1, vm), arg_i(args, 3, vm) != 0,
                       arg_i(args, 2, vm) * kFrameTime, arg_i(args, 4, vm, 255));
        args.clear();
        return true;
    case 0xA6:
        media_.stop_se(arg_i(args, 0, vm), arg_i(args, 1, vm) * kFrameTime);
        args.clear();
        return true;
    case 0xA7:
        return true;
    case 0xA8:
        wait_for(media_.se_remaining(arg_i(args, 0, vm)));
        args.clear();
        return false;
    case 0xA9:
        return false;
    case 0xAA:
        time_mode_ = arg_i(args, 0, vm);
        return true;
    case 0xAB:
        return true;
    case 0xAC:
        effect_mode_ = arg_s(args, 0, vm);
        return true;
    case 0xAD:
    case 0xE5:
        set_weather(args, vm, function == 0xE5);
        return true;
    case 0xAE:
    case 0xE6:
        if (arg_i(args, 0, vm, -1000) != -1000) scene_.weather.speed_x = arg_i(args, 0, vm);
        if (arg_i(args, 1, vm, -1000) != -1000) scene_.weather.speed_y = arg_i(args, 1, vm);
        if (arg_i(args, 2, vm, -1000) != -1000) scene_.weather.count = arg_i(args, 2, vm);
        if (arg_i(args, 4, vm, -1000) != -1000) scene_.weather.frame = arg_i(args, 4, vm);
        return true;
    case 0xAF:
        scene_.weather = {};
        scene_.weather.type = -1;
        return true;
    case 0xB0:
    case 0xB1: {
        SpriteState sprite;
        sprite.key = arg_i(args, 0, vm);
        sprite.path = arg_s(args, 1, vm);
        sprite.layer = arg_i(args, 2, vm);
        sprite.animated = function == 0xB1;
        scene_.sprites[sprite.key] = std::move(sprite);
        return true;
    }
    case 0xB2:
    case 0xB3:
        return false;
    case 0xB4:
        scene_.sprites.erase(arg_i(args, 0, vm));
        return true;
    case 0xB5:
        return false;
    case 0xB6:
    case 0xB7:
    case 0xB8:
        return true;
    case 0xB9: {
        const auto found = scene_.sprites.find(arg_i(args, 0, vm));
        if (found != scene_.sprites.end()) {
            SpriteState& sprite = found->second;
            sprite.mode = arg_i(args, 1, vm);
            float current_alpha = sprite.alpha;
            if (sprite.alpha_transition_active && sprite.alpha_duration_seconds > 0.0F) {
                const float progress = std::clamp(
                    static_cast<float>((now_ - sprite.alpha_start_seconds) / sprite.alpha_duration_seconds),
                    0.0F, 1.0F);
                current_alpha = sprite.alpha_from + (sprite.alpha_to - sprite.alpha_from) * progress;
            }
            const int requested_alpha = arg_i(args, 2, vm);
            const int frames = arg_i(args, 3, vm);
            const int adjusted_alpha = frames <= 0 && sprite.animated ? requested_alpha + 17 : requested_alpha;
            const float target_alpha = std::clamp(adjusted_alpha, 0, 255) / 255.0F;
            sprite.alpha = target_alpha;
            sprite.alpha_from = current_alpha;
            sprite.alpha_to = target_alpha;
            sprite.alpha_start_seconds = now_;
            sprite.alpha_duration_seconds = std::max(0, frames) * kFrameTime;
            sprite.alpha_transition_active = frames > 0 && !skip();
        }
        return true;
    }
    case 0xBA:
    case 0xBB:
        return true;
    case 0xBC: {
        const auto found = scene_.sprites.find(arg_i(args, 0, vm));
        if (found != scene_.sprites.end()) found->second.position = {arg_f(args, 1, vm), arg_f(args, 2, vm)};
        return true;
    }
    case 0xBD:
        return true;
    case 0xBE: {
        const auto found = scene_.sprites.find(arg_i(args, 0, vm));
        if (found != scene_.sprites.end()) {
            found->second.position = {arg_f(args, 1, vm), arg_f(args, 2, vm)};
            found->second.scale = {arg_f(args, 3, vm), arg_f(args, 4, vm)};
        }
        return true;
    }
    case 0xBF: {
        const auto found = scene_.sprites.find(arg_i(args, 0, vm));
        if (found != scene_.sprites.end()) {
            found->second.offset = {-arg_f(args, 1, vm), -arg_f(args, 2, vm)};
            found->second.scale = {arg_f(args, 3, vm), arg_f(args, 3, vm)};
        }
        return true;
    }
    case 0xC0:
        return true;
    case 0xC1: {
        skip_ = false;
        media_.stop_all();
        const std::string relative = movie_relative_path(arg_s(args, 0, vm));
        if (!relative.empty() && media_.play_movie(join_path(archives_.resource_root(), relative))) {
            wait_mode_ = WaitMode::movie;
        }
        return false;
    }
    case 0xC2:
    case 0xEC:
        wait_for(arg_i(args, 0, vm) * kFrameTime);
        return false;
    case 0xC3:
        timer_start_ = now_;
        return true;
    case 0xC4: {
        const double target = timer_start_ + arg_i(args, 0, vm) / 1000.0;
        wait_for(std::max(0.0, target - now_));
        args.clear();
        return false;
    }
    case 0xC5:
        at_title_ = true;
        running_ = false;
        return true;
    case 0xC6:
        vm.push_integer(system_store_.flag(static_cast<std::size_t>(arg_i(args, 0, vm))));
        return true;
    case 0xC7:
        system_store_.set_flag(static_cast<std::size_t>(arg_i(args, 0, vm)), arg_i(args, 1, vm));
        return true;
    case 0xC8:
        return false;
    case 0xC9:
        system_store_.set_cg_flag(static_cast<std::size_t>(arg_i(args, 0, vm) * 10 + arg_i(args, 1, vm)),
                                  static_cast<std::uint8_t>(arg_i(args, 2, vm)));
        return true;
    case 0xCA:
        return true;
    case 0xCB: {
        scene_.calendar.year = arg_i(args, 0, vm);
        scene_.calendar.month = arg_i(args, 1, vm);
        scene_.calendar.day = arg_i(args, 2, vm);
        scene_.calendar.day_of_week = arg_i(args, 3, vm, -1);
        if (scene_.calendar.day_of_week < 0) {
            int year = scene_.calendar.year;
            int month = scene_.calendar.month;
            if (month <= 2) { --year; month += 12; }
            scene_.calendar.day_of_week = (scene_.calendar.day + year + year / 4 - year / 100 +
                                           (13 * month + 8) / 5) % 7;
        }
        scene_.calendar.visible = true;
        wait_mode_ = WaitMode::click;
        return false;
    }
    case 0xCC:
        vm.push_integer(static_cast<std::int32_t>((now_ - timer_start_) * 1000.0));
        return true;
    case 0xCD:
    case 0xED:
    case 0xEE:
        vm.push_integer(skip() ? 1 : 0);
        return true;
    case 0xCE:
        vm.push_integer(wait_mode_ == WaitMode::none ? 0 : 1);
        return true;
    case 0xCF:
        return false;
    case 0xD0: {
        ChoiceState choice;
        choice.text = arg_s(args, 0, vm);
        choice.flag = arg_i(args, 1, vm);
        choice.required_value = arg_i(args, 2, vm);
        choice.value = arg_i(args, 3, vm);
        choice.enabled = choice.flag <= 0 || system_store_.flag(static_cast<std::size_t>(choice.flag)) == choice.required_value;
        scene_.choices.push_back(std::move(choice));
        return true;
    }
    case 0xD1:
        choice_has_target_ = !args.empty();
        choice_target_ = args.empty() ? 0 : args.size() - 1;
        wait_mode_ = WaitMode::choice;
        return false;
    case 0xD2:
        scene_.background.offset = {arg_f(args, 0, vm), arg_f(args, 1, vm)};
        return true;
    case 0xD5:
        return false;
    case 0xDB:
        skip_ = false;
        auto_mode_ = false;
        return false;
    case 0xDC:
        scene_.novel_mode = arg_i(args, 0, vm) != 0;
        return false;
    case 0xDD:
        ero_mode_ = arg_i(args, 0, vm) == 1;
        return true;
    case 0xDE:
        vm.push_integer(replay_mode_);
        return true;
    case 0xE9:
        scene_.message_visible = false;
        wait_for(arg_i(args, 0, vm) * kFrameTime);
        return false;
    case 0xEA:
        scene_.message_visible = true;
        wait_for(arg_i(args, 0, vm) * kFrameTime);
        return false;
    case 0xEB:
        return true;
    default:
        warn_unsupported(function);
        return !is_false_yield(function);
    }
}

bool Runtime::is_active(const ScriptVm& vm) const {
    return !script_stack_.empty() && script_stack_.back().get() == &vm;
}

void Runtime::script_finished(ScriptVm& vm) {
    pending_finished_ = &vm;
}

void Runtime::apply_pending_script() {
    if (pending_script_.action == ScriptAction::none) return;
    PendingScript pending = std::move(pending_script_);
    pending_script_ = {};
    if (pending.action == ScriptAction::replace) script_stack_.clear();
    script_stack_.push_back(std::make_unique<ScriptVm>(archives_, *this, pending.name, pending.point));
    running_ = true;
}

void Runtime::apply_script_finish() {
    if (pending_finished_ == nullptr) return;
    if (!script_stack_.empty() && script_stack_.back().get() == pending_finished_) script_stack_.pop_back();
    pending_finished_ = nullptr;
    if (script_stack_.empty()) running_ = false;
}

void Runtime::wait_for(double seconds, WaitMode mode) {
    if (seconds <= 0.0 || skip()) {
        wait_mode_ = WaitMode::none;
        return;
    }
    wait_mode_ = mode;
    wait_until_ = now_ + seconds;
}

void Runtime::render_image(std::int32_t id, std::int32_t effect, bool preserve_characters,
                           std::int32_t type, std::int32_t frames, std::int32_t offset,
                           std::int32_t x, std::int32_t y, float scale_x, float scale_y) {
    const float transition_seconds = std::max(0, frames) * kFrameTime;
    begin_visual_transition(transition_seconds);
    if (id >= 0) {
        if (type == 1) {
            scene_.background.path = format_image("v%06d.tga", id);
            system_store_.set_cg_flag(static_cast<std::size_t>(id), 1);
        } else if (type == 2) {
            scene_.background.path = format_image("h%06d.tga", id);
        } else {
            scene_.background.path = format_image("B%04d%d%d.tga", id / 10, id % 10, time_mode_);
        }
    }
    scene_.background.mask_path = effect >= 128 ? format_image("f0%03d.bmp", effect & 0x7F) : "";
    scene_.visual_transition.mask_path = scene_.background.mask_path;
    scene_.background.offset = {static_cast<float>(x - offset), static_cast<float>(y)};
    scene_.background.scale = {scale_x, scale_y};
    scene_.background.transition_seconds = transition_seconds;
    scene_.background.preserve_characters = preserve_characters;
    if (!preserve_characters) scene_.characters.clear();
    wait_for(scene_.background.transition_seconds);
}

void Runtime::begin_visual_transition(float seconds) {
    scene_.visual_transition.active = seconds > 0.0F && !skip();
    scene_.visual_transition.start_seconds = now_;
    scene_.visual_transition.duration_seconds = std::max(0.0F, seconds);
    scene_.visual_transition.mask_path.clear();
    scene_.visual_transition.previous.background = scene_.background;
    scene_.visual_transition.previous.characters = scene_.characters;
    scene_.visual_transition.previous.sprites = scene_.sprites;
}

void Runtime::begin_color_transition(Color from, Color to, float seconds) {
    scene_.color_transition.active = seconds > 0.0F && !skip();
    scene_.color_transition.start_seconds = now_;
    scene_.color_transition.duration_seconds = std::max(0.0F, seconds);
    scene_.color_transition.from = from;
    scene_.color_transition.to = to;
    scene_.fade_color = to;
}

Color Runtime::current_fade_color() const {
    if (!scene_.color_transition.active || scene_.color_transition.duration_seconds <= 0.0F) {
        return scene_.fade_color;
    }
    const float progress = std::clamp(
        static_cast<float>((now_ - scene_.color_transition.start_seconds) /
                           scene_.color_transition.duration_seconds), 0.0F, 1.0F);
    const Color& from = scene_.color_transition.from;
    const Color& to = scene_.color_transition.to;
    return {from.r + (to.r - from.r) * progress,
            from.g + (to.g - from.g) * progress,
            from.b + (to.b - from.b) * progress,
            1.0F};
}

void Runtime::finish_transitions() {
    scene_.visual_transition.active = false;
    if (scene_.color_transition.active) scene_.fade_color = scene_.color_transition.to;
    scene_.color_transition.active = false;
}

void Runtime::refresh_character_paths() {
    for (auto& character : scene_.characters) {
        character.path = format_image((std::string(character_prefix(character.id)) + "%06d.tga").c_str(), character.image);
    }
}

void Runtime::add_character(std::int32_t id, std::int32_t image, std::int32_t position) {
    const auto found = std::find_if(scene_.characters.begin(), scene_.characters.end(), [position](const CharacterState& item) {
        return item.position == position;
    });
    CharacterState state{id, image, position, {}};
    if (found == scene_.characters.end()) scene_.characters.push_back(state);
    else *found = state;
    refresh_character_paths();
}

void Runtime::remove_character(std::int32_t id) {
    const auto found = std::find_if(scene_.characters.begin(), scene_.characters.end(), [id](const CharacterState& item) {
        return item.id == id;
    });
    if (found != scene_.characters.end()) scene_.characters.erase(found);
}

void Runtime::set_weather(const std::vector<Value>& args, ScriptVm& vm, bool extended) {
    scene_.weather.type = arg_i(args, 0, vm);
    scene_.weather.speed_x = arg_i(args, 1, vm);
    scene_.weather.speed_y = arg_i(args, 2, vm);
    scene_.weather.turbulence = extended ? arg_i(args, 6, vm) : 0;
    scene_.weather.count = arg_i(args, 3, vm);
    scene_.weather.flags = arg_i(args, 4, vm);
    scene_.weather.frame = arg_i(args, 5, vm);
}

void Runtime::warn_unsupported(std::uint32_t function) {
    std::ostringstream stream;
    stream << "unsupported script function 0x" << std::hex << function;
    const std::string warning = stream.str();
    if (std::find(warnings_.begin(), warnings_.end(), warning) == warnings_.end()) warnings_.push_back(warning);
}

std::int32_t Runtime::arg_i(const std::vector<Value>& args, std::size_t index, ScriptVm& vm,
                            std::int32_t fallback) const {
    return index < args.size() ? vm.integer(args[index]) : fallback;
}

float Runtime::arg_f(const std::vector<Value>& args, std::size_t index, ScriptVm& vm, float fallback) const {
    return index < args.size() ? vm.floating(args[index]) : fallback;
}

std::string Runtime::arg_s(const std::vector<Value>& args, std::size_t index, ScriptVm& vm) const {
    return index < args.size() ? vm.string(args[index]) : std::string{};
}

} // namespace wa2

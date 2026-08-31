#pragma once

#include "wa2/archive.hpp"
#include "wa2/system_store.hpp"
#include "wa2/vm.hpp"

#include <cstddef>
#include <cstdint>
#include <memory>
#include <string>
#include <unordered_map>
#include <vector>

namespace wa2 {

struct Vec2 {
    float x = 0.0F;
    float y = 0.0F;
};

struct Color {
    float r = 0.0F;
    float g = 0.0F;
    float b = 0.0F;
    float a = 1.0F;
};

struct BackgroundState {
    std::string path;
    Vec2 offset{};
    Vec2 scale{1.0F, 1.0F};
    std::string mask_path;
    float transition_seconds = 0.0F;
    bool preserve_characters = false;
};

struct CharacterState {
    std::int32_t id = -1;
    std::int32_t image = 0;
    std::int32_t position = 0;
    std::string path;
};

struct SpriteState {
    std::int32_t key = 0;
    std::string path;
    std::int32_t layer = 0;
    std::int32_t mode = 0;
    Vec2 position{};
    Vec2 offset{};
    Vec2 scale{1.0F, 1.0F};
    float alpha = 1.0F;
    float alpha_from = 1.0F;
    float alpha_to = 1.0F;
    double alpha_start_seconds = 0.0;
    float alpha_duration_seconds = 0.0F;
    bool alpha_transition_active = false;
    bool animated = false;
};

struct ChoiceState {
    std::string text;
    std::int32_t flag = 0;
    std::int32_t required_value = 0;
    std::int32_t value = 0;
    bool enabled = true;
};

struct CalendarState {
    std::int32_t year = 0;
    std::int32_t month = 0;
    std::int32_t day = 0;
    std::int32_t day_of_week = 0;
    bool visible = false;
};

struct WeatherState {
    std::int32_t type = -1;
    std::int32_t speed_x = 0;
    std::int32_t speed_y = 0;
    std::int32_t turbulence = 0;
    std::int32_t count = 0;
    std::int32_t flags = 0;
    std::int32_t frame = 0;
};

struct SceneSnapshot {
    BackgroundState background;
    std::vector<CharacterState> characters;
    std::unordered_map<std::int32_t, SpriteState> sprites;
};

struct VisualTransitionState {
    bool active = false;
    double start_seconds = 0.0;
    float duration_seconds = 0.0F;
    std::string mask_path;
    SceneSnapshot previous;
};

struct ColorTransitionState {
    bool active = false;
    double start_seconds = 0.0;
    float duration_seconds = 0.0F;
    Color from{0.5F, 0.5F, 0.5F, 1.0F};
    Color to{0.5F, 0.5F, 0.5F, 1.0F};
};

struct SceneState {
    BackgroundState background;
    std::vector<CharacterState> characters;
    std::unordered_map<std::int32_t, SpriteState> sprites;
    std::vector<ChoiceState> choices;
    CalendarState calendar;
    WeatherState weather;
    Color fade_color{0.5F, 0.5F, 0.5F, 1.0F};
    VisualTransitionState visual_transition;
    ColorTransitionState color_transition;
    std::string speaker;
    std::string message;
    bool message_visible = false;
    bool novel_mode = false;
};

class MediaSink {
public:
    virtual ~MediaSink() = default;
    virtual void play_bgm(std::int32_t id, bool loop, std::int32_t volume) = 0;
    virtual void stop_bgm(float fade_seconds) = 0;
    virtual void play_voice(std::int32_t label, std::int32_t id, std::int32_t character,
                            std::int32_t volume, bool loop, std::int32_t channel) = 0;
    virtual void stop_voice(std::int32_t channel, float fade_seconds) = 0;
    virtual double voice_remaining(std::int32_t channel) const = 0;
    virtual void play_se(std::int32_t channel, std::int32_t id, bool loop,
                         float fade_seconds, std::int32_t volume) = 0;
    virtual void stop_se(std::int32_t channel, float fade_seconds) = 0;
    virtual double se_remaining(std::int32_t channel) const = 0;
    virtual void stop_all() = 0;
    virtual bool play_movie(const std::string& path) = 0;
    virtual void stop_movie() = 0;
};

class NullMediaSink final : public MediaSink {
public:
    void play_bgm(std::int32_t, bool, std::int32_t) override {}
    void stop_bgm(float) override {}
    void play_voice(std::int32_t, std::int32_t, std::int32_t, std::int32_t, bool, std::int32_t) override {}
    void stop_voice(std::int32_t, float) override {}
    double voice_remaining(std::int32_t) const override { return 0.0; }
    void play_se(std::int32_t, std::int32_t, bool, float, std::int32_t) override {}
    void stop_se(std::int32_t, float) override {}
    double se_remaining(std::int32_t) const override { return 0.0; }
    void stop_all() override {}
    bool play_movie(const std::string&) override { return false; }
    void stop_movie() override {}
};

class Runtime final : public VmHost {
public:
    enum class Language { chinese, japanese };
    enum class WaitMode { none, click, timer, choice, movie };

    Runtime(std::string resource_root, std::string save_root, MediaSink& media);

    bool initialize(Language language, std::string* error = nullptr);
    bool start_script(const std::string& name, std::int32_t point = 0, std::string* error = nullptr);
    void tick(double now_seconds);
    void advance();
    bool choose(std::size_t index);
    void movie_finished();

    bool running() const noexcept { return running_; }
    bool at_title() const noexcept { return at_title_; }
    WaitMode wait_mode() const noexcept { return wait_mode_; }
    const SceneState& scene() const noexcept { return scene_; }
    SceneState& scene() noexcept { return scene_; }
    const std::vector<std::string>& warnings() const noexcept { return warnings_; }
    ArchiveIndex& archives() noexcept { return archives_; }
    const ArchiveIndex& archives() const noexcept { return archives_; }
    SystemStore& system_store() noexcept { return system_store_; }
    ScriptVm* active_script() noexcept;

    void set_skip(bool enabled) noexcept { skip_ = enabled; }
    bool skip() const noexcept { return skip_ && !skip_disabled_; }
    void set_auto_mode(bool enabled) noexcept { auto_mode_ = enabled; }
    bool auto_mode() const noexcept { return auto_mode_; }

    std::int32_t game_flag(std::size_t index) const override;
    void set_game_flag(std::size_t index, std::int32_t value) override;
    bool call_function(std::uint32_t function, std::vector<Value>& arguments, ScriptVm& vm) override;
    bool is_active(const ScriptVm& vm) const override;
    void script_finished(ScriptVm& vm) override;

private:
    enum class ScriptAction { none, replace, push };
    struct PendingScript {
        ScriptAction action = ScriptAction::none;
        std::string name;
        std::int32_t point = 0;
    };

    void apply_pending_script();
    void apply_script_finish();
    void wait_for(double seconds, WaitMode mode = WaitMode::timer);
    void begin_visual_transition(float seconds);
    void begin_color_transition(Color from, Color to, float seconds);
    Color current_fade_color() const;
    void finish_transitions();
    void render_image(std::int32_t id, std::int32_t effect, bool preserve_characters,
                      std::int32_t type, std::int32_t frames, std::int32_t offset,
                      std::int32_t x, std::int32_t y, float scale_x, float scale_y);
    void refresh_character_paths();
    void add_character(std::int32_t id, std::int32_t image, std::int32_t position);
    void remove_character(std::int32_t id);
    void set_weather(const std::vector<Value>& arguments, ScriptVm& vm, bool extended);
    void warn_unsupported(std::uint32_t function);

    std::int32_t arg_i(const std::vector<Value>& arguments, std::size_t index, ScriptVm& vm,
                       std::int32_t fallback = 0) const;
    float arg_f(const std::vector<Value>& arguments, std::size_t index, ScriptVm& vm,
                float fallback = 0.0F) const;
    std::string arg_s(const std::vector<Value>& arguments, std::size_t index, ScriptVm& vm) const;

    ArchiveIndex archives_;
    std::string save_root_;
    MediaSink& media_;
    SystemStore system_store_;
    Language language_ = Language::chinese;
    SceneState scene_;
    std::vector<std::int32_t> game_flags_ = std::vector<std::int32_t>(0x1D);
    std::vector<std::unique_ptr<ScriptVm>> script_stack_;
    std::vector<std::string> warnings_;
    PendingScript pending_script_;
    ScriptVm* pending_finished_ = nullptr;
    WaitMode wait_mode_ = WaitMode::none;
    double now_ = 0.0;
    double wait_until_ = 0.0;
    double timer_start_ = 0.0;
    bool running_ = false;
    bool at_title_ = true;
    bool skip_ = false;
    bool skip_disabled_ = false;
    bool auto_mode_ = false;
    bool demo_mode_ = false;
    bool ero_mode_ = false;
    std::int32_t replay_mode_ = 0;
    std::int32_t label_ = -1;
    std::int32_t time_mode_ = 0;
    std::int32_t current_message_ = 0;
    std::size_t choice_target_ = 0;
    bool choice_has_target_ = false;
    std::string effect_mode_;
};

} // namespace wa2

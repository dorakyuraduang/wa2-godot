#include "wa2/archive.hpp"
#include "wa2/runtime.hpp"
#include "wa2/sdl_frontend.hpp"

#include <SDL.h>

#include <algorithm>
#include <cerrno>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <optional>
#include <sstream>
#include <string>
#include <vector>

#ifdef _WIN32
#include <direct.h>
#else
#include <sys/stat.h>
#include <sys/types.h>
#endif

namespace {

struct Chapter {
    const char* title;
    const char* script;
};

constexpr Chapter kChapters[] = {
    {"Introductory Chapter", "1001"},
    {"Closing Chapter", "2001"},
    {"Coda", "3001"},
    {"Digital Novel 1", "5000"},
    {"Digital Novel 2", "5100"},
};

struct Options {
#ifdef __SWITCH__
    std::string resource_root = "sdmc:/switch/wa2-cpp/Wa2Res";
    std::string save_root = "sdmc:/switch/wa2-cpp/sav";
    std::string font_path = "sdmc:/switch/wa2-cpp/font.ttf";
#else
    std::string resource_root = "Wa2Res";
    std::string save_root = "sav";
    std::string font_path;
#endif
    std::string script;
    std::int32_t point = 0;
    wa2::Runtime::Language language = wa2::Runtime::Language::chinese;
    bool help = false;
    bool trace = false;
};

bool file_exists(const std::string& path) {
    std::ifstream stream(path, std::ios::binary);
    return static_cast<bool>(stream);
}

bool create_one_directory(const std::string& path) {
    if (path.empty()) return true;
#ifdef _WIN32
    return _mkdir(path.c_str()) == 0 || errno == EEXIST;
#else
    return mkdir(path.c_str(), 0755) == 0 || errno == EEXIST;
#endif
}

bool create_directories(std::string path) {
    std::replace(path.begin(), path.end(), '\\', '/');
    while (path.size() > 1 && path.back() == '/') path.pop_back();
    for (std::size_t i = 1; i <= path.size(); ++i) {
        if (i != path.size() && path[i] != '/') continue;
        const std::string part = path.substr(0, i);
        if (part.empty() || part.back() == ':') continue;
        if (!create_one_directory(part)) return false;
    }
    return true;
}

bool parse_i32(const char* text, std::int32_t* value) {
    if (text == nullptr || *text == '\0') return false;
    char* end = nullptr;
    const long parsed = std::strtol(text, &end, 10);
    if (*end != '\0') return false;
    *value = static_cast<std::int32_t>(parsed);
    return true;
}

bool parse_options(int argc, char** argv, Options* options, std::string* error) {
    for (int i = 1; i < argc; ++i) {
        const std::string argument = argv[i];
        auto next = [&]() -> const char* {
            if (i + 1 >= argc) return nullptr;
            return argv[++i];
        };
        if (argument == "--help" || argument == "-h") options->help = true;
        else if (argument == "--trace") options->trace = true;
        else if (argument == "--res") {
            const char* value = next();
            if (value == nullptr) { *error = "--res requires a path"; return false; }
            options->resource_root = value;
        } else if (argument == "--save") {
            const char* value = next();
            if (value == nullptr) { *error = "--save requires a path"; return false; }
            options->save_root = value;
        } else if (argument == "--font") {
            const char* value = next();
            if (value == nullptr) { *error = "--font requires a path"; return false; }
            options->font_path = value;
        } else if (argument == "--script") {
            const char* value = next();
            if (value == nullptr) { *error = "--script requires a name"; return false; }
            options->script = value;
        } else if (argument == "--point") {
            const char* value = next();
            if (!parse_i32(value, &options->point)) { *error = "--point requires an integer"; return false; }
        } else if (argument == "--lang") {
            const char* value = next();
            if (value == nullptr) { *error = "--lang requires cn or jp"; return false; }
            const std::string language = value;
            if (language == "cn") options->language = wa2::Runtime::Language::chinese;
            else if (language == "jp") options->language = wa2::Runtime::Language::japanese;
            else { *error = "--lang must be cn or jp"; return false; }
        } else {
            *error = "unknown option: " + argument;
            return false;
        }
    }
    return true;
}

void print_help() {
    std::cout
        << "Usage: wa2_cpp [options]\n"
        << "  --res PATH       Resource directory containing the original PAK files\n"
        << "  --save PATH      Writable save directory\n"
        << "  --font PATH      TTF/OTF font with Chinese and Japanese glyphs\n"
        << "  --lang cn|jp     Use ck-gal.pak or script.pak (default: cn)\n"
        << "  --script NAME    Start a script directly instead of showing the chapter menu\n"
        << "  --point NUMBER   Script point used with --script (default: 0)\n"
        << "  --trace          Print runtime state changes to stderr\n";
}

void install_title_choices(wa2::Runtime& runtime) {
    auto& choices = runtime.scene().choices;
    if (!runtime.at_title() || choices.size() == sizeof(kChapters) / sizeof(kChapters[0])) return;
    choices.clear();
    for (const Chapter& chapter : kChapters) choices.push_back({chapter.title, 0, 0, 0, true});
}

double clock_seconds() {
    return static_cast<double>(SDL_GetPerformanceCounter()) /
           static_cast<double>(SDL_GetPerformanceFrequency());
}

std::optional<std::size_t> choice_at(wa2::Runtime& runtime, SDL_Renderer* renderer,
                                     int window_x, int window_y) {
    const auto& choices = runtime.scene().choices;
    if (choices.empty()) return std::nullopt;
    float x = static_cast<float>(window_x);
    float y = static_cast<float>(window_y);
    SDL_RenderWindowToLogical(renderer, window_x, window_y, &x, &y);
    constexpr int item_x = 220;
    constexpr int item_width = 840;
    constexpr int item_height = 64;
    constexpr int item_stride = 76;
    const int top = runtime.at_title() ? 190 : 250 - static_cast<int>(choices.size()) * 5;
    if (x < item_x || x >= item_x + item_width || y < top) return std::nullopt;
    const int relative_y = static_cast<int>(y) - top;
    const std::size_t index = static_cast<std::size_t>(relative_y / item_stride);
    if (index >= choices.size() || relative_y % item_stride >= item_height) return std::nullopt;
    return index;
}

const char* wait_mode_name(wa2::Runtime::WaitMode mode) {
    switch (mode) {
    case wa2::Runtime::WaitMode::none: return "none";
    case wa2::Runtime::WaitMode::click: return "click";
    case wa2::Runtime::WaitMode::timer: return "timer";
    case wa2::Runtime::WaitMode::choice: return "choice";
    case wa2::Runtime::WaitMode::movie: return "movie";
    }
    return "unknown";
}

std::string runtime_trace(wa2::Runtime& runtime) {
    const auto& scene = runtime.scene();
    std::ostringstream output;
    output << "running=" << runtime.running()
           << " title=" << runtime.at_title()
           << " wait=" << wait_mode_name(runtime.wait_mode());
    if (const wa2::ScriptVm* script = runtime.active_script()) {
        output << " script='" << script->name() << "'";
    }
    output << " bg='" << scene.background.path << "'"
           << " transition=" << scene.visual_transition.active;
    if (scene.visual_transition.active && !scene.visual_transition.mask_path.empty()) {
        output << " mask='" << scene.visual_transition.mask_path << "'";
    }
    output << " color_transition=" << scene.color_transition.active
           << " message=" << scene.message_visible
           << " choices=" << scene.choices.size()
           << " calendar=" << scene.calendar.visible;
    return output.str();
}

} // namespace

int main(int argc, char** argv) {
    Options options;
    std::string error;
    if (!parse_options(argc, argv, &options, &error)) {
        std::cerr << "wa2-cpp: " << error << '\n';
        print_help();
        return 2;
    }
    if (options.help) {
        print_help();
        return 0;
    }

#ifdef SDL_MAIN_HANDLED
    SDL_SetMainReady();
#endif

#ifndef __SWITCH__
    if (options.font_path.empty()) {
        const std::vector<std::string> candidates{
            wa2::join_path(options.resource_root, "font.ttf"), "font.ttf",
#ifdef _WIN32
            "C:/Windows/Fonts/msyh.ttc", "C:/Windows/Fonts/msgothic.ttc",
#endif
        };
        for (const std::string& candidate : candidates) {
            if (file_exists(candidate)) { options.font_path = candidate; break; }
        }
    }
#endif

    if (!create_directories(options.save_root)) {
        std::cerr << "wa2-cpp: cannot create save directory: " << options.save_root << '\n';
        return 1;
    }

    wa2::SdlFrontend frontend;
    if (!frontend.initialize("WA2 C++ Runtime", options.font_path, &error)) {
        std::cerr << "wa2-cpp: SDL initialization failed: " << error << '\n';
        return 1;
    }

    wa2::Runtime runtime(options.resource_root, options.save_root, frontend);
    frontend.attach(runtime);
    if (!runtime.initialize(options.language, &error)) {
        std::cerr << "wa2-cpp: runtime initialization failed: " << error << '\n';
        SDL_ShowSimpleMessageBox(SDL_MESSAGEBOX_ERROR, "WA2 C++ Runtime", error.c_str(), frontend.window());
        return 1;
    }
    for (const std::string& warning : runtime.warnings()) std::cerr << "wa2-cpp: warning: " << warning << '\n';

    if (!options.script.empty() && !runtime.start_script(options.script, options.point, &error)) {
        std::cerr << "wa2-cpp: cannot start script: " << error << '\n';
        return 1;
    }
    install_title_choices(runtime);

    SDL_GameController* controller = nullptr;
    for (int i = 0; i < SDL_NumJoysticks(); ++i) {
        if (SDL_IsGameController(i)) { controller = SDL_GameControllerOpen(i); break; }
    }

    bool quit = false;
    bool skip_held = false;
    std::size_t selected = 0;
    double next_skip = 0.0;
    double next_auto = 0.0;
    std::string previous_trace;
    while (!quit) {
        bool activate = false;
        SDL_Event event{};
        while (SDL_PollEvent(&event)) {
            if (event.type == SDL_QUIT) quit = true;
            else if (event.type == SDL_CONTROLLERDEVICEADDED && controller == nullptr && SDL_IsGameController(event.cdevice.which)) {
                controller = SDL_GameControllerOpen(event.cdevice.which);
            } else if (event.type == SDL_CONTROLLERDEVICEREMOVED && controller != nullptr &&
                       SDL_JoystickInstanceID(SDL_GameControllerGetJoystick(controller)) == event.cdevice.which) {
                SDL_GameControllerClose(controller);
                controller = nullptr;
            } else if (event.type == SDL_KEYDOWN && event.key.repeat == 0) {
                const SDL_Keycode key = event.key.keysym.sym;
                if (key == SDLK_ESCAPE) quit = true;
                else if (key == SDLK_RETURN || key == SDLK_SPACE) activate = true;
                else if (key == SDLK_UP && selected > 0) --selected;
                else if (key == SDLK_DOWN) ++selected;
                else if (key == SDLK_x) runtime.set_auto_mode(!runtime.auto_mode());
                else if (key == SDLK_TAB || key == SDLK_LCTRL || key == SDLK_RCTRL) skip_held = true;
            } else if (event.type == SDL_KEYUP) {
                const SDL_Keycode key = event.key.keysym.sym;
                if (key == SDLK_TAB || key == SDLK_LCTRL || key == SDLK_RCTRL) skip_held = false;
            } else if (event.type == SDL_CONTROLLERBUTTONDOWN) {
                if (event.cbutton.button == SDL_CONTROLLER_BUTTON_A) activate = true;
                else if (event.cbutton.button == SDL_CONTROLLER_BUTTON_B) quit = true;
                else if (event.cbutton.button == SDL_CONTROLLER_BUTTON_DPAD_UP && selected > 0) --selected;
                else if (event.cbutton.button == SDL_CONTROLLER_BUTTON_DPAD_DOWN) ++selected;
                else if (event.cbutton.button == SDL_CONTROLLER_BUTTON_X) runtime.set_auto_mode(!runtime.auto_mode());
                else if (event.cbutton.button == SDL_CONTROLLER_BUTTON_LEFTSHOULDER) skip_held = true;
            } else if (event.type == SDL_CONTROLLERBUTTONUP &&
                       event.cbutton.button == SDL_CONTROLLER_BUTTON_LEFTSHOULDER) {
                skip_held = false;
            } else if (event.type == SDL_MOUSEBUTTONDOWN && event.button.button == SDL_BUTTON_LEFT) {
                const auto hit = choice_at(runtime, frontend.renderer(), event.button.x, event.button.y);
                if (hit) {
                    selected = *hit;
                    activate = true;
                } else if (!runtime.at_title()) {
                    activate = true;
                }
            } else if (event.type == SDL_MOUSEMOTION) {
                const auto hit = choice_at(runtime, frontend.renderer(), event.motion.x, event.motion.y);
                if (hit) selected = *hit;
            }
        }

        install_title_choices(runtime);
        const std::size_t count = runtime.scene().choices.size();
        if (count > 0) selected = std::min(selected, count - 1);
        else selected = 0;

        if (activate) {
            if (runtime.at_title()) {
                const std::size_t chapter = std::min(selected, sizeof(kChapters) / sizeof(kChapters[0]) - 1);
                if (!runtime.start_script(kChapters[chapter].script, 0, &error)) {
                    std::cerr << "wa2-cpp: cannot start script: " << error << '\n';
                }
            } else if (runtime.wait_mode() == wa2::Runtime::WaitMode::choice) {
                runtime.choose(selected);
            } else {
                runtime.advance();
            }
            next_auto = 0.0;
        }

        const double now = clock_seconds();
        runtime.set_skip(skip_held);
        if (skip_held && now >= next_skip) {
            runtime.advance();
            next_skip = now + 0.04;
        }
        if (runtime.auto_mode() && runtime.wait_mode() == wa2::Runtime::WaitMode::click) {
            if (next_auto == 0.0) next_auto = now + 1.8;
            if (now >= next_auto) { runtime.advance(); next_auto = 0.0; }
        } else if (runtime.wait_mode() != wa2::Runtime::WaitMode::click) {
            next_auto = 0.0;
        }

        runtime.tick(now);
        if (options.trace) {
            const std::string trace = runtime_trace(runtime);
            if (trace != previous_trace) {
                std::cerr << "wa2-cpp: state: " << trace << '\n';
                previous_trace = trace;
            }
        }
        frontend.render(now, selected);
        SDL_Delay(1);
    }

    if (controller != nullptr) SDL_GameControllerClose(controller);
    if (!runtime.system_store().flush(&error)) std::cerr << "wa2-cpp: save warning: " << error << '\n';
    return 0;
}

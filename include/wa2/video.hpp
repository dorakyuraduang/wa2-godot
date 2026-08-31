#pragma once

#include <memory>
#include <string>

struct SDL_Renderer;

namespace wa2 {

class VideoPlayer {
public:
    VideoPlayer();
    ~VideoPlayer();
    VideoPlayer(const VideoPlayer&) = delete;
    VideoPlayer& operator=(const VideoPlayer&) = delete;

    bool open(SDL_Renderer* renderer, const std::string& path, std::string* error = nullptr);
    void update(double now_seconds);
    void render(SDL_Renderer* renderer, int width, int height);
    void stop();
    bool playing() const;
    bool finished() const;

private:
    struct Impl;
    std::unique_ptr<Impl> impl_;
};

} // namespace wa2

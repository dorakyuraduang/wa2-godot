#include "wa2/video.hpp"

#include <SDL.h>

#ifdef WA2_WITH_FFMPEG
extern "C" {
#include <libavcodec/avcodec.h>
#include <libavformat/avformat.h>
#include <libavutil/imgutils.h>
#include <libswscale/swscale.h>
}
#endif

#include <cstdint>
#include <string>

namespace wa2 {

struct VideoPlayer::Impl {
    bool playing = false;
    bool ended = false;
#ifdef WA2_WITH_FFMPEG
    AVFormatContext* format = nullptr;
    AVCodecContext* codec = nullptr;
    AVFrame* frame = nullptr;
    AVFrame* rgba = nullptr;
    AVPacket* packet = nullptr;
    SwsContext* scaler = nullptr;
    SDL_Texture* texture = nullptr;
    int stream = -1;
    int width = 0;
    int height = 0;
    double start_time = 0.0;
    double next_pts = 0.0;
#endif
};

VideoPlayer::VideoPlayer() : impl_(std::make_unique<Impl>()) {}
VideoPlayer::~VideoPlayer() { stop(); }

bool VideoPlayer::open(SDL_Renderer* renderer, const std::string& path, std::string* error) {
    stop();
#ifndef WA2_WITH_FFMPEG
    (void)renderer;
    (void)path;
    if (error != nullptr) *error = "movie support was built without FFmpeg";
    return false;
#else
    if (avformat_open_input(&impl_->format, path.c_str(), nullptr, nullptr) < 0 ||
        avformat_find_stream_info(impl_->format, nullptr) < 0) {
        if (error != nullptr) *error = "FFmpeg could not open movie: " + path;
        stop();
        return false;
    }
    impl_->stream = av_find_best_stream(impl_->format, AVMEDIA_TYPE_VIDEO, -1, -1, nullptr, 0);
    if (impl_->stream < 0) {
        if (error != nullptr) *error = "movie has no video stream";
        stop();
        return false;
    }
    const AVCodecParameters* parameters = impl_->format->streams[impl_->stream]->codecpar;
    const AVCodec* decoder = avcodec_find_decoder(parameters->codec_id);
    if (decoder == nullptr) {
        if (error != nullptr) *error = "FFmpeg decoder is unavailable";
        stop();
        return false;
    }
    impl_->codec = avcodec_alloc_context3(decoder);
    if (impl_->codec == nullptr || avcodec_parameters_to_context(impl_->codec, parameters) < 0 ||
        avcodec_open2(impl_->codec, decoder, nullptr) < 0) {
        if (error != nullptr) *error = "FFmpeg decoder initialization failed";
        stop();
        return false;
    }
    impl_->width = impl_->codec->width;
    impl_->height = impl_->codec->height;
    impl_->frame = av_frame_alloc();
    impl_->rgba = av_frame_alloc();
    impl_->packet = av_packet_alloc();
    impl_->scaler = sws_getContext(impl_->width, impl_->height, impl_->codec->pix_fmt,
                                   impl_->width, impl_->height, AV_PIX_FMT_RGBA,
                                   SWS_BILINEAR, nullptr, nullptr, nullptr);
    impl_->texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGBA32, SDL_TEXTUREACCESS_STREAMING,
                                       impl_->width, impl_->height);
    if (impl_->frame == nullptr || impl_->rgba == nullptr || impl_->packet == nullptr ||
        impl_->scaler == nullptr || impl_->texture == nullptr) {
        if (error != nullptr) *error = "movie frame allocation failed";
        stop();
        return false;
    }
    impl_->playing = true;
    impl_->ended = false;
    impl_->start_time = SDL_GetTicks64() / 1000.0;
    impl_->next_pts = 0.0;
    return true;
#endif
}

void VideoPlayer::update(double now_seconds) {
#ifdef WA2_WITH_FFMPEG
    if (!impl_->playing || now_seconds - impl_->start_time + 0.05 < impl_->next_pts) return;
    while (av_read_frame(impl_->format, impl_->packet) >= 0) {
        if (impl_->packet->stream_index != impl_->stream) {
            av_packet_unref(impl_->packet);
            continue;
        }
        const int sent = avcodec_send_packet(impl_->codec, impl_->packet);
        av_packet_unref(impl_->packet);
        if (sent < 0) continue;
        if (avcodec_receive_frame(impl_->codec, impl_->frame) < 0) continue;

        void* pixels = nullptr;
        int pitch = 0;
        if (SDL_LockTexture(impl_->texture, nullptr, &pixels, &pitch) == 0) {
            std::uint8_t* target[] = {static_cast<std::uint8_t*>(pixels), nullptr, nullptr, nullptr};
            int target_lines[] = {pitch, 0, 0, 0};
            sws_scale(impl_->scaler, impl_->frame->data, impl_->frame->linesize, 0,
                      impl_->height, target, target_lines);
            SDL_UnlockTexture(impl_->texture);
        }
        if (impl_->frame->best_effort_timestamp != AV_NOPTS_VALUE) {
            impl_->next_pts = impl_->frame->best_effort_timestamp *
                              av_q2d(impl_->format->streams[impl_->stream]->time_base);
        } else {
            impl_->next_pts += 1.0 / 30.0;
        }
        return;
    }
    impl_->playing = false;
    impl_->ended = true;
#else
    (void)now_seconds;
#endif
}

void VideoPlayer::render(SDL_Renderer* renderer, int width, int height) {
#ifdef WA2_WITH_FFMPEG
    if (impl_->texture == nullptr) return;
    SDL_Rect destination{0, 0, width, height};
    SDL_RenderCopy(renderer, impl_->texture, nullptr, &destination);
#else
    (void)renderer; (void)width; (void)height;
#endif
}

void VideoPlayer::stop() {
#ifdef WA2_WITH_FFMPEG
    if (impl_->texture != nullptr) SDL_DestroyTexture(impl_->texture);
    if (impl_->scaler != nullptr) sws_freeContext(impl_->scaler);
    if (impl_->packet != nullptr) av_packet_free(&impl_->packet);
    if (impl_->rgba != nullptr) av_frame_free(&impl_->rgba);
    if (impl_->frame != nullptr) av_frame_free(&impl_->frame);
    if (impl_->codec != nullptr) avcodec_free_context(&impl_->codec);
    if (impl_->format != nullptr) avformat_close_input(&impl_->format);
    impl_->texture = nullptr;
    impl_->scaler = nullptr;
    impl_->stream = -1;
#endif
    impl_->playing = false;
}

bool VideoPlayer::playing() const { return impl_->playing; }
bool VideoPlayer::finished() const { return impl_->ended; }

} // namespace wa2

#include "wa2/text.hpp"

#include "wa2/error.hpp"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#elif defined(WA2_WITH_SDL)
#include <SDL.h>
#else
#include <cerrno>
#include <iconv.h>
#endif

#include <cstdlib>

namespace wa2 {

std::string decode_cp932(const std::vector<std::uint8_t>& bytes) {
    if (bytes.empty()) {
        return {};
    }
#ifdef _WIN32
    const auto* input = reinterpret_cast<const char*>(bytes.data());
    const int input_size = static_cast<int>(bytes.size());
    const int wide_size = MultiByteToWideChar(932, 0, input, input_size, nullptr, 0);
    if (wide_size <= 0) {
        throw Error("CP932 conversion failed");
    }
    std::wstring wide(static_cast<std::size_t>(wide_size), L'\0');
    MultiByteToWideChar(932, 0, input, input_size, wide.data(), wide_size);
    const int utf8_size = WideCharToMultiByte(CP_UTF8, 0, wide.data(), wide_size, nullptr, 0, nullptr, nullptr);
    if (utf8_size <= 0) {
        throw Error("UTF-8 conversion failed");
    }
    std::string result(static_cast<std::size_t>(utf8_size), '\0');
    WideCharToMultiByte(CP_UTF8, 0, wide.data(), wide_size, result.data(), utf8_size, nullptr, nullptr);
    return result;
#elif defined(WA2_WITH_SDL)
    char* converted = SDL_iconv_string("UTF-8", "CP932", reinterpret_cast<const char*>(bytes.data()), bytes.size());
    if (converted == nullptr) {
        throw Error("SDL CP932 conversion failed");
    }
    std::string result(converted);
    SDL_free(converted);
    return result;
#else
    iconv_t converter = iconv_open("UTF-8", "CP932");
    if (converter == reinterpret_cast<iconv_t>(-1)) {
        converter = iconv_open("UTF-8", "SHIFT-JIS");
    }
    if (converter == reinterpret_cast<iconv_t>(-1)) {
        throw Error("iconv does not provide CP932");
    }
    std::string output(bytes.size() * 4 + 4, '\0');
    auto* input = const_cast<char*>(reinterpret_cast<const char*>(bytes.data()));
    std::size_t input_left = bytes.size();
    char* target = output.data();
    std::size_t output_left = output.size();
    if (iconv(converter, &input, &input_left, &target, &output_left) == static_cast<std::size_t>(-1)) {
        iconv_close(converter);
        throw Error("iconv CP932 conversion failed");
    }
    iconv_close(converter);
    output.resize(output.size() - output_left);
    return output;
#endif
}

std::vector<std::string> split_script_text(const std::string& text) {
    std::vector<std::string> result;
    std::size_t begin = 0;
    while (begin <= text.size()) {
        const std::size_t comma = text.find(',', begin);
        if (comma == std::string::npos) {
            result.emplace_back(text.substr(begin));
            break;
        }
        result.emplace_back(text.substr(begin, comma - begin));
        begin = comma + 1;
    }
    return result;
}

} // namespace wa2

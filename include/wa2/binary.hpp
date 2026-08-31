#pragma once

#include "wa2/error.hpp"

#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <cstring>
#include <string>
#include <vector>

namespace wa2 {

class BinaryReader {
public:
    explicit BinaryReader(const std::vector<std::uint8_t>& bytes) : bytes_(bytes) {}

    std::size_t position() const noexcept { return position_; }
    std::size_t remaining() const noexcept { return bytes_.size() - position_; }
    bool can_read(std::size_t count) const noexcept { return count <= remaining(); }

    void seek(std::size_t position) {
        if (position > bytes_.size()) {
            throw Error("binary seek outside buffer");
        }
        position_ = position;
    }

    void skip(std::size_t count) {
        require(count);
        position_ += count;
    }

    std::uint8_t u8() {
        require(1);
        return bytes_[position_++];
    }

    std::uint32_t u32() {
        require(4);
        const auto* p = bytes_.data() + position_;
        position_ += 4;
        return static_cast<std::uint32_t>(p[0]) |
               (static_cast<std::uint32_t>(p[1]) << 8U) |
               (static_cast<std::uint32_t>(p[2]) << 16U) |
               (static_cast<std::uint32_t>(p[3]) << 24U);
    }

    std::int32_t i32() { return static_cast<std::int32_t>(u32()); }

    float f32() {
        const std::uint32_t bits = u32();
        float value = 0.0F;
        std::memcpy(&value, &bits, sizeof(value));
        return value;
    }

    std::vector<std::uint8_t> bytes(std::size_t count) {
        require(count);
        auto begin = bytes_.begin() + static_cast<std::ptrdiff_t>(position_);
        position_ += count;
        return {begin, begin + static_cast<std::ptrdiff_t>(count)};
    }

    std::string ascii(std::size_t count) {
        auto value = bytes(count);
        const auto nul = std::find(value.begin(), value.end(), std::uint8_t{0});
        return {value.begin(), nul};
    }

private:
    void require(std::size_t count) const {
        if (!can_read(count)) {
            throw Error("truncated binary data");
        }
    }

    const std::vector<std::uint8_t>& bytes_;
    std::size_t position_ = 0;
};

inline std::uint32_t read_u32_at(const std::vector<std::uint8_t>& bytes, std::size_t offset) {
    if (offset > bytes.size() || bytes.size() - offset < 4) {
        throw Error("truncated 32-bit value");
    }
    return static_cast<std::uint32_t>(bytes[offset]) |
           (static_cast<std::uint32_t>(bytes[offset + 1]) << 8U) |
           (static_cast<std::uint32_t>(bytes[offset + 2]) << 16U) |
           (static_cast<std::uint32_t>(bytes[offset + 3]) << 24U);
}

} // namespace wa2

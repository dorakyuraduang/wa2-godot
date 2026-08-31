#include "wa2/system_store.hpp"

#include "wa2/error.hpp"

#include <algorithm>
#include <cstdio>
#include <fstream>

namespace wa2 {
namespace {

constexpr std::size_t kCgOffset = 0x80000;
constexpr std::size_t kFlagOffset = 0x268480;
constexpr std::size_t kReadBytesPerScript = 512;

std::uint32_t read_u32(const std::vector<std::uint8_t>& bytes, std::size_t offset) {
    if (offset > bytes.size() || bytes.size() - offset < 4) throw Error("system store read outside file");
    return static_cast<std::uint32_t>(bytes[offset]) |
           (static_cast<std::uint32_t>(bytes[offset + 1]) << 8U) |
           (static_cast<std::uint32_t>(bytes[offset + 2]) << 16U) |
           (static_cast<std::uint32_t>(bytes[offset + 3]) << 24U);
}

void write_u32(std::vector<std::uint8_t>& bytes, std::size_t offset, std::uint32_t value) {
    if (offset > bytes.size() || bytes.size() - offset < 4) throw Error("system store write outside file");
    bytes[offset] = static_cast<std::uint8_t>(value);
    bytes[offset + 1] = static_cast<std::uint8_t>(value >> 8U);
    bytes[offset + 2] = static_cast<std::uint8_t>(value >> 16U);
    bytes[offset + 3] = static_cast<std::uint8_t>(value >> 24U);
}

} // namespace

bool SystemStore::load_or_create(const std::string& path, std::string* error) {
    try {
        path_ = path;
        std::ifstream stream(path, std::ios::binary | std::ios::ate);
        if (!stream) {
            bytes_.assign(kStoreSize, 0);
            if (!flush(error)) return false;
            return true;
        }
        const auto size = stream.tellg();
        if (size < 0) throw Error("cannot determine system store size");
        stream.seekg(0);
        bytes_.assign(std::max<std::size_t>(kStoreSize, static_cast<std::size_t>(size)), 0);
        if (size > 0 && !stream.read(reinterpret_cast<char*>(bytes_.data()), size)) {
            throw Error("cannot read system store");
        }
        return true;
    } catch (const std::exception& ex) {
        if (error != nullptr) *error = ex.what();
        return false;
    }
}

bool SystemStore::flush(std::string* error) const {
    try {
        if (path_.empty()) throw Error("system store path is empty");
        const std::string temporary = path_ + ".tmp";
        {
            std::ofstream stream(temporary, std::ios::binary | std::ios::trunc);
            if (!stream) throw Error("cannot create temporary system store");
            if (!bytes_.empty()) {
                stream.write(reinterpret_cast<const char*>(bytes_.data()), static_cast<std::streamsize>(bytes_.size()));
            }
            if (!stream) throw Error("cannot write system store");
        }
        std::remove(path_.c_str());
        if (std::rename(temporary.c_str(), path_.c_str()) != 0) {
            std::remove(temporary.c_str());
            throw Error("cannot replace system store");
        }
        return true;
    } catch (const std::exception& ex) {
        if (error != nullptr) *error = ex.what();
        return false;
    }
}

std::int32_t SystemStore::flag(std::size_t index) const {
    return static_cast<std::int32_t>(read_u32(bytes_, kFlagOffset + index * 4U));
}

void SystemStore::set_flag(std::size_t index, std::int32_t value) {
    write_u32(bytes_, kFlagOffset + index * 4U, static_cast<std::uint32_t>(value));
}

std::uint8_t SystemStore::cg_flag(std::size_t index) const {
    if (kCgOffset + index >= bytes_.size()) throw Error("CG flag index outside system store");
    return bytes_[kCgOffset + index];
}

void SystemStore::set_cg_flag(std::size_t index, std::uint8_t value) {
    if (kCgOffset + index >= bytes_.size()) throw Error("CG flag index outside system store");
    bytes_[kCgOffset + index] = value;
}

bool SystemStore::message_read(std::size_t script_index, std::size_t message_index) const {
    if (message_index >= 4096) return false;
    const std::size_t offset = script_index * kReadBytesPerScript + message_index / 8U;
    if (offset >= bytes_.size()) return false;
    const std::size_t bit = 7U - message_index % 8U;
    return ((bytes_[offset] >> bit) & 1U) != 0;
}

void SystemStore::set_message_read(std::size_t script_index, std::size_t message_index) {
    if (message_index >= 4096) return;
    const std::size_t offset = script_index * kReadBytesPerScript + message_index / 8U;
    if (offset >= bytes_.size()) return;
    const std::size_t bit = 7U - message_index % 8U;
    bytes_[offset] |= static_cast<std::uint8_t>(1U << bit);
}

} // namespace wa2

#include "wa2/archive.hpp"

#include "wa2/binary.hpp"
#include "wa2/error.hpp"

#include <algorithm>
#include <array>
#include <cctype>
#include <fstream>
#include <limits>

namespace wa2 {
namespace {

constexpr std::uint32_t kPackMagic = 0x5041434BU;
constexpr std::uint32_t kLacMagic = 0x0043414CU;
constexpr std::size_t kMaxDecodedFile = 512U * 1024U * 1024U;

std::string archive_name(const std::vector<std::uint8_t>& raw, bool invert) {
    std::string result;
    result.reserve(raw.size());
    for (std::uint8_t byte : raw) {
        if (byte == 0) {
            break;
        }
        if (invert) {
            byte = static_cast<std::uint8_t>(~byte);
        }
        // Resource names used by the runtime are ASCII even though the archive field is CP932.
        result.push_back(static_cast<char>(byte));
    }
    return ArchiveIndex::normalized_name(std::move(result));
}

bool range_inside(std::uint64_t offset, std::uint64_t size, std::uint64_t total) {
    return offset <= total && size <= total - offset;
}

std::uint32_t u32_at(const std::uint8_t* bytes) {
    return static_cast<std::uint32_t>(bytes[0]) |
           (static_cast<std::uint32_t>(bytes[1]) << 8U) |
           (static_cast<std::uint32_t>(bytes[2]) << 16U) |
           (static_cast<std::uint32_t>(bytes[3]) << 24U);
}

void read_exact(std::ifstream& stream, std::uint8_t* target, std::size_t size,
                const std::string& path) {
    if (size != 0 && !stream.read(reinterpret_cast<char*>(target), static_cast<std::streamsize>(size))) {
        throw Error("archive table is truncated: " + path);
    }
}

} // namespace

ArchiveIndex::ArchiveIndex(std::string resource_root) : root_(std::move(resource_root)) {}

void ArchiveIndex::set_resource_root(std::string resource_root) {
    root_ = std::move(resource_root);
    clear();
}

bool ArchiveIndex::load_archive(const std::string& relative_path, std::string* error) {
    try {
        const auto full_path = join_path(root_, relative_path);
        std::ifstream stream(full_path, std::ios::binary | std::ios::ate);
        if (!stream) throw Error("cannot open archive: " + full_path);
        const auto end = stream.tellg();
        if (end < 0) throw Error("cannot determine archive size: " + full_path);
        const std::uint64_t total_size = static_cast<std::uint64_t>(end);
        stream.seekg(0);

        std::array<std::uint8_t, 16> header{};
        read_exact(stream, header.data(), 8, full_path);
        const std::uint32_t magic = u32_at(header.data());

        if (magic == kPackMagic) {
            read_exact(stream, header.data() + 8, 8, full_path);
            const std::uint32_t count = u32_at(header.data() + 12);
            constexpr std::size_t header_size = 16;
            constexpr std::size_t entry_size = 44;
            if (total_size < header_size || count > (total_size - header_size) / entry_size) {
                throw Error("PACK entry table is truncated: " + full_path);
            }

            std::array<std::uint8_t, entry_size> entry{};
            for (std::uint32_t i = 0; i < count; ++i) {
                stream.seekg(static_cast<std::streamoff>(header_size + static_cast<std::size_t>(i) * entry_size));
                read_exact(stream, entry.data(), entry.size(), full_path);
                const bool compressed = u32_at(entry.data()) != 0;
                const std::string name = archive_name(
                    std::vector<std::uint8_t>(entry.begin() + 4, entry.begin() + 28), false);
                const std::uint64_t offset = u32_at(entry.data() + 36);
                const std::uint64_t size = u32_at(entry.data() + 40);
                if (name.empty() || !range_inside(offset, size, total_size)) {
                    if (name.empty()) {
                        continue;
                    }
                    throw Error("PACK entry is outside archive: " + name);
                }
                entries_[name] = FileEntry{relative_path, offset, size, compressed};
            }
            return true;
        }

        if (magic == kLacMagic) {
            const std::uint32_t count = u32_at(header.data() + 4);
            constexpr std::size_t header_size = 8;
            constexpr std::size_t entry_size = 40;
            if (total_size < header_size || count > (total_size - header_size) / entry_size) {
                throw Error("LAC entry table is truncated: " + full_path);
            }

            std::array<std::uint8_t, entry_size> entry{};
            for (std::uint32_t i = 0; i < count; ++i) {
                stream.seekg(static_cast<std::streamoff>(header_size + static_cast<std::size_t>(i) * entry_size));
                read_exact(stream, entry.data(), entry.size(), full_path);
                auto raw_name = std::vector<std::uint8_t>(entry.begin(), entry.begin() + 32);
                for (auto& byte : raw_name) {
                    if (byte != 0) {
                        byte = static_cast<std::uint8_t>(~byte);
                    }
                }
                const std::string name = archive_name(raw_name, false);
                const std::uint64_t size = u32_at(entry.data() + 32);
                const std::uint64_t offset = u32_at(entry.data() + 36);
                if (name.empty() || !range_inside(offset, size, total_size)) {
                    if (name.empty()) {
                        continue;
                    }
                    throw Error("LAC entry is outside archive: " + name);
                }
                entries_[name] = FileEntry{relative_path, offset, size, false};
            }
            return true;
        }

        throw Error("unknown archive magic: " + full_path);
    } catch (const std::exception& ex) {
        if (error != nullptr) {
            *error = ex.what();
        }
        return false;
    }
}

bool ArchiveIndex::contains(const std::string& name) const {
    return entries_.find(normalized_name(name)) != entries_.end();
}

std::optional<FileEntry> ArchiveIndex::find(const std::string& name) const {
    const auto found = entries_.find(normalized_name(name));
    if (found == entries_.end()) {
        return std::nullopt;
    }
    return found->second;
}

std::vector<std::uint8_t> ArchiveIndex::read(const std::string& name) const {
    const auto entry = find(name);
    if (!entry) {
        return {};
    }

    const std::string archive_path = join_path(root_, entry->archive_path);
    std::ifstream stream(archive_path, std::ios::binary);
    if (!stream) {
        throw Error("cannot open archive: " + archive_path);
    }
    if (entry->size > static_cast<std::uint64_t>(std::numeric_limits<std::streamsize>::max())) {
        throw Error("archive entry is too large: " + name);
    }
    stream.seekg(static_cast<std::streamoff>(entry->offset));
    std::vector<std::uint8_t> bytes(static_cast<std::size_t>(entry->size));
    if (!bytes.empty() && !stream.read(reinterpret_cast<char*>(bytes.data()), static_cast<std::streamsize>(bytes.size()))) {
        throw Error("cannot read archive entry: " + name);
    }
    return entry->compressed ? decompress_lzss(bytes) : bytes;
}

std::string ArchiveIndex::normalized_name(std::string name) {
    std::replace(name.begin(), name.end(), '\\', '/');
    std::transform(name.begin(), name.end(), name.begin(), [](unsigned char ch) {
        return static_cast<char>(std::tolower(ch));
    });
    return name;
}

std::vector<std::uint8_t> ArchiveIndex::decompress_lzss(const std::vector<std::uint8_t>& input) {
    BinaryReader reader(input);
    const std::uint32_t encoded_size = reader.u32();
    const std::uint32_t decoded_size = reader.u32();
    if (decoded_size > kMaxDecodedFile) {
        throw Error("compressed entry expands beyond safety limit");
    }
    std::size_t payload_size = encoded_size;
    if (payload_size > reader.remaining()) {
        // Game PACK entries store the complete entry size here, including this
        // eight-byte header. Some tools instead store only the payload size.
        if (encoded_size == input.size() && encoded_size >= 8U) payload_size = encoded_size - 8U;
        else throw Error("compressed entry is truncated");
    }

    std::array<std::uint8_t, 4096> window{};
    std::fill(window.begin(), window.begin() + 0xFEE, std::uint8_t{0x20});
    std::size_t window_write = 0xFEE;
    std::size_t input_read = 0;
    std::vector<std::uint8_t> output;
    output.reserve(decoded_size);
    const auto encoded = reader.bytes(payload_size);

    while (input_read < encoded.size() && output.size() < decoded_size) {
        std::uint8_t flags = encoded[input_read++];
        for (int bit = 0; bit < 8 && output.size() < decoded_size; ++bit, flags >>= 1U) {
            if (input_read >= encoded.size()) {
                break;
            }
            const std::uint8_t first = encoded[input_read++];
            if ((flags & 1U) != 0U) {
                window[window_write++ & 0xFFFU] = first;
                output.push_back(first);
                continue;
            }
            if (input_read >= encoded.size()) {
                throw Error("compressed back-reference is truncated");
            }
            const std::uint8_t second = encoded[input_read++];
            std::size_t window_read = first | (static_cast<std::size_t>(second & 0xF0U) << 4U);
            std::size_t count = (second & 0x0FU) + 3U;
            while (count-- > 0 && output.size() < decoded_size) {
                const std::uint8_t byte = window[window_read++ & 0xFFFU];
                window[window_write++ & 0xFFFU] = byte;
                output.push_back(byte);
            }
        }
    }

    if (output.size() != decoded_size) {
        throw Error("compressed entry ended before declared output size");
    }
    return output;
}

std::string join_path(const std::string& left, const std::string& right) {
    if (left.empty() || left == ".") {
        return left.empty() ? right : left + "/" + right;
    }
    if (right.empty()) return left;
    const char tail = left.back();
    if (tail == '/' || tail == '\\') return left + right;
    return left + "/" + right;
}

std::vector<std::uint8_t> read_file(const std::string& path) {
    std::ifstream stream(path, std::ios::binary | std::ios::ate);
    if (!stream) {
        throw Error("cannot open file: " + path);
    }
    const auto size = stream.tellg();
    if (size < 0) {
        throw Error("cannot determine file size: " + path);
    }
    stream.seekg(0);
    std::vector<std::uint8_t> bytes(static_cast<std::size_t>(size));
    if (!bytes.empty() && !stream.read(reinterpret_cast<char*>(bytes.data()), size)) {
        throw Error("cannot read file: " + path);
    }
    return bytes;
}

} // namespace wa2

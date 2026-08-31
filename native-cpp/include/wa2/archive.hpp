#pragma once

#include <cstddef>
#include <cstdint>
#include <optional>
#include <string>
#include <unordered_map>
#include <vector>

namespace wa2 {

struct FileEntry {
    std::string archive_path;
    std::uint64_t offset = 0;
    std::uint64_t size = 0;
    bool compressed = false;
};

class ArchiveIndex {
public:
    explicit ArchiveIndex(std::string resource_root = {});

    void set_resource_root(std::string resource_root);
    const std::string& resource_root() const noexcept { return root_; }

    // Later archives intentionally override earlier entries, matching the C# port.
    bool load_archive(const std::string& relative_path, std::string* error = nullptr);
    bool contains(const std::string& name) const;
    std::optional<FileEntry> find(const std::string& name) const;
    std::vector<std::uint8_t> read(const std::string& name) const;

    std::size_t file_count() const noexcept { return entries_.size(); }
    void clear() { entries_.clear(); }

    static std::string normalized_name(std::string name);
    static std::vector<std::uint8_t> decompress_lzss(const std::vector<std::uint8_t>& input);

private:
    std::string root_;
    std::unordered_map<std::string, FileEntry> entries_;
};

std::string join_path(const std::string& left, const std::string& right);
std::vector<std::uint8_t> read_file(const std::string& path);

} // namespace wa2

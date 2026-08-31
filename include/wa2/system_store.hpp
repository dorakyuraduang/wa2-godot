#pragma once

#include <cstddef>
#include <cstdint>
#include <string>
#include <vector>

namespace wa2 {

class SystemStore {
public:
    static constexpr std::size_t kStoreSize = 0x26A000;

    bool load_or_create(const std::string& path, std::string* error = nullptr);
    bool flush(std::string* error = nullptr) const;

    std::int32_t flag(std::size_t index) const;
    void set_flag(std::size_t index, std::int32_t value);
    std::uint8_t cg_flag(std::size_t index) const;
    void set_cg_flag(std::size_t index, std::uint8_t value);
    bool message_read(std::size_t script_index, std::size_t message_index) const;
    void set_message_read(std::size_t script_index, std::size_t message_index);

private:
    std::string path_;
    std::vector<std::uint8_t> bytes_;
};

} // namespace wa2

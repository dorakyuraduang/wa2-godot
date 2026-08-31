#pragma once

#include <cstdint>
#include <string>
#include <vector>

namespace wa2 {

// Converts the original game's CP932/Shift-JIS text to UTF-8.
std::string decode_cp932(const std::vector<std::uint8_t>& bytes);
std::vector<std::string> split_script_text(const std::string& text);

} // namespace wa2

#include "wa2/archive.hpp"
#include "wa2/error.hpp"
#include "wa2/system_store.hpp"
#include "wa2/vm.hpp"

#include <cstdint>
#include <cstdio>
#include <fstream>
#include <iostream>
#include <stdexcept>
#include <string>
#include <vector>

namespace {

void require(bool condition, const std::string& message) {
    if (!condition) throw std::runtime_error(message);
}

void append_u32(std::vector<std::uint8_t>& bytes, std::uint32_t value) {
    bytes.push_back(static_cast<std::uint8_t>(value));
    bytes.push_back(static_cast<std::uint8_t>(value >> 8U));
    bytes.push_back(static_cast<std::uint8_t>(value >> 16U));
    bytes.push_back(static_cast<std::uint8_t>(value >> 24U));
}

void append_fixed(std::vector<std::uint8_t>& bytes, const std::string& value, std::size_t size) {
    for (std::size_t i = 0; i < size; ++i) {
        bytes.push_back(i < value.size() ? static_cast<std::uint8_t>(value[i]) : 0);
    }
}

std::vector<std::uint8_t> make_script() {
    std::vector<std::uint8_t> bytes;
    append_u32(bytes, 0x5243534CU);
    append_u32(bytes, 0);
    append_u32(bytes, 1);
    append_u32(bytes, 0);
    append_u32(bytes, 20);

    append_u32(bytes, 5); append_u32(bytes, 3); append_u32(bytes, 7);
    append_u32(bytes, 5); append_u32(bytes, 3); append_u32(bytes, 5);
    append_u32(bytes, 6); append_u32(bytes, 0x10);
    append_u32(bytes, 4); append_u32(bytes, 0x123);
    append_u32(bytes, 0); append_u32(bytes, 1);
    return bytes;
}

void write_pack(const std::string& path) {
    const auto script = make_script();
    const std::vector<std::uint8_t> text{'z','e','r','o',',','h','e','l','l','o'};
    const std::uint32_t first_offset = 16 + 44 * 2;
    const std::uint32_t second_offset = first_offset + static_cast<std::uint32_t>(script.size());

    std::vector<std::uint8_t> bytes;
    append_u32(bytes, 0x5041434BU);
    append_u32(bytes, 0); append_u32(bytes, 0);
    append_u32(bytes, 2);

    append_u32(bytes, 0); append_fixed(bytes, "test.bnr", 24);
    append_u32(bytes, 0); append_u32(bytes, 0);
    append_u32(bytes, first_offset); append_u32(bytes, static_cast<std::uint32_t>(script.size()));

    append_u32(bytes, 0); append_fixed(bytes, "test.txt", 24);
    append_u32(bytes, 0); append_u32(bytes, 0);
    append_u32(bytes, second_offset); append_u32(bytes, static_cast<std::uint32_t>(text.size()));

    bytes.insert(bytes.end(), script.begin(), script.end());
    bytes.insert(bytes.end(), text.begin(), text.end());

    std::ofstream stream(path, std::ios::binary);
    stream.write(reinterpret_cast<const char*>(bytes.data()), static_cast<std::streamsize>(bytes.size()));
}

class TestHost final : public wa2::VmHost {
public:
    std::int32_t game_flag(std::size_t index) const override { return flags.at(index); }
    void set_game_flag(std::size_t index, std::int32_t value) override { flags.at(index) = value; }
    bool call_function(std::uint32_t function, std::vector<wa2::Value>& arguments, wa2::ScriptVm& vm) override {
        require(function == 0x123, "unexpected function id");
        require(arguments.size() == 1, "unexpected argument count");
        require(vm.integer(arguments.back()) == 12, "calculation result mismatch");
        called = true;
        return true;
    }
    bool is_active(const wa2::ScriptVm& vm) const override { return active == &vm; }
    void script_finished(wa2::ScriptVm&) override { finished = true; }

    std::vector<std::int32_t> flags = std::vector<std::int32_t>(1024);
    wa2::ScriptVm* active = nullptr;
    bool called = false;
    bool finished = false;
};

void test_lzss() {
    std::vector<std::uint8_t> encoded;
    append_u32(encoded, 4);
    append_u32(encoded, 3);
    encoded.push_back(0x07);
    encoded.push_back('a'); encoded.push_back('b'); encoded.push_back('c');
    const auto decoded = wa2::ArchiveIndex::decompress_lzss(encoded);
    require(std::string(decoded.begin(), decoded.end()) == "abc", "LZSS literal decode mismatch");

    // Original PACK files count the eight-byte compression header in inlim.
    encoded[0] = static_cast<std::uint8_t>(encoded.size());
    const auto decoded_total_size = wa2::ArchiveIndex::decompress_lzss(encoded);
    require(std::string(decoded_total_size.begin(), decoded_total_size.end()) == "abc",
            "LZSS total-size header decode mismatch");
}

void test_archive_and_vm(const std::string& root) {
    write_pack(wa2::join_path(root, "wa2_core_test.pak"));
    wa2::ArchiveIndex archives(root);
    std::string error;
    require(archives.load_archive("wa2_core_test.pak", &error), error);
    require(archives.file_count() == 2, "PACK index count mismatch");
    require(archives.contains("TEST.BNR"), "case-insensitive archive lookup failed");

    TestHost host;
    wa2::ScriptVm vm(archives, host, "test");
    host.active = &vm;
    vm.run();
    require(host.called, "VM did not dispatch function");
    require(host.finished, "VM did not finish");
}

void test_system_store(const std::string& root) {
    const std::string path = wa2::join_path(root, "wa2_core_test_sys.sav");
    std::remove(path.c_str());
    std::remove((path + ".tmp").c_str());

    std::string error;
    wa2::SystemStore store;
    require(store.load_or_create(path, &error), error);
    store.set_flag(17, -12345);
    store.set_cg_flag(42, 7);
    store.set_message_read(2, 123);
    require(store.flush(&error), error);

    wa2::SystemStore loaded;
    require(loaded.load_or_create(path, &error), error);
    require(loaded.flag(17) == -12345, "system flag round trip failed");
    require(loaded.cg_flag(42) == 7, "CG flag round trip failed");
    require(loaded.message_read(2, 123), "message read-bit round trip failed");
    require(!loaded.message_read(2, 124), "message read-bit neighbor was modified");

    std::remove(path.c_str());
    std::remove((path + ".tmp").c_str());
}

} // namespace

int main() {
    const std::string root = ".";
    const std::string test_file = wa2::join_path(root, "wa2_core_test.pak");
    try {
        std::remove(test_file.c_str());
        test_lzss();
        test_archive_and_vm(root);
        test_system_store(root);
        std::remove(test_file.c_str());
        std::cout << "wa2_core_tests: all tests passed\n";
        return 0;
    } catch (const std::exception& ex) {
        std::remove(test_file.c_str());
        std::cerr << "wa2_core_tests: " << ex.what() << '\n';
        return 1;
    }
}

#pragma once

#include "wa2/archive.hpp"

#include <array>
#include <cstddef>
#include <cstdint>
#include <string>
#include <unordered_map>
#include <variant>
#include <vector>

namespace wa2 {

enum class CommandType : std::int32_t {
    none = 0,
    global_variable = 1,
    local_variable = 2,
    string_variable = 3,
    function = 4,
    value = 5,
    calculation = 6,
};

enum class ValueType : std::int32_t {
    reference = 0,
    integer = 3,
    floating = 4,
};

using Number = std::variant<std::int32_t, float>;

struct Value {
    CommandType command = CommandType::value;
    std::int32_t value_type = static_cast<std::int32_t>(ValueType::integer);
    std::int32_t value0 = 0;
    std::int32_t integer = 0;
    float floating = 0.0F;
};

struct JumpEntry {
    std::uint32_t type = 0;
    std::uint32_t count = 0;
    std::uint32_t position = 0;
    std::int32_t flag = 0;
    std::array<std::uint32_t, 64> positions{};
    std::array<std::uint32_t, 64> flags{};
};

class ScriptVm;

class VmHost {
public:
    virtual ~VmHost() = default;
    virtual std::int32_t game_flag(std::size_t index) const = 0;
    virtual void set_game_flag(std::size_t index, std::int32_t value) = 0;
    virtual bool call_function(std::uint32_t function, std::vector<Value>& arguments, ScriptVm& vm) = 0;
    virtual bool is_active(const ScriptVm& vm) const = 0;
    virtual void script_finished(ScriptVm& vm) = 0;
};

class ScriptVm {
public:
    ScriptVm(ArchiveIndex& archives, VmHost& host, std::string script_name, std::int32_t point = 0);

    const std::string& name() const noexcept { return name_; }
    std::uint32_t position() const noexcept { return position_; }
    void set_position(std::uint32_t position);
    bool exited() const noexcept { return exited_; }
    bool finished() const noexcept { return finished_; }

    // Executes until a script function asks the runtime to wait or this VM stops being active.
    void run();

    Number number(const Value& value) const;
    std::int32_t integer(const Value& value) const;
    float floating(const Value& value) const;
    std::string string(const Value& value) const;
    void assign(Value& target, Number value);

    void push_integer(std::int32_t value);
    void push_float(float value);

    std::array<std::int32_t, 26>& local_integers() noexcept { return local_integers_; }
    std::array<float, 26>& local_floats() noexcept { return local_floats_; }
    std::vector<Value>& arguments() noexcept { return arguments_; }
    std::vector<JumpEntry>& jumps() noexcept { return jumps_; }

private:
    bool parse_jump();
    void parse_calculation();
    void push_reference(CommandType command, std::int32_t index);
    void push_literal(std::int32_t raw_type);
    std::uint32_t read_u32();
    float read_f32();
    void complete();

    ArchiveIndex& archives_;
    VmHost& host_;
    std::string name_;
    std::vector<std::uint8_t> bytecode_;
    std::unordered_map<std::int32_t, std::uint32_t> points_;
    std::vector<std::string> texts_;
    std::uint32_t position_ = 0;
    bool exited_ = false;
    bool finished_ = false;
    std::array<std::int32_t, 26> local_integers_{};
    std::array<float, 26> local_floats_{};
    std::vector<Value> arguments_;
    std::vector<JumpEntry> jumps_;
};

std::int32_t number_as_int(Number value);
float number_as_float(Number value);

} // namespace wa2

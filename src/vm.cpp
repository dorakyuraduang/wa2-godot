#include "wa2/vm.hpp"

#include "wa2/binary.hpp"
#include "wa2/error.hpp"
#include "wa2/text.hpp"

#include <algorithm>
#include <cmath>
#include <cstring>
#include <limits>
#include <sstream>

namespace wa2 {
namespace {

constexpr std::uint32_t kScriptMagic = 0x5243534CU;
constexpr std::size_t kMaxJumpDepth = 15;

bool number_is_int(const Number& value) {
    return std::holds_alternative<std::int32_t>(value);
}

Number add(Number left, Number right) {
    if (number_is_int(left) && number_is_int(right)) {
        return number_as_int(left) + number_as_int(right);
    }
    return number_as_float(left) + number_as_float(right);
}

} // namespace

ScriptVm::ScriptVm(ArchiveIndex& archives, VmHost& host, std::string script_name, std::int32_t point)
    : archives_(archives), host_(host), name_(std::move(script_name)) {
    bytecode_ = archives_.read(name_ + ".bnr");
    if (bytecode_.size() < 12 || read_u32_at(bytecode_, 0) != kScriptMagic) {
        throw Error("invalid or missing script bytecode: " + name_ + ".bnr");
    }
    const std::uint32_t point_count = read_u32_at(bytecode_, 8);
    if (point_count > (bytecode_.size() - 12U) / 8U) {
        throw Error("truncated script point table: " + name_);
    }
    for (std::uint32_t i = 0; i < point_count; ++i) {
        const std::size_t offset = 12U + static_cast<std::size_t>(i) * 8U;
        points_[static_cast<std::int32_t>(read_u32_at(bytecode_, offset))] = read_u32_at(bytecode_, offset + 4U);
    }
    const auto found = points_.find(point);
    if (found == points_.end()) {
        throw Error("script point not found: " + name_ + ":" + std::to_string(point));
    }
    set_position(found->second);

    const auto text_bytes = archives_.read(name_ + ".txt");
    if (text_bytes.empty()) {
        throw Error("missing script text: " + name_ + ".txt");
    }
    texts_ = split_script_text(decode_cp932(text_bytes));
}

void ScriptVm::set_position(std::uint32_t position) {
    if (position > bytecode_.size()) {
        throw Error("script position outside bytecode: " + name_);
    }
    position_ = position;
}

void ScriptVm::run() {
    bool keep_running = true;
    while (keep_running && !finished_ && position_ < bytecode_.size() && host_.is_active(*this)) {
        const auto command = static_cast<CommandType>(read_u32());
        switch (command) {
        case CommandType::none:
            keep_running = parse_jump();
            break;
        case CommandType::global_variable:
        case CommandType::local_variable:
        case CommandType::string_variable:
            push_reference(command, static_cast<std::int32_t>(read_u32()));
            break;
        case CommandType::function: {
            const std::uint32_t function = read_u32();
            keep_running = host_.call_function(function, arguments_, *this);
            break;
        }
        case CommandType::value:
            push_literal(static_cast<std::int32_t>(read_u32()));
            break;
        case CommandType::calculation:
            parse_calculation();
            break;
        default:
            throw Error("unknown script command " + std::to_string(static_cast<std::uint32_t>(command)) +
                        " in " + name_ + " at " + std::to_string(position_ - 4U));
        }

        if (!jumps_.empty() && jumps_.back().type >= 2U && jumps_.back().type <= 7U) {
            while (!jumps_.empty() && jumps_.back().position == position_) {
                jumps_.pop_back();
            }
        }
        if (position_ >= bytecode_.size() || exited_) {
            complete();
        }
    }
}

Number ScriptVm::number(const Value& value) const {
    if (value.value_type == static_cast<std::int32_t>(ValueType::floating)) {
        return value.floating;
    }
    if (value.value_type == static_cast<std::int32_t>(ValueType::integer)) {
        return value.integer;
    }
    switch (value.command) {
    case CommandType::local_variable:
        if (value.integer >= 26) {
            return local_floats_.at(static_cast<std::size_t>(value.integer % 26));
        }
        return local_integers_.at(static_cast<std::size_t>(value.integer));
    case CommandType::global_variable:
        return host_.game_flag(static_cast<std::size_t>(value.integer));
    default:
        return std::int32_t{0};
    }
}

std::int32_t ScriptVm::integer(const Value& value) const {
    return number_as_int(number(value));
}

float ScriptVm::floating(const Value& value) const {
    return number_as_float(number(value));
}

std::string ScriptVm::string(const Value& value) const {
    if (value.command != CommandType::string_variable) {
        if (number_is_int(number(value))) {
            return std::to_string(integer(value));
        }
        std::ostringstream stream;
        stream << floating(value);
        return stream.str();
    }
    if (value.integer == 0) {
        // The original VM uses the protagonist's default name for string slot zero.
        return "春希";
    }
    if (value.integer > 0 && static_cast<std::size_t>(value.integer) < texts_.size()) {
        return texts_[static_cast<std::size_t>(value.integer)];
    }
    return {};
}

void ScriptVm::assign(Value& target, Number value) {
    if (target.command == CommandType::global_variable) {
        host_.set_game_flag(static_cast<std::size_t>(target.integer), number_as_int(value));
        return;
    }
    if (target.command == CommandType::local_variable) {
        if (target.integer >= 26) {
            local_floats_.at(static_cast<std::size_t>(target.integer % 26)) = number_as_float(value);
        } else {
            local_integers_.at(static_cast<std::size_t>(target.integer)) = number_as_int(value);
        }
        return;
    }
    if (target.command == CommandType::value) {
        if (number_is_int(value)) {
            target.value_type = static_cast<std::int32_t>(ValueType::integer);
            target.integer = number_as_int(value);
        } else {
            target.value_type = static_cast<std::int32_t>(ValueType::floating);
            target.floating = number_as_float(value);
        }
    }
}

void ScriptVm::push_integer(std::int32_t value) {
    arguments_.push_back(Value{CommandType::value, static_cast<std::int32_t>(ValueType::integer), 0, value, 0.0F});
}

void ScriptVm::push_float(float value) {
    arguments_.push_back(Value{CommandType::value, static_cast<std::int32_t>(ValueType::floating), 0, 0, value});
}

bool ScriptVm::parse_jump() {
    const std::uint32_t flag = read_u32();
    switch (flag) {
    case 0:
        arguments_.clear();
        return false;
    case 1:
        exited_ = true;
        break;
    case 2: {
        if (jumps_.size() < kMaxJumpDepth) jumps_.emplace_back();
        auto& jump = jumps_.back();
        jump.type = 2;
        jump.positions[0] = read_u32();
        jump.position = read_u32();
        break;
    }
    case 3: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        if (jump.flag != 0) {
            set_position(jump.position);
        } else {
            jump.type = 3;
            jump.positions[0] = read_u32();
        }
        break;
    }
    case 4: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        if (jump.flag != 0) set_position(jump.position);
        break;
    }
    case 5: {
        if (jumps_.size() < kMaxJumpDepth) jumps_.emplace_back();
        auto& jump = jumps_.back();
        jump.type = 5;
        jump.position = read_u32();
        jump.positions[0] = read_u32();
        jump.positions[1] = read_u32();
        jump.positions[2] = read_u32();
        break;
    }
    case 6: {
        if (jumps_.size() < kMaxJumpDepth) jumps_.emplace_back();
        auto& jump = jumps_.back();
        jump.type = 6;
        jump.position = read_u32();
        break;
    }
    case 7: {
        if (jumps_.size() < kMaxJumpDepth) jumps_.emplace_back();
        auto& jump = jumps_.back();
        jump.type = 7;
        jump.count = read_u32();
        if (jump.count > jump.positions.size()) {
            throw Error("script switch table exceeds 64 entries");
        }
        for (std::size_t i = 0; i < jump.count; ++i) {
            jump.positions[i] = read_u32();
            jump.flags[i] = read_u32();
        }
        jump.position = read_u32();
        break;
    }
    case 8:
    case 9:
        break;
    case 10:
        while (!jumps_.empty()) {
            const auto type = jumps_.back().type;
            if (type == 5 || type == 6 || type == 7) break;
            jumps_.pop_back();
        }
        break;
    case 11:
        while (!jumps_.empty()) {
            const auto type = jumps_.back().type;
            if (type == 5) {
                set_position(jumps_.back().positions[2]);
                break;
            }
            if (type == 6) {
                set_position(jumps_.back().positions[0]);
                break;
            }
            jumps_.pop_back();
        }
        break;
    case 12:
        set_position(read_u32());
        break;
    case 13: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        jump.flag = integer(arguments_.at(arguments_.size() - 1));
        if (jump.flag == 0) set_position(jump.positions[0] != 0 ? jump.positions[0] : jump.position);
        break;
    }
    case 14: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        jump.flag = integer(arguments_.at(arguments_.size() - 1));
        set_position(jump.flag == 0 ? jump.position : jump.positions[2]);
        break;
    }
    case 15: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        jump.flag = integer(arguments_.at(arguments_.size() - 1));
        if (jump.flag == 0) set_position(jump.position);
        break;
    }
    case 16: {
        auto& jump = jumps_.at(jumps_.size() - 1);
        jump.flag = integer(arguments_.at(arguments_.size() - 1));
        if (jump.type == 7) {
            bool matched = false;
            for (std::size_t i = 0; i < jump.count; ++i) {
                if (jump.flags[i] == static_cast<std::uint32_t>(jump.flag)) {
                    set_position(jump.positions[i]);
                    matched = true;
                    break;
                }
            }
            if (!matched) set_position(jump.position);
        }
        break;
    }
    default:
        throw Error("unknown jump opcode " + std::to_string(flag));
    }
    arguments_.clear();
    return true;
}

void ScriptVm::parse_calculation() {
    const std::uint32_t operation = read_u32();
    if (operation > 0x1E) return;

    Value empty;
    Value a = empty;
    Value b = empty;
    const bool has_a = !arguments_.empty() && operation <= 0x1B;
    const bool binary = ((operation >= 1 && operation < 0x17) || operation == 0x1B || operation == 0) &&
                        arguments_.size() > 1;
    if (has_a) a = arguments_.back();
    if (binary) {
        b = arguments_[arguments_.size() - 2];
        arguments_.pop_back();
    }
    if (operation >= 8 && operation <= 0x16 && !arguments_.empty()) {
        arguments_.pop_back();
    }

    const Number av = number(a);
    const Number bv = number(b);
    auto assign_b = [this, &b](Number result) {
        if (b.command == CommandType::global_variable || b.command == CommandType::local_variable) {
            assign(b, result);
        } else if (!arguments_.empty()) {
            assign(arguments_.back(), result);
        }
    };
    auto assign_a = [this, &a](Number result) {
        if (a.command == CommandType::global_variable || a.command == CommandType::local_variable) {
            assign(a, result);
        } else if (!arguments_.empty()) {
            assign(arguments_.back(), result);
        }
    };
    switch (operation) {
    case 0: assign_b(av); break;
    case 1: assign_b(number_as_int(av) + number_as_int(bv)); break;
    case 2: assign_b(number_as_int(bv) - number_as_int(av)); break;
    case 3: assign_b(number_as_int(av) * number_as_int(bv)); break;
    case 4: assign_b(number_as_int(av) == 0 ? 0 : number_as_int(bv) / number_as_int(av)); break;
    case 5: assign_b(number_as_int(av) == 0 ? 0 : number_as_int(bv) % number_as_int(av)); break;
    case 6: assign_b(number_as_int(bv) & number_as_int(av)); break;
    case 7: assign_b(number_as_int(bv) | number_as_int(av)); break;
    case 8: push_integer(number_as_float(av) == number_as_float(bv)); break;
    case 9: push_integer(number_as_float(bv) < number_as_float(av)); break;
    case 0xA: push_integer(number_as_float(bv) > number_as_float(av)); break;
    case 0xB: push_integer(number_as_float(bv) <= number_as_float(av)); break;
    case 0xC: push_integer(number_as_float(bv) >= number_as_float(av)); break;
    case 0xD: push_integer(number_as_float(bv) != 0.0F && number_as_float(av) != 0.0F); break;
    case 0xE: push_integer(number_as_float(bv) != 0.0F || number_as_float(av) != 0.0F); break;
    case 0xF: push_integer(number_as_float(bv) != number_as_float(av)); break;
    case 0x10: {
        const Number result = add(av, bv);
        number_is_int(result) ? push_integer(number_as_int(result)) : push_float(number_as_float(result));
        break;
    }
    case 0x11:
        if (number_is_int(av) && number_is_int(bv)) push_integer(number_as_int(bv) - number_as_int(av));
        else push_float(number_as_float(bv) - number_as_float(av));
        break;
    case 0x12:
        if (number_is_int(av) && number_is_int(bv)) push_integer(number_as_int(bv) * number_as_int(av));
        else push_float(number_as_float(bv) * number_as_float(av));
        break;
    case 0x13:
        if (number_as_float(av) == 0.0F) push_integer(0);
        else if (number_is_int(av) && number_is_int(bv)) push_integer(number_as_int(bv) / number_as_int(av));
        else push_float(number_as_float(bv) / number_as_float(av));
        break;
    case 0x14: push_integer(number_as_int(av) == 0 ? 0 : number_as_int(bv) % number_as_int(av)); break;
    case 0x15: push_integer(number_as_int(bv) & number_as_int(av)); break;
    case 0x16: push_integer(number_as_int(bv) | number_as_int(av)); break;
    case 0x17:
        number_is_int(av) ? assign_a(-number_as_int(av)) : assign_a(-number_as_float(av));
        break;
    case 0x18: assign_a(number_as_float(av) == 0.0F ? 1 : 0); break;
    case 0x19: assign_a(add(av, std::int32_t{1})); break;
    case 0x1A:
        number_is_int(av) ? assign_a(number_as_int(av) - 1) : assign_a(number_as_float(av) - 1.0F);
        break;
    case 0x1B:
        if (!arguments_.empty()) arguments_.back().value_type = number_as_int(av);
        break;
    case 0x1C:
    case 0x1D:
        break;
    case 0x1E:
        arguments_.clear();
        break;
    default:
        break;
    }
}

void ScriptVm::push_reference(CommandType command, std::int32_t index) {
    arguments_.push_back(Value{command, -1, 0, index, 0.0F});
}

void ScriptVm::push_literal(std::int32_t raw_type) {
    if (raw_type == static_cast<std::int32_t>(ValueType::floating)) {
        push_float(read_f32());
    } else {
        Value value;
        value.command = CommandType::value;
        value.value_type = raw_type;
        value.integer = static_cast<std::int32_t>(read_u32());
        arguments_.push_back(value);
    }
}

std::uint32_t ScriptVm::read_u32() {
    if (position_ > bytecode_.size() || bytecode_.size() - position_ < 4U) {
        throw Error("truncated script command in " + name_);
    }
    const std::uint32_t result = read_u32_at(bytecode_, position_);
    position_ += 4U;
    return result;
}

float ScriptVm::read_f32() {
    const std::uint32_t bits = read_u32();
    float result = 0.0F;
    std::memcpy(&result, &bits, sizeof(result));
    return result;
}

void ScriptVm::complete() {
    if (finished_) return;
    finished_ = true;
    host_.script_finished(*this);
}

std::int32_t number_as_int(Number value) {
    if (const auto* integer = std::get_if<std::int32_t>(&value)) return *integer;
    return static_cast<std::int32_t>(std::get<float>(value));
}

float number_as_float(Number value) {
    if (const auto* floating = std::get_if<float>(&value)) return *floating;
    return static_cast<float>(std::get<std::int32_t>(value));
}

} // namespace wa2

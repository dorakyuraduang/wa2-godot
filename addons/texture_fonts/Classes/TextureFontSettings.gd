@tool
extends Resource

const CharUtils = preload("./Utils.gd")

const DEFAULT_CHAR_SETTING = {
	advance = 0.0,
	offset = Vector2.ZERO
}

# {
# 	for_chars: String = {
# 		advance: int,
# 		offset: Vector2
# 	}
# }
@export var char_settings: Dictionary = {}

# [
# 	{
# 		from: String,
# 		to: String,
# 		kerning: int
# 	}
# ]
@export var kerning_pairs: Array[Dictionary] = []

@export var gap := 2                  ## Horizontal spacing between each character.
@export var alignment := Vector2.ZERO ## Offset to apply to every character in the font.
@export var monospace := false        ## If [code]true[/code] all characters have the same width.
@export var descent := 0.0            ## Vertical offset applied under every character. Can be seen as the font height.
@export var ascent := 0.0             ## Vertical offset applied above every character, as spacing for every new line.

# TODO: Move these settings to local editor data.
@export var preview_color := Color("202431") ## @deprecated
@export var preview_chars: String  ## @deprecated

# ------ Settings Functions ------

func add_setting(for_chars: String):
	char_settings[for_chars] = DEFAULT_CHAR_SETTING.duplicate()

func insert_setting(for_chars: String, setting: Dictionary):
	char_settings[for_chars] = setting

func remove_setting(for_chars: String):
	char_settings.erase(for_chars)


func set_setting(for_chars: String, advance: int, offset: Vector2):
	char_settings[for_chars] = {
		advance = advance,
		offset = offset
	}

func get_setting(char_code: int) -> Dictionary:
	if char_code == -1:
		return DEFAULT_CHAR_SETTING
	
	for char_string in char_settings:
		var char_codes: Array = CharUtils.chars_to_codes(char_string).front()
		if char_code in char_codes:
			return char_settings[char_string]
	
	return DEFAULT_CHAR_SETTING


func set_advance(for_chars: String, advance: int):
	if not char_settings.has(for_chars):
		add_setting(for_chars)
	
	char_settings[for_chars].advance = advance

func set_offset(for_chars: String, offset: Vector2):
	if not char_settings.has(for_chars):
		add_setting(for_chars)
	
	char_settings[for_chars].offset = offset


func get_advance(for_chars: String) -> float:
	if char_settings.has(for_chars):
		return char_settings[for_chars].advance
	else:
		push_warning("Returning default advance of 0.0 for '", for_chars, "' character(s)")
		return 0.0

func get_offset(for_chars: String) -> Vector2:
	if char_settings.has(for_chars):
		return char_settings[for_chars].offset
	else:
		push_warning("Returning default offset of Vector2(0.0, 0.0) for '", for_chars, "' character(s)")
		return Vector2.ZERO


# ------ Kerning Pair Functions ------

func add_kerning_pair() -> Dictionary:
	var new_pair := {
		from = "",
		to = "",
		kerning = 0
	}
	
	kerning_pairs.append(new_pair)
	return new_pair

func insert_kerning_pair(idx: int, new_kerning: Dictionary):
	kerning_pairs.insert(idx, new_kerning)

func remove_kerning_pair(idx: int):
	kerning_pairs.remove_at(idx)


func set_kerning_pair(idx: int, new_kerning: Dictionary):
	kerning_pairs[idx] = new_kerning


func set_kerning_pair_from(idx: int, new_from: String):
	kerning_pairs[idx].from = new_from

func set_kerning_pair_to(idx: int, new_to: String):
	kerning_pairs[idx].to = new_to

func set_kerning_pair_kerning(idx: int, new_kerning: int):
	kerning_pairs[idx].kerning = new_kerning


func get_kerning_pair(idx: int) -> Dictionary:
	return kerning_pairs[idx]


func solve_kerning_pairs() -> Array[Dictionary]:
	var solved_list: Array[Dictionary] = []
	for pair in kerning_pairs:
		if pair.from == "" or pair.to == "" or pair.kerning == 0:
			continue
		
		var from_array: Array = CharUtils.chars_to_codes(pair.from).front() # Weird
		var to_array: Array = CharUtils.chars_to_codes(pair.to).front() # Weird
		
		for a in from_array:
			for b in to_array:
				solved_list.append({
					char_a = a,
					char_b = b,
					kerning = pair.kerning
				})
	
	return solved_list


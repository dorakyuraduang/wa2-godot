@tool
extends RefCounted

static var num_regex := RegEx.new()
static var unicode_regex := RegEx.new()

static var regex_compiled := false

# Split multi-line String into 2D Array (Array[Array[int]]) containing char codes.
static func chars_to_codes(chars: String) -> Array[Array]:
	var codes: Array[Array] = []
	
	if not regex_compiled:
		num_regex.compile("[0-9A-Fa-f]+")
		unicode_regex.compile("\\\\U\\+[0-9A-Fa-f]+;")
		regex_compiled = true
	
	var i := 0
	var length := chars.length()
	var line := 0
	
	while i < length:
		var next_char := chars[i]
		var code_int := -1
		
		# dont save new-line symbol, but switch to new line
		if next_char == "\n":
			if line + 1 > codes.size():
				codes.append([])
			line += 1
			i += 1
			continue
		# escape char might indicate unicode block
		elif next_char == "\\":
			var code := unicode_regex.search(chars, i)
			if code and code.get_start() == i:
				var num_code := num_regex.search(code.get_string())
				code_int = ("0x" + num_code.get_string()).hex_to_int()
				
				i += code.get_string().length() - 1
			else:
				code_int = "\\".unicode_at(0)
		# standard char
		else:
			code_int = next_char.unicode_at(0)
		
		# add line, if not present
		if line + 1 > codes.size():
			codes.append([code_int])
		else:
			codes[line].append(code_int)
		
		i += 1
	
	return codes

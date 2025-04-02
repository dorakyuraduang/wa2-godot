@icon("res://addons/texture_fonts/Assets/TextureFont.svg")
@tool
extends FontFile
class_name TextureFont

## Font that uses textures to represent characters.
##
## TextureFont is a font type that uses one or more textures to represent individual characters.[br]
## It contains one or more mappings [member texture_mappings], 
## which each defining how the texture applies to individual characters.
## Further attributes (such as ascent or character settings) can be accessed from [member font_settings].[br]
## I recommend using the editor to modify a TextureFont resource.

const Mapping = preload("./TextureFontMapping.gd")
const Settings = preload("./TextureFontSettings.gd")

const FONT_SIZE := Vector2i(16, 0)
const DEFAULT_TEXTURE_MAPPING := {
	rect_size = Vector2.ONE * 16,
	rect_gap = Vector2.ZERO,
	texture_offset = Vector2.ZERO,
	chars = \
"""abcdefgh
ijklmnop
qrstuvwx
yz.,!?" """
}

var texture_mappings: Array[Mapping]: ## Array of texture mappings assigned to this font.
	set(new):
		texture_mappings = new
		# Images are not saved in ".tres" file to avoid bloat.
		# Set them again on load just to be sure.
		_set_real_images()
var font_settings: Settings = Settings.new() ## Contains further settings that define this font.

## Creates a new, default texture mapping and assigns it the given [param texture].
func add_mapping_from_texture(texture: Texture2D) -> void:
	var mapping := Mapping.new()
	mapping.set_texture(texture)
	mapping.rect_size = DEFAULT_TEXTURE_MAPPING.rect_size
	mapping.rect_gap = DEFAULT_TEXTURE_MAPPING.rect_gap
	mapping.texture_offset = DEFAULT_TEXTURE_MAPPING.texture_offset
	mapping.chars = DEFAULT_TEXTURE_MAPPING.chars
	
	texture_mappings.append(mapping)

## Removes the texture mapping at the given [param index].
func remove_mapping(index: int) -> void:
	texture_mappings.remove_at(index)

## Forces the font to be recompiled.
func build_font():
	#clear_cache()
	clear_textures(0, FONT_SIZE)
	clear_glyphs(0, FONT_SIZE)
	clear_kerning_map(0, FONT_SIZE.x)
	clear_size_cache(0)
	
	fixed_size = FONT_SIZE.x
	fixed_size_scale_mode = TextServer.FIXED_SIZE_SCALE_ENABLED
	oversampling = 1.0
	
	var mono := font_settings.monospace
	var alignment := font_settings.alignment
	var gap := font_settings.gap
	
	var is_space_defined := false
	
	set_cache_descent(0, FONT_SIZE.x, font_settings.descent)
	set_cache_ascent(0, FONT_SIZE.x, font_settings.ascent)
	
	for i in texture_mappings.size():
		var mapping := texture_mappings[i]
		set_texture_image(0, FONT_SIZE, i, mapping.scaled_image)
		
		var char_codes := mapping.char_codes
		
		for line in char_codes:
			for code in line:
				if code == 32:
					is_space_defined = true
				
				var rect := (mapping.get_char_rect(code) if mono
						else mapping.get_cropped_char_rect(code))
				
				if rect == Mapping.INVALID_CHAR_RECT:
					continue
				
				var char_setting = font_settings.get_setting(code)
				var offset = char_setting.offset + alignment
				var advance := Vector2(rect.size.x + char_setting.advance + gap, 0)
				
				set_glyph_texture_idx(0, FONT_SIZE, code, i)
				set_glyph_uv_rect(0, FONT_SIZE, code, rect)
				set_glyph_offset(0, FONT_SIZE, code, offset)
				set_glyph_advance(0, FONT_SIZE.x, code, advance)
				set_glyph_size(0, FONT_SIZE, code, rect.size)
#				print("Added glyph for character '%s', advance: %s" % [String.chr(code), advance])
	
	# Add default empty space chararacter.
	if not is_space_defined and texture_mappings.size() > 0:
		const SPACE_CHAR = 32
		var char_setting := font_settings.get_setting(SPACE_CHAR)
		
		var extra_space := 0
		if mono:
			extra_space = texture_mappings[0].rect_size.x * texture_mappings[0].scale
		
		var advance := Vector2(char_setting.advance + extra_space + gap, 0)
		
		set_glyph_texture_idx(0, FONT_SIZE, SPACE_CHAR, 0)
		set_glyph_uv_rect(0, FONT_SIZE, SPACE_CHAR, Rect2())
		set_glyph_offset(0, FONT_SIZE, SPACE_CHAR, Vector2.ZERO)
		set_glyph_advance(0, FONT_SIZE.x, SPACE_CHAR, advance)
	
	if not font_settings.kerning_pairs.is_empty():
		var kerning_pairs := font_settings.solve_kerning_pairs()
		for pair in kerning_pairs:
			set_kerning(0, FONT_SIZE.x, Vector2i(pair.char_a, pair.char_b), Vector2(pair.kerning, 0))
	
	emit_changed()
	notify_property_list_changed()

#var _in_init := true
var _save_frame: int
func _get_property_list() -> Array[Dictionary]:
	if not Engine.is_editor_hint():
		return []
	
	# What follows is a dumb workaround to prevent bundling HUGE images in the ".tres" file.
	# It would be nice to not generate errors. But alas.
	#if _in_init:
		# I don't want this to happen at the start, as it can cause EVEN MORE errors.
		#_in_init = false
		#return []
	if _save_frame == Engine.get_process_frames():
		return [] # Not more than once per frame.
	_save_frame = Engine.get_process_frames()
	
	#clear_textures(0, FONT_SIZE)
	_set_placeholder_images()
	#push_warning("TextureFont: Ignore the errors for '%s' below, if any." % resource_path.get_file())
	
	#build_font.call_deferred()
	Engine.get_main_loop().create_timer(0.1).timeout.connect(_set_real_images)
	
	return []

func _validate_property(property: Dictionary) -> void:
	if property.name == &"texture_mappings" or property.name == &"font_settings":
		property.usage = PROPERTY_USAGE_NO_EDITOR
	elif property.name == &"fallbacks":
		property.usage = PROPERTY_USAGE_DEFAULT

func _set_real_images():
	for i in texture_mappings.size():
		var mapping := texture_mappings[i]
		set_texture_image(0, FONT_SIZE, i, mapping.scaled_image)
	
	emit_changed()

func _set_placeholder_images():
	for i in texture_mappings.size():
		set_texture_image(0, FONT_SIZE, i, preload("res://addons/texture_fonts/Assets/placeholder.png"))

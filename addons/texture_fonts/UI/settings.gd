@tool
extends HSplitContainer

signal changed

const Preview = preload("./settings_preview.gd")
const Vector2Edit = preload("./Components/Vector2Edit.gd")
const CharSetting = preload("./Components/CharSetting.gd")
const KerningPair = preload("./Components/KerningPair.gd")

const CHAR_SETTING_SCENE = preload("./Components/CharSetting.tscn")
const KERNING_PAIR_SCENE = preload("./Components/KerningPair.tscn")

@export var char_setting_list: VBoxContainer
@export var kerning_pair_list: VBoxContainer

@export var descent: SpinBox
@export var ascent: SpinBox
@export var gap: SpinBox
@export var alignment: Vector2Edit
@export var monospace: CheckBox

@export var preview: Preview
@export var preview_textbox: TextEdit


var font_settings: TextureFont.Settings
var font_ref: WeakRef


func set_font(new_font: TextureFont):
	font_ref = weakref(new_font)
	
	descent.set_value_no_signal(new_font.get_cache_descent(0, TextureFont.FONT_SIZE.x))
	ascent.set_value_no_signal(new_font.get_cache_ascent(0, TextureFont.FONT_SIZE.x))
	
	# clear char settings list
	for child in char_setting_list.get_children():
		char_setting_list.remove_child(child)
		child.queue_free()
	
	for child in kerning_pair_list.get_children():
		print(child)
		kerning_pair_list.remove_child(child)
		child.queue_free()
	
	font_settings = new_font.font_settings
	
	# Populate char settings list.
	for for_char in font_settings.char_settings:
		_add_char_setting(for_char)
	
	for for_kerning_pair in font_settings.kerning_pairs:
		_add_kerning_pair(for_kerning_pair)
	
	gap.set_value_no_signal(font_settings.gap)
	alignment.set_value_no_signal(font_settings.alignment)
	monospace.button_pressed = font_settings.monospace
	
	preview.set_preview_text(font_settings.preview_chars)
	preview.set_preview_color(font_settings.preview_color)


func _add_char_setting(for_char := ""):
	var char_setting_node: CharSetting = CHAR_SETTING_SCENE.instantiate()
	
	if for_char:
		char_setting_node.for_char = for_char
	
	char_setting_node.font_settings = font_settings
	
	char_setting_list.add_child(char_setting_node)
	char_setting_node.owner = owner
	char_setting_node.changed.connect(_emit_changed)
	char_setting_node.delete_requested.connect(_on_char_setting_delete.bind(char_setting_node))
	
	_emit_changed()


func _add_kerning_pair(pair := {}):
	# FIXME: Kerning pair values are not displayed at all on load.
	var kerning_pair_node: KerningPair = KERNING_PAIR_SCENE.instantiate()
	
	if pair.is_empty():
		pair = font_settings.add_kerning_pair()
	
	kerning_pair_node.set_kerning_pair(pair)
	kerning_pair_node.font_settings = font_settings
	
	kerning_pair_list.add_child(kerning_pair_node)
	kerning_pair_node.owner = owner
	kerning_pair_node.changed.connect(_emit_changed)
	kerning_pair_node.delete_requested.connect(_on_kerning_pair_delete.bind(kerning_pair_node))
	
	_emit_changed()


func _on_kerning_pair_delete(kerning_node: KerningPair):
	var idx := kerning_node.get_index()
	
	kerning_node.queue_free()
	font_settings.remove_kerning_pair(idx)
	_emit_changed()


func _on_char_setting_delete(char_node: CharSetting):
	char_node.queue_free()
	font_settings.remove_setting(char_node.for_char)
	_emit_changed()


func _emit_changed():
	emit_signal("changed")


func _on_AddCharSettingButton_pressed():
	_add_char_setting()

func _on_AddKerningButton_pressed():
	_add_kerning_pair()

# ==============================================================

func _on_Ascent_value_changed(value: float):
	var font := font_ref.get_ref() as TextureFont
	if font and font_settings:
		font_settings.ascent = value
	_emit_changed()

func _on_Descent_value_changed(value: float):
	var font := font_ref.get_ref() as TextureFont
	if font and font_settings:
		font_settings.descent = value
#		font.set_cache_descent(0, TextureFont.FONT_SIZE.x, value)
	_emit_changed()

func _on_Gap_value_changed(value: float):
	font_settings.gap = value
	_emit_changed()

func _on_MonoSpaced_toggled(button_pressed: bool):
	font_settings.monospace = button_pressed
	_emit_changed()

func _on_Alignment_value_changed(value: Vector2):
	font_settings.alignment = value
	_emit_changed()


func _on_TextEdit_text_changed():
	font_settings.preview_chars = preview_textbox.text
	_emit_changed()


func _on_Scale_value_changed(value: float):
	preview.set_preview_scale(value / 100.0)


func _on_ColorPickerButton_color_changed(color: Color):
	preview.set_preview_color(color)
	font_settings.preview_color = color
	_emit_changed()

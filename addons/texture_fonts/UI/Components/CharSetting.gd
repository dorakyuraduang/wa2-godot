@tool
extends VBoxContainer

signal changed
signal delete_requested

const Vector2Edit = preload("./Vector2Edit.gd")

var font_settings: TextureFont.Settings
var for_char: String

@export var char_edit: LineEdit
@export var advance: SpinBox
@export var offset: Vector2Edit

func _ready():
	if for_char:
		char_edit.text = for_char
		advance.value = font_settings.get_advance(for_char)
		offset.value = font_settings.get_offset(for_char)


func _on_DeleteButton_pressed():
	emit_signal("delete_requested")


func _on_Char_text_changed(to_char: String):
	if to_char.is_empty():
		if for_char:
			font_settings.remove_setting(for_char)
			emit_signal("changed")
		return
	
	if to_char in font_settings.char_settings:
		return # Already exists, ignore.
	
	if for_char:
		# Already set before, delete old entry (as if renaming).
		font_settings.remove_setting(for_char)
	
	# Add new char setting.
	font_settings.add_setting(to_char)
	font_settings.set_setting(to_char, advance.value, offset.value)
	
	for_char = to_char
	
	emit_signal("changed")


func _on_AdvanceEdit_value_changed(value: float):
	if for_char:
		font_settings.set_advance(for_char, value)
		emit_signal("changed")


func _on_OffsetEdit_value_changed(value: Vector2):
	if for_char:
		font_settings.set_offset(for_char, value)
		emit_signal("changed")

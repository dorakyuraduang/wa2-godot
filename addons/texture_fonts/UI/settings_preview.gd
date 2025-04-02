@tool
extends VBoxContainer

@onready var text_edit: TextEdit = $MarginContainer/TextBox
@onready var colorpicker: ColorPickerButton = $HeadingBox/ColorPickerButton

func set_font(new_font: FontFile):
	text_edit.set("theme_override_fonts/font", new_font)

func set_preview_text(new_text: String):
	text_edit.text = new_text


func set_preview_color(color: Color):
	colorpicker.color = color
	
	var stylebox: StyleBoxFlat = text_edit.get("theme_override_styles/normal")
	stylebox.bg_color = color.darkened(0.2)
	stylebox = text_edit.get("theme_override_styles/focus")
	stylebox.bg_color = color
	
	text_edit.set("theme_override_colors/caret_color", color.inverted())

func set_preview_scale(preview_scale: float):
	text_edit.scale = Vector2.ONE * preview_scale


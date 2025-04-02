@tool
extends TextureRect

var hovering := false
var texture_font_mapping: TextureFont.Mapping

@onready var rect_preview: ColorRect = $RectPreview
@onready var label: Label = $RectPreview/Label

func _ready():
	rect_preview.hide()

func _gui_input(event):
	if hovering and event is InputEventMouseMotion:
		var character := texture_font_mapping.get_char_at_position(event.position)
		if character.is_empty():
			rect_preview.hide()
			return
		
		var rect := texture_font_mapping.get_rect_for_position(event.position)
		rect_preview.position = rect.position
		rect_preview.size = rect.size
		rect_preview.show()
		
		label.add_theme_font_size_override("font_size", rect.size.y * 4)
		label.text = character if character != " " else "Space"
		label.pivot_offset = label.size / 2.0
		label.scale = Vector2.ONE / 4


func _on_TextureView_mouse_entered():
	hovering = true
	rect_preview.show()

func _on_TextureView_mouse_exited():
	hovering = false
	rect_preview.hide()


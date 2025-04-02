@tool
extends MarginContainer

signal file_removed(itself) # Should rename to "remove_requested"?
signal file_changed(itself) # Should rename to "file_selected".

@onready var name_label := $MarginContainer/HBoxContainer/ClipLabel/Label
@onready var preview := $MarginContainer/HBoxContainer/ImageContainer/TexturePreview
@onready var panel := $Panel

var texture: Texture2D:
	set(new):
		texture = new
		
		if not is_node_ready(): await ready
		name_label.text = texture.resource_path
		preview.texture = texture
@onready var selected := false:
	set(new):
		selected = new
		
		if not is_inside_tree():
			return
		
		var stylebox: StyleBoxFlat = panel.get_theme_stylebox("panel")
		
		if selected:
			stylebox.bg_color.a = 0.4
			$Button.disabled = true
			$Button.focus_mode = FOCUS_NONE
		else:
			stylebox.bg_color.a = 0.0
			$Button.disabled = false
			$Button.focus_mode = FOCUS_ALL


func _on_DeleteButton_pressed():
	emit_signal("file_removed", self)
	queue_free()


func _on_ToolButton_pressed():
	emit_signal("file_changed", self)
	selected = true

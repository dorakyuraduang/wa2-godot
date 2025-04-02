@tool
extends VBoxContainer

const TextureView = preload("./TextureView.gd")
const TextureContainer = preload("./TextureContainer.gd")

@onready var texture_view: TextureView = $Control/TextureContainer/TextureView
@onready var texture_container: TextureContainer = $Control/TextureContainer
@onready var zoom_spinbox: SpinBox = $Heading/SpinBox

var hovering := false

var zoom := 100.0:
	set(new_zoom):
		_zoom = clamp(new_zoom, 10, 5000)
		texture_view.scale = Vector2(float(_zoom) / 100.0, float(_zoom) / 100.0)
		texture_view.pivot_offset = texture_view.size / 2.0
		zoom_spinbox.value = zoom
	get:
		return _zoom
var _zoom := 100.0


func set_mapping(new_mapping: TextureFont.Mapping):
	texture_view.texture_font_mapping = new_mapping
	texture = ImageTexture.create_from_image(new_mapping.scaled_image)


func zoom_in():
	zoom *= 1.5

func zoom_out():
	zoom *= 0.5

var texture: Texture2D:
	set(new):
		texture = new
		if not is_node_ready(): await ready
		
		texture_container.texture = texture
		texture_view.texture = texture
		texture_view.size = texture.get_size()
		texture_view.pivot_offset = texture_view.size / 2.0

func _input(event):
	if not hovering:
		return
	
	if event is InputEventMouseButton and event.pressed:
		match event.button_index:
			MOUSE_BUTTON_WHEEL_UP:
				zoom_in()
			MOUSE_BUTTON_WHEEL_DOWN:
				zoom_out()
	elif event is InputEventMouseMotion:
		if event.button_mask > 0:
			texture_container.offset += event.relative


func _on_SpinBox_value_changed(value: float):
	zoom = value

func _on_TextureContainer_mouse_entered():
	hovering = true

func _on_TextureContainer_mouse_exited():
	hovering = false

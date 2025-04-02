@tool
extends MarginContainer

signal changed

const Vector2Edit = preload("./Components/Vector2Edit.gd")
const TextureViewer = preload("./Components/TextureViewer/TextureViewer.gd")

@export var size_edit: Vector2Edit
@export var gap: Vector2Edit
@export var offset: Vector2Edit
@export var chars: TextEdit
@export var texture_viewer: TextureViewer

#@export var scale_edit: SpinBox = $VBoxContainer/RectSettings/Scaling/HBoxContainer/Scale
#@export var interpolation: OptionButton = $VBoxContainer/RectSettings/Scaling/HBoxContainer/Interpolation

var current_mapping: TextureFont.Mapping


#var interpolation_options: Array[Image.Interpolation] = [
	#Image.INTERPOLATE_BILINEAR,
	#Image.INTERPOLATE_CUBIC,
	#Image.INTERPOLATE_LANCZOS,
	#Image.INTERPOLATE_NEAREST,
	#Image.INTERPOLATE_TRILINEAR
#]


#func _ready():
	#interpolation.clear()
	#interpolation.add_item("Bilinear")
	#interpolation.add_item("Cubic")
	#interpolation.add_item("Lanczos")
	#interpolation.add_item("Nearest")
	#interpolation.add_item("Trilinear")


func set_mapping(mapping: TextureFont.Mapping):
	current_mapping = mapping
	
	var image: Image = mapping.source_image
	var max_size := image.get_size()
	
	size_edit.max_value = max_size
	gap.max_value = max_size
	offset.max_value = max_size
	
	size_edit.value = mapping.rect_size
	gap.value = mapping.rect_gap
	offset.value = mapping.texture_offset
	chars.text = mapping.chars
	#scale_edit.value = mapping.scale
	#interpolation.selected = interpolation_options.find(mapping.interpolation)


func _on_Size_value_changed(value: Vector2):
	if current_mapping:
		current_mapping.rect_size = value
		emit_signal("changed")

func _on_Gap_value_changed(value: Vector2):
	if current_mapping:
		current_mapping.rect_gap = value
		emit_signal("changed")

func _on_Offset_value_changed(value: Vector2):
	if current_mapping:
		current_mapping.texture_offset = value
		emit_signal("changed")

func _on_TextEdit_text_changed():
	if current_mapping:
		current_mapping.chars = chars.text
		emit_signal("changed")


#func _on_Scale_value_changed(value):
	#if current_mapping:
		#current_mapping.scale = value
		#var texture := ImageTexture.create_from_image(current_mapping.scaled_image)
		#texture_viewer.set_texture(texture)
		#emit_signal("changed")
#
#
#func _on_OptionButton_item_selected(index):
	#if current_mapping:
		#current_mapping.interpolation = interpolation_options[index]
		#var texture := ImageTexture.create_from_image(current_mapping.scaled_image)
		#texture_viewer.set_texture(texture)
		#emit_signal("changed")

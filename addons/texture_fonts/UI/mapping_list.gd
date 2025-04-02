@tool
extends VBoxContainer

signal textures_dropped(texture_paths: Array[String])

func _can_drop_data(_at_position: Vector2, data: Variant) -> bool:
	var dict := data as Dictionary
	return Array(dict.get("files", [])).any(is_valid_texture)

func _drop_data(_at_position: Vector2, data: Variant) -> void:
	var dict := data as Dictionary
	var dropped_textures: Array[String] = Array(Array(dict.get("files", [])).filter(is_valid_texture), TYPE_STRING, "", null)
	if dropped_textures.is_empty():
		return
	
	textures_dropped.emit(dropped_textures)


static func is_valid_texture(file_path: String) -> bool:
	return ResourceLoader.exists(file_path, "Texture2D")

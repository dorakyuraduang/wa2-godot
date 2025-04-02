@tool
extends MarginContainer

signal closed_requested

# ------ Resources ------

const FileNode = preload("./Components/File.gd")
const MappingSettings = preload("./mapping.gd")
const TextureViewer = preload("./Components/TextureViewer/TextureViewer.gd")
const FontSettings = preload("./settings.gd")

const FILE_NODE_SCENE = preload("./Components/File.tscn")

# ------ References ------

@export var file_list: Container
@export var file_dialog: FileDialog
@export var mapping_settings: MappingSettings
@export var no_selection_overlay: ColorRect
@export var font_settings: FontSettings

@onready var texture_viewer: TextureViewer = mapping_settings.texture_viewer
@onready var font_preview: FontSettings.Preview = font_settings.preview

# ------ Variables ------

var selected_file_node: FileNode:
	set(new):
		selected_file_node = new
		no_selection_overlay.visible = not is_instance_valid(selected_file_node)
var font_ref: WeakRef # Contains a TextureFont reference.

# ------ Inherited Methods -----

# ------ Methods ------

# Reset editor, and populate for new font.
func edit_font(new_font: TextureFont) -> void:
	if font_ref:
		save_now()
	
	font_ref = weakref(new_font)
	for node in file_list.get_children():
		node.queue_free()
	
	no_selection_overlay.show()
	
	for mapping in new_font.texture_mappings:
		_add_mapping_to_ui(mapping.source_texture)
	
	if file_list.get_children().size() > 0:
		change_selected_mapping(0)
	
	if is_inside_tree():
		font_settings.set_font(new_font)
		font_preview.set_font(new_font)


func get_font_from_ref() -> TextureFont:
	var font: TextureFont = font_ref.get_ref()
	
	if not font:
		emit_signal("closed_requested")
	
	return font

func save_now():
	if font_ref and font_ref.get_ref(): # Weird.
		var font := get_font_from_ref()
		
		font.build_font()
	else:
		emit_signal("closed_requested")

# ------ Actions ------


func add_texture(texture: Texture2D, idx := -1):
	_add_mapping_to_ui(texture, idx)
	var font = get_font_from_ref()
	font.add_mapping_from_texture(texture)
	change_selected_mapping(file_list.get_child_count() - 1)

func _add_mapping_to_ui(texture: Texture2D, idx := -1):
	var file_node := FILE_NODE_SCENE.instantiate()
	
	file_list.add_child(file_node)
	file_list.move_child(file_node, idx)
	file_node.texture = texture
	
	file_node.file_removed.connect(_on_file_removed)
	file_node.file_changed.connect(_on_file_changed)


func delete_texture(file_node: FileNode):
	file_node.queue_free()
	
	if file_node == selected_file_node:
		selected_file_node = null
	
	var font := get_font_from_ref()
	font.remove_mapping(file_node.get_index())


func change_selected_mapping(index: int):
	var file_node := file_list.get_child(index)
	if not file_node:
		push_error("TextureFont Editor: Mapping file node at index %d does not exist." % index)
		return
		
	file_node.selected = true
	if selected_file_node:
		selected_file_node.selected = false
	selected_file_node = file_node
	
	var font := get_font_from_ref()
	mapping_settings.set_mapping(font.texture_mappings[index])
	texture_viewer.set_mapping(font.texture_mappings[index])


# ------ Signals ------

func _on_file_removed(file: FileNode): # This looks odd.
	if file == selected_file_node:
		selected_file_node = null
		if file_list.get_children().size() > 0:
			selected_file_node = file_list.get_child(0)
			selected_file_node.selected = true
	
	delete_texture(file)

func _on_file_changed(file: FileNode):
	var idx := file.get_index()
	if idx == -1:
		return
	
	change_selected_mapping(idx)


func _on_AddTextureButton_pressed():
	file_dialog.popup_centered()


func _on_FileDialog_file_selected(path: String):
	var texture := load(path)
	if texture is Texture2D:
		add_texture(texture)
	else:
		printerr("Error loading Texture2D at path ", path, " (Type: ", texture.get_class(), ")")

func _on_Files_textures_dropped(texture_paths: Array[String]) -> void:
	for path in texture_paths:
		var texture = load(path)
		if texture is Texture2D:
			add_texture(texture)

func _on_Settings_changed():
	save_now()

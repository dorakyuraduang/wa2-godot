@tool
extends EditorPlugin

const FontEditor = preload("./UI/font_editor.gd")

var _handled_font_ref: WeakRef
var _font_editor: FontEditor
var _is_font_saved_on_disk := false

func _enter_tree():
	EditorInterface.get_resource_filesystem().filesystem_changed.connect(_on_filesystem_changed)
	#add_custom_type("TextureFont", "FontFile", TextureFont, preload("./Assets/TextureFont.svg"))

func _exit_tree():
	_close_editor()
	#remove_custom_type("TextureFont")

func _handles(object: Object) -> bool:
	if object is TextureFont:
		return true
	if _handled_font_ref: # No longer editing texture fonts.
		_close_editor()
	
	return false

func _edit(object: Object):
	var font := object as TextureFont
	if not font:
		return
	
	_is_font_saved_on_disk = font.resource_path != ""
	_handled_font_ref = weakref(font)
	
	_open_editor(font)


func _open_editor(font: TextureFont):
	if not is_instance_valid(_font_editor):
		_font_editor = preload("./UI/FontEditor.tscn").instantiate()
		add_control_to_bottom_panel(_font_editor, "Texture Font")
		_font_editor.closed_requested.connect(_close_editor)
	
	_font_editor.edit_font(font)
	make_bottom_panel_item_visible(_font_editor)

func _close_editor():
	if is_instance_valid(_font_editor):
		_font_editor.save_now()
		hide_bottom_panel()
		remove_control_from_bottom_panel(_font_editor)
		_font_editor.queue_free()
	_handled_font_ref = null

func _on_filesystem_changed():
	if _handled_font_ref and _is_font_saved_on_disk:
		if not _handled_font_ref.get_ref() or _handled_font_ref.get_ref().resource_path == "":
			_close_editor()

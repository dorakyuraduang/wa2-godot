@tool
extends VBoxContainer

signal value_changed(value: Vector2)

@export var min_value := Vector2(-100.0, -100.0):
	set(new):
		min_value = new
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			x_node.min_value = new.x
			y_node.min_value = new.y
	get:
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			return Vector2(x_node.min_value, y_node.min_value)
		return Vector2.ZERO
@export var max_value := Vector2(100.0, 100.0):
	set(new):
		max_value = new
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			x_node.max_value = new.x
			y_node.max_value = new.y
	get:
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			return Vector2(x_node.max_value, y_node.max_value)
		return Vector2.ZERO
@export var value: Vector2:
	set(new):
		value = new
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			x_node.value = new.x
			y_node.value = new.y
	get:
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			return Vector2(x_node.value, y_node.value)
		return Vector2.ZERO

@export var label := "Label":
	set(new_label):
		label = new_label
		if is_instance_valid(label_node):
			label_node.text = new_label
	get:
		return label_node.text if is_instance_valid(label_node) else ""
@export var suffix := "":
	set(new_suffix):
		suffix = new_suffix
		if is_instance_valid(x_node) and is_instance_valid(y_node):
			_update_suffix(x_node)
			_update_suffix(y_node)

@export var hide_separator := false:
	set(new):
		hide_separator = new
		var h_separator: HSeparator = $Heading/HSeparator
		if h_separator:
			h_separator.visible = not hide_separator


@export_group("Nodes")
@export var label_node: Label
@export var x_node: SpinBox
@export var y_node: SpinBox

func _ready():
	label = label
	value = value
	min_value = min_value
	max_value = max_value
	suffix = suffix
	
	x_node.get_line_edit().text_changed.connect(_update_suffix.bind(x_node).unbind(1))
	y_node.get_line_edit().text_changed.connect(_update_suffix.bind(y_node).unbind(1))


func set_value_no_signal(new_value: Vector2):
	value = new_value
	if is_instance_valid(x_node) and is_instance_valid(y_node):
		x_node.set_value_no_signal(new_value.x)
		y_node.set_value_no_signal(new_value.y)


func _update_suffix(spinbox: SpinBox):
	var suffix_node := spinbox.get_node("Suffix") as Label
	if suffix.is_empty():
		suffix_node.hide()
		return
	
	var original_text = spinbox.get_line_edit().text
	var font := spinbox.get_line_edit().get_theme_default_font()
	var font_size := spinbox.get_line_edit().get_theme_default_font_size()
	var string_size := font.get_string_size(original_text, spinbox.alignment, -1, font_size)
	
	suffix_node.set_anchors_preset(Control.PRESET_FULL_RECT)
	suffix_node.offset_left = spinbox.get_theme_stylebox("normal").content_margin_left + 1 + string_size.x
	suffix_node.offset_top = spinbox.get_theme_stylebox("normal").content_margin_top
	
	suffix_node.text = suffix
	suffix_node.show()


func _on_X_value_changed(new_value: float):
	value.x = new_value
	emit_signal("value_changed", value)
	_update_suffix(x_node)

func _on_Y_value_changed(new_value: float):
	value.y = new_value
	emit_signal("value_changed", value)
	_update_suffix(y_node)


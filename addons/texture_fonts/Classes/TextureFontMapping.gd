@tool
extends Resource

const CharUtils = preload("./Utils.gd")

const INVALID_CHAR_POS := Vector2.INF
const INVALID_CHAR_RECT := Rect2(Vector2.INF, Vector2.INF)


@export var source_texture: Texture2D:
	set(new):
		source_texture = new
		if source_texture:
			source_image = source_texture.get_image()
			scaled_image = create_scaled_image()
		else:
			print("Source texture is null for this texturefontmapping")
var source_image: Image
var scaled_image: Image


func set_texture(new_texture: Texture2D):
	source_texture = new_texture
	#source_image = source_texture.get_image()
	#scaled_image = create_scaled_image()
func get_image() -> Image:
	return scaled_image


@export var rect_size: Vector2
@export var rect_gap: Vector2
@export var texture_offset: Vector2

# Unused.
var scale := 1
#@export var scale := 1:
	#set(new_scale):
		#scale = new_scale
		#scaled_image = create_scaled_image()
#@export var interpolation := Image.INTERPOLATE_NEAREST:
	#set(new_in):
		#interpolation = new_in
		#scaled_image = create_scaled_image()

@export var chars: String:
	set(new_chars):
		chars = new_chars
		char_codes = CharUtils.chars_to_codes(new_chars)

# Array[Array[int]] - 2D Array containing integer char codes.
var char_codes: Array[Array]


func create_scaled_image() -> Image:
	# TODO: Remove scaled image feature. It is not very useful and bloats Resource file size.
	# Perhaps replace it with something better at run-time
	#if scale == 1:
		#return source_image
	#
	#var size := source_image.get_size()
	#var scaled := size * scale
	#
	#if scaled.x > Image.MAX_WIDTH or scaled.y > Image.MAX_HEIGHT:
		#push_error("Could not upscale Image! Scaled Image becomes too large.")
		#return source_image
	#
	#var img := Image.new()
	#img.copy_from(source_image)
	#img.resize(scaled.x, scaled.y, interpolation)
	#
	#return img
	return source_image


func get_char_width(char_code: int) -> int:
	if char_codes.find(char_code) == -1:
		return -1
	
	var rect := get_char_rect(char_code)
	return rect.size.x

func get_char_pos(char_code: int) -> Vector2:
	for i in char_codes.size():
		var line: Array = char_codes[i]
		var pos := line.find(char_code)
		if pos != -1:
			return Vector2(pos, i)
	
	return INVALID_CHAR_POS


func _get_char_rect_unscaled(char_code: int) -> Rect2:
	var pos := get_char_pos(char_code)
	if pos == INVALID_CHAR_POS:
		return INVALID_CHAR_RECT
	
	var rect := Rect2(
			(rect_size + rect_gap) * pos + texture_offset,
			rect_size
	)
	
	var image_size := source_image.get_size()
	if rect.end.x > image_size.x or rect.end.y > image_size.y:
		return INVALID_CHAR_RECT
	
	return rect

func get_char_rect(char_code: int) -> Rect2:
	var rect := _get_char_rect_unscaled(char_code)
	if rect == INVALID_CHAR_RECT:
		return INVALID_CHAR_RECT
	
	rect.position *= scale
	rect.size *= scale
	
	return rect

func get_cropped_char_rect(char_code: int) -> Rect2:
	var rect := _get_char_rect_unscaled(char_code)
	if rect == INVALID_CHAR_RECT:
		return INVALID_CHAR_RECT
	
	var empty_left := _scan_empty_pixels(rect.position.x, rect.end.x, rect.position.y, rect.end.y)
	
	rect.position.x += empty_left
	rect.size.x -= empty_left
	
	if rect.size.x == 0:
		return rect
	
	var empty_right := _scan_empty_pixels(rect.end.x - 1, rect.position.x - 1, rect.position.y, rect.end.y)
	
	rect.size.x -= empty_right
	
	rect.position *= scale
	rect.size *= scale
	
	return rect


# Returns the number of empty columns.
func _scan_empty_pixels(from_x: int, to_x: int, from_y: int, to_y: int) -> int:
	var empty_column_count := 0
	
	var dir := 1
	if from_x > to_x:
		dir = -1
	
	for x in range(from_x, to_x, dir):
		for y in range(from_y, to_y):
			var pix := source_image.get_pixel(x, y)
			if pix.a != 0.0:
				return empty_column_count
		empty_column_count += 1
	
	return empty_column_count


func get_local_position(position: Vector2) -> Vector2:
	var pos := ((position - texture_offset * scale) / ((rect_size + rect_gap) * scale)).floor()
	return pos


func get_char_at_position(position: Vector2) -> String:
	var image_size := scaled_image.get_size()
	if position.x < 0.0 or position.y < 0.0 or position.x > image_size.x or position.y > image_size.y:
		return ""
	
	var pos := get_local_position(position)
	
	if pos.y >= char_codes.size() or pos.y < 0:
		return ""
	
	var line: Array = char_codes[pos.y]
	
	if pos.x >= line.size() or pos.x < 0:
		return ""
	
	return char(line[pos.x])

func get_rect_for_position(position: Vector2) -> Rect2:
	var image_size := scaled_image.get_size()
	if position.x < 0.0 or position.y < 0.0 or position.x > image_size.x or position.y > image_size.y:
		return INVALID_CHAR_RECT
	
	var pos := get_local_position(position)
	return Rect2(
			((rect_size + rect_gap) * pos + texture_offset) * scale,
			rect_size * scale
	)


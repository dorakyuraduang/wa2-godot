@tool
extends Container

const TextureView = preload("./TextureView.gd")

@onready var texture_view: TextureView = $TextureView

var texture: Texture2D:
	set(new):
		texture = new
		offset = Vector2.ZERO

var offset := Vector2.ZERO:
	set(new_offset):
		if not texture:
			return
		
		var rect := get_rect()
		var view_scale = texture_view.scale # Weird.
		
		var total_bounds = (rect.size + texture.get_size() * view_scale) / 2.0
		
		const MARGIN = 8.0
		offset.x = clamp(new_offset.x, -total_bounds.x + MARGIN, total_bounds.x - MARGIN)
		offset.y = clamp(new_offset.y, -total_bounds.y + MARGIN, total_bounds.y - MARGIN)
		rect.position += offset
		
		fit_child_in_rect(texture_view, rect)
		texture_view.scale = view_scale
	get:
		return offset


func _on_TextureContainer_resized():
	offset = offset # Force-calling setter here.

shader_type canvas_item;
uniform sampler2D sampler;
uniform float color_offset:hint_range(0.0, 10.0, 0.1);
uniform int x;
uniform int y;
vec2 flipbook(vec2 uv, vec2 size, float progress){
	progress = floor( mod(progress, (size.x * size.y)) );
	vec2 frame_size = vec2(1.0, 1.0) / vec2(size.x, size.y);
	vec2 frame = fract(uv / size) + frame_size;
	
	frame.x += ( (progress / size.x) - frame_size.x * floor(progress/size.x) * size.x ) - frame_size.x;
	frame.y += (frame_size.y * floor(progress/size.x) ) - frame_size.y ;
	
	return frame;
}

void fragment()
{
	vec2 flip_uv = flipbook(UV, vec2(float(x),float(y)), TIME * 10.);
	vec4 flip_texture = texture(sampler, flip_uv);
	flip_texture=texture(TEXTURE,UV)+vec4(vec3(flip_texture.r,flip_texture.g,flip_texture.b)*color_offset,flip_texture.r*color_offset);
	COLOR = flip_texture;
}
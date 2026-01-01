shader_type canvas_item;
void fragment() {
	vec4 tex_color = texture(TEXTURE, UV);
	COLOR.a=tex_color.r*COLOR.a;
}

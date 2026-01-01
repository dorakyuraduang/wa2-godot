shader_type canvas_item;
void fragment() {
	vec4 tex_color = texture(TEXTURE, UV);
	COLOR.rgb=vec3(0.0);
	COLOR.a=(1.0-tex_color.r)*COLOR.a;
}

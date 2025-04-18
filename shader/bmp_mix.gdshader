shader_type canvas_item;
render_mode blend_mix;
global uniform  vec3 fb;
void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    float bias = length(fb- vec3(0.5)) * 2.0; 
    vec3 target_color = vec3(step(0.5, fb));
    vec3 final_color = mix(tex_color.rgb, target_color, clamp(bias, 0.0, 1.0));
    COLOR = vec4(final_color, tex_color.a);
}
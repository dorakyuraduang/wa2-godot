shader_type canvas_item;
uniform sampler2D cur_texture;
uniform sampler2D mask_texture;
uniform sampler2D next_texture;
uniform vec2 cur_scale=vec2(1.0,1.0);
uniform vec2 cur_offset=vec2(0.0,0.0);
uniform vec2 next_scale=vec2(1.0,1.0);
uniform vec2 next_offset=vec2(0.0,0.0);
uniform float blend:hint_range(0.0, 1.0, 0.01)=0;
uniform float fead_weight:hint_range(0,50)=1;
uniform vec2 canvas_size=vec2(1280,720);
uniform bool center=false;
global uniform vec3 fb;
//vec4 blend_color(vec4 color1,vec4 color2){
		//vec4 res;
		//float sa = 1.0f - color2.a;
		//res.a = color1.a * sa + color2.a;
		//if (res.a == 0.0) {
			//return vec4(0.0);
		//} else {
			//res.r = (color1.r * color1.a * sa + color2.r * color2.a) / res.a;
			//res.g = (color1.g * color1.a * sa + color2.g * color2.a) / res.a;
			//res.b = (color1.b * color1.a * sa + color2.b * color2.a) / res.a;
		//}
		//return res;
//}
vec4 fb_color(vec4 tex_color){
    float bias = length(fb- vec3(0.5)) * 2.0;
    vec3 target_color = vec3(step(0.5, fb));
    vec3 final_color = mix(tex_color.rgb, target_color, clamp(bias*0.6, 0.0, 1.0));
    return vec4(final_color, tex_color.a);
}
vec4 get_color(sampler2D input_texture,vec2 uv,vec2 scale,vec2 offset){
	vec2 texture_size=vec2(textureSize(input_texture,0));
	if(texture_size==vec2(4,4)){
		return vec4(0);
	}
	vec2 size=vec2(textureSize(input_texture,0))/canvas_size;
	vec2 scaled_uv = (uv /scale);
	if(!center){
		offset-=vec2(0,0);
	}else{
		offset-=vec2((canvas_size.x-texture_size.x)*0.5,canvas_size.y-texture_size.y);
	}

    vec2 bg_offset = ((vec2(1.0)-scale)*0.5/scale)-(offset/canvas_size);
    vec2 centered_uv = (scaled_uv - bg_offset)/size;

	if (centered_uv.x > 1.0 || centered_uv.y > 1.0 || centered_uv.x < 0.0 ||centered_uv.y < 0.0) {
		return vec4(0.0);
	}
	vec4 bg_color=texture(input_texture,centered_uv);
	return bg_color;
}
void fragment() {
	vec4 mask_color=texture(mask_texture,UV);
	vec4 cur_color=get_color(cur_texture,UV,cur_scale,cur_offset);
	vec4 next_color=get_color(next_texture,UV,next_scale,next_offset);
	if (next_color.a==0.0){
		next_color=vec4(0.0);
	}
	float result=(blend*2.0-(1.0-mask_color.r))*fead_weight;
	float final=clamp(result,0.0,1.0);

	if(cur_color.a==0.0){
		COLOR=next_color*vec4(1.0,1.0,1.0,blend);
	}else if(next_color.a==0.0){
		COLOR=cur_color*vec4(1.0,1.0,1.0,1.0-blend);
	}else{
		COLOR=mix(cur_color,next_color,final);
	}
	COLOR=fb_color(COLOR);
}

shader_type canvas_item;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D bg2_texture;
uniform sampler2D bg1_texture;
uniform bool chars_priority=false;
uniform vec2 bg1_scale=vec2(1.0,1.0);
uniform vec2 bg1_offset=vec2(0.0,0.0);
uniform vec2 bg2_scale=vec2(1.0,1.0);
uniform vec2 bg2_offset=vec2(0.0);
uniform sampler2D amp;
uniform int amp_mode=2;
uniform bool amp_flag=false;
uniform float amp_strength=0.0;
uniform float bg2_alpha:hint_range(0.0, 1.0, 0.0001);
uniform float time:hint_range(0.0, 1.0, 0.0001);
vec4 blend_color(vec4 color1,vec4 color2){
		vec4 res;
		float sa = 1.0f - color2.a;
		res.a = color1.a * sa + color2.a;
		if (res.a == 0.0) {
			return vec4(0.0);
		} else {
			res.r = (color1.r * color1.a * sa + color2.r * color2.a) / res.a;
			res.g = (color1.g * color1.a * sa + color2.g * color2.a) / res.a;
			res.b = (color1.b * color1.a * sa + color2.b * color2.a) / res.a;
		}
		return res;
}
vec4 get_bg_color(sampler2D bg_texture,vec2 uv,vec2 offset,vec2 scale){
	vec2 size=vec2(textureSize(bg_texture,0))/vec2(1280,720);
	vec2 scaled_uv = (uv /scale);
    vec2 bg_offset = ((vec2(1.0)-scale)/2.0/scale)-(offset/vec2(1280,720));
    vec2 centered_uv = (scaled_uv - bg_offset)/size;
	vec4 bg_color=texture(bg_texture,centered_uv);
	if(textureSize(bg_texture,0)==ivec2(4,4)){	
		bg_color=vec4(0);
	}
	return bg_color;
}
void fragment() {
	vec4 chars_color;
	vec4 bg1_color=get_bg_color(bg1_texture,UV,bg1_offset,bg1_scale);
	vec4 bg2_color=get_bg_color(bg2_texture,UV,bg2_offset,bg2_scale);
    vec4 tex1 = texture(texture1, UV);
    vec4 tex2 = texture(texture2, UV);
    float alpha1 = clamp(1.0 - time, 0.0, 1.0); 
    float alpha2 = clamp(time, 0.0, 1.0); 
	//float bg2_alpha1 = clamp(1.0 - bg2_alpha, 0.0, 1.0); 
    //float bg2_alpha2 = clamp(bg2_alpha, 0.0, 1.0); 
	if(textureSize(texture1,0)==ivec2(4,4) || tex1.a==0.0){	
		tex1=vec4(0);
	}
	if(textureSize(texture2,0)==ivec2(4,4)|| tex2.a==0.0){	
		tex2=vec4(0);
	}
	if (tex1==vec4(0) && tex2!=vec4(0)){
		chars_color=tex2*vec4(1,1,1,alpha2);
	}else if (tex2==vec4(0) && tex1!=vec4(0)){
		chars_color=tex1*vec4(1,1,1,alpha1);
	}else 
	if (tex2==vec4(0) && tex1==vec4(0)){
		chars_color=vec4(0);
	}
	else{
    // 混合两张图片
    	vec4 blendedColor =tex2 * alpha2+tex1 * alpha1;
		chars_color=blendedColor;
	}
	bg2_color.a=bg2_alpha;
	if (amp_flag){
		float bg1_gray=(bg1_color.r+bg1_color.g+bg1_color.b)/3.0;
		float bg2_gray=(bg2_color.r+bg2_color.g+bg2_color.b)/3.0;
		float chars_gray=(chars_color.r+chars_color.g+chars_color.b)/3.0;
		vec4 bg1_colora=vec4(vec3(bg1_gray),1.0);
		vec4 bg2_colora=vec4(vec3(bg2_gray),1.0);
		vec4 chars_colora=vec4(vec3(chars_gray),1.0);
		if ((amp_mode&1)==0){
			bg1_colora=texture(amp,vec2(bg1_gray,bg1_gray));
			bg2_colora=texture(amp,vec2(bg2_gray,bg2_gray));
		}
		if ((amp_mode&2)==0){
			chars_colora=texture(amp,vec2(chars_gray,chars_gray));
		}
		chars_color=mix(vec4(vec3(chars_gray),chars_color.a)* chars_colora,chars_color*chars_colora,amp_strength);
		bg1_color=mix(vec4(vec3(bg1_gray),bg1_color.a)* bg1_colora,bg1_color*bg1_colora,amp_strength);
		bg2_color=mix(vec4(vec3(bg2_gray),bg2_color.a)* bg2_colora,bg2_color*bg2_colora,amp_strength);
	}
	if (chars_priority){
		vec4 final_color=blend_color(bg1_color,bg2_color);
		COLOR=blend_color(final_color,chars_color);
	}else{
		//color1 * (1.0 - color2.a) + color2 * color2.a;
		vec4 final_color=blend_color(bg1_color,chars_color);
		COLOR=blend_color(final_color,bg2_color);
	}
}

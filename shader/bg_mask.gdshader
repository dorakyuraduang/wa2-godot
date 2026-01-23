shader_type canvas_item;
uniform sampler2D mask_texture;
uniform sampler2D next_texture;
uniform vec2 next_scale=vec2(1.0,1.0);
uniform vec2 next_offset=vec2(0.0,0.0);
uniform float blend:hint_range(0.0, 1.0, 0.01)=0;
uniform float fead_weight:hint_range(0,50)=1;
uniform vec2 canvas_size=vec2(1280,720);
uniform bool center=false;
varying vec2 cur_texture_size;
varying vec2 mask_texture_size;
varying vec2 next_texture_size;
void vertex() {
	mask_texture_size = vec2(textureSize(mask_texture, 0));
	next_texture_size = vec2(textureSize(next_texture, 0));
}
vec4 get_color(sampler2D input_texture,vec2 input_size,vec2 uv,vec2 scale,vec2 offset){
	if(input_size==vec2(4,4)){
		return vec4(0);
	}
	vec2 size=vec2(input_size)/canvas_size;
	vec2 scaled_uv = (uv /scale);
	if(!center){
		offset-=vec2(0,0);
	}else{
		offset-=vec2((canvas_size.x-input_size.x)*0.5,canvas_size.y-input_size.y);
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
	vec4 next_color=get_color(next_texture,next_texture_size,UV,next_scale,next_offset);
	if (next_color.a==0.0){
		next_color=vec4(0.0);
	}
	float result=(blend*2.0-(1.0-mask_color.r))*fead_weight;
	float final=clamp(result,0.0,1.0);
	COLOR=mix(vec4(0.0),next_color,final);
}

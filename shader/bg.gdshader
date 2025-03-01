shader_type canvas_item;
uniform sampler2D screen_texture:hint_screen_texture;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform float time:hint_range(0.0, 1.0, 0.0001);
void fragment() {
	float alpha1 = clamp(1.0 - time, 0.0, 1.0); // 第一张图的透明度从1.0到0
	float alpha2 = clamp(time, 0.0, 1.0); 
	vec4 color2=texture(screen_texture,SCREEN_UV);
	vec4 color1=texture(texture1,UV);
	vec4 color3=texture(texture2,UV);
	if(textureSize(texture1,0)==ivec2(4,4)){	
		color2=vec4(0.0,0.0,0.0,0.0);
	}
	else if(textureSize(texture2,0)==ivec2(4,4)){	
		color3=vec4(0.0,0.0,0.0,0.0);
	}
	vec4 color4=mix(color1,color2,color2.a)*alpha1;
	vec4 color5=color3*alpha2;
	COLOR=color4+color5;
}

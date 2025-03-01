shader_type canvas_item;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform float time:hint_range(0.0, 1.0, 0.0001);
void fragment() {
    vec4 tex1 = texture(texture1, UV);
    vec4 tex2 = texture(texture2, UV);
    float alpha1 = clamp(1.0 - time, 0.0, 1.0); // 第一张图的透明度从1.0到0
    float alpha2 = clamp(time, 0.0, 1.0); // 第二张图的透明度从0到1.0
	if(textureSize(texture1,0)==ivec2(4,4) || tex1.a==0.0){	
		COLOR =tex2*vec4(1,1,1,alpha2);
	}
	else if(textureSize(texture2,0)==ivec2(4,4)|| tex2.a==0.0){	
		COLOR =tex1*vec4(1,1,1,alpha1);
	}else{
    // 混合两张图片
    	vec4 blendedColor =tex2 * alpha2+tex1 * alpha1;
    	COLOR = blendedColor;
	}
}

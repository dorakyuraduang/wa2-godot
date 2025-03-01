shader_type canvas_item;
uniform sampler2D amp;
uniform bool activation=false;
uniform float amount=0.0;
void fragment() {
	vec4 color=texture(TEXTURE,UV);
	float gray=(color.r+color.g+color.b)/3.0;
	vec4 colora=texture(amp,vec2(gray,gray));
	if (activation==true){
		COLOR=mix(vec4(gray,gray,gray,1.0)* colora,color*colora,amount);
	}
}
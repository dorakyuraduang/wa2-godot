shader_type particles;
render_mode disable_velocity;
uniform float speed_x;
uniform float speed_y;
uniform int active_count=0;
float rand(uint seed) {
	seed = (seed << 13u) ^ seed;
	return float((seed * (seed * seed * 15731u + 789221u) + 1376312589u) & 0x7fffffffu) / 2147483647.0;
}
void active(bool flag,uint seed,inout vec4 custom){
	int t=int(rand(seed))*4;
	float v1=rand(seed+1u)*360.0;
	float v2=float(v1)/3.141592;
	float v3=rand(seed+2u)*1300.0+100.0;
	float v4=rand(seed+3u)*360.0;
	float v5=rand(seed+4u)*360.0;

	if (flag){

	}else{

	}

}
void reset(){

}
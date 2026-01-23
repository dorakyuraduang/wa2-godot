shader_type particles;
render_mode disable_velocity;
uniform float speed_x;
uniform float speed_y;
uniform bool disable;
const int drift_table[256] =int[256](
		0, 100, 200, 301, 401, 501, 601, 700, 799, 897, 995, 1092, 1189, 1284, 1379, 1474,
		1567, 1659, 1751, 1841, 1930, 2018, 2105, 2191, 2275, 2358, 2439, 2519, 2598, 2675, 2750, 2824,
		2896, 2966, 3034, 3101, 3166, 3229, 3289, 3348, 3405, 3460, 3513, 3563, 3612, 3658, 3702, 3744,
		3784, 3821, 3856, 3889, 3919, 3947, 3973, 3996, 4017, 4035, 4051, 4065, 4076, 4084, 4091, 4094,
		4096, 4094, 4091, 4084, 4076, 4065, 4051, 4035, 4017, 3996, 3973, 3947, 3919, 3889, 3856, 3821,
		3784, 3744, 3702, 3658, 3612, 3563, 3513, 3460, 3405, 3348, 3289, 3229, 3166, 3101, 3034, 2966,
		2896, 2824, 2750, 2675, 2598, 2519, 2439, 2358, 2275, 2191, 2105, 2018, 1930, 1841, 1751, 1659,
		1567, 1474, 1379, 1284, 1189, 1092, 995, 897, 799, 700, 601, 501, 401, 301, 200, 100,
		0,-100,-200,-301,-401,-501,-601,-700,-799,-897,-995,-1092,-1189,-1284,-1379,-1474,
		-1567,-1659,-1751,-1841,-1930,-2018,-2105,-2191,-2275,-2358,-2439,-2519,-2598,-2675,-2750,-2824,
		-2896,-2966,-3034,-3101,-3166,-3229,-3289,-3348,-3405,-3460,-3513,-3563,-3612,-3658,-3702,-3744,
		-3784,-3821,-3856,-3889,-3919,-3947,-3973,-3996,-4017,-4035,-4051,-4065,-4076,-4084,-4091,-4094,
		-4096,-4094,-4091,-4084,-4076,-4065,-4051,-4035,-4017,-3996,-3973,-3947,-3919,-3889,-3856,-3821,
		-3784,-3744,-3702,-3658,-3612,-3563,-3513,-3460,-3405,-3348,-3289,-3229,-3166,-3101,-3034,-2966,
		-2896,-2824,-2750,-2675,-2598,-2519,-2439,-2358,-2275,-2191,-2105,-2018,-1930,-1841,-1751,-1659,
		-1567,-1474,-1379,-1284,-1189,-1092,-995,-897,-799,-700,-601,-501,-401,-301,-200,-100
	   );
float rand(uint seed) {
	seed = (seed << 13u) ^ seed;
	return float((seed * (seed * seed * 15731u + 789221u) + 1376312589u) & 0x7fffffffu) / 2147483647.0;
}
void active(bool flag1,bool flag2,uint seed,inout vec4 custom,inout vec4 userdata1){
	int t=int(rand(seed))*4;
	userdata1.y=float(t);
	float v1=rand(seed+1u)*360.0;
	custom.x=float(v1)/3.141592;
	float v3;
	float v4;
	float v5;
	//float v3=rand(seed+2u)*1300.0+100.0;
	//float v4=rand(seed+3u)*360.0;
	//float v5=rand(seed+4u)*360.0;
	if (flag1){

	}else{
		if(flag2){

		}else{
			custom.y=rand(seed+2u)*100.0+100.0;
		}
		v4=1.0;
		v5=0.0;
	}
	custom.w=0.0;
	userdata1.x=rand(seed+3u)*30.0/10.0-1.0;
	if (userdata1.x>=0.0){
		if(userdata1.x<0.4){
			userdata1.x=0.4;
		}
	}
	if (0.0>userdata1.x && userdata1.x>-0.4){
		userdata1.x=-0.4;
	}
	custom.z=rand(seed+4u)*360.0;
	float v10=100.0*float(4-t);
	float v11=rand(seed+5u)*v10;
	float v12=v11/100.0+1.0;
	float v13=v12*0.5;
}
void start(){
	if (USERDATA3.x==0.0){
		active(false,false,NUMBER,CUSTOM,USERDATA1);
		float v29=CUSTOM.w;
		float a4=0.0;
		float v1=sin(CUSTOM.z/3.141592)/(CUSTOM.y*0.5+1.0)+(CUSTOM.y-1.0);
		float pos_x=(cos(v1)*CUSTOM.y*USERDATA1.x+640.0)+(CUSTOM.w*float(drift_table[int(CUSTOM.w)%4096%256]) * 0.000244140625 *a4/100.0);
		float pos_y=(sin(v1)*CUSTOM.y*USERDATA1.x+360.0)+(CUSTOM.w*float(drift_table[int(CUSTOM.w)%4096%256]) * 0.000244140625 *a4/100.0);
		TRANSFORM[3].xy = vec2(pos_x, pos_y);
		USERDATA3.x=1.0;
	}
}
void process(){
	uint seed=NUMBER;

	if (CUSTOM.z<360.0){
		CUSTOM.z+=0.2;
	}else{
		CUSTOM.z=0.0;
	}
	CUSTOM.y+=(CUSTOM.y+100.0+speed_y)/400.0;
	if (CUSTOM.y>=1400.0){
		CUSTOM.y=rand(seed)*100.0+100.0;
	}
	//float v29=CUSTOM.w;
	float a4=0.0;
	float v1=sin(CUSTOM.z/3.141592)/(CUSTOM.y*0.5+1.0)+(CUSTOM.x);
	float pos_x=(cos(v1)*CUSTOM.y*USERDATA1.x+640.0)+(CUSTOM.w*float(drift_table[int(CUSTOM.w)%4096%256]) * 0.000244140625 *a4/100.0);
	float pos_y=(sin(v1)*CUSTOM.y*USERDATA1.x+360.0)+(CUSTOM.w*float(drift_table[(int(CUSTOM.w)%4096%+64)%256]) * 0.000244140625 *a4/100.0);
	if (CUSTOM.y>=450.0){
		if (CUSTOM.y>=1300.0){
			COLOR.a=(32.0-(CUSTOM.y-1300.0)*32.0/100.0)/255.0;
		}else{
			COLOR.a=(125.0-(CUSTOM.y-450.0)*0.25)/255.0;;
			if (COLOR.a>=1.0){
				COLOR.a=1.0;
			}else if(COLOR.a<=0.125){
				COLOR.a=0.125;
			}
		}
	}else{
		COLOR.a=(CUSTOM.y-200.0)*0.5/255.0;
	}
	TRANSFORM[3].xy = vec2(pos_x, pos_y);
	float scale=(5.0-USERDATA1.y)*CUSTOM.y/100.0/abs(USERDATA1.x)*0.5/32.0;
	TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * scale;
	TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * scale;
}
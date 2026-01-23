// NOTE: Shader automatically converted from Godot Engine 4.5.1.stable's ParticleProcessMaterial.

shader_type particles;
render_mode disable_velocity;
uniform float speed_x;
uniform float speed_y;
uniform int mask;
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
void reset(uint seed,inout vec4 custom, inout vec4 userdata1,inout vec4 userdata2,inout vec4 userdata3){
	int v3=int(rand(seed + 78u) * 32.0);
	float t;
	float ssx;
	float ssy;
	bool run_label7=false;
	userdata2.z=256.0;
	userdata1.z=0.0;
	if(v3==0){
		t = rand(seed + 77u) * 16.0;
		ssx = (rand(seed + 4u) *(100.0*(64.0 - t))/100.0 + 1.0) * 0.5;
		ssy = (rand(seed + 5u) *(100.0*(64.0 - t))/100.0 + 1.0) * 0.5;
		ssx = rand(seed + 4u) *8.0+10.0;
		ssy = rand(seed + 5u) *8.0+10.0;
		if ((mask&0x200)!=0){
			t = rand(seed + 77u) * 48.0+192.0;
			ssx = rand(seed + 4u) *4.0+5.0;
			ssy = rand(seed + 5u) *4.0+5.0;
			if ((mask&0x400)!=0){
				run_label7=true;
			}
		}
	}
	else if (3<=2){
		t = rand(seed + 77u) * 48.0+192.0;
		ssx = rand(seed + 4u) *4.0+5.0;
		ssy = rand(seed + 5u) *4.0+5.0;
		if ((mask&0x400)!=0){
			run_label7=true;
		}
	}else{
		run_label7=true;
	}
	if (run_label7){
		t = rand(seed + 77u) * 8.0+248.0;
		if ((mask&0x800)!=0){
			ssx=rand(seed + 4u)*3.0+1.0;
			ssy=(rand(seed+5u)*3.0-2.0*t+513.0)*0.5;
			userdata3.z=rand(seed+11u)*4.0;
		}else{
			ssx=(rand(seed + 4u) *(100.0*(256.0 - t))/100.0 + 1.0) * 0.5;
			ssy= (rand(seed + 5u) *(100.0*(256.0 - t))/100.0 + 1.0) * 0.5;
		}		
	}
	float start_x = rand(seed + 2u) * (4.0 * (576.0 - t)) - (2.0 * (256.0 - t));
	float start_y = -(rand(seed + 3u) * 100.0) - (2.0 * (256.0 - t));
	userdata2.w=t;
	userdata2.x=start_x;
	userdata2.y=start_y;
	if(t>16.0){
		if(t>240.0){
			userdata1.y=((256.0-t)*8.0+32.0)/256.0;
		}else{
			userdata1.y=0.5;
		}
	}else{
		userdata1.y=0.125;
	}
	if((mask&0x800)!=0){
		userdata1.y=0.75;
	}
	custom.xy = vec2(start_x, start_y); // 坐标
	custom.zw = vec2(ssx, ssy);         // 速度缩放系数 (持久化存储)
	userdata1.x=rand(seed + 6u) *256.0;
	//userdata1.z=0.0;
	//userdata1.w=0.0;
	if (t<=240.0){
		userdata3.y=((userdata2.z-t+1.0)*2.0)/32.0;
	}else{	
		if((mask&0x0800)!=0){
			userdata3.y=((userdata2.z-t+1.0)*2.0)/32.0;
		}else{
			userdata3.y=((userdata2.z-t+1.0)*4.0)/32.0;
		}
	}
}
void start(){

	if (USERDATA3.x==0.0){
		reset(NUMBER + uint(TIME * 100.0),CUSTOM,USERDATA1,USERDATA2,USERDATA3);
		TRANSFORM[3].xy = CUSTOM.xy;
		TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * USERDATA3.y;
		TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * USERDATA3.y;
		COLOR.a=USERDATA1.y;
		USERDATA3.x=1.0;
	}
}
void process() {
	uint seed = NUMBER;
    // 垂直下落，并使用 mod 实现超出屏幕底部后自动回到顶部
	float ssx = CUSTOM.z;
	float ssy = CUSTOM.w;
	float my_speed_x = speed_x * ssx;
	float my_speed_y = speed_y * ssy;
	USERDATA1.x++;
	// 更新 y 坐标 (匀速下落示例)
	float v69;
	float v70;
	float v71;
	bool mask800_flag=((mask&0x800)!=0);
	if (mask800_flag){
		v69=0.000244140625*float(drift_table[(int(USERDATA1.x)+64)%256])*speed_y*ssx;
		v70=(USERDATA2.x-640.0)/(5120.0/((USERDATA2.z+1.0)-USERDATA2.w));
		v71=v69/40.0+my_speed_x/10.0;	
	}else{
		v70=0.000244140625*float(drift_table[(int(USERDATA1.x)+64)%256])*speed_y*ssx/40.0;
		v71=my_speed_x/10.0;
	}
	CUSTOM.x += v70+v71;
	CUSTOM.y+=my_speed_y/10.0;
	if((CUSTOM.y>720.0||((CUSTOM.x<(4.0*USERDATA2.w-1024.0)||CUSTOM.x>(2.0*(896.0-USERDATA2.w))))&&mask800_flag)){
		reset(NUMBER,CUSTOM,USERDATA1,USERDATA2,USERDATA3);
		TRANSFORM[3].xy = CUSTOM.xy;
		TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * USERDATA3.y;
		TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * USERDATA3.y;
		COLOR.a=USERDATA1.y;
	}
	else if(CUSTOM.x>(2.0*(896.0-USERDATA2.w))&&!mask800_flag){
		CUSTOM.x-=2.0*(896.0-USERDATA2.w);
		TRANSFORM[3].xy = vec2(CUSTOM.x, CUSTOM.y);
	}else if(CUSTOM.x<(4.0*USERDATA2.w-1024.0)&&!mask800_flag){
		CUSTOM.x+=2.0*(896.0-USERDATA2.w);
		TRANSFORM[3].xy = vec2(CUSTOM.x, CUSTOM.y);
	}else{
		TRANSFORM[3].xy = vec2(CUSTOM.x, CUSTOM.y);
	}

}

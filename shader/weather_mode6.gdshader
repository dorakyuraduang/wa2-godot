shader_type particles;
render_mode disable_velocity;
uniform float speed_x;
uniform float speed_y;
uniform float thrbulence = 0.0;
uniform int mask = 0;
uniform bool disable;
uniform int large_layer_mode = 0;
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

void active(bool initial_fill, uint seed, inout vec4 custom, inout vec4 userdata1) {
	float particle_type = floor(rand(seed) * 4.0);
	if (large_layer_mode == 1) {
		particle_type = floor(rand(seed) * 3.0) + 1.0;
	} else if (large_layer_mode == 2) {
		particle_type = 0.0;
	}
	userdata1.y = particle_type;
	custom.x = rand(seed + 4u) * 360.0;
	custom.y = 1.0;
	custom.z = 0.0;
	if (initial_fill) {
		custom.w = floor(rand(seed + 2u) * 360.0);
		float initial_accel = speed_y * (particle_type + 2.0) / 10000.0;
		custom.z = initial_accel * custom.w;
		custom.y += initial_accel * custom.w * (custom.w + 1.0) * 0.5;
	} else {
		custom.w = 0.0;
	}
	float scale_limit = 100.0 * (4.0 - particle_type);
	userdata1.x = (floor(rand(seed + 5u) * scale_limit) / 100.0 + 1.0) * 0.5;
	userdata1.z = rand(seed + 3u) * 3.0 - 1.0;
	if (userdata1.z >= 0.0 && userdata1.z < 0.4) {
		userdata1.z = 0.4;
	}
	if (userdata1.z < 0.0 && userdata1.z > -0.4) {
		userdata1.z = -0.4;
	}
}

vec2 particle_position(vec4 custom, vec4 userdata1) {
	float angle = (custom.x + userdata1.x * custom.w * speed_x / 50.0) * 0.01745329238474369;
	int drift_base = int(mod(custom.w, 4096.0));
	float drift_x = custom.w * float(drift_table[drift_base % 256]) * 0.000244140625 * thrbulence / 100.0;
	float drift_y = custom.w * float(drift_table[(drift_base + 64) % 256]) * 0.000244140625 * thrbulence / 100.0;
	return vec2(
		640.0 - cos(angle) * custom.y * speed_y * 0.5 + drift_x,
		360.0 + sin(angle) * custom.y * speed_y * 0.5 + drift_y
	);
}

bool outside_particle_area(vec2 pos) {
	float lower = 2.0 * (-150.0 - thrbulence);
	return pos.x < lower || pos.x >= 2.0 * thrbulence + 1580.0 || pos.y < lower || pos.y >= 2.0 * thrbulence + 1020.0;
}

float particle_alpha_from_radius(float radius) {
	float distance = radius * speed_y;
	float alpha;
	if (distance >= 450.0) {
		alpha = 200.0 - (distance - 450.0) / 5.0;
	} else {
		alpha = distance - 30.0;
	}
	alpha = clamp(alpha, 0.0, 200.0);
	return alpha / 255.0;
}

float particle_scale(vec4 custom, vec4 userdata1) {
	return (5.0 - userdata1.y) * max(custom.y, 1.0) * 0.25 / 32.0;
}

void start(){
	if (USERDATA3.x==0.0){
		active(true, NUMBER, CUSTOM, USERDATA1);
		TRANSFORM[3].xy = particle_position(CUSTOM, USERDATA1);
		COLOR.a = particle_alpha_from_radius(CUSTOM.y);
		float scale = particle_scale(CUSTOM, USERDATA1);
		TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * scale;
		TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * scale;
		USERDATA3.x=1.0;
	}
}
void process(){
	uint seed=NUMBER;

	if (CUSTOM.x<360.0){
		CUSTOM.x += (USERDATA1.y + 2.0) * speed_x / 100.0;
	}else{
		CUSTOM.x=0.0;
	}
	vec2 pos = particle_position(CUSTOM, USERDATA1);
	if (outside_particle_area(pos)) {
		active(false, seed + uint(CUSTOM.w) + 17u, CUSTOM, USERDATA1);
		pos = particle_position(CUSTOM, USERDATA1);
	}
	CUSTOM.z += speed_y * (USERDATA1.y + 2.0) / 20000.0;
	CUSTOM.y += CUSTOM.z;
	CUSTOM.w += 1.0;
	TRANSFORM[3].xy = pos;
	COLOR.a = particle_alpha_from_radius(CUSTOM.y);
	float scale = particle_scale(CUSTOM, USERDATA1);
	TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * scale;
	TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * scale;
}

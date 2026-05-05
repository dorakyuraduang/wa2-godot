shader_type particles;
render_mode disable_velocity;

uniform float speed_x;
uniform float speed_y;
uniform float texture_scale = 1.0;
uniform int initial_fill_count = 0;
uniform int forced_type = -1;
uniform bool disable;

const int drift_table[256] = int[256](
	0, 100, 200, 301, 401, 501, 601, 700, 799, 897, 995, 1092, 1189, 1284, 1379, 1474,
	1567, 1659, 1751, 1841, 1930, 2018, 2105, 2191, 2275, 2358, 2439, 2519, 2598, 2675, 2750, 2824,
	2896, 2966, 3034, 3101, 3166, 3229, 3289, 3348, 3405, 3460, 3513, 3563, 3612, 3658, 3702, 3744,
	3784, 3821, 3856, 3889, 3919, 3947, 3973, 3996, 4017, 4035, 4051, 4065, 4076, 4084, 4091, 4094,
	4096, 4094, 4091, 4084, 4076, 4065, 4051, 4035, 4017, 3996, 3973, 3947, 3919, 3889, 3856, 3821,
	3784, 3744, 3702, 3658, 3612, 3563, 3513, 3460, 3405, 3348, 3289, 3229, 3166, 3101, 3034, 2966,
	2896, 2824, 2750, 2675, 2598, 2519, 2439, 2358, 2275, 2191, 2105, 2018, 1930, 1841, 1751, 1659,
	1567, 1474, 1379, 1284, 1189, 1092, 995, 897, 799, 700, 601, 501, 401, 301, 200, 100,
	0, -100, -200, -301, -401, -501, -601, -700, -799, -897, -995, -1092, -1189, -1284, -1379, -1474,
	-1567, -1659, -1751, -1841, -1930, -2018, -2105, -2191, -2275, -2358, -2439, -2519, -2598, -2675, -2750, -2824,
	-2896, -2966, -3034, -3101, -3166, -3229, -3289, -3348, -3405, -3460, -3513, -3563, -3612, -3658, -3702, -3744,
	-3784, -3821, -3856, -3889, -3919, -3947, -3973, -3996, -4017, -4035, -4051, -4065, -4076, -4084, -4091, -4094,
	-4096, -4094, -4091, -4084, -4076, -4065, -4051, -4035, -4017, -3996, -3973, -3947, -3919, -3889, -3856, -3821,
	-3784, -3744, -3702, -3658, -3612, -3563, -3513, -3460, -3405, -3348, -3289, -3229, -3166, -3101, -3034, -2966,
	-2896, -2824, -2750, -2675, -2598, -2519, -2439, -2358, -2275, -2191, -2105, -2018, -1930, -1841, -1751, -1659,
	-1567, -1474, -1379, -1284, -1189, -1092, -995, -897, -799, -700, -601, -501, -401, -301, -200, -100
);

float rand(uint seed) {
	seed = (seed << 13u) ^ seed;
	return float((seed * (seed * seed * 15731u + 789221u) + 1376312589u) & 0x7fffffffu) / 2147483647.0;
}

void activate(bool initial_fill, uint seed, inout vec4 custom, inout vec4 userdata1) {
	float type_roll = rand(seed + 10u);
	float particle_type = 0.0;
	if (forced_type >= 0) {
		particle_type = float(forced_type);
	} else if (type_roll >= 0.05) {
		particle_type = floor(rand(seed + 11u) * 2.0) + 1.0;
	}

	custom.x = floor(rand(seed + 1u) * 1280.0);
	if (initial_fill) {
		custom.y = floor(rand(seed + 2u) * 720.0);
	} else {
		custom.y = -40.0 - floor(rand(seed + 2u) * 100.0);
	}

	if (particle_type == 0.0) {
		custom.z = 8.0;
		custom.w = 8.0;
		userdata1.z = forced_type >= 0 ? texture_scale : 4.0;
	} else {
		float speed_limit = 100.0 * (3.0 - particle_type);
		custom.z = (floor(rand(seed + 3u) * speed_limit) / 100.0 + 1.0) * 0.5;
		custom.w = (floor(rand(seed + 4u) * speed_limit) / 100.0 + 1.0) * 0.5;
		userdata1.z = forced_type >= 0 ? texture_scale : (particle_type == 1.0 ? 1.0 : 0.5);
	}

	userdata1.x = floor(rand(seed + 5u) * 256.0);
	userdata1.y = particle_type;
}

void start() {
	if (USERDATA3.x == 0.0) {
		activate(float(NUMBER) < float(initial_fill_count), NUMBER, CUSTOM, USERDATA1);
		TRANSFORM[3].xy = CUSTOM.xy;
		COLOR.a = 1.0;
		float scale = USERDATA1.z;
		TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * scale;
		TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * scale;
		USERDATA3.x = 1.0;
	}
}

void process() {
	uint seed = NUMBER + uint(CUSTOM.y + CUSTOM.x) + uint(USERDATA1.x);
	float drift = float(drift_table[(int(USERDATA1.x) + 64) % 256]) * CUSTOM.z * 0.000244140625;
	CUSTOM.x += drift + CUSTOM.z * speed_x / 10.0;
	CUSTOM.y += CUSTOM.w * speed_y / 10.0;

	if (CUSTOM.y > 720.0) {
		activate(false, seed + 17u, CUSTOM, USERDATA1);
	} else {
		if (CUSTOM.x >= 1280.0) {
			CUSTOM.x -= 1312.0;
		}
		if (CUSTOM.x < -32.0) {
			CUSTOM.x += 1312.0;
		}
	}

	USERDATA1.x += 1.0;
	TRANSFORM[3].xy = CUSTOM.xy;
	COLOR.a = 1.0;
	float scale = USERDATA1.z;
	TRANSFORM[0].xy = normalize(TRANSFORM[0].xy) * scale;
	TRANSFORM[1].xy = normalize(TRANSFORM[1].xy) * scale;
}

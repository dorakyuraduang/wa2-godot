shader_type canvas_item;


uniform sampler2D y_data;
uniform sampler2D u_data;
uniform sampler2D v_data;

uniform vec2 resolution;
uniform vec4 color_profile;
uniform float rotation;



void fragment() {
	vec2 uv = UV;
	vec2 centered_uv = uv - vec2(0.5);
	float cos_angle = cos(rotation);
	float sin_angle = sin(rotation);
	vec2 rotated_uv = vec2(
		centered_uv.x * cos_angle - centered_uv.y * sin_angle,
		centered_uv.x * sin_angle + centered_uv.y * cos_angle
	);

	uv = rotated_uv + vec2(0.5);

	vec2 y_uv = uv * resolution / vec2(textureSize(y_data, 0));
	y_uv = clamp(y_uv, vec2(0.0), vec2(1.0));

	vec2 u_uv = uv * ((resolution / 2.0) / vec2(textureSize(u_data, 0)));
    vec2 v_uv = uv * ((resolution / 2.0) / vec2(textureSize(v_data, 0)));

    u_uv = clamp(u_uv, vec2(0.0), vec2(1.0));
    v_uv = clamp(v_uv, vec2(0.0), vec2(1.0));
	
	float Y = float(texture(y_data, y_uv).r);
	float U = float(texture(u_data, u_uv).r - 0.5);
	float V = float(texture(v_data, v_uv).r - 0.5);
	
	float R = Y + color_profile.x * V;
	float G = Y - color_profile.y * U - color_profile.z * V;
	float B = Y + color_profile.w * U;
	
	COLOR = vec4(R, G, B, 1.0);
}


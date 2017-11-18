#version 410 core

in GS_OUT {
	// vec3 frag_pos;
	vec3 normal;
} gs_out;

out vec3 fs_color;

uniform vec3 camera_direction;
uniform vec3 object_color;

void main()
{
	vec3 lightColor = vec3(1.0, 1.0, 1.0);

	// Ambient
	float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

	// Diffuse
	vec3 lightDir = normalize(camera_direction);
	vec3 diffuse = lightColor * abs(dot(lightDir, gs_out.normal)); // Diffuse lighting

	// Specular
	// float specularStrength = 0.5;
	// vec3 viewDir = normalize(camera_position - gs_out.frag_pos);
	// vec3 reflectDir = reflect(- lightDir, gs_out.normal);
	// float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	// vec3 specular = specularStrength * spec * lightColor;
	
	// Final color
	fs_color = (ambient + diffuse) * object_color;
}

#version 330 core

in vec2 UV;
out vec3 color;

uniform sampler2D texture_sampler;

void main()
{
	color = texture(texture_sampler, UV).rgb;
}
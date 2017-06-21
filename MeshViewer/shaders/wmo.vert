#version 410 core

layout (location = 0) in vec3 vertexPosition_modelSpace;
layout (location = 2) in mat4 instance_position;

uniform mat4 projection_view;

void main()
{
	vec4 position = instance_position * vec4(vertexPosition_modelSpace, 1);

	gl_Position = projection_view * vec4(-position.x, -position.y, position.z, 1.0);
}

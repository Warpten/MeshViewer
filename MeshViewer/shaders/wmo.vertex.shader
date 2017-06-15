#version 330 core

layout (location = 0) in vec3 vertexPosition_modelSpace;
layout (location = 2) in mat4 instance_position;

uniform mat4 projection_view;

out vec2 UV;

void main()
{
	gl_Position = projection_view * instance_position * vec4(vertexPosition_modelSpace, 1);

	UV = vertexPosition_modelSpace.xy;
}

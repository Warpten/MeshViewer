#version 330 core

in vec3 vertexPosition_modelSpace;
uniform mat4 modelViewProjection;

out vec2 UV;

void main()
{
	gl_Position = modelViewProjection * vec4(vertexPosition_modelSpace, 1.0);

	UV = vertexPosition_modelSpace.xy;
}

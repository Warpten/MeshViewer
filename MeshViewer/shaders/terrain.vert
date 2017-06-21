#version 410 core

layout (location = 0) in vec3 vertexPosition_modelSpace;

uniform mat4 modelViewProjection;

void main()
{
	gl_Position = modelViewProjection * vec4(vertexPosition_modelSpace, 1.0);

	// vs_out.uv = vertexPosition_modelSpace.xy;
}

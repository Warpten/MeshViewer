#version 410 core

layout(points) in;
layout(line_strip, max_vertices = 64) out;
layout(invocations = 2) in;

const float PI = 3.1415926f;
const float radius = 2.0f;

void main()
{
	for (int i = 0; i < 32; ++i)
	{
		float angle = PI * 2.0f / 10.0f * i;

		vec4 offset = vec4(cos(angle) * 0.3, -sin(angle) * 0.4, gl_InvocationID * 2 - 1.0f, 0.0);
		gl_Position = gl_in[0].gl_Position + offset;
		EmitVertex();
	}

	EndPrimitive();
}

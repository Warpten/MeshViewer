#version 410 core

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

out GS_OUT {
	vec3 normal;
} gs_out;

void main()
{
	vec3 a = vec3(gl_in[1].gl_Position) - vec3(gl_in[0].gl_Position);
	vec3 b = vec3(gl_in[2].gl_Position) - vec3(gl_in[0].gl_Position);
	gs_out.normal = normalize(cross(a, b));
	
	gl_Position = gl_in[0].gl_Position;
	EmitVertex();

	gl_Position = gl_in[1].gl_Position;
	EmitVertex();

	gl_Position = gl_in[2].gl_Position;
	EmitVertex();

	EndPrimitive();
}

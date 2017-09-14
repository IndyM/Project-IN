#version 430 core				

uniform float time;
uniform mat4 camera;

in vec3 position;
in vec3 normal;
in vec3 instancePosition;

out vec3 n;
out vec4 pos_vertex;

void main() 
{
	n = normal;

	vec3 pos = position + instancePosition;
	vec4 pos_vertex = camera * vec4(pos, 1.0);
	gl_Position = pos_vertex;
}
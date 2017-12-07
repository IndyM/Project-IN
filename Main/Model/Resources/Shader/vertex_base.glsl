#version 430 core				

uniform mat4 camera;
uniform vec3 instancePosition;// just for Cut
uniform vec4 baseColor;

in vec3 position;
in vec3 normal;

in vec3 edgePosition; // just for Cut

out vec3 n;
out vec3 pos_vertex;
out vec4 colorBase;
out vec3 pos_edge; // just for Cut

void main() 
{
	n = normal;
	colorBase = baseColor;
	pos_edge = edgePosition;

	pos_vertex = position ;
	vec4 pos = camera * vec4(pos_vertex+ instancePosition, 1.0);
	gl_Position = pos;
}
#version 430 core

in vec3 n;
in vec3 pos_vertex;
in vec3 pos_edge;

out vec4 color;

void main() 
{
	float edge_thickness = 0.02;

	float w_edge = 1.0-edge_thickness;
	vec3 abs_pos =  abs(pos_edge);

	vec3 step_pos = step(w_edge, abs_pos);
	float sum = step_pos.x + step_pos.y + step_pos.z;

	if(sum<=1){
		color = vec4(.0,.3,.3, .1);
	}
	else{
		color = vec4(.0,1.0,1.0, 1.0);
	
	}
}
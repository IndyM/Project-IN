#version 430 core

in vec3 n;
in vec3 pos_vertex;

out vec4 color;

void main() 
{
	float w_edge = 1.0-0.01;
	float scale = 100.0;
	vec3 abs_pos = abs(pos_vertex.xyz/(0.5*scale));
	vec3 pow_abs_pos = pow(abs_pos,vec3(50.0));
	//if(any(lessThan(abs_pos, vec3(1.0-w_edge)))){
	int count1=0;
	//abs_pos+= vec3(.0,.0,0.011);
	if(abs_pos.x>=w_edge){
		count1++;
		}
	if(abs_pos.y>=w_edge){
		count1++;
		}
	if(abs_pos.z>=w_edge){
		count1++;
		}
	if(count1==1){
	//if(!any(greaterThan(abs_pos, vec3(w_edge)))){
		color = vec4(.0,.3,.5, .1);
		}
	else{
		color = vec4(.0,1.0,1.0, 1.0);
	
	}

	//color = vec4(n,1.0);
	
}
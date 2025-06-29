shader_type particles;
uniform int mode;
int random(float x)
{
    float y = fract(sin(x)*100000.0);
    return int(y);
}
void start(){
	if (mode==3){
		VELOCITY.y=float(random(1000));
	}
}
void process(){
	VELOCITY.y+=10.0;
}
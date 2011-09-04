#define MAX_LIGHTS 8
#define NUM_LIGHTS 1
varying vec3 normal, lightDir[MAX_LIGHTS], eyeVec;
varying vec2 texCoord;

void main()
{	
  gl_Position = ftransform();		
  normal = gl_NormalMatrix * gl_Normal;
  vec4 vVertex = gl_ModelViewMatrix * gl_Vertex;
  eyeVec = -vVertex.xyz;
  texCoord = gl_MultiTexCoord0.xy;
  int i;
  for (i=0; i<NUM_LIGHTS; ++i)
    lightDir[i] = vec3(gl_LightSource[i].position.xyz - vVertex.xyz);
}

#define MAX_LIGHTS 8
#define NUM_LIGHTS 1
varying vec3 normal, lightDir[MAX_LIGHTS], eyeVec;
varying vec2 texCoord;

uniform sampler2D colorMap;
//uniform int numLights;

void main (void)
{
  int numLights = 1;
  vec4 base_color = texture2D(colorMap, texCoord);
  //vec4 final_color = gl_FrontLightModelProduct.sceneColor;
  vec4 final_color = (gl_FrontLightModelProduct.sceneColor * gl_FrontMaterial.ambient) + 
	(gl_LightSource[0].ambient * gl_FrontMaterial.ambient);
  vec3 N = normalize(normal);
  int i;
  for (i=0; i<NUM_LIGHTS; ++i)
  {  
    vec3 L = normalize(lightDir[i]);
    float lambertTerm = dot(N,L);
    if (lambertTerm > 0.0)
    {
      final_color += gl_LightSource[i].diffuse * gl_FrontMaterial.diffuse * lambertTerm;	
      vec3 E = normalize(eyeVec);
      vec3 R = reflect(-L, N);
      float specular = clamp(pow( max(dot(R, E), 0.0), gl_FrontMaterial.shininess), 0.001, 1.0);
      final_color += gl_LightSource[i].specular * specular;
    }
  }
  final_color = clamp(final_color, 0.0, 1.0);
  gl_FragColor = final_color * base_color;
}

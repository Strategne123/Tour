
Shader "NAR/N_Noise"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.5
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
//LOD100
 
        CGPROGRAM
        #pragma surface surf Lambert
 
sampler2D _MainTex;
sampler2D _NoiseTex;
float _Intensity;
float _NoiseIntensity;
 
struct Input
{
    float2 uv_MainTex;
};
 
void surf(Input IN, inout SurfaceOutput o)
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    fixed3 bw = dot(c.rgb, float3(0.299, 0.587, 0.114));
    fixed3 noise = tex2D(_NoiseTex, IN.uv_MainTex).rgb;
 
    o.Albedo = lerp(bw, noise, _Intensity * _NoiseIntensity);
}
        ENDCG
    }
FallBack"Diffuse"
}
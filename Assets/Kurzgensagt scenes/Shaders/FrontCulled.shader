Shader "Custom/FrontCulled"{
   Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Color", Color) = (1,1,1,1)
	_Alpha ("Alpha", Range(0,1)) = 1
	_Range ("Range", Range(0,5)) = 1
	_Cutout ("Cutout", Range(0,1)) = 0.5
	_Power ("_Power", Range(0,4)) = 0.5
  }
  SubShader {
    Tags { "RenderType"="Opaque" "Queue"="Geometry" "ForceNoShadowCasting"="True" }
    LOD 200
    Offset 0, 0
    Cull OFF
    CGPROGRAM
    #pragma surface surf NoLighting alpha:blend
    
    sampler2D _MainTex;
    fixed4  _Color;
	float _Alpha;
	float _Range;

    struct Input {
      float2 uv_MainTex;
    };

	fixed4 _Emission;
    half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten) {
        half4 c;
        c.rgb = _Color;
        c.a = 1;
        return c;
    }


    void surf (Input IN, inout SurfaceOutput o) {
        //half4 c = tex2D (_MainTex, IN.uv_MainTex);
		//half NdotL = abs(_Range*(dot (o.Normal, float3(0,0,-1))));
		
		//NdotL = NdotL>0?NdotL:0;
       
		
		if (dot(o.Normal, float3(0,0,1)) > 0)
         { 
		 //o.Albedo = _Color;
        o.Alpha = _Alpha;
             // Front side is facing the camera, render as normal
             //fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
             //o.Albedo = c.rgb;
              //o.Alpha = NdotL*_Alpha;
         }
         else
         {
			clip(-1);
             o.Alpha = 0;
         }
      }
    ENDCG
    }
}
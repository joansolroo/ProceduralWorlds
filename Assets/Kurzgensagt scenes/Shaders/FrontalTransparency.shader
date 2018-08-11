Shader "Custom/atmosphere"{
  Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Color", Color) = (1,1,1,1)
	_Alpha ("Alpha", Range(0,1)) = 1
	_Range ("Range", Range(0,5)) = 1
	_Cutout ("Cutout", Range(0,1)) = 0.5
	_Power ("_Power", Range(0,4)) = 0.5
	_Culling ("_FrontCulling", Range(-1,1)) = 0
  }
  SubShader {
    Tags { "RenderType"="Opaque" "Queue"="Geometry+1" "ForceNoShadowCasting"="True" }
    LOD 200
    Offset -1, -1
     Cull OFF
    CGPROGRAM
    #pragma surface surf NoLighting alpha:blend

    sampler2D _MainTex;
    fixed4  _Color;
	float _Alpha;
	float _Range;
	float _Cutout;
	float _Power;

	 half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten) {

        return half4(s.Albedo,s.Alpha);
    }


    struct Input {
      float2 uv_MainTex;
	  float3 worldNormal;
	  float3 worldPos;
    };

	fixed _Culling;

    void surf (Input IN, inout SurfaceOutput o) {

		fixed3 n = UnityWorldToViewPos (IN.worldPos+IN.worldNormal)-UnityWorldToViewPos (IN.worldPos);//mul ((float3x3)UNITY_MATRIX_MVP,o.Normal);
		if (_Culling* dot(n, float3(0,0,1)) > 0)
         { 

		   half NdotL = abs(_Range*(dot (n, float3(0,0,1))-_Cutout));
			NdotL = NdotL>0?NdotL:0;
			o.Albedo = _Color*_Power;
			o.Alpha = NdotL*_Alpha;

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
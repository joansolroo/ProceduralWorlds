﻿Shader "Custom/Posterized" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Emission ("Emission", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
    #pragma surface surf NoLighting addshadow

	fixed4  _Color;
	fixed4 _Emission;
	 sampler2D _Ramp;

    half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten) {
        half NdotL = dot (s.Normal, lightDir);
        half diff = NdotL * 0.5 + 0.5;
        half ramp = tex2D (_Ramp, float2(diff,0)).rgb;;//diff>0.5?1:_Dark;//tex2D (_Ramp, float2(diff)).rgb;
        half4 c;
        c.rgb = _Color*ramp;
        c.a = 1;
        return c;
    }

    struct Input {
        float2 uv_MainTex;
    };
    
    sampler2D _MainTex;
    
    void surf (Input IN, inout SurfaceOutput o) {
        //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		 o.Emission = _Emission;
         o.Alpha = _Color.a;
    }
    ENDCG
	}
	FallBack "Diffuse"
}

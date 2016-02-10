//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// Sixense STEM Test Application
// Version 0.1
//

Shader "Custom/Stipple" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1.0, 0.6, 0.6, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _Color;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float2 screen = (_ScreenParams.xy-0.5f) * IN.screenPos.xy / IN.screenPos.w * 0.5f;

			float a = frac(screen.x + screen.y);

			clip(a - 0.5f);

			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

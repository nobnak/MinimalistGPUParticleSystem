Shader "Custom/BaseGPUParticle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)
		
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float4x4> _ObjectToWorldList;
		StructuredBuffer<float4x4> _WorldToObjectList;
		#endif

		void setup() {
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			//unity_ObjectToWorld = _ObjectToWorldList[unity_InstanceID];
			//unity_WorldToObject = _WorldToObjectList[unity_InstanceID];

			unity_ObjectToWorld = 0;
			unity_ObjectToWorld._11_22_33_44 = 1;
			unity_ObjectToWorld._14_24_34 = 0;

			unity_WorldToObject = unity_ObjectToWorld;
			unity_WorldToObject._14_24_34 *= -1;
			unity_WorldToObject._11_22_33 = 1.0 / unity_ObjectToWorld._11_22_33;
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	//FallBack "Diffuse"
}

Shader "GPUParticle/Transparent" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
        _Size ("Size", Float) = 1
	}
	SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
		Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
            #pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "GPUParticle.cginc"

			struct vsin {
                uint vid : SV_VertexID;
			};

            struct gsin {
                float4 vertex : POSITION;
                float2 size : TEXCOORD0;
            };

			struct psin {
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

            DEFINE_PARTICLES_PROP(particles)
			
			gsin vert (vsin v) {
                Particle p = particles[v.vid];

				gsin o;
                o.vertex = float4(p.pos, 1);
                o.size = p.size;
				return o;
			}

            [maxvertexcount(6)]
            void geom(point gsin input[1], inout TriangleStream<psin> stream) {
                static float2 uvs[4] = { float2(0,0), float2(1,0), float2(0,1), float2(1,1) };
                static uint indices[6] = { 0, 2, 3, 0, 3, 1 };

                gsin p = input[0];

                float3 right = p.size.x * normalize(mul(float4(1, 0, 0, 0), UNITY_MATRIX_IT_MV).xyz);
                float3 up = p.size.y * normalize(mul(float4(0, 1, 0, 0), UNITY_MATRIX_IT_MV).xyz);
                float3 center = p.vertex.xyz;

                float4 v[4];
                v[0] = UnityObjectToClipPos(center - up - right);
                v[1] = UnityObjectToClipPos(center - up + right);
                v[2] = UnityObjectToClipPos(center + up - right);
                v[3] = UnityObjectToClipPos(center + up + right);

                psin output;
                for (uint i = 0; i < 6; i++) {
                    uint j = indices[i];
                    output.vertex = v[j];
                    output.uv = uvs[j];
                    stream.Append(output);
                    if (i % 3 == 2)
                        stream.RestartStrip();
                }
            }
			
			fixed4 frag (psin i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}

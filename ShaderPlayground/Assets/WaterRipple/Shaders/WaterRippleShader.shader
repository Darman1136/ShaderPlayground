// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterRippleShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Scale("Scale", float) = 1
		_Speed("Speed", float) = 1
		_Frequency("Frequency", float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Lambert vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 4.0

			sampler2D _MainTex;
			float _Scale, _Speed, _Frequency;

			struct Input {
				float2 uv_MainTex;
				float3 normalValue;
			};

			uniform float _WaveAmplitude[8];
			uniform half4 _Offset[8];
			uniform float _DrawDistance[8];
			uniform half4 _Impact[8];
			uniform float _Timer;

			half _Glossiness;
			half _Metallic;
			half4 _Color;

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				half ripple = ((v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z));
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				for (int i = 0; i < 8; i++) {
					half value = _Scale * sin(_Timer * _Speed * _Frequency + ripple + ((v.vertex.x * _Offset[i].x) + (v.vertex.z * _Offset[i].z)));
					if (distance(v.vertex, _Impact[i]) < _DrawDistance[i]) {
						o.normalValue += (value  * _WaveAmplitude[i]);
						v.vertex.y += (value * _WaveAmplitude[i]);
					}
				}
			}

			void surf(Input IN, inout SurfaceOutput o) {
				// Albedo comes from a texture tinted by color
				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = _Color.rgb;
				o.Alpha = c.a;
				o.Normal.y = IN.normalValue;
			}

ENDCG
		}
			FallBack "Diffuse"
}

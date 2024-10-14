Shader "Custom/RealisticBlackHoleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlackHolePosition("Black Hole Position", Vector) = (0,0,0,0)
        _BlackHoleMass("Black Hole Mass", Float) = 1.0
        _MaxSteps("Max Steps", Int) = 100
        _StepSize("Step Size", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _BlackHolePosition;
            float _BlackHoleMass;
            int _MaxSteps;
            float _StepSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 gravitationalLensing(float3 pos, float3 dir, float3 bhPos, float bhMass)
            {
                // Schwarzschild radius
                float rs = 2.0 * bhMass;

                for (int i = 0; i < _MaxSteps; i++)
                {
                    float r = length(pos - bhPos);
                    float3 acc = -normalize(pos - bhPos) * (rs / (r * r));
                    dir = normalize(dir + acc * _StepSize);
                    pos += dir * _StepSize;

                    // Break if we're inside the event horizon
                    if (r < rs * 1.5)
                        break;
                }

                return dir;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = float3(i.uv, 1.0);
                float3 rayDir = normalize(float3(i.uv - 0.5, 1.0));
                float3 bhPos = _BlackHolePosition;
                float bhMass = _BlackHoleMass;

                rayDir = gravitationalLensing(rayOrigin, rayDir, bhPos, bhMass);

                // Sample the texture with the distorted UVs
                float2 uv = rayDir.xy / rayDir.z * 0.5 + 0.5;
                half4 col = tex2D(_MainTex, uv);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
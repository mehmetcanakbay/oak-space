Shader "ExampleRendererShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            StructuredBuffer<int> _Triangles;
            StructuredBuffer<float3> _Positions;
            uniform uint _StartIndex;
            uniform uint _BaseVertexIndex;
            uniform float4x4 _ObjectToWorld;

            v2f vert(uint vertexID : SV_VertexID)
            {
                v2f o;
                // Fetch the position based on triangle index
                float3 pos = _Positions[_Triangles[vertexID + _StartIndex] + _BaseVertexIndex];

                // Transform to world space
                float4 wpos = mul(_ObjectToWorld, float4(pos, 1.0));
                o.pos = UnityObjectToClipPos(wpos); // Clip space position
                o.color = float4(1.0f, 1.0f, 1.0f, 1.0f); // White color
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color; // Output color
            }
            ENDCG
        }
    }
}
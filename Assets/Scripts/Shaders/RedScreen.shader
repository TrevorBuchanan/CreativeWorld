// Create a shader file in Unity named "RedScreen.shader"
Shader "Shaders/RedScreen"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            HLSLPROGRAM
            #include "UnityCG.cginc" // Include Unity's shader utilities

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // Use UNITY_MATRIX_MVP to transform vertex position to clip space
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1.0, 0.0, 0.0, 1.0); // Red color
            }
            ENDHLSL
        }
    }
}
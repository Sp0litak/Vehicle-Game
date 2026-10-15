Shader "Vehicle Game/Laser"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct Input
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct Output
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };

            Output vert(Input input)
            {
                Output output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.color = input.color;
                return output;
            }

            fixed4 frag(Output input) : SV_Target
            {
                return input.color;
            }
            ENDCG
        }
    }
}

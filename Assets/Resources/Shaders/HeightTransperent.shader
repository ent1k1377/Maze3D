Shader "CustomShader/HeightTransperent"
{
    Properties
    {
        _TopColor("Top Color", Color) = (1,1,1,1)
        _BottomColor("Bottom Color", Color) = (1,1,1,1)
        _TopHeight("Top Height", float) = 1
        _BottomHeight("Top Height", float) = 0
    }

        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 200

        Pass {
            ZWrite On
            ColorMask 0
        }

        CGPROGRAM
        #pragma surface surf Standard alpha:blend  

        struct Input {
            float3 worldPos;
        };
        float4 _TopColor, _BottomColor;
        float _TopHeight, _BottomHeight;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float value = smoothstep(_BottomHeight, _TopHeight, IN.worldPos.y);
            o.Albedo = _TopColor * value + _BottomColor * (1 - value);
            o.Alpha = value;
        }
        ENDCG
    }
}
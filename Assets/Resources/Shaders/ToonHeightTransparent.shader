Shader "ToonHeightTransparent" {
    //show values to edit in inspector
    Properties{
        [Header(Base Parameters)]
        _Color("Tint", Color) = (0, 0, 0, 1)
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Emission("Emission", color) = (0 ,0 ,0 , 1)
        [Header(HeightTransparentParameters)]
        _BottomColor("Bottom Color", Color) = (1,1,1,1)
        _TopHeight("Top Height", float) = 1
        _BottomHeight("Bot Height", float) = 0
    }
        SubShader{
            //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
            //Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}


            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 200

            Pass {
                ZWrite On
                ColorMask 0
            }

            CGPROGRAM

            //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
            //our surface shader function is called surf and we use our custom lighting model
            //fullforwardshadows makes sure unity adds the shadow passes the shader might need
            #pragma surface surf Standard alpha:blend
            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            half3 _Emission;

            //input struct which is automatically filled by unity
            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
            };

            float4 _BottomColor;
            float _TopHeight, _BottomHeight;

            //the surface shader function which sets parameters the lighting function then uses
            void surf(Input i, inout SurfaceOutputStandard o) {

                float value = smoothstep(_BottomHeight, _TopHeight, i.worldPos.y);
                o.Alpha = value * _Color.a;

                //sample and tint albedo texture
                fixed4 col = tex2D(_MainTex, i.uv_MainTex);
                col *= _Color;
                o.Albedo = col * value + _BottomColor * (1 - value);

                o.Emission = _Emission;
            }
            ENDCG
        }
            FallBack "Standard"
}
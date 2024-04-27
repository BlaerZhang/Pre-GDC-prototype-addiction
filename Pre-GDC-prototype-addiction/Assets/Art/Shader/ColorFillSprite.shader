Shader "Custom/ColorFillShader"
{
    Properties
    {
        _Color ("Fill Color", Color) = (0.1015625, 0.1015625, 0.1015625, 1)
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.01
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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
            fixed4 _Color;
            float _AlphaThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                // Check if the alpha is not zero
                if (tex.a > _AlphaThreshold)
                {
                    // Use the alpha from the texture, but the color from the _Color property
                    return fixed4(_Color.rgba);
                }
                // If alpha is zero, keep it fully transparent
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

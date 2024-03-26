Shader "LTN/FourColorDynamicGradient" {
    Properties {
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 1, 0, 1)
        _Color3 ("Color 3", Color) = (0, 0, 1, 1)
        _Color4 ("Color 4", Color) = (1, 1, 0, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 0)
        _IsAnimating ("_IsAnimating", Int) = 1  // Default to animate
        _BlendFactor ("Blend Factor", Range (0, 1)) = 0.5
        _BlendChange ("Blend Change", Range (0, 1)) = 0.2
        _Speed ("Animation Speed", Range (0, 5)) = 0.2
        _MaxMovement ("Max Movement", Range (0, 1)) = 0.3
        _InitialPos1 ("Initial Position 1", Vector) = (-0.5, -0.5, 0, 0)
        _InitialPos2 ("Initial Position 2", Vector) = (-0.5, 0.25, 0, 0)
        _InitialPos3 ("Initial Position 3", Vector) = (0.4, 0.2, 0, 0)
        _InitialPos4 ("Initial Position 4", Vector) = (0.4, -0.5, 0, 0)
        _ColorPreset ("Color Preset", Int) = 0
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            fixed4 _Color4;
            fixed4 _BackgroundColor;
            float _BlendFactor;
            float _BlendChange;
            float _Speed;
            float _Animate;
            float _MaxMovement;
            float4 _InitialPos1;
            float4 _InitialPos2;
            float4 _InitialPos3;
            float4 _InitialPos4;
            int _IsAnimating;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float time = _IsAnimating ? _Time.y * _Speed : 0;  // Use time if animation is enabled
                float2 pos1 = _InitialPos1 + _MaxMovement * float2(sin(time), cos(time));
                float2 pos2 = _InitialPos2 + _MaxMovement * float2(cos(time), sin(time));
                float2 pos3 = _InitialPos3 + _MaxMovement * float2(sin(-time), cos(-time));
                float2 pos4 = _InitialPos4 + _MaxMovement * float2(cos(-time), sin(-time));

                float blend1 = _BlendFactor + _BlendChange * pow(sin(time), 2);
                float blend2 = _BlendFactor + _BlendChange * pow(sin(time + 3.14159265/2), 2);
                float blend3 = _BlendFactor + _BlendChange * pow(sin(time + 3.14159265), 2);
                float blend4 = _BlendFactor + _BlendChange * pow(sin(time + 3*3.14159265/2), 2);

                float dist1 = distance(i.uv, pos1);
                float dist2 = distance(i.uv, pos2);
                float dist3 = distance(i.uv, pos3);
                float dist4 = distance(i.uv, pos4);
                
                fixed4 col1 = _Color1 * (1.0 - smoothstep(0.0, blend1, dist1));
                fixed4 col2 = _Color2 * (1.0 - smoothstep(0.0, blend2, dist2));
                fixed4 col3 = _Color3 * (1.0 - smoothstep(0.0, blend3, dist3));
                fixed4 col4 = _Color4 * (1.0 - smoothstep(0.0, blend4, dist4));

                fixed4 finalColor = col1 + col2 + col3 + col4;
                return lerp(_BackgroundColor, finalColor, finalColor.a);
            }

            ENDCG
        }
    }
    CustomEditor "FourColorDynamicGradientInspector"
}

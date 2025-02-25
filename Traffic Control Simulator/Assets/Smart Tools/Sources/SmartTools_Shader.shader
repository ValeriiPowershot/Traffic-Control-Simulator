Shader "Hidden/SmartTools_Shader"
{
    Properties 
    {
        [NoScaleOffset] _Grid("Grid", 2D) = "white" {}
        _GridColor ("GridColor", Color) = (0.5,0.5,0.5,1)
        _GridSteps ("GridSteps", Float ) = 1
        _Scale ("Scale", Float ) = 1
        _SnapTransparency ("SnapTransparency", Float ) = 1
        _SnapOffset ("SnapOffset", Vector ) = (0,0,0)
        _StreakColor ("StreakColor", Color) = (0.7794118,0.7794118,0.7794118,1)
        _GridOffset ("GridOffset", Vector ) = (0.5,0.5,0.5)
        _Fresnel ("Fresnel", Float ) = 10
        _VertexPush ("VertexPush", Float ) = 0
        _ObjectScale ("ObjectScale", Float ) = 50
    }

    SubShader 
    {
        Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            CBUFFER_START(UnityPerMaterial)
                uniform sampler2D _Grid;
                uniform float _Scale;
                uniform float _Fresnel;
                uniform float3 _SnapOffset;
                uniform float _VertexPush;
                uniform float _SnapTransparency;
                uniform float _GridSteps;
                uniform float3 _GridOffset;
                uniform float4 _GridColor;
                uniform float4 _StreakColor;
                uniform float _ObjectScale;
            CBUFFER_END

            struct v2f 
            {
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
                float3 normal    : TEXCOORD2;
                float3 centerPos : TEXCOORD3;
            };

            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 texcoord : TEXCOORD0, float3 centerPos : TEXCOORD3)
            {
                v2f o;
                o.uv = texcoord;
                o.normal = normal;//normalize(mul(float4(normal,0), unity_WorldToObject).xyz);
                vertex.xyz -= (0.5 * normal);
                vertex.xyz += (_VertexPush * 0.0001 * normal);
                o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                o.centerPos = mul(unity_ObjectToWorld, float4(0, 0, 0, vertex.w)).xyz;
                o.pos = UnityObjectToClipPos(vertex);
                return o;
            }

            float4 frag(v2f i) : COLOR 
            {
                float3 normal = i.normal;
                float3 normalAbs = abs(normal);
                float3 pos = i.worldPos;
                float2 uv = i.uv;

                float grid = 0;
                grid += tex2D(_Grid, pos.yz / _GridSteps + _GridOffset.yz).r * normalAbs.x;
                grid += tex2D(_Grid, pos.xz / _GridSteps + _GridOffset.xz).r * normalAbs.y;
                grid += tex2D(_Grid, pos.xy / _GridSteps + _GridOffset.xy).r * normalAbs.z;

                float2 streakUV = uv * _ObjectScale / _Scale - ((_ObjectScale / _Scale) * 0.5 - 0.5);
                float streak = tex2D(_Grid, saturate(streakUV)).b;
                float streakAplha = streak * _StreakColor.a;

                float3 xyz = (i.worldPos - i.centerPos);
                float x = length(xyz.yz);
                float y = length(xyz.xz);
                float z = length(xyz.xy);
                float mask = smoothstep(0.51, 0.49, (min(min(x, y), z)) / _Scale);

                float2 crossUV = uv * _ObjectScale / _Scale - ((_ObjectScale / _Scale) * 0.5 - 0.5);
                float cross = tex2D(_Grid, crossUV).g * mask;


                float3 color = 0.0;
                color = lerp(color, lerp(normalAbs, _GridColor.rgb, _GridColor.a), grid);
                color = lerp(color, _StreakColor.rgb, streakAplha);
                color = lerp(color, 1, cross);

                float alpha = grid;//max(grid, _SnapTransparency);
                alpha = max(alpha, streakAplha);
                alpha = max(alpha, cross);
                // Falloff
                alpha *= smoothstep(1,0.5,saturate(distance(i.centerPos, pos) / (_ObjectScale / 2)));
                // Fresnel
                float fresnel = saturate(pow(dot(normal, UNITY_MATRIX_V[2].xyz), _Fresnel) * 1.1 - 0.1);
                float fresnel2 = smoothstep(0.6, 0.8, saturate(pow(dot(normal, UNITY_MATRIX_V[2].xyz), 1)));
                alpha *= fresnel;

                alpha = max(alpha, _SnapTransparency * fresnel2);

                return float4(color, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}

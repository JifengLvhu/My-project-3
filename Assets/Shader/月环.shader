Shader "Unlit/月环"
{
    Properties
    {
        MainTex ("贴图", 2D) = "white" {}
        OuterRadius ("外半径", float) = 0.45
        InnerRadius ("内半径", float) = 0.2
        Soft ("边缘羽化", Range(0.001,0.1)) = 0.02
        Col("颜色", Color) = (1, 1, 1, 1)
    }
    
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };
            
            sampler2D MainTex;
            float4 MainTex_ST;
            float OuterRadius;
            float InnerRadius;
            float Soft;
            fixed4 Col;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, MainTex);
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(MainTex,i.uv);
                float2 delta = i.uv - float2(0.5, 0.5);
                float dist = length(delta);
                float inner = smoothstep(InnerRadius, InnerRadius + Soft, dist);
                float outer = 1 - smoothstep(OuterRadius - Soft, OuterRadius, dist);
                col *= Col;
                col.a *= inner * outer;
                return col;
            }
            ENDHLSL
        }
    }
}
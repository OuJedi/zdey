﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "AkilliMum/SRP/PostProcessing/Pixelisation" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader 
    {
        Pass
        {
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _Val;


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
            };   

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            float4 frag (v2f i) : COLOR
            {
                float2 uv = i.texcoord;

                float pixelRatio = _ScreenParams.x / _ScreenParams.y;

                float _ValY = _Val * pixelRatio;

                _Val = 512/_Val;              
                _ValY = 512/_ValY;               
                
                uv.x = floor(uv.x*_Val)/_Val;
                uv.y = floor(uv.y*_ValY)/_ValY;

                fixed3 c = tex2D(_MainTex, uv);
                return float4(c, 1.0);
            }
            ENDCG
        }
    }
}
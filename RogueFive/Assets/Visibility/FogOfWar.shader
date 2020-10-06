Shader "Hidden/FogOfWar"
{
    Properties
    {
        _MainTex ("FogOfWar Texture", 2D) = "white" {}
		_MaskBuffer("Mask Buffer", 2D) = "" {}
		_FogTexture("Fog Texture Buffer", 2D) = "" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _MaskBuffer;
			sampler2D _FogTexture;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 mask = tex2D(_MaskBuffer, i.uv);
				fixed4 fogTex = tex2D(_FogTexture, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv);

				// Mask fog
				fogTex *= 1 - mask.r;

				// Blend with base render
				col = lerp(col, fogTex, fogTex.a);

                return col;
            }
            ENDCG
        }
    }
}

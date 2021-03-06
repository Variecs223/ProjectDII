Shader "Unlit/Tiling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            int _FieldSizeX;
            int _FieldSizeY;
            int _TextureSizeX;
            int _TextureSizeY;
            StructuredBuffer<int> _TileBuffer;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const uint tileIndex = floor(i.uv.y * _FieldSizeY) * _FieldSizeX + floor(i.uv.x * _FieldSizeX);
                const uint tileType = _TileBuffer[tileIndex];
                const float2 tileCoords = float2(tileType % _TextureSizeX + frac(i.uv.x * _FieldSizeX), tileType / _TextureSizeX + frac(i.uv.y * _FieldSizeY));
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(tileCoords.x / _TextureSizeX, tileCoords.y / _TextureSizeY));
                return col;
            }
            ENDCG
        }
    }
}

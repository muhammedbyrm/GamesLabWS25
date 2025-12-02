Shader "Hidden/VignetteSanity"
{
    Properties
    {
        _VignetteColor ("Vignette Color", Color) = (1, 0, 0, 0.5)
        _MainTex ("Dummy Texture", 2D) = "white" {} // This satisfies Unity UI
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _VignetteColor;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // This sample satisfies Unity's UI system, but is unused
                fixed4 dummy = tex2D(_MainTex, i.uv);

                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float vignette = smoothstep(0.05, 0.75, dist); // Edge = 1, center = 0

                return _VignetteColor * vignette;
            }
            ENDCG
        }
    }
}

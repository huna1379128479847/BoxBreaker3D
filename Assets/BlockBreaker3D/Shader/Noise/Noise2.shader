Shader "Hidden/Noise2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    // 0 ~ 1
    float _strength = 1;
    // 乱数シード
    int _Seed;

    // 乱数生成
    // http://neareal.net/index.php?ComputerGraphics%2FHLSL%2FCommon%2FGenerateRandomNoiseInPixelShader
    float rnd(float2 value, int Seed)
    {
        return frac(sin(dot(value.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
    }

    fixed4 frag (v2f_img i) : SV_Target
    {
        float rndValue = rnd(i.uv, _Seed);
        // -1 or 1 左右どちらにずれるかランダムにする
        int tmp = step(rndValue, 0.5)*2 - 1;
        // ピクセルジャンプ値
        float rndU = _strength * tmp * rndValue;
        float2 uv = float2(frac(i.uv.x + rndU), i.uv.y);
        fixed4 col = tex2D(_MainTex, uv);
        return col;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
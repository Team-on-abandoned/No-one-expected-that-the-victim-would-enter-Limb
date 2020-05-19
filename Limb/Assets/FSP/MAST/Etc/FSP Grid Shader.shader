Shader "FSP/SimpleTintTranslucentTexture" 
{
	Properties 
	{
		_Color ("Diffuse Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		
		Cull off
		
		ZWrite Off
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		struct Input 
		{
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * _Color;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}
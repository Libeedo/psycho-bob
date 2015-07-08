Shader "Custom/ItemGlow" {
	Properties {
		_ColorTint("Colour Tint",Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha(A)", 2D) = "white" {}
		_BumpMap ("Normal Map" ,2D)="bump"{}
		_RimColor("Rim Color",Color)=(1,1,1,1)
		//_RimPower("Rim Power",Range(0.1,6.0))=3.0
		
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		//ZWrite On
         Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		

		struct Input 
		{
			float4 color : Color;
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};
		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
		
			IN.color = _ColorTint;
			
			o.Albedo = tex2D (_MainTex,IN.uv_MainTex).rgb * IN.color;
			o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			//o.Normal.y *= sign(unity_Scale.y);
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir),o.Normal));
			
			o.Emission = _RimColor.rgb * pow(rim,0.7);//<---_RimPower goes there,
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}


Shader "Custom/GrayPic" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 1)) = 1
        _Contrast ("Contrast", Range(0, 2)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Alpha;
		float _Contrast;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = dot(c.rgb, float3(0.299, 0.587, 0.114));
            
            // 对比度
            float3 avgLumin = float3(0.5, 0.5, 0.5);
            o.Albedo = lerp(avgLumin, o.Albedo, _Contrast);
            o.Alpha = _Alpha;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

Shader "Custom/SinFlow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_A("Amplitude",Range(0,1)) = 1
		_W("Period", Range(0,10)) = 10
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _A;
		float _W;

		struct Input {
			float2 uv_MainTex;
		};

 		void surf (Input IN, inout SurfaceOutput o)
        {
            float d = sqrt(dot( IN.uv_MainTex, IN.uv_MainTex));
            float dist = d  + _Time.y;
            float rd= d + _A * _A * _W * sin(2 * _W * dist) / 2;
                        
            float2 uv =  IN.uv_MainTex * rd / d;
            o.Albedo = tex2D (_MainTex, uv);
            o.Alpha = 0.0;
            
        }
		ENDCG
	} 
	FallBack "Diffuse"
}

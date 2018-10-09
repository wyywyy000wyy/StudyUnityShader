Shader "Unlit/Wave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Ai("Height", Float) = 1.0
		_W("W", Float) = 1.0
		_Speed("Speed", Float) = 1.0
		_Center("Center", Vector) = (0,0,0,0)
		_Diffuse("Diffuse", Color) = (1, 1, 1, 1)
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8.0, 256)) = 20

	}
		SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		LOD 100

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		//Cull off
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha, One Zero
		//Blend DstColor SrcColor
		//Blend DstColor Zero

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog
#pragma multi_compile_fwdbase

#include "UnityCG.cginc"
#include "Lighting.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float2 normal : NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 worldNormal : TEXCOORD2;
		float3 worldPos : TEXCOORD3;
	};

	sampler2D _MainTex;
	sampler2D _WaveTex;
	//float4 _WaveTex[700];
	float4 _MainTex_ST;
	fixed4 _Diffuse;
	fixed4 _Specular;
	float _Gloss;
	float	_Ai;
	float	_W;
	float	_Speed;
	float	_Time1;
	float   _Qi;
	int		_Width;
	uniform float4	_Center;
	float	_AlphaScale;

	v2f vert(appdata v)
	{
		float4 vt = v.vertex - _Center;
		float2 D1 = vt.xz;// normalize(vt.xz);
		float2 t1;
		float2 l = vt.xz;
		//v.vertex.y = _Ai * sin(dot(l, D1) * _W + _Time1* _Speed);
		//v.vertex.y = _Ai * pow(sin(dot(l, D1) * _W + _Time1* _Speed),1);

		//v.vertex.y = _Ai * pow(sin(vt.x * _W + _Time1* _Speed) / 2,4);

		//v.vertex.x = v.vertex.x + _Qi * _Ai * vt.x * cos(_W * vt.x + _Time1 * _Speed);
		//v.vertex.y = _Ai * sin(vt.x * _W + _Time1 * _Speed);


		//float4 st = _WaveTex[v.vertex.x + _Width * (v.vertex.z + 1)];
		//v.vertex.x = v.vertex.x + st.x;
		//v.vertex.y = st.y;

		//v.vertex.x = v.vertex.x + tex2Dlod(_WaveTex, float4(v.uv,0,0)).r;
		float4 tp = tex2Dlod(_WaveTex, float4(v.uv, 0, 0));
		v.vertex.x = v.vertex.x + tp.x;
		v.vertex.y = tp.y;
		//v.vertex.y = v.uv.y;




		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

		// Transform the vertex from object spacet to world space
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv);
	col.w = _AlphaScale;

	fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

	fixed3 worldNormal = normalize(i.worldNormal);
	fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

	fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

	// Get the view direction in world space
	fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
	// Get the half direction in world space
	fixed3 halfDir = normalize(worldLightDir + viewDir);
	// Compute specular term
	fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

	fixed4 col2 = fixed4(ambient + diffuse + specular, _AlphaScale);

	/*col.x = col.x * col2.x;
	col.y = col.y * col2.y;
	col.z = col.z * col2.z;*/

	// apply fog
	//UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
	}
		ENDCG
	}

		Pass{
		// Pass for other pixel lights
		Tags{ "LightMode" = "ForwardAdd" }
		//Cull off
		//ZWrite Off
		Blend One One

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fwdadd

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float2 normal : NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 worldNormal : TEXCOORD2;
		float3 worldPos : TEXCOORD3;
	};

	sampler2D _MainTex;
	sampler2D _WaveTex;
	//float4 _WaveTex[700];
	float4 _MainTex_ST;
	fixed4 _Diffuse;
	fixed4 _Specular;
	float _Gloss;
	float	_Ai;
	float	_W;
	float	_Speed;
	float	_Time1;
	float   _Qi;
	int		_Width;
	uniform float4	_Center;
	float	_AlphaScale;

	v2f vert(appdata v)
	{
		float4 vt = v.vertex - _Center;
		float2 D1 = vt.xz;// normalize(vt.xz);
		float2 t1;
		float2 l = vt.xz;
		//v.vertex.y = _Ai * sin(dot(l, D1) * _W + _Time1* _Speed);
		//v.vertex.y = _Ai * pow(sin(dot(l, D1) * _W + _Time1* _Speed),1);

		//v.vertex.y = _Ai * pow(sin(vt.x * _W + _Time1* _Speed) / 2,4);

		//v.vertex.x = v.vertex.x + _Qi * _Ai * vt.x * cos(_W * vt.x + _Time1 * _Speed);
		//v.vertex.y = _Ai * sin(vt.x * _W + _Time1 * _Speed);


		//float4 st = _WaveTex[v.vertex.x + _Width * (v.vertex.z + 1)];
		//v.vertex.x = v.vertex.x + st.x;
		//v.vertex.y = st.y;

		//v.vertex.x = v.vertex.x + tex2Dlod(_WaveTex, float4(v.uv,0,0)).r;
		float4 tp = tex2Dlod(_WaveTex, float4(v.uv, 0, 0));
		v.vertex.x = v.vertex.x + tp.x;
		v.vertex.y = tp.y;
		//v.vertex.y = v.uv.y;




		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

		// Transform the vertex from object spacet to world space
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed3 worldNormal = normalize(i.worldNormal);
#ifdef USING_DIRECTIONAL_LIGHT
	fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
#else
	fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
#endif

	fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

	// Get the view direction in world space
	fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
	// Get the half direction in world space
	fixed3 halfDir = normalize(worldLightDir + viewDir);
	// Compute specular term
	fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

#ifdef USING_DIRECTIONAL_LIGHT
	fixed atten = 1.0;
#else
#if defined (POINT)
	float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
	fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
#elif defined (SPOT)
	float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
	fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
#else
	fixed atten = 1.0;
#endif
#endif
	atten = 0;
	return fixed4((diffuse + specular) * atten, 1.0);
	}
		ENDCG
	}
	}

		Fallback "Transparent/VertexLit"
}

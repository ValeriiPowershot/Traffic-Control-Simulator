// SKGames height fog shader. Copyright (c) 2018 Sergey Klimenko. 11.05.2018

Shader "SKGames/Height Fog" {
	Properties{
		[Header(Main properties)]
		[PerRendererData] _Color("Diffuse Color", Color) = (1,1,1,1)
        [PerRendererData][HDR] _FogEmissionColor("Fog Emission Color", Color) = (1,1,1,1)
		[PerRendererData] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[PerRendererData] _EmissionPower("Emission Power", Range(0, 1)) = 1
		_SpecularPower("Specular Power", Range(0, 1)) = 0.4
		_Metallic("Metallic Power", Range(0, 1)) = 0.9
		_NormalAmount("Normal Power", Range(0.01, 3)) = 2
		_SpecularTex("Specular Map", 2D) = "black" {}
		_MetallicTex("Metallic Map", 2D) = "black" {}
		[Enum(R,0,G,1,B,2,A,3,AlbedoAlpha,4)]_SpecChannel("Specular source", Int) = 3
		[Enum(R,0,G,1,B,2,A,3,AlbedoAlpha,4)]_MetChannel("Metllic source", Int) = 3

		_MainTex("Main Texture", 2D) = "white" {}
		[NoScaleOffset][Normal] _NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]_AOMap("Ambient Occlusion Map", 2D) = "white" {}

		[Header(Fog properties)]
		[PerRendererData][Enum(World,1,Local,0)] _FogRelativeWorldOrLocal("Fog Simulation Space", Int) = 1
		[PerRendererData] _FogColor("Fog Color", Color) = (1,1,1,1)
		[PerRendererData] _FogMin("Height Fog Min", Float) = -20
		[PerRendererData] _FogMax("Height Fog Max", Float) = 0
		[PerRendererData][PowerSlider(3.0)] _FogEmissionPower("Fog Emission Power", Range(0, 100)) = 20
		[PerRendererData][PowerSlider(3.0)] _FogEmissionFalloff("Fog Emission Falloff", Range(0.01, 20)) = 0.5
		[PerRendererData][PowerSlider(3.0)] _FogFalloff("Fog Falloff", Range(0.01, 20)) = 1
		[Header(STANDARD fog properties overrides)]
		[PerRendererData] _STANDARD_FOG("Combine with STANDARD fog", Float) = 0
		[PerRendererData] _OVERRIDE_FOG_COLOR("Override STANDARD fog color (forward only)", Float) = 0
		[Header(Fog animation properties)]
		[PerRendererData] _ANIMATION("Use fog animation", Float) = 0
		[PerRendererData] _FogWaveSpeedX("Fog Wave Speed X", Range(-50, 50)) = 2
		[PerRendererData] _FogWaveSpeedZ("Fog Wave Speed Z", Range(-50, 50)) = 2
		[PerRendererData] _FogWaveAmplitudeX("Fog Wave Amplitude X", Range(0, 1)) = 0.3
		[PerRendererData] _FogWaveAmplitudeZ("Fog Wave Amplitude Z", Range(0, 1)) = 0.3
		[PerRendererData] _FogWaveFreqX("Fog Frequency X", Range(0, 20)) = 0.5
		[PerRendererData] _FogWaveFreqZ("Fog Frequency Z", Range(0, 20)) = 0.5
		
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma vertex vert
		#pragma surface surf Standard finalcolor:verticalFogHidder fullforwardshadows exclude_path:deferred
		#pragma multi_compile_instancing

		//TODO: correct deffered path
		//#pragma finalgbuffer:defferedHidder

		#pragma multi_compile_fog

		sampler2D _MainTex, _NormalMap, _AOMap, _SpecularTex, _MetallicTex;
		fixed4 _Color, _EmissionColor, _FogColor, _FogEmissionColor;
		fixed _EmissionPower, _FogMin, _FogMax, _FogEmissionPower, _FogEmissionFalloff, _FogFalloff, _SpecularPower, _Metallic, _FogRelativeWorldOrLocal, _NormalAmount, _SpecChannel, _MetChannel;
		fixed _FogWaveSpeedX,_FogWaveSpeedZ,_FogWaveAmplitudeX,_FogWaveAmplitudeZ,_FogWaveFreqX,_FogWaveFreqZ, _ANIMATION, _STANDARD_FOG, _OVERRIDE_FOG_COLOR;
		
		struct Input {
			fixed2 uv_MainTex;
			fixed3 worldPos;
			UNITY_FOG_COORDS(1)
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			UNITY_TRANSFER_FOG(o, UnityObjectToClipPos(v.vertex));
		}

		fixed3 waveCalc(float3 worldPos)
		{
			if (_ANIMATION > 0) {
				fixed timeX = _Time.x * 20 * -_FogWaveSpeedX;
				fixed timeZ = _Time.x * 20 * -_FogWaveSpeedZ;
				fixed waveValueX = sin(timeX + worldPos.x * _FogWaveFreqX) * _FogWaveAmplitudeX;
				fixed waveValueZ = sin(timeZ + worldPos.z * _FogWaveFreqZ) * _FogWaveAmplitudeZ;
				fixed waveValue = (waveValueX + waveValueZ) / 2;
				return fixed3(worldPos.x, worldPos.y + waveValue, worldPos.z);
			}
			else {
				return worldPos;
			}
		}

		//TODO: correct deffered path
		/*void defferedHidder(Input IN, SurfaceOutputStandard o, inout half4 outDiffuse, inout half4 outSpecSmoothness, inout half4 outNormal, inout half4 outEmission) {
			fixed3 wPos = waveCalc(IN.worldPos);
			fixed3 localPos = waveCalc(mul(unity_WorldToObject, fixed4(IN.worldPos, 1)));
			float lerpValue = clamp((((localPos.y * clamp(1 - _FogRelativeWorldOrLocal, 0, 1)) + (wPos.y * clamp(0 + _FogRelativeWorldOrLocal, 0, 1))) - _FogMin) / (_FogMax - _FogMin), 0, 1);
			outDiffuse = lerp(_FogColor, outDiffuse, pow(lerpValue, _FogFalloff));
			outSpecSmoothness = lerp(_FogColor, outSpecSmoothness, pow(lerpValue, _FogFalloff));
			outEmission = lerp(_FogColor, outEmission, pow(lerpValue, _FogFalloff));
			#if STANDARD_FOG
				UNITY_APPLY_FOG_COLOR(IN.fogCoord, outDiffuse,
				#if OVERRIDE_FOG_COLOR
					_FogColor
				#else
					unity_FogColor
				#endif	
				);
			#endif
		}*/

		void verticalFogHidder(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			#ifndef UNITY_PASS_FORWARDADD
				fixed3 wPos = waveCalc(IN.worldPos);
				fixed3 localPos = waveCalc(mul(unity_WorldToObject, fixed4(IN.worldPos, 1)));
				float lerpValue = clamp((((localPos.y * clamp(1 - _FogRelativeWorldOrLocal, 0, 1)) + (wPos.y * clamp(0 + _FogRelativeWorldOrLocal, 0, 1))) - _FogMin) / (_FogMax - _FogMin), 0, 1);
				
				float3 emission = _FogColor + _FogEmissionColor * _FogEmissionPower;
				float3 fogEmissionColor = lerp(_FogColor, emission, pow(lerpValue, _FogEmissionFalloff));
				color = lerp(half4(fogEmissionColor, color.a), color, pow(lerpValue, _FogFalloff));

				//color = lerp(_FogColor, color, pow(lerpValue, _FogFalloff));
				if (_STANDARD_FOG > 0) 
				{
					fixed4 clr = unity_FogColor;
					if (_OVERRIDE_FOG_COLOR > 0)
					{
						clr = _FogColor;
					}
					UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, clr);
				}
			#else 
				fixed3 wPos = waveCalc(IN.worldPos);
				fixed3 localPos = waveCalc(mul(unity_WorldToObject, fixed4(IN.worldPos, 1)));
				float lerpValue = clamp((((localPos.y * clamp(1 - _FogRelativeWorldOrLocal, 0, 1)) + (wPos.y * clamp(0 + _FogRelativeWorldOrLocal, 0, 1))) - _FogMin) / (_FogMax - _FogMin), 0, 1);
				if (_STANDARD_FOG > 0) {
					UNITY_APPLY_FOG(IN.fogCoord, color);
				}
				color = lerp(0, color, pow(lerpValue, _FogFalloff)) * _FogRelativeWorldOrLocal + color * (1 - _FogRelativeWorldOrLocal);
			#endif
			
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed3 localPos = waveCalc(mul(unity_WorldToObject, fixed4(IN.worldPos, 1)));
			fixed3 normalMap = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)).rgb;
			normalMap = float3(normalMap.x * _NormalAmount, normalMap.y * _NormalAmount, normalMap.z);
			o.Normal = normalMap;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 ao = tex2D(_AOMap, IN.uv_MainTex);
			o.Albedo = c.rgb *_Color * ao;
			fixed3 wPos = waveCalc(IN.worldPos);
			float lerpValue = clamp((((localPos.y * clamp(1 - _FogRelativeWorldOrLocal, 0, 1)) + (wPos.y * clamp(0 + _FogRelativeWorldOrLocal, 0, 1))) - _FogMin) / (_FogMax - _FogMin), 0, 1);
			//o.Emission = lerp(_FogColor.rgb, _EmissionColor.rgb * _EmissionPower, pow(lerpValue, _FogEmissionFalloff)) * _FogEmissionPower;
			o.Emission = _EmissionColor.rgb * _EmissionPower;
			fixed4 spec = tex2D(_SpecularTex, IN.uv_MainTex);
			fixed4 met = tex2D(_MetallicTex, IN.uv_MainTex);
			o.Smoothness = _SpecularPower * (spec.r * (1 - abs(sign((0 - _SpecChannel)))) + spec.g * (1 - abs(sign((1 - _SpecChannel)))) + spec.b * (1 - abs(sign((2 - _SpecChannel)))) + spec.a * (1 - abs(sign((3 - _SpecChannel)))) + c.a * (1 - abs(sign((4 - _SpecChannel)))));
			o.Metallic = _Metallic * (met.r * (1 - abs(sign((0 - _MetChannel)))) + met.g * (1 - abs(sign((1 - _MetChannel)))) + met.b * (1 - abs(sign((2 - _MetChannel)))) + met.a * (1 - abs(sign((3 - _MetChannel)))) + c.a * (1 - abs(sign((4 - _MetChannel)))));
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
	CustomEditor "HeightFogGUI"
}
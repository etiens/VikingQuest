Shader "Custom/PlaneDeformer"
{
	
	Properties
	{
		_MainTex ("RGBA Texture Image", 2D) = "white" {}
		_Alpha ("Alpha level", Range(0.0,1.0)) = 1.0
      	//_Cutoff ("Alpha Cutoff", Float) = 0.5
	
		_Color ("Color", Color) = (1, 1, 1, 1)
		_PhaseOffset ("PhaseOffset", Range(0,1)) = 0
		_Speed ("Speed", Range(0.0,10)) = 1.0
		_Depth ("Depth", Range(0.0,1)) = 0.2
		_Smoothing ("Smoothing", Range(0,1)) = 0.0
		_XDrift ("X Drift", Range(0.0,2.0)) = 0.05
		_ZDrift ("Z Drift", Range(0.0,2.0)) = 0.12
		_Scale ("Scale", Range(0.1,10)) = 1.0
		_InTime ("Time", Float) = 0.0
		
		_Wave1Activated ("Wave 1 activated", Float) = 0
		_Wave1Origin ("Wave 1 position", Vector) = (0,0,0)
		_Wave1Orientation ("Wave 1 orientation", Vector) = (0,0,0)
		_Wave1Height ("Wave 1 Height", Float) = 0.2
		_Wave1Width ("Wave 1 Width", Float) = 0.2
		_Wave1Length ("Wave 1 Length", Float) = 0.2
		
		_Wave2Activated ("Wave 2 activated", Float) = 0
		_Wave2Origin ("Wave 2 position", Vector) = (0,0,0)
		_Wave2Orientation ("Wave 2 orientation", Vector) = (0,0,0)
		_Wave2Height ("Wave 2 Height", Float) = 0.2
		_Wave2Width ("Wave 2 Width", Float) = 0.2
		_Wave2Length ("Wave 2 Length", Float) = 0.2
		
		_ThrowHitMarkerActivated ("Throw Hit marker activated", Float) = 0
		_ThrowHitPosition ("Throw Hit position", Vector) = (0,0,0)
		_ThrowHitRadius ("Throw Hit Radius", Float) = 5.0
		
		_SmashHitMarkerActivated ("Smash marker activated", Float) = 0
		_SmashHitPosition1 ("Smash Hit position1", Vector) = (0,0,0)
		_SmashHitPosition2 ("Smash Hit position2", Vector) = (0,0,0)
		_SmashHitWidth ("Smash Hit Width", Float) = 5.0
		
		_BoatPosition1 ("Boat position 1", Vector) = (0,0,0)
		_BoatPosition2 ("Boat position 2", Vector) = (0,0,0)
		_BoatPosition3 ("Boat position 3", Vector) = (0,0,0)
		_BoatPosition4 ("Boat position 4", Vector) = (0,0,0)
		_BoatPosition5 ("Boat position 5", Vector) = (0,0,0)
		_BoatPosition6 ("Boat position 6", Vector) = (0,0,0)
		_ySpecularRatio ("Y Specular Ratio", Float) = 0.0
	}
	
	
	SubShader
	{
		Tags
		{
			"Queue" = "AlphaTest"
			"RenderType" = "Opaque"
			//"IgnoreProjector" = "True"
			//"LightMode" = "ForwardBase"
		}
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 xbox360 gles
		//#pragma surface surf Lambert alpha vertex:vert
		//#pragma surface surf Lambert finalcolor:showNormals vertex:vert noforwardadd
		#pragma target 3.0
		#pragma surface surf BlinnPhong alpha exclude_path:prepass vertex:vert noforwardadd
		
		uniform sampler2D _MainTex;
		float _Alpha;
		half3 _Color;
		float _PhaseOffset;
		float _Speed;
		float _Depth;
		float _Smoothing;
		float _XDrift;
		float _ZDrift;
		float _Scale;
		float _InTime;
		
		float _Wave1Activated;
		float3 _Wave1Origin;
		float3 _Wave1Orientation;
		float _Wave1Height;
		float _Wave1Width;
		float _Wave1Length;
		
		float _Wave2Activated;
		float3 _Wave2Origin;
		float3 _Wave2Orientation;
		float _Wave2Height;
		float _Wave2Width;
		float _Wave2Length;
		
		float _ThrowHitMarkerActivated;
		float3 _ThrowHitPosition;
		float _ThrowHitRadius;
		
		float _SmashHitMarkerActivated;
		float3 _SmashHitPosition1;
		float3 _SmashHitPosition2;
		float _SmashHitWidth;
		
		float3 _BoatPosition1;
		float3 _BoatPosition2;
		float3 _BoatPosition3;
		float3 _BoatPosition4;
		float3 _BoatPosition5;
		float3 _BoatPosition6;
		
		float _ySpecularRatio;
		
		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			half3 debugColor;
			//float3 customColor;
		};
		
		// I ran into an enormous, annoying error where the Cg sin() function was causing some geometry mayhem,
		//   altering properties it shouldn't, if it was given a value of exactly 1.
		// Any modulation fixed it.  No other sin() implementation has the same problem.
		// No idea what the problem is!  Using this as a workaround, may go to the forums to see what's up later.
		float sine( float x )
		{
			float b = (1-x)<0.001 ? 1.0001 : 1.0;
			return sin(x) * b;
		}
		
		float3 ProjectPointLine(float3 point1, float3 lineStart, float3 lineEnd)
	    {
	        float3 rhs = point1 - lineStart;
	        float3 lhs = lineEnd - lineStart;
	        float magnitude = abs(length(lhs));
	        if (magnitude > 1E-06f)
	        {
	            lhs = (lhs / magnitude);
	        }
	        float num2 = clamp(dot(lhs, rhs), 0, magnitude);
	        return (lineStart + (lhs * num2));
	    }
	    
		float DistancePointLine(float3 point1, float3 lineStart, float3 lineEnd)
	    {
	        return length(ProjectPointLine(point1, lineStart, lineEnd) - point1);
	    }
	    
	    bool PointIsBetweenLine(float3 c, float3 a, float3 b)
	    {
	    float dotproduct = (c.x - a.x) * (b.x - a.x) + (c.z - a.z)*(b.z - a.z);
	    if (dotproduct < 0)
	    	return false;
	
	    float squaredlengthba = (b.x - a.x)*(b.x - a.x) + (b.z - a.z)*(b.z - a.z);
	    if (dotproduct > squaredlengthba)
	    	return false;
	    	
	    	return true;
	    }
	    
	    float DistancePointLine2(float3 point1, float3 lineStart, float3 lineEnd)
	    {
	        //return length(ProjectPointLine(point1, lineStart, lineEnd) - point1);
	        
	        //float num = abs((lineEnd.z - lineStart.z)*point1.x-(lineEnd.x - lineStart.x)*point1.z+lineEnd.x*lineStart.z-lineEnd.z*lineStart.x);
	        //float denum = sqrt(pow(lineEnd.z-lineStart.z,2) + pow(lineEnd.x-lineStart.x,2));
	        //return num/denum;
	        
	        float num = abs((lineEnd.x - lineStart.x)*(lineStart.z - point1.z)-(lineStart.x - point1.x)*(lineEnd.z-lineStart.z));
	        float denum = sqrt(pow(lineEnd.x-lineStart.x,2) + pow(lineEnd.z-lineStart.z,2));
	        return num/denum;
	    }
	    
	    bool PointInBoat(float3 point1, float3 boatPosition1, float3 boatPosition2, float3 boatPosition3, float3 boatPosition4){
	    	if(PointIsBetweenLine(point1, boatPosition1, boatPosition2) && PointIsBetweenLine(point1, boatPosition1, boatPosition3)){
	    		return true;
	    	}
	    	return false;
	    }
    	
    	bool InternalPointInPolygon(float3 point1, float3 poly1, float3 poly2, bool previousValue){
			if ( ((poly1.z>point1.z) != (poly2.z>point1.z)) &&
				 (point1.x < (poly2.x-poly1.x) * (point1.z-poly1.z) / (poly2.z-poly1.z) + poly1.x) )
			       previousValue = !previousValue;
			return previousValue;
		}
    	
		bool PointInPolygon(float3 point1, float3 poly1, float3 poly2, float3 poly3, float3 poly4, float3 poly5, float3 poly6)
		{
			int i, j;
			bool c = false;

			c = InternalPointInPolygon(point1, poly1, poly6, c);
			c = InternalPointInPolygon(point1, poly2, poly1, c);
			c = InternalPointInPolygon(point1, poly3, poly2, c);
			c = InternalPointInPolygon(point1, poly4, poly3, c);
			c = InternalPointInPolygon(point1, poly5, poly4, c);
			c = InternalPointInPolygon(point1, poly6, poly5, c);
			return c;
		}
		
		void vert( inout appdata_full v, out Input o)
		{
			// Note that, to start off, all work is in object (local) space.
			// We will eventually move normals to world space to handle arbitrary object orientation.
			// There is no real need for tangent space in this case.
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			// Do all work in world space
			float x = v.vertex.x;
			float z = v.vertex.z;
			
			float3 vertexPosition = mul( _Object2World, v.vertex ).xyz;
			float3 v0 = vertexPosition;
			
//			if(_ThrowHitMarkerActivated){
//				vertexPosition.y = 0;
//				_ThrowHitPosition.y = 0;
//				
//				float3 distanceVector = vertexPosition - _ThrowHitPosition;
//				float dist = length(distanceVector);
//				
//				if(dist < _ThrowHitRadius){
//					o.customColor.x = 1.0;
//					o.customColor.y = 0.0;
//					o.customColor.z = 0.0;
//	          		//customColor.w = 1.0;
//				}else{
//					o.customColor.x = 0.0;
//					o.customColor.y = 0.0;
//					o.customColor.z = 0.0;
//	          		//customColor.w = 1.0;
//				}
//			}else if(_SmashHitMarkerActivated){
//				vertexPosition.y = 0;
//				_SmashHitPosition1.y = 0;
//				_SmashHitPosition2.y = 0;
//				
//				if(PointIsBetweenLine(vertexPosition, _SmashHitPosition1, _SmashHitPosition2))
//				{
//					float distanceFromSmash = DistancePointLine2(vertexPosition, _SmashHitPosition1, _SmashHitPosition2);
//					
//					if(distanceFromSmash < _SmashHitWidth){
//						o.customColor.x = 1.0;
//						o.customColor.y = 0.0;
//						o.customColor.z = 0.0;
//		          		//customColor.w = 1.0;
//					}else{
//						o.customColor.x = 0.0;
//						o.customColor.y = 0.0;
//						o.customColor.z = 0.0;
//		          		//customColor.w = 1.0;
//					}
//				}
//			}
			
			// Create two fake neighbor vertices.
			// The important thing is that they be distorted in the same way that a real vertex in their location would.
			// This is pretty easy since we're just going to do some trig based on position, so really any samples will do.
			float3 v1 = v0 + float3( 0.05, 0, 0 ); // +X
			float3 v2 = v0 + float3( 0, 0, 0.05 ); // +Z
			
			
			// Some animation values
			float phase = _PhaseOffset * (3.14 * 2);
			float phase2 = _PhaseOffset * (3.14 * 1.123);
			float speed = _InTime * _Speed;
			float speed2 = _InTime * (_Speed * 0.33 );
			float _Depth2 = _Depth * 1.0;
			float v0alt = v0.x * _XDrift + v0.z * _ZDrift;
			float v1alt = v1.x * _XDrift + v1.z * _ZDrift;
			float v2alt = v2.x * _XDrift + v2.z * _ZDrift;
			
			// Modify the real vertex and two theoretical samples by the distortion algorithm (here a simple sine wave on Y, driven by local X pos)
			v0.y += sin( phase  + speed  + ( v0.x  * _Scale ) ) * _Depth;
			v0.y += sin( phase2 + speed2 + ( v0alt * _Scale ) ) * _Depth2; // This is just another wave being applied for a bit more complexity.
			
			v1.y += sin( phase  + speed  + ( v1.x  * _Scale ) ) * _Depth;
			v1.y += sin( phase2 + speed2 + ( v1alt * _Scale ) ) * _Depth2;
			
			v2.y += sin( phase  + speed  + ( v2.x  * _Scale ) ) * _Depth;
			v2.y += sin( phase2 + speed2 + ( v2alt * _Scale ) ) * _Depth2;
			
			if(_Wave1Activated > 0)
			{
				float distanceFromWave = length(cross(normalize(_Wave1Orientation), v0 - _Wave1Origin));
				
				if(distanceFromWave < _Wave1Width){
					float addedY = _Wave1Height * cos(3.14*distanceFromWave/_Wave1Width)* exp(-2*3.14*distanceFromWave*distanceFromWave/_Wave1Width);
					v0.y += addedY;
					v1.y += addedY;
					v2.y += addedY;
				}
			}
			
			if(_Wave2Activated > 0)
			{
				float distanceFromWave = length(cross(normalize(_Wave2Orientation), v0 - _Wave2Origin));
				
				if(distanceFromWave < _Wave2Width){
					float addedY = _Wave2Height * cos(3.14*distanceFromWave/_Wave2Width)* exp(-2*3.14*distanceFromWave*distanceFromWave/_Wave2Width);
					v0.y += addedY;
					v1.y += addedY;
					v2.y += addedY;
				}
			}
			
			// By reducing the delta on Y, we effectively restrict the amout of variation the normals will exhibit.
			// This appears like a smoothing effect, separate from the actual displacement depth.
			// It's basically undoing the change to the normals, leaving them straight on Y.
			v1.y -= (v1.y - v0.y) * _Smoothing;
			v2.y -= (v2.y - v0.y) * _Smoothing;
			
			// Solve worldspace normal
			float3 vna = cross( v2-v0, v1-v0 );
			
			// OPTIONAL worldspace normal out to a custom value.  Uncomment the showNormals finalcolor profile option above to see the result
			//o.debugColor = ( normalize( vna ) * 0.5 ) + 0.5;
			//o.debugColor = ( normalize( vna )  );
			
			// Put normals back in object space
			float3 vn = mul( float4x4(_World2Object), vna );
			
			// Normalize
			v.normal = normalize( vn );
			
			// Put vertex back in object space, Unity will automatically do the MVP projection
			v.vertex.xyz = mul( float4x4(_World2Object), v0 );
			v.vertex.x = x;
			v.vertex.z = z;
		}
		
		// Optional normal debug function, unccoment profile option to invoke
		void showNormals( Input IN, SurfaceOutput o, inout fixed4 color )
		{
			color.rgb = IN.debugColor.rgb;
			color.a = 1;
		}
		
		
		
		// Regular old surface shader
		void surf (Input IN, inout SurfaceOutput o)
		{
			half3 c = tex2D(_MainTex, IN.uv_MainTex) * _Color; 
			//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            float3 vertexPosition = IN.worldPos;
			float3 customColor;
			customColor.x = 0.0;
			customColor.y = 0.0;
			customColor.z = 0.0;
			if(_ThrowHitMarkerActivated){
				vertexPosition.y = 0;
				_ThrowHitPosition.y = 0;
				
				float3 distanceVector = vertexPosition - _ThrowHitPosition;
				float dist = length(distanceVector);
				
				if(dist < _ThrowHitRadius){
					customColor.x = 0.3 + sin( dist  + _InTime * 5 ) * 0.1;
					customColor.y = 0.0;
					customColor.z = 0.0;
	          		//customColor.w = 1.0;
				}else{
					customColor.x = 0.0;
					customColor.y = 0.0;
					customColor.z = 0.0;
	          		//customColor.w = 1.0;
				}
			}else if(_SmashHitMarkerActivated){
				vertexPosition.y = 0;
				_SmashHitPosition1.y = 0;
				_SmashHitPosition2.y = 0;
				
				if(PointIsBetweenLine(vertexPosition, _SmashHitPosition1, _SmashHitPosition2))
				{
					float distanceFromSmash = DistancePointLine2(vertexPosition, _SmashHitPosition1, _SmashHitPosition2);
					
					if(distanceFromSmash < _SmashHitWidth){
						customColor.x = 0.3 + sin( distanceFromSmash  - _InTime * 5 ) * 0.1;
						customColor.y = 0.0;
						customColor.z = 0.0;
		          		//customColor.w = 1.0;
					}else{
						customColor.x = 0.0;
						customColor.y = 0.0;
						customColor.z = 0.0;
		          		//customColor.w = 1.0;
					}
				}
			}
            if(customColor.x == 0.0 && customColor.y == 0.0 && customColor.z == 0.0)
            {
				if(IN.worldPos.y > 0.0){
					customColor.x = IN.worldPos.y*_ySpecularRatio;
					customColor.y = IN.worldPos.y*_ySpecularRatio;
					customColor.z = IN.worldPos.y*_ySpecularRatio;
				}
				else
				{
					customColor.x = IN.worldPos.y*_ySpecularRatio*0.5;
					customColor.y = IN.worldPos.y*_ySpecularRatio*0.5;
					customColor.z = IN.worldPos.y*_ySpecularRatio*0.5;
				}
          		//customColor.w = 1.0;
			}
            o.Emission = customColor.rgb;
            //bool inBoat = PointInBoat(IN.worldPos, _BoatPosition1, _BoatPosition2, _BoatPosition3, _BoatPosition4);
            bool inBoat = PointInPolygon(IN.worldPos, _BoatPosition1, _BoatPosition2, _BoatPosition3, _BoatPosition4, _BoatPosition5, _BoatPosition6);
            if(inBoat){
            	//float boatY = _BoatPosition1.y+_BoatPosition2.y+_BoatPosition3.y+_BoatPosition4.y+_BoatPosition5.y+_BoatPosition6.y/6;
            	//if(boatY < IN.worldPos.y)
            		o.Alpha = 0.0;
            	//else
            	//	o.Alpha = _Alpha;
            }
            else{
            	o.Alpha = _Alpha;
            }
            //o.Alpha = c.a;
		}
		//void surf (Input IN, inout SurfaceOutput o) {
		//	half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		//	o.Albedo = c.rgb;
		//	o.Alpha = c.a * tex2D(_AlphaMap, IN.uv_MainTex).r;
		//}
		
		ENDCG
		
		
         
         // Pass to render object as a shadow collector
         Pass 
         {
             Name "ShadowCollector"
             Tags { "LightMode" = "ShadowCollector" }
        
             Fog {Mode Off}
             ZWrite On ZTest Less
             
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma fragmentoption ARB_precision_hint_fastest
             #pragma multi_compile_shadowcollector
 
             #define SHADOW_COLLECTOR_PASS
             #include "UnityCG.cginc"
 
             uniform float _Scale;
             
             struct appdata 
             {
                 float4 vertex : POSITION;
             };
 
             struct v2f 
             {
                 V2F_SHADOW_COLLECTOR;
             };
 
             v2f vert (appdata v)
             {
                 v2f o;
                 v.vertex.xyz *= _Scale;
                 TRANSFER_SHADOW_COLLECTOR(o)
                 return o;
             }
 
             fixed4 frag (v2f i) : COLOR
             {
                 SHADOW_COLLECTOR_FRAGMENT(i)
             }
             ENDCG
 
         }
	}
Fallback "Transparent\VertexLit"
}
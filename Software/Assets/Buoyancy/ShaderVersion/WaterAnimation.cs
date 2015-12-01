using UnityEngine;
using System.Collections;

public class WaterAnimation : Singleton<MonoBehaviour>{

	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	public Vector2 uvAnitmationScale = new Vector2(1.0f, 0.0f);
	Vector2 uvOffset = Vector2.zero;
	private float totalTime = 0;

	public WaterAnimation(){

	}

	public Vector2 GetOffset(){
		return uvOffset;
	}

	void LateUpdate() 
	{
		totalTime += Time.deltaTime;
		float uvOffsetX = uvAnitmationScale.x*Mathf.Sin(uvAnimationRate.x * 2 * Mathf.PI * totalTime);
		float uvOffsetY = uvAnitmationScale.y*Mathf.Cos(uvAnimationRate.y * 2 * Mathf.PI * totalTime);
		uvOffset.Set(uvOffsetX, uvOffsetY);
	}

}

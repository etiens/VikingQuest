using UnityEngine;
using System.Collections;

public class AnimatedUVs : MonoBehaviour 
{
	public int materialIndex = 0;
	public string textureName = "_MainTex";
	//private Vector2 startingOffset = null;

	private WaterAnimation waterAnimation = null;

	void Awake(){
		//waterAnimation = (WaterAnimation)WaterAnimation.Instance;
		//startingOffset = renderer.materials[ materialIndex ].GetTextureOffset(textureName);
	}
	
	void LateUpdate() 
	{
		if(waterAnimation == null){
			//waterAnimation = (WaterAnimation)WaterAnimation.Instance;
			return;
		}
		if( renderer.enabled )
		{
			Vector2 offset = waterAnimation.GetOffset();
			renderer.materials[ materialIndex ].SetTextureOffset( textureName, offset);
		}
	}
}
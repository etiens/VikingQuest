using UnityEngine;
using System.Collections;

public class ScreenFader : Singleton<ScreenFader>
{
	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
	
	
	public bool SceneStarting = true;
	public bool SceneEnding = false;
	
	private float lastTime;
	
	private Color clearColor;
	private Color solidColor;
	
	
	void Awake ()
	{
		// Set the texture so that it is the the size of the screen and covers it.
		//guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		lastTime = Time.realtimeSinceStartup;
		
		clearColor = Color.clear;
		solidColor = Color.white;
		
		guiTexture.color = solidColor = Color.white;
	}
	
	
	void Update ()
	{
		if(SceneStarting){
			StartScene();
		}
		else if(SceneEnding){
			EndScene();
		}
		lastTime = Time.realtimeSinceStartup;
	}
	
	
	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		guiTexture.color = Color.Lerp(guiTexture.color, clearColor, fadeSpeed * (Time.realtimeSinceStartup - lastTime));
	}
	
	
	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		guiTexture.color = Color.Lerp(guiTexture.color, solidColor, fadeSpeed * (Time.realtimeSinceStartup - lastTime));
	}
	
	
	void StartScene ()
	{
		// Fade the texture to clear.
		FadeToClear();
		
		// If the texture is almost clear...
		if(guiTexture.color.a <= 0.05f)
		{
			// ... set the colour to clear and disable the GUITexture.
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
			OnFadedInCompleted();
			
			// The scene is no longer starting.
			SceneStarting = false;
		}
	}
	
	
	public void EndScene ()
	{
		// Make sure the UI is disabled
		UIScript.Instance.DisableAllUI ();

		// Make sure the texture is enabled.
		guiTexture.enabled = true;
		
		// Start fading towards black.
		FadeToBlack();
		
		// If the screen is almost black...
		if(guiTexture.color.a >= 0.95f){
			// ... reload the level.
			OnFadedOutCompleted();
			SceneEnding = false;
		}
	}
	
	public delegate void FadedOutCompletedHandler();
	private FadedOutCompletedHandler m_FadedOutCompleted;
	public event FadedOutCompletedHandler FadedOutCompletedEvent{
		add
		{
			m_FadedOutCompleted += value;
		}
		
		remove
		{
			m_FadedOutCompleted -= value;
		}
	}
	public void OnFadedOutCompleted()
	{
		
		if (m_FadedOutCompleted != null) {
			m_FadedOutCompleted();
		}
	}
	
	public delegate void FadedInCompletedHandler();
	private FadedInCompletedHandler m_FadedInCompleted;
	public event FadedInCompletedHandler FadedInCompletedEvent{
		add
		{
			m_FadedInCompleted += value;
		}
		
		remove
		{
			m_FadedInCompleted -= value;
		}
	}
	public void OnFadedInCompleted()
	{
		
		if (m_FadedInCompleted != null) {
			m_FadedInCompleted();
		}
	}
}
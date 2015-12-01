using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIScript : Singleton<UIScript> {

	private bool uiDisabled = false;
	[SerializeField]
	private GameObject generalScriptContainer = null;

	[Header("Driver Canvas")]
	[SerializeField]
	private GameObject driverCanvas = null;

	[SerializeField]
	private RectTransform hpDriverScale = null;
	[SerializeField]
	private GameObject fillBarScaler = null;
	[SerializeField]
	private Image fillBarImage = null;
	[SerializeField]
	private GameObject switchNotificationDriver = null;

	[Header("Driver Overlay")]
	[SerializeField]
	private GameObject driverOverlay = null;

	private bool overlayActive = true;

	[Header("Harpooner Canvas")]
	[SerializeField]
	private GameObject harpoonerCanvas = null;

	[SerializeField]
	private RectTransform hpHarpoonerScale = null;
	[SerializeField]
	private RectTransform reloadScale = null;
	[SerializeField]
	private Image reloadImage = null;
	[SerializeField]
	private Image harpoonerSprite = null;
	[SerializeField]
	private Sprite harpoonerSpriteIceBreaker = null;
	[SerializeField]
	private Sprite harpoonerSpriteHunting = null;
	[SerializeField]
	private Image harpoonerPositionImage = null;
	[SerializeField]
	private Sprite harpoonerPositionLeft = null;
	[SerializeField]
	private Sprite harpoonerPositionRight = null;
	[SerializeField]
	private Sprite harpoonerPositionFront = null;
	[SerializeField]
	private Text harpoonerTypeText = null;
	private string harpoonerTypeTextIceBreaker = "Ice Breaker";
	private string harpoonerTypeTextHunting = "Hunting";
	[SerializeField]
	private GameObject switchNotificationHarpooner = null;

	[Header("Harpooner Overlay")]
	[SerializeField]
	private GameObject harpoonerOverlay = null;

	void Start () {
		driverCanvas.SetActive (false);
		harpoonerCanvas.SetActive (false);
		harpoonerOverlay.SetActive (false);
		driverOverlay.SetActive (false);
		generalScriptContainer.SetActive (true);
	}

	// Update is called once per frame
	void Update () {

		if (uiDisabled)
		{
			return;
		}

		// Switch notification (Harpooner <--> Driver)
		var switching = PositionSwitchingManager.Instance.State == PositionSwitchingManager.SwitchState.prompting;
		switchNotificationDriver.SetActive(switching);
		switchNotificationHarpooner.SetActive(switching);

		// Overlay activation (help when select pressed)
		if (overlayActive) {
			if (harpoonerCanvas.activeSelf)
			{
				if (!harpoonerOverlay.activeSelf)
				{
					harpoonerOverlay.SetActive(true);
					driverOverlay.SetActive(false);
				}
			}
			if (driverCanvas.activeSelf)
			{
				if (!driverOverlay.activeSelf)
				{
					driverOverlay.SetActive(true);
					harpoonerOverlay.SetActive(false);
				}
			}
		}

		if (Utils.Instance.Player1.SelectDown())
		{
			overlayActive = !overlayActive;
			harpoonerOverlay.SetActive(false);
			driverOverlay.SetActive(false);
		}

		// Switch between UI canvas
		if (!harpoonerCanvas.activeSelf && GlobalScript.Instance.Harpooner.HasControl)
		{
			driverCanvas.SetActive(false);
			harpoonerCanvas.SetActive(true);
		}
		if (!driverCanvas.activeSelf && GlobalScript.Instance.Driver.HasControl)
		{
			driverCanvas.SetActive(true);
			harpoonerCanvas.SetActive(false);
		}

		// Active UI elements (healt, boost, harpoon cooldown)
		var healthPercent = (GlobalScript.Instance.Boat.MaxHealth - GlobalScript.Instance.Boat.Health)/GlobalScript.Instance.Boat.MaxHealth;

		// DRIVER
		if (driverCanvas.activeSelf)
		{
			hpDriverScale.transform.localScale = new Vector3(1f, healthPercent, 1f);

			var scale = fillBarScaler.transform.localScale;
			scale.x = GlobalScript.Instance.Driver.BoostAmount/100f;
			fillBarScaler.transform.localScale = scale;

			if (GlobalScript.Instance.Driver.CoolingDown)
			{
				fillBarImage.color = Color.red;
			}
			else
			{
				fillBarImage.color = Color.green;
			}
		}
		// HARPOONER
		else
		{
			hpHarpoonerScale.transform.localScale = new Vector3(1f, healthPercent, 1f);

			if (GlobalScript.Instance.Harpooner.GetCurrentStation().CoolingDown)
			{
				float cooldownProgress = GlobalScript.Instance.Harpooner.GetCurrentStation().getCooldownProgressPercent();

				reloadScale.localScale = new Vector3(1f, cooldownProgress/100f, 1f);
				reloadImage.color = Color.white*0.4f;
			}
			else
			{
				reloadScale.localScale = Vector3.one;
				var color = reloadImage.color;
				reloadImage.color = Color.white*0.95f;
			}

			var station = GlobalScript.Instance.Harpooner.CurrentStation;
			if (station == Utils.Stations.Front)
			{
				harpoonerPositionImage.sprite = harpoonerPositionFront;
				harpoonerSprite.sprite = harpoonerSpriteIceBreaker;
				harpoonerTypeText.text = harpoonerTypeTextIceBreaker;
			}
			if (station == Utils.Stations.Left)
			{
				harpoonerPositionImage.sprite = harpoonerPositionLeft;
				harpoonerSprite.sprite = harpoonerSpriteHunting;
				harpoonerTypeText.text = harpoonerTypeTextHunting;
			}
			if (station == Utils.Stations.Right)
			{
				harpoonerPositionImage.sprite = harpoonerPositionRight;
				harpoonerSprite.sprite = harpoonerSpriteHunting;
				harpoonerTypeText.text = harpoonerTypeTextHunting;
			}
		}
	}

	/// <summary>
	/// This should be used to disable all of the UI.
	/// Remember to use EnableAllUI() after.
	/// </summary>
	public void DisableForQTE()
	{
		if (uiDisabled == false)
		{
			uiDisabled = true;

			driverCanvas.SetActive (false);
			harpoonerCanvas.SetActive (false);
			harpoonerOverlay.SetActive (false);
			driverOverlay.SetActive (false);
			generalScriptContainer.SetActive (true);
		}
	}

	/// <summary>
	/// This should be used to disable all of the UI.
	/// Remember to use EnableAllUI() after.
	/// </summary>
	public void DisableAllUI()
	{
		if (uiDisabled == false)
		{
			uiDisabled = true;

			driverCanvas.SetActive (false);
			harpoonerCanvas.SetActive (false);
			harpoonerOverlay.SetActive (false);
			driverOverlay.SetActive (false);
			generalScriptContainer.SetActive (false);
		}
	}

	/// <summary>
	/// This should be used to disable all of the UI.
	/// Remember to use EnableAllUI() after.
	/// </summary>
	public void EnableAllUI()
	{
		uiDisabled = false;
		generalScriptContainer.SetActive (true);
	}

}

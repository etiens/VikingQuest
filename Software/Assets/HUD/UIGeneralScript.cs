using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UIGeneralScript: MonoBehaviour {

	[SerializeField]
	private GameObject checkpointNotificationArea = null;
	[SerializeField]
	private Image checkpointImage = null;
	[SerializeField]
	private Text checkpointText = null;
	private List<int> checkpointNotificationShown = new List<int>();

	private bool checkpointActive = false;
	private float checkpointCurrentTimer = 0f;

	private float checkpointAppearTime = 1f;
	private float checkpointFadeTime = 4f;

	private List<string> checkpointTitles = new List<string>();

	[SerializeField]
	private GameObject krakenArea = null;
	[SerializeField]
	private RectTransform krakenHPScaling = null;
	private float currentScale = 0f;
	private float targetScale;

	private Kraken krakenScript = null;

	[SerializeField]
	private GameObject QTEContainer = null;
	[SerializeField]
	private RectTransform QTESlider = null;
	private float QTEMin = -384f;
	private float QTEMax = 384f;

	void Start()
	{
		checkpointTitles.Add ("Arrival to the Floe");
		checkpointTitles.Add ("Ice Corridors");
		checkpointTitles.Add (string.Empty);
		checkpointTitles.Add ("The Kraken");

		krakenScript = FindObjectOfType<Kraken> ();
	}

	void Update()
	{
		// QTE UI Management
		if (GlobalScript.Instance.QTEManager.InQTEMode)
		{
			
			// Make sure the UI is disabled in the QTE
			UIScript.Instance.DisableForQTE ();

			QTEContainer.SetActive(true);

			checkpointNotificationArea.SetActive(false);
			krakenArea.SetActive(false);

			var range = QTEMax - QTEMin;

			float sliderPosition = GlobalScript.Instance.QTEManager.QTEProgress * range;
			sliderPosition -= QTEMax;

			var position = QTESlider.localPosition;
			position.x = sliderPosition;
			QTESlider.localPosition = position;

			return;
		}
		else
		{
			QTEContainer.SetActive(false);
		}

		// Checkpoint notification check
		if (!checkpointNotificationShown.Contains(GlobalScript.CheckpointToLoad))
		{
			checkpointNotificationShown.Add(GlobalScript.CheckpointToLoad);
			if (checkpointTitles[GlobalScript.CheckpointToLoad - 1] != string.Empty)
			{
				checkpointActive = true;
				checkpointCurrentTimer = 0;
				checkpointText.text = checkpointTitles[GlobalScript.CheckpointToLoad - 1];
			}
		}

		// Checkpoint fancy alpha calculations
		if (checkpointActive)
		{
			checkpointNotificationArea.SetActive(true);
			float alpha = 1f;
			checkpointCurrentTimer += Time.deltaTime;
			if (checkpointCurrentTimer <= checkpointAppearTime)
			{
				alpha = checkpointCurrentTimer;
			}
			else if (checkpointCurrentTimer >= checkpointFadeTime - checkpointAppearTime)
			{
				alpha = checkpointFadeTime - checkpointCurrentTimer;
			}

			var color = checkpointText.color;
			color.a = alpha;
			checkpointText.color = color;
			color = checkpointImage.color;
			color.a = alpha;
			checkpointImage.color = color;

			if (checkpointCurrentTimer > checkpointFadeTime)
			{
				checkpointActive = false;
			}
		}
		else
		{
			checkpointNotificationArea.SetActive(false);
		}


		// Kraken HP check
		if (krakenScript.Engaged)
		{
			krakenArea.SetActive(true);
			targetScale = (float)krakenScript.HealthPoints / (float)krakenScript.MaxHP;
			if (Mathf.Abs(currentScale - targetScale) > 0.1f * Time.deltaTime)
			{
				if (Mathf.Sign(targetScale - currentScale) > 0)
				{
					currentScale += 0.2f * Time.deltaTime;
				}
				else
				{
					currentScale -= 0.1f * Time.deltaTime;
				}
			}
			else
			{
				currentScale = targetScale;
			}
			var scale = krakenHPScaling.localScale;
			scale.x = currentScale;
			krakenHPScaling.localScale = scale;
		}
		else
		{
			krakenArea.SetActive(false);
		}
	}

}

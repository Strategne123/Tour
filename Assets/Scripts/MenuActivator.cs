using UnityEngine;
using UnityEngine.UI;

public class MenuActivator : MonoBehaviour
{
	[SerializeField] GameObject menu;
	[SerializeField] Image fillImage;
	private float timeToHold = 1.5f;
	private GameObject fillImageParent;
	private float holdTime = 0f;
	private float delayBeforeFill = 0.2f;
	private float delayTimer = 0f;
	private bool isDelayPassed = false;

	private bool buttonPressed = false;

	private void Start()
	{
		fillImage.fillAmount = 0;
		fillImageParent = fillImage.transform.parent.gameObject;
		fillImageParent.SetActive(false);
		menu.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			buttonPressed = true; // надо добавить проверку на то что курсор не смотрит на интерактивный объект

			delayTimer = 0f;
			isDelayPassed = false;
			return;
		}

		if (!buttonPressed) return;

		if (Input.GetButtonUp("Fire1"))
		{
			fillImageParent.SetActive(false);
			fillImage.fillAmount = 0;
			holdTime = 0;
			delayTimer = 0f;
			isDelayPassed = false;
			buttonPressed = false;
			return;
		}


		if (Input.GetButton("Fire1"))
		{
			delayTimer += Time.deltaTime;

			if (delayTimer >= delayBeforeFill && !isDelayPassed)
			{
				fillImageParent.SetActive(true);
				isDelayPassed = true;
			}

			if (isDelayPassed)
			{
				holdTime += Time.deltaTime;
				fillImage.fillAmount = holdTime / timeToHold;

				if (holdTime >= timeToHold)
				{
					if (menu.activeInHierarchy) CloseMenu();
					ActivateMenu();
					fillImageParent.SetActive(false);
					fillImage.fillAmount = 0;
					holdTime = 0;
				}
			}
		}
	}

	public void ActivateMenu()
	{
		menu.transform.SetParent(null);
		menu.SetActive(true);
	}

	public void CloseMenu()
	{
		menu.transform.SetParent(transform);
		menu.transform.localPosition = Vector3.zero;
		menu.transform.localRotation = Quaternion.identity;
		menu.SetActive(false);
	}
}

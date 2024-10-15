using UnityEngine;
using UnityEngine.UI;
using Vrs.Internal;

public class MenuActivator : MonoBehaviour
{
	[SerializeField] GameObject menu;
	[SerializeField] Image fillImage;
	private GameObject fillImageParent;
	private float ipd;

	private void Start()
	{
		InitIPD();
		fillImage.fillAmount = 0;
		fillImageParent = fillImage.transform.parent.gameObject;
		fillImageParent.SetActive(false);
	}

	private void Update()
	{
        if (Input.GetButton("Cancel"))
		{
            if (menu.activeInHierarchy) CloseMenu();
            ActivateMenu();
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
		menu.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		menu.SetActive(false);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	private void InitIPD()
	{
		if (PlayerPrefs.HasKey("IPD"))
		{
			ipd = PlayerPrefs.GetFloat("IPD");
		}
		else
		{
			ipd = 58;
		}
		ChangeIPD(0);
	}

	public void ChangeIPD(int value)
	{
		ipd = PlayerPrefs.GetFloat("IPD");
		if (ipd == 0)
		{
			ipd = 58;
		}
		if ((ipd + value > 70) || (ipd + value <= 50))
			return;
		ipd += value;
		PlayerPrefs.SetFloat("IPD", ipd);
		var dist = ipd / 1000.0f;
		VrsViewer.Instance.SetIpd(dist);
	}

}

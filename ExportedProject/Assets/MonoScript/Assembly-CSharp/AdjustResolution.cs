using UnityEngine;
using UnityEngine.UI;

public class AdjustResolution : MonoBehaviour
{
	private void Awake()
	{
		float num = 1920f;
		float num2 = 1080f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		num3 = Screen.width;
		num4 = Screen.height;
		float num6 = num / num2;
		float num7 = num3 / num4;
		if (num7 < num6)
		{
			num5 = num6 / num7;
		}
		CanvasScaler component = base.transform.GetComponent<CanvasScaler>();
		if (num5 == 0f)
		{
			component.matchWidthOrHeight = 1f;
		}
		else
		{
			component.matchWidthOrHeight = 0f;
		}
	}
}

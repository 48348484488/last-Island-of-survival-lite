using UnityEngine;

public class SetImageBg : MonoBehaviour
{
	public string strImageName = string.Empty;

	public bool setNativeSize;

	private void Start()
	{
		if (!string.IsNullOrEmpty(strImageName))
		{
			View.SetSpriteABCommon(strImageName, base.gameObject, setNativeSize);
		}
	}
}

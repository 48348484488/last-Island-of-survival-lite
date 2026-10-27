using System.Collections.Generic;
using UnityEngine;

public class BuildingEffects : MonoBehaviour
{
	public List<GameObject> UsingEffect;

	public List<GameObject> UpingLvEffect;

	public List<GameObject> YouChanchuEffect;

	private void Awake()
	{
		HideAll();
	}

	public void HideAllUsing()
	{
		foreach (GameObject item in UsingEffect)
		{
			item.gameObject.SetActiveBetter(false);
		}
	}

	public void HideAllUping()
	{
		foreach (GameObject item in UpingLvEffect)
		{
			item.gameObject.SetActiveBetter(false);
		}
	}

	public void HideAllYouChanchu()
	{
		foreach (GameObject item in YouChanchuEffect)
		{
			item.gameObject.SetActiveBetter(false);
		}
	}

	public void HideAll()
	{
		HideAllUsing();
		HideAllUping();
		HideAllYouChanchu();
	}
}

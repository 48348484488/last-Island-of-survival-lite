using UnityEngine;

public class SdkMgr : Singleton<SdkMgr>
{
	public void Init()
	{
		GameObject gameObject = new GameObject("SdkMessager");
		gameObject.AddComponent<SdkMessager>();
	}
}

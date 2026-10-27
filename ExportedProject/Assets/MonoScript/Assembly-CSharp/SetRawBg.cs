using UnityEngine;

public class SetRawBg : MonoBehaviour
{
	public string RawName = string.Empty;

	private void Start()
	{
		View.SetTexture(base.gameObject, "texture/" + RawName);
	}
}

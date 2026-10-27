using UnityEngine;

public class SetEffectRotation : MonoBehaviour
{
	public Vector3 rotation = new Vector3(0f, 0f, 0f);

	private void Awake()
	{
		base.gameObject.transform.eulerAngles = rotation;
	}

	private void Update()
	{
		base.gameObject.transform.eulerAngles = rotation;
	}
}

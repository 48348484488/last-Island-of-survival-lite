using UnityEngine;

public class AutoRotation : MonoBehaviour
{
	public float Speed = 1f;

	public bool X;

	public bool Y;

	public bool Z;

	private void Start()
	{
	}

	private void Update()
	{
		if (Z)
		{
			base.gameObject.transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * Speed));
		}
		else if (X)
		{
			base.gameObject.transform.Rotate(new Vector3(Time.deltaTime * Speed, 0f, 0f));
		}
		else if (Y)
		{
			base.gameObject.transform.Rotate(new Vector3(0f, Time.deltaTime * Speed, 0f));
		}
	}
}

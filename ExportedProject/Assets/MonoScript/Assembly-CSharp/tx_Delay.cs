using UnityEngine;

public class tx_Delay : MonoBehaviour
{
	public float delayTime = 1f;

	private bool Init;

	private void Start()
	{
		base.gameObject.SetActive(false);
		Invoke("DelayFunc", delayTime);
	}

	private void DelayFunc()
	{
		base.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		if (Init)
		{
			base.gameObject.SetActive(false);
			Invoke("DelayFunc", delayTime);
			Init = false;
		}
	}

	private void OnDisable()
	{
		Init = true;
	}
}

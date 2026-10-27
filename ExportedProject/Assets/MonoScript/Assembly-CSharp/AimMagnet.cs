using UnityEngine;

public class AimMagnet : MonoBehaviour
{
	public Transform cameraTransform;

	public Transform selfTransform;

	public Collider SelfCollider;

	private const float MaxDistance = 450f;

	private const float MaxHeight = 6f;

	private const float MaxWidth = 10f;

	private const float MinHeight = 2.2f;

	private const float MinWidth = 2f;

	private const float MinDis = 5f;

	private Vector3 _tmpPos = Vector3.zero;

	public GameObject AimEffect;

	private void Start()
	{
		selfTransform = base.transform;
		cameraTransform = Battle.Ins.MainCamera.transform;
		SelfCollider = GetComponent<Collider>();
	}

	private void Update()
	{
		float num = Vector3.Distance(cameraTransform.position, selfTransform.position);
		if (num < 5f)
		{
			if (SelfCollider.enabled)
			{
				SelfCollider.enabled = false;
			}
			return;
		}
		if (!SelfCollider.enabled)
		{
			SelfCollider.enabled = true;
		}
		float value = num / 450f;
		value = Mathf.Clamp(value, 0f, 1f);
		float num2 = 2.2f + 3.8f * value;
		float newX = 2f + 8f * value;
		_tmpPos.Set(newX, num2, 0.01f);
		selfTransform.localScale = _tmpPos;
		_tmpPos.Set(0f, num2 * 0.5f, 0f);
		selfTransform.localPosition = _tmpPos;
		Vector3 eulerAngles = selfTransform.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		eulerAngles.y = cameraTransform.eulerAngles.y;
		selfTransform.eulerAngles = eulerAngles;
	}
}

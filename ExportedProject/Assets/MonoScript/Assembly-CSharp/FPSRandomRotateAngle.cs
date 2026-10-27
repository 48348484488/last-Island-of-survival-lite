using UnityEngine;

public class FPSRandomRotateAngle : MonoBehaviour
{
	public bool RotateX;

	public bool RotateY;

	public bool RotateZ = true;

	private Transform t;

	private void Awake()
	{
		t = base.transform;
	}

	private void OnEnable()
	{
		Vector3 zero = Vector3.zero;
		if (RotateX)
		{
			zero.x = Random.Range(0, 360);
		}
		if (RotateY)
		{
			zero.y = Random.Range(0, 360);
		}
		if (RotateZ)
		{
			zero.z = Random.Range(0, 360);
		}
		t.Rotate(zero);
	}
}

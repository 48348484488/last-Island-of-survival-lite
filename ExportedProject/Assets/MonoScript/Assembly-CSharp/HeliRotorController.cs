using UnityEngine;

public class HeliRotorController : MonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	internal Axis RotateAxis;

	internal bool _inputChangeRotateSpeed;

	private float _rotarSpeed;

	private float _inputChangeDegree = 150f;

	private HelicopterController _helicopterController;

	private float rotateDegree;

	private Vector3 OriginalRotate;

	public float RotarSpeed
	{
		get
		{
			return _rotarSpeed;
		}
		set
		{
			_rotarSpeed = Mathf.Clamp(value, 0f, 3000f);
			if (_rotarSpeed < 10f)
			{
				_rotarSpeed = 0f;
			}
		}
	}

	private void Start()
	{
		OriginalRotate = base.transform.localEulerAngles;
		_helicopterController = GetComponentInParent<HelicopterController>();
	}

	private void Update()
	{
		if (_inputChangeRotateSpeed)
		{
			rotateDegree += (RotarSpeed - _helicopterController.leftHorizontalInput * _inputChangeDegree) * Time.deltaTime;
		}
		else
		{
			rotateDegree += RotarSpeed * Time.deltaTime;
		}
		rotateDegree %= 360f;
		switch (RotateAxis)
		{
		case Axis.Y:
			base.transform.localRotation = Quaternion.Euler(OriginalRotate.x, rotateDegree, OriginalRotate.z);
			break;
		case Axis.Z:
			base.transform.localRotation = Quaternion.Euler(OriginalRotate.x, OriginalRotate.y, rotateDegree);
			break;
		default:
			base.transform.localRotation = Quaternion.Euler(rotateDegree, OriginalRotate.y, OriginalRotate.z);
			break;
		}
	}
}

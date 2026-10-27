using UnityEngine;

public class Rolling : MonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	private Axis _axis;

	[SerializeField]
	[Range(1f, 360f)]
	private float _speed = 70f;

	private Transform _selfTrans;

	private Vector3 _rollingEuler;

	private void Start()
	{
		_selfTrans = base.transform;
		switch (_axis)
		{
		case Axis.X:
			_rollingEuler = Vector3.right;
			break;
		case Axis.Y:
			_rollingEuler = Vector3.up;
			break;
		case Axis.Z:
			_rollingEuler = Vector3.forward;
			break;
		}
	}

	private void Update()
	{
		_selfTrans.localEulerAngles += _rollingEuler * _speed * Time.deltaTime;
	}
}

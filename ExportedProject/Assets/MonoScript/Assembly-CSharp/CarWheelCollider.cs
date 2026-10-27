using UnityEngine;
using gs.battle.scmsg;

[RequireComponent(typeof(WheelCollider))]
public class CarWheelCollider : MonoBehaviour
{
	private CarController carController;

	private Rigidbody rigid;

	private WheelCollider _wheelCollider;

	private int wheelPos;

	private float _mYRotation;

	private float _mRpm;

	public Transform wheelModel;

	private float wheelRotation;

	internal float steerAngle;

	internal float rpm;

	internal float wheelRPMToSpeed;

	private float wheelRotationToSync;

	public WheelCollider wheelCollider
	{
		get
		{
			if (_wheelCollider == null)
			{
				_wheelCollider = GetComponent<WheelCollider>();
			}
			return _wheelCollider;
		}
		set
		{
			_wheelCollider = value;
		}
	}

	private void Awake()
	{
	}

	public void Init(int wheelPos)
	{
		this.wheelPos = wheelPos;
		carController = GetComponentInParent<CarController>();
		rigid = carController.GetComponent<Rigidbody>();
		wheelCollider = GetComponent<WheelCollider>();
		wheelCollider.mass = rigid.mass / 15f;
	}

	private void Update()
	{
		if (carController.enabled)
		{
			if (!carController.sleepingRigid)
			{
				WheelAlign();
			}
			if (!carController.needSync)
			{
				SyncRotation();
			}
		}
	}

	private void FixedUpdate()
	{
		if (carController.enabled)
		{
			WheelHit hit;
			wheelCollider.GetGroundHit(out hit);
			steerAngle = wheelCollider.steerAngle;
			rpm = wheelCollider.rpm;
			wheelRPMToSpeed = wheelCollider.rpm * wheelCollider.radius / 2.8f * Mathf.Lerp(1f, 0.75f, hit.forwardSlip) * rigid.transform.lossyScale.y;
		}
	}

	public void WheelAlign()
	{
		if (!wheelModel)
		{
			Debug.LogError(base.transform.name + " wheel of the " + carController.transform.name + " is missing wheel model. This wheel is disabled");
			base.enabled = false;
			return;
		}
		Vector3 vector = wheelCollider.transform.TransformPoint(wheelCollider.center);
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, -wheelCollider.transform.up, out hitInfo, (wheelCollider.suspensionDistance + wheelCollider.radius) * base.transform.localScale.y) && !hitInfo.transform.IsChildOf(carController.transform) && !hitInfo.collider.isTrigger)
		{
			wheelModel.transform.position = hitInfo.point + wheelCollider.transform.up * wheelCollider.radius * base.transform.localScale.y;
		}
		else
		{
			wheelModel.transform.position = Vector3.Lerp(wheelModel.transform.position, vector - wheelCollider.transform.up * wheelCollider.suspensionDistance * base.transform.localScale.y, Time.deltaTime * 10f);
		}
		wheelRotation += wheelCollider.rpm * 6f * Time.deltaTime;
		wheelModel.transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(wheelRotation, wheelCollider.steerAngle, wheelCollider.transform.rotation.z);
	}

	internal void OnSSyncCarWheelRotation(SSyncCarWheelRotation sSyncCarWheelRotation)
	{
		if (wheelPos == 0 || wheelPos == 1)
		{
			_mYRotation = sSyncCarWheelRotation.frontYRatation;
		}
		else
		{
			_mYRotation = sSyncCarWheelRotation.tailYRatation;
		}
		_mRpm = sSyncCarWheelRotation.rpm;
	}

	private void SyncRotation()
	{
		float currentSpeed = carController.GetCurrentSpeed();
		if (currentSpeed > 0f)
		{
			wheelRotationToSync += currentSpeed * 12f * Time.deltaTime;
			wheelModel.transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(wheelRotationToSync, _mYRotation, 0f);
		}
	}
}

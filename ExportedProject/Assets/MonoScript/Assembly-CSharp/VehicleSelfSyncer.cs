using System.Collections;
using UnityEngine;
using gs.battle.scmsg;

public class VehicleSelfSyncer : MonoBehaviour
{
	private const float SYNC_TIME = 0.2f;

	private const float DELTA_POS = 0.01f;

	private const float DELTA_ORIENTATION = 2f;

	private const float DELTA_VELOCITY = 0.1f;

	private const float DELTA_FUEL = 2f;

	private Vector3 pos = default(Vector3);

	private Vector3 orientation = default(Vector3);

	private Vector3 velocity = default(Vector3);

	private float fuel;

	private static readonly CSyncVehiclePos cSyncVehiclePos = new CSyncVehiclePos();

	private static readonly CSyncVehicleOrientation cSyncVehicleOrientation = new CSyncVehicleOrientation();

	private static readonly CSyncVehicleFuel cSyncVehicleFuel = new CSyncVehicleFuel();

	private Transform mTransform;

	private Rigidbody mRigidbody;

	private IVehicle iVehicle;

	private int _mVehicleId;

	private void Awake()
	{
		mTransform = base.transform;
		mRigidbody = GetComponent<Rigidbody>();
		iVehicle = GetComponent<IVehicle>();
		UpdatePos();
		UpdateOrientation();
		UpdateVelocity();
		UpdateFuel();
		iVehicle.SetNeedSync(true);
	}

	private bool NeedUpdateFuel()
	{
		if (Mathf.Abs(iVehicle.GetFuel() - fuel) >= 2f)
		{
			return true;
		}
		return false;
	}

	private void UpdateFuel()
	{
		fuel = iVehicle.GetFuel();
	}

	private void ReportFuelChange()
	{
		cSyncVehicleFuel.id = _mVehicleId;
		cSyncVehicleFuel.fuel = fuel;
		Client2Gs.Ins.Send(cSyncVehicleFuel);
	}

	public void SetVehicleId(int vehicleId)
	{
		_mVehicleId = vehicleId;
	}

	private bool NeedUpdatePos()
	{
		if (Mathf.Abs(mTransform.position.x - pos.x) > 0.01f || Mathf.Abs(mTransform.position.y - pos.y) > 0.01f || Mathf.Abs(mTransform.position.z - pos.z) > 0.01f)
		{
			return true;
		}
		return false;
	}

	private void UpdatePos()
	{
		pos.Set(mTransform.position.x, mTransform.position.y, mTransform.position.z);
	}

	private void ReportPosChange()
	{
		cSyncVehiclePos.id = _mVehicleId;
		cSyncVehiclePos.pos.x = pos.x;
		cSyncVehiclePos.pos.y = pos.y;
		cSyncVehiclePos.pos.z = pos.z;
		Client2Gs.Ins.Send(cSyncVehiclePos);
	}

	private bool NeedUpdateOrientation()
	{
		if (Mathf.Abs(mTransform.eulerAngles.x - orientation.x) >= 2f || Mathf.Abs(mTransform.eulerAngles.y - orientation.y) >= 2f || Mathf.Abs(mTransform.eulerAngles.z - orientation.z) >= 2f)
		{
			return true;
		}
		return false;
	}

	private void UpdateOrientation()
	{
		orientation.Set(mTransform.eulerAngles.x, mTransform.eulerAngles.y, mTransform.eulerAngles.z);
	}

	private void ReportOrientaionChange()
	{
		cSyncVehicleOrientation.id = _mVehicleId;
		cSyncVehicleOrientation.orientation.x = MathUtils.Float2Short(orientation.x);
		cSyncVehicleOrientation.orientation.y = MathUtils.Float2Short(orientation.y);
		cSyncVehicleOrientation.orientation.z = MathUtils.Float2Short(orientation.z);
		Client2Gs.Ins.Send(cSyncVehicleOrientation);
	}

	private bool NeedUpdateVelocity()
	{
		if (Mathf.Abs(mRigidbody.velocity.x - velocity.x) >= 0.1f || Mathf.Abs(mRigidbody.velocity.y - velocity.y) >= 0.1f || Mathf.Abs(mRigidbody.velocity.z - velocity.z) >= 0.1f)
		{
			return true;
		}
		return false;
	}

	private void UpdateVelocity()
	{
		velocity.Set(mRigidbody.velocity.x, mRigidbody.velocity.y, mRigidbody.velocity.z);
	}

	private void Start()
	{
		StartCoroutine(Sync());
	}

	private IEnumerator Sync()
	{
		while (true)
		{
			if (NeedUpdatePos())
			{
				UpdatePos();
				ReportPosChange();
			}
			if (NeedUpdateOrientation())
			{
				UpdateOrientation();
				ReportOrientaionChange();
			}
			if (NeedUpdateFuel())
			{
				UpdateFuel();
				ReportFuelChange();
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void OnDestroy()
	{
		iVehicle.SetNeedSync(false);
	}
}

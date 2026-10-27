using System.Collections;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class VehicleMonitor : MapObject
{
	private SVehicleInfo sVehicleInfo;

	private Transform[] attachPoints;

	private Transform[] tanshenPoints;

	private GameObject[] gameObjects;

	private Rigidbody SelfRigidbody;

	private bool isDriver;

	private VehicleCfg vehicleCfg;

	private IVehicle iVehicle;

	private int vehicleId;

	private Vehicle3rdSyncer vehicle3RdSyncer;

	private VehicleSelfSyncer vehicleSelfSyncer;

	private CCauseDamgeByVehicle cCauseDamgeByVehicle;

	private static readonly CStopVehicleSound cStopVehicleSound = new CStopVehicleSound();

	private static readonly CVehicleExplosion cVehicleExplosion = new CVehicleExplosion();

	private long[] roleIds;

	private Coroutine _mWaitStopCoroutine;

	private Coroutine _mMakeCarToFall;

	private List<Collider> vehicleColliders;

	public Transform VehicleTransform;

	private bool _mSleeping;

	public int CullingIndex = -1;

	public bool Culled = true;

	public SceneLod Lod;

	private int _lodLevel = -1;

	private static int s_GetOutCheckLayer = 0;

	private static readonly RaycastHit[] s_Hits = new RaycastHit[32];

	public Transform[] Seats
	{
		get
		{
			return attachPoints;
		}
	}

	public Transform[] TanshenPosints
	{
		get
		{
			return tanshenPoints;
		}
	}

	private new void Awake()
	{
		SelfRigidbody = GetComponent<Rigidbody>();
		CullingIndex = -1;
		Culled = false;
		Lod = GetComponent<SceneLod>();
	}

	public void Init(SVehicleInfo sVehicleInfo)
	{
		VehicleTransform = base.transform;
		this.sVehicleInfo = sVehicleInfo;
		vehicleId = sVehicleInfo.vehicleInfo.id;
		InsId = vehicleId;
		iVehicle = GetComponent<IVehicle>();
		vehicleCfg = VehicleCfg.Get(sVehicleInfo.vehicleInfo.type);
		isDriver = sVehicleInfo.vehicleInfo.driverRoleId == Singleton<RoleMgr>.Ins.info.roleId;
		InitAttachPoint();
		UpdateVehicleInput();
		if (isDriver)
		{
			OnSelfDrive();
		}
		else if (sVehicleInfo.vehicleInfo.driverRoleId > 0)
		{
			OnOtherDrive();
		}
		UpdateRigidBody(isDriver);
		if (sVehicleInfo.vehicleInfo.seatInfo.Count > 0)
		{
			StartCoroutine(InitPlayerPos());
		}
		else if (vehicleCfg.vehicleType == 0 || vehicleCfg.vehicleType == 3)
		{
			_mMakeCarToFall = StartCoroutine(MakeCarToFall());
		}
		vehicleColliders = new List<Collider>();
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			if (LayerMask.LayerToName(collider.gameObject.layer).Equals("Car"))
			{
				vehicleColliders.Add(collider);
			}
		}
		Battle.Ins.RegisVehicleForCulling(this);
	}

	private IEnumerator MakeCarToFall()
	{
		float y = Battle.Ins.GetGroundPosForCar(base.transform.position).y;
		if (y < base.transform.position.y)
		{
			UpdateRigidBody(true);
			yield return new WaitForSeconds(1.5f);
			UpdateRigidBody(false);
			if (vehicleCfg.vehicleType == 0)
			{
				((CarController)iVehicle).DiableWheelCollidar();
			}
			else if (vehicleCfg.vehicleType == 3)
			{
				((TwoWheelMotoController)iVehicle).DiableWheelCollidar();
			}
		}
		else
		{
			UpdateRigidBody(false);
			if (vehicleCfg.vehicleType == 0)
			{
				((CarController)iVehicle).DiableWheelCollidar();
			}
			else if (vehicleCfg.vehicleType == 3)
			{
				((TwoWheelMotoController)iVehicle).DiableWheelCollidar();
			}
			yield return null;
		}
	}

	public int GetSeatIndex(GameObject player)
	{
		for (int i = 0; i < gameObjects.Length; i++)
		{
			if (gameObjects[i] == player)
			{
				return i;
			}
		}
		return -1;
	}

	public void BackToSeat(GameObject player)
	{
		int seatIndex = GetSeatIndex(player);
		if (seatIndex != -1)
		{
			player.transform.SetParent(attachPoints[seatIndex]);
			player.transform.localPosition = Vector3.zero;
		}
	}

	public void MoveToTanshenSeat(GameObject player)
	{
		int seatIndex = GetSeatIndex(player);
		if (seatIndex != -1)
		{
			player.transform.SetParent(tanshenPoints[seatIndex]);
			player.transform.localPosition = Vector3.zero;
		}
	}

	public void OnPlayerDie(GameObject go, bool isSelf)
	{
		for (int i = 0; i < gameObjects.Length; i++)
		{
			if (!(go == gameObjects[i]))
			{
				continue;
			}
			BasePlayerController component = go.GetComponent<BasePlayerController>();
			if (isDriver && isSelf)
			{
				cStopVehicleSound.vehicleId = vehicleId;
				cStopVehicleSound.soundId = iVehicle.GetEngineSoundId();
				Client2Gs.Ins.Send(cStopVehicleSound);
			}
			Vector3 pos;
			if (FindGetOutPos(i, component.PlayerCollider, out pos))
			{
				if (isSelf)
				{
					Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.Stand, pos, true);
					Battle.Ins.SendGetOutVehicle(sVehicleInfo.vehicleInfo.id, pos);
				}
				else
				{
					component.PlayerTransform.position = pos;
				}
				attachPoints[i].DetachChildren();
				gameObjects[i] = null;
				roleIds[i] = 0L;
			}
			if (i == 0 && isSelf)
			{
				isDriver = false;
				RemoveInput();
			}
			break;
		}
	}

	private void RemoveInput()
	{
		if (vehicleCfg.vehicleType == 0)
		{
			UpdateCarInputScript();
		}
		else if (vehicleCfg.vehicleType == 2)
		{
			UpdatePlaneInputScript();
		}
		else if (vehicleCfg.vehicleType == 1)
		{
			UpdateShipInputScript();
		}
		else if (vehicleCfg.vehicleType == 3)
		{
			UpdateTwoWheelMotoInputScript();
		}
	}

	public int GetVehicleType()
	{
		return vehicleCfg.vehicleType;
	}

	public int GetId()
	{
		return vehicleId;
	}

	public IVehicle GetIVehicle()
	{
		return iVehicle;
	}

	public int GetEngineSoundId()
	{
		return iVehicle.GetEngineSoundId();
	}

	public void SyncWheelRotation(SSyncCarWheelRotation sSyncCarWheelRotation)
	{
		if (vehicleCfg.vehicleType == 0)
		{
			((CarController)iVehicle).SyncWheelRotation(sSyncCarWheelRotation);
		}
		else if (vehicleCfg.vehicleType == 3)
		{
			((TwoWheelMotoController)iVehicle).SyncWheelRotation(sSyncCarWheelRotation);
		}
	}

	private IEnumerator InitPlayerPos()
	{
		foreach (KeyValuePair<int, long> pair in sVehicleInfo.vehicleInfo.seatInfo)
		{
			int seat = pair.Key;
			long roleId = pair.Value;
			BasePlayerController p;
			while (true)
			{
				p = Battle.Ins.GetPlayer(roleId);
				if (p != null)
				{
					break;
				}
				yield return null;
			}
			SitDown(p.gameObject, attachPoints[seat], seat);
			if (roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				Battle.Ins.SelfPlayer.NearCar = this;
				Battle.Ins.SelfPlayer.OnGetInCar(vehicleId);
				if (seat == 0)
				{
					iVehicle.KillOrStartEngine();
				}
			}
			if (p.Tanshen)
			{
				MoveToTanshenSeat(p.gameObject);
			}
			p.OnGetInCar(vehicleId);
		}
	}

	private void UpdateRigidBody(bool usePhysicsEngine)
	{
		SelfRigidbody.useGravity = usePhysicsEngine;
		SelfRigidbody.isKinematic = !usePhysicsEngine;
	}

	private void AddSelfSync()
	{
		if (vehicleSelfSyncer == null)
		{
			vehicleSelfSyncer = base.gameObject.AddComponent<VehicleSelfSyncer>();
		}
		vehicleSelfSyncer.SetVehicleId(sVehicleInfo.vehicleInfo.id);
	}

	private void RemoveSelfSync()
	{
		if (vehicleSelfSyncer != null)
		{
			Object.Destroy(vehicleSelfSyncer);
			vehicleSelfSyncer = null;
		}
	}

	private void Add3rdSync()
	{
		if (vehicle3RdSyncer == null)
		{
			vehicle3RdSyncer = base.gameObject.AddComponent<Vehicle3rdSyncer>();
		}
	}

	private void Remove3rdSync()
	{
		if (vehicle3RdSyncer != null)
		{
			Object.Destroy(vehicle3RdSyncer);
			vehicle3RdSyncer = null;
		}
	}

	private void InitAttachPoint()
	{
		attachPoints = iVehicle.GetAttachPoints();
		if (vehicleCfg.vehicleType == 0)
		{
			tanshenPoints = ((CarController)iVehicle).GetTanshenPoints();
		}
		gameObjects = new GameObject[attachPoints.Length];
		roleIds = new long[attachPoints.Length];
	}

	public long[] GetRoleIds()
	{
		return roleIds;
	}

	private void UpdateVehicleInput()
	{
		if (vehicleCfg.vehicleType == 0)
		{
			UpdateCarInputScript();
		}
		else if (vehicleCfg.vehicleType == 2)
		{
			UpdatePlaneInputScript();
		}
		else if (vehicleCfg.vehicleType == 1)
		{
			UpdateShipInputScript();
		}
		else if (vehicleCfg.vehicleType == 3)
		{
			UpdateTwoWheelMotoInputScript();
		}
	}

	private void UpdateCarInputScript()
	{
		CarInput component = base.gameObject.GetComponent<CarInput>();
		if (isDriver)
		{
			if (component == null)
			{
				base.gameObject.AddComponent<CarInput>();
			}
		}
		else if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void UpdatePlaneInputScript()
	{
		HelicopterInput component = base.gameObject.GetComponent<HelicopterInput>();
		if (isDriver)
		{
			if (component == null)
			{
				base.gameObject.AddComponent<HelicopterInput>();
			}
		}
		else if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void UpdateShipInputScript()
	{
		ShipInput component = base.gameObject.GetComponent<ShipInput>();
		if (isDriver)
		{
			if (component == null)
			{
				base.gameObject.AddComponent<ShipInput>();
			}
		}
		else if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void UpdateTwoWheelMotoInputScript()
	{
		TwoWheelMotoInput component = base.gameObject.GetComponent<TwoWheelMotoInput>();
		if (isDriver)
		{
			if (component == null)
			{
				base.gameObject.AddComponent<TwoWheelMotoInput>();
			}
		}
		else if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void OnSelfDrive()
	{
		AddSelfSync();
		Remove3rdSync();
		UpdateRigidBody(true);
		if (vehicleCfg.vehicleType == 0 && iVehicle != null)
		{
			((CarController)iVehicle).EnableWheelCollidar();
		}
		if (vehicleCfg.vehicleType == 3 && iVehicle != null)
		{
			((TwoWheelMotoController)iVehicle).EnableWheelCollidar();
		}
	}

	private void OnOtherDrive()
	{
		RemoveSelfSync();
		Add3rdSync();
		UpdateRigidBody(false);
		if (vehicleCfg.vehicleType == 0 && iVehicle != null)
		{
			((CarController)iVehicle).DiableWheelCollidar();
		}
		if (vehicleCfg.vehicleType == 3 && iVehicle != null)
		{
			((TwoWheelMotoController)iVehicle).DiableWheelCollidar();
		}
	}

	public bool IsBroken()
	{
		return iVehicle.IsBroken();
	}

	public void GetInVehicle(PlayerController p, bool onDriverSeat)
	{
		if (IsBroken())
		{
			return;
		}
		int num = -1;
		if (onDriverSeat)
		{
			if (gameObjects[0] == null)
			{
				num = 0;
			}
		}
		else
		{
			for (int i = 1; i < attachPoints.Length; i++)
			{
				if (gameObjects[i] == null)
				{
					num = i;
					break;
				}
			}
		}
		if (num >= 0)
		{
			Battle.Ins.SendGetInVehicle(sVehicleInfo.vehicleInfo.id, num);
		}
	}

	public void SwitchToDrive(GameObject go)
	{
		int currentSeat = GetCurrentSeat(go);
		if (currentSeat > 0)
		{
			if (gameObjects[0] == null)
			{
				Battle.Ins.OnSwitchSeat(vehicleId, 0);
			}
			else
			{
				AlertBox.Show(221);
			}
		}
	}

	private int GetCurrentSeat(GameObject go)
	{
		int num = 0;
		GameObject[] array = gameObjects;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == go)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public void SwitchSeat(GameObject go)
	{
		int num = -1;
		int currentSeat = GetCurrentSeat(go);
		if (currentSeat < 0)
		{
			return;
		}
		for (int i = 0; i < attachPoints.Length; i++)
		{
			num = (i + 1 + currentSeat) % attachPoints.Length;
			if (gameObjects[num] == null && num != currentSeat)
			{
				Battle.Ins.OnSwitchSeat(vehicleId, num);
				break;
			}
		}
	}

	public void SwitchTo(GameObject go, int newSeat, bool isSelf)
	{
		int currentSeat = GetCurrentSeat(go);
		if (currentSeat < 0)
		{
			return;
		}
		attachPoints[currentSeat].DetachChildren();
		gameObjects[currentSeat] = null;
		roleIds[currentSeat] = 0L;
		SitDown(go, attachPoints[newSeat], newSeat);
		if (newSeat == 0 && !isSelf)
		{
			OnOtherDrive();
		}
		if (!isSelf)
		{
			return;
		}
		if (isDriver != (newSeat == 0))
		{
			iVehicle.KillOrStartEngine();
		}
		isDriver = newSeat == 0;
		UpdateVehicleInput();
		if (isDriver)
		{
			OnSelfDrive();
		}
		if (Battle.Ins.SelfPlayer != null)
		{
			Battle.Ins.SelfPlayer.Tanshen = false;
			if (vehicleCfg.vehicleType == 2 && !Battle.Ins.SelfPlayer.HaveGunInHand && Battle.Ins.SelfPlayer.HaveGun)
			{
				Battle.Ins.SelfPlayer.PlayChangeWeapen(-1);
			}
		}
	}

	public void GetInVehicle(OtherPlayerController p, int index)
	{
		SitDown(p.gameObject, attachPoints[index], index);
		if (index == 0)
		{
			OnOtherDrive();
		}
		p.OnGetInCar(vehicleId);
		if (BattleEvent.OnOtherGetInVehicle != null)
		{
			BattleEvent.OnOtherGetInVehicle(vehicleId);
		}
	}

	public void GetInVehicle(PlayerController p, int index)
	{
		SitDown(p.gameObject, attachPoints[index], index);
		if (index == 0)
		{
			OnSelfDrive();
		}
		p.OnGetInCar(vehicleId);
		isDriver = index == 0;
		if (isDriver)
		{
			iVehicle.KillOrStartEngine();
		}
		UpdateVehicleInput();
		if (BattleEvent.OnGetInVehicle != null)
		{
			BattleEvent.OnGetInVehicle();
		}
	}

	private void SitDown(GameObject go, Transform attachPoint, int seat)
	{
		gameObjects[seat] = go;
		go.transform.SetParent(attachPoint);
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
		roleIds[seat] = go.GetComponent<BasePlayerController>().RoleId;
		if (_mMakeCarToFall != null)
		{
			StopCoroutine(_mMakeCarToFall);
		}
		if (seat == 0 && _mWaitStopCoroutine != null)
		{
			StopCoroutine(_mWaitStopCoroutine);
		}
	}

	public bool GetOutVehicle(BasePlayerController p, bool isSelf)
	{
		GameObject gameObject = p.gameObject;
		int num = 0;
		GameObject[] array = gameObjects;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 == gameObject)
			{
				if (isSelf)
				{
					Vector3 pos;
					if (!FindGetOutPos(num, Battle.Ins.SelfPlayer.PlayerCollider, out pos))
					{
						return false;
					}
					DoGetoutVehicleDamage();
					if (isDriver)
					{
						cStopVehicleSound.vehicleId = vehicleId;
						cStopVehicleSound.soundId = iVehicle.GetEngineSoundId();
						Client2Gs.Ins.Send(cStopVehicleSound);
					}
					isDriver = false;
					Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.Stand, pos, true);
					Battle.Ins.SendGetOutVehicle(sVehicleInfo.vehicleInfo.id, pos);
					UpdateVehicleInput();
					if (num == 0)
					{
						iVehicle.KillOrStartEngine();
					}
					if (vehicleSelfSyncer != null)
					{
						_mWaitStopCoroutine = StartCoroutine(WaitForVehicleStop());
					}
					if (BattleEvent.onGetOutOfVehicle != null)
					{
						BattleEvent.onGetOutOfVehicle(vehicleId, base.gameObject.transform.position);
					}
				}
				gameObjects[num] = null;
				attachPoints[num].DetachChildren();
				p.OnGetOutCar();
				roleIds[num] = 0L;
				return true;
			}
			num++;
		}
		return false;
	}

	private void DoGetoutVehicleDamage()
	{
		if (vehicleCfg.vehicleType == 0)
		{
			CarCfg carCfg = CarCfg.Get(vehicleCfg.id);
			float num = 0f;
			if (isDriver)
			{
				num = GetSpeed();
			}
			else if (vehicle3RdSyncer != null)
			{
				num = vehicle3RdSyncer.GetSpeed();
			}
			float num2 = num * carCfg.jumpCarPlayerDropHpRate1 + carCfg.jumpCarPlayerDropHpRate2;
			if (!(num2 > 0f))
			{
			}
		}
		else if (vehicleCfg.vehicleType == 3)
		{
			TwoWheelMotoCfg twoWheelMotoCfg = TwoWheelMotoCfg.Get(vehicleCfg.id);
			float num3 = 0f;
			if (isDriver)
			{
				num3 = GetSpeed();
			}
			else if (vehicle3RdSyncer != null)
			{
				num3 = vehicle3RdSyncer.GetSpeed();
			}
			float num4 = num3 * twoWheelMotoCfg.jumpCarPlayerDropHpRate1 + twoWheelMotoCfg.jumpCarPlayerDropHpRate2;
			if (!(num4 > 0f))
			{
			}
		}
	}

	private IEnumerator WaitForVehicleStop()
	{
		yield return new WaitForSeconds(10f);
		while (SelfRigidbody.velocity.magnitude > 0.1f)
		{
			yield return new WaitForSeconds(0.5f);
		}
		OnOtherDrive();
	}

	public void SetPos(float x, float y, float z)
	{
		if (vehicle3RdSyncer != null)
		{
			vehicle3RdSyncer.SetPos(x, y, z);
			Battle.Ins.SetVehicleCullingPos(CullingIndex, new Vector3(x, y, z));
		}
	}

	public void SetOrientation(float x, float y, float z)
	{
		if (vehicle3RdSyncer != null)
		{
			vehicle3RdSyncer.SetOrientation(x, y, z);
		}
	}

	public float GetFuel()
	{
		return iVehicle.GetFuel();
	}

	public float GetMaxFuel()
	{
		return iVehicle.GetMaxFuel();
	}

	public float GetSpeed()
	{
		if (vehicleSelfSyncer != null)
		{
			return iVehicle.GetSpeed();
		}
		return Get3rdSpeed();
	}

	public void SetFuel(float fuel)
	{
		iVehicle.SetFuel(fuel);
	}

	public bool IsDriver()
	{
		return isDriver;
	}

	public void SyncEngineSound(float volume)
	{
		iVehicle.SyncEngineSound(volume);
	}

	public void OnDamage(float damage, Vector3 pos)
	{
		iVehicle.OnDamage(damage, pos);
	}

	public void OnExplosion()
	{
		cVehicleExplosion.vehicleId = vehicleId;
		cVehicleExplosion.roleDamageInfo.Clear();
		cVehicleExplosion.vehicleDamageInfo.Clear();
		Collider[] array = Physics.OverlapSphere(base.transform.position, 5f);
		if (array != null && array.Length > 0)
		{
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (collider == null)
				{
					continue;
				}
				if (collider.gameObject.tag == "OtherPlayerCollider" || collider.gameObject.tag == "SelfCollider")
				{
					BasePlayerController componentInParent = collider.GetComponentInParent<BasePlayerController>();
					if (componentInParent != null && !IsPassenger(componentInParent.gameObject))
					{
						float num = Vector3.Distance(base.transform.position, componentInParent.transform.position);
						float num2 = (float)ConstsBs.VEHICLE_EXPLOSION_DAMAGE_TO_PLAYER * (1f - num * 0.1f);
						if (num2 > 0f)
						{
							cVehicleExplosion.roleDamageInfo[componentInParent.RoleId] = (int)num2;
						}
					}
				}
				if (collider.gameObject.layer != LayerMask.NameToLayer("Car"))
				{
					continue;
				}
				VehicleMonitor componentInParent2 = collider.GetComponentInParent<VehicleMonitor>();
				if (componentInParent2 != null && componentInParent2.vehicleId != vehicleId)
				{
					float num3 = Vector3.Distance(base.transform.position, componentInParent2.transform.position);
					float num4 = (float)ConstsBs.VEHICLE_EXPLOSION_DAMAGE_TO_VEHICLE * (1f - num3 * 0.1f);
					if (num4 > 0f)
					{
						cVehicleExplosion.vehicleDamageInfo[componentInParent2.vehicleId] = (int)num4;
					}
				}
			}
		}
		GameObject[] array3 = gameObjects;
		foreach (GameObject gameObject in array3)
		{
			if (gameObject != null)
			{
				BasePlayerController componentInParent3 = gameObject.GetComponentInParent<BasePlayerController>();
				if (componentInParent3 != null)
				{
					cVehicleExplosion.roleDamageInfo[componentInParent3.RoleId] = ConstsBs.VEHICLE_EXPLOSION_DAMAGE_TO_PLAYER;
				}
			}
		}
		if (cVehicleExplosion.vehicleDamageInfo.Count > 0 || cVehicleExplosion.roleDamageInfo.Count > 0)
		{
			Client2Gs.Ins.Send(cVehicleExplosion);
		}
	}

	public int GetHp()
	{
		return iVehicle.GetHp();
	}

	public int GetMaxHp()
	{
		return iVehicle.GetMaxHp();
	}

	public void OnHit(float relativeSpeed)
	{
		iVehicle.OnHit(relativeSpeed);
	}

	public void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		iVehicle.OnSStopVehicleSound(sStopVehicleSound);
	}

	public void OnSVehicleHpChange(SVehicleHpChange sVehicleHpChange)
	{
		iVehicle.SetHp(sVehicleHpChange.hp);
	}

	private bool FindGetOutPos(int seatIndex, CapsuleCollider playerCollider, out Vector3 pos)
	{
		float checkColliderRadius = playerCollider.radius - 0.05f;
		float height = playerCollider.height;
		if (FindGetOutPosForSeat(attachPoints[seatIndex].position, height, checkColliderRadius, out pos))
		{
			return true;
		}
		for (int num = (seatIndex + 1) % attachPoints.Length; num != seatIndex; num = (num + 1) % attachPoints.Length)
		{
			if (FindGetOutPosForSeat(attachPoints[num].position, height, checkColliderRadius, out pos))
			{
				return true;
			}
		}
		if (FindGetOutPosFrontBackUp(attachPoints[seatIndex].position, height, checkColliderRadius, out pos))
		{
			return true;
		}
		for (int num = (seatIndex + 1) % attachPoints.Length; num != seatIndex; num = (num + 1) % attachPoints.Length)
		{
			if (FindGetOutPosFrontBackUp(attachPoints[num].position, height, checkColliderRadius, out pos))
			{
				return true;
			}
		}
		return false;
	}

	private bool FindGetOutPosForSeat(Vector3 seatPos, float checkColliderHeight, float checkColliderRadius, out Vector3 pos)
	{
		Vector3 normalized = ((!(base.transform.InverseTransformPoint(seatPos).x <= 0f)) ? Vector3.Cross(Vector3.up, base.transform.forward) : Vector3.Cross(base.transform.forward, Vector3.up)).normalized;
		return FindGetOutPosOnDirection(seatPos, normalized, checkColliderHeight, checkColliderRadius, 2f, out pos);
	}

	private bool FindGetOutPosFrontBackUp(Vector3 seatPos, float checkColliderHeight, float checkColliderRadius, out Vector3 pos)
	{
		if (FindGetOutPosOnDirection(seatPos, Vector3.up, checkColliderHeight, checkColliderRadius, 2f, out pos))
		{
			return true;
		}
		if (FindGetOutPosOnDirection(seatPos, -base.transform.forward, checkColliderHeight, checkColliderRadius, 3f, out pos))
		{
			return true;
		}
		if (FindGetOutPosOnDirection(seatPos, base.transform.forward, checkColliderHeight, checkColliderRadius, 3f, out pos))
		{
			return true;
		}
		return false;
	}

	private bool FindGetOutPosOnDirection(Vector3 fromPos, Vector3 direction, float checkColliderHeight, float checkColliderRadius, float maxCheckDis, out Vector3 pos)
	{
		if (s_GetOutCheckLayer == 0)
		{
			s_GetOutCheckLayer = ~LayerMask.GetMask("Bullet", "SelfPlayer", "CarBullet", "Gun", "Water");
		}
		Vector3 point = fromPos;
		point.y += checkColliderHeight - checkColliderRadius;
		Vector3 point2 = fromPos;
		point2.y += checkColliderRadius;
		float num = maxCheckDis;
		int num2 = Physics.CapsuleCastNonAlloc(point, point2, checkColliderRadius, direction, s_Hits, num, s_GetOutCheckLayer, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num2; i++)
		{
			if (!Utils.IsParentTransform(s_Hits[i].transform, base.transform))
			{
				num = s_Hits[i].distance - 0.05f;
				break;
			}
		}
		pos = fromPos + direction * num;
		pos = Battle.Ins.GetGroundPos(pos);
		if (pos.y < base.transform.position.y)
		{
			pos.y = base.transform.position.y;
		}
		point = (point2 = pos);
		point.y = pos.y + (checkColliderHeight - checkColliderRadius);
		point2.y = pos.y + checkColliderRadius;
		return !Physics.CheckCapsule(point, point2, checkColliderRadius, s_GetOutCheckLayer, QueryTriggerInteraction.Ignore);
	}

	public float GetCurrentHeight()
	{
		if (GetVehicleType() == 2)
		{
			return ((HelicopterController)iVehicle).GetHeight();
		}
		return base.transform.position.y;
	}

	public float GetMaxHeight()
	{
		if (GetVehicleType() == 2)
		{
			return ((HelicopterController)iVehicle).GetMaxHeigth();
		}
		return 1000f;
	}

	public bool IsPassenger(GameObject go)
	{
		if (gameObjects != null)
		{
			GameObject[] array = gameObjects;
			foreach (GameObject gameObject in array)
			{
				if (go == gameObject)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void PlayHornSound()
	{
		iVehicle.PlayHornSound();
	}

	public float Get3rdSpeed()
	{
		if (vehicle3RdSyncer != null)
		{
			return vehicle3RdSyncer.GetSpeed();
		}
		return 0f;
	}

	private void OnDestroy()
	{
		if (Battle.Ins != null)
		{
			Battle.Ins.UnRegisVehicleForCulling(this);
		}
		CullingIndex = -1;
		Culled = true;
	}

	public void UpdateLod()
	{
		if (Lod != null)
		{
			Lod.DoCheckLod();
		}
		Vector3 position = Battle.Ins.MainCamera.SelfTransform.position;
		float num = Battle.Ins.MainCamera.Fov / 48f;
		float num2 = Vector3.SqrMagnitude(position - VehicleTransform.position) * num * num;
		if (num2 < 40000f)
		{
			if (_lodLevel != 0)
			{
				_lodLevel = 0;
				iVehicle.SetAllEffectVisible(true);
			}
		}
		else if (_lodLevel != 1)
		{
			_lodLevel = 1;
			iVehicle.SetAllEffectVisible(false);
		}
	}

	public SVehicleInfo GetSVehicleInfo()
	{
		SVehicleInfo sVehicleInfo = new SVehicleInfo();
		sVehicleInfo.vehicleInfo.hp = GetHp();
		sVehicleInfo.vehicleInfo.driverRoleId = roleIds[0];
		sVehicleInfo.vehicleInfo.fuel = GetFuel();
		sVehicleInfo.vehicleInfo.id = this.sVehicleInfo.vehicleInfo.id;
		sVehicleInfo.vehicleInfo.orientation.x = base.transform.eulerAngles.x;
		sVehicleInfo.vehicleInfo.orientation.y = base.transform.eulerAngles.y;
		sVehicleInfo.vehicleInfo.orientation.z = base.transform.eulerAngles.z;
		sVehicleInfo.vehicleInfo.pos.x = base.transform.position.x;
		sVehicleInfo.vehicleInfo.pos.y = base.transform.position.y;
		sVehicleInfo.vehicleInfo.pos.z = base.transform.position.z;
		sVehicleInfo.vehicleInfo.type = this.sVehicleInfo.vehicleInfo.type;
		for (int i = 0; i < roleIds.Length; i++)
		{
			long num = roleIds[i];
			if (num > 0)
			{
				sVehicleInfo.vehicleInfo.seatInfo[i] = num;
			}
		}
		return sVehicleInfo;
	}
}

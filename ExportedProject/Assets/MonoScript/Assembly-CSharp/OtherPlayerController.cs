using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class OtherPlayerController : BasePlayerController
{
	private Vector3 m_Velocity = Vector3.zero;

	private Vector3 m_TargetPos = Vector3.zero;

	private Quaternion m_TargetRotation = Quaternion.identity;

	private bool m_Rotating;

	private bool m_Moving;

	private GameObject m_LowModel;

	[NonSerialized]
	public PlayerInfo Info;

	[NonSerialized]
	public bool IsWatched;

	[SerializeField]
	private AttackedIK m_AttackedIk;

	public int CullingIndex = -1;

	public bool Culled = true;

	public bool IsPa;

	public Vector3 Cameraforward;

	private int m_Lod = -1;

	private static readonly CSyncPlayerPos m_SyncPos = new CSyncPlayerPos();

	private static readonly CSyncOrientation m_SyncEulerAngles = new CSyncOrientation();

	private Vector3 m_OldEulerAngles;

	private Vector3 m_OldPos;

	private static readonly CSyncAnimator m_SyncAnimator = new CSyncAnimator();

	private static readonly CSyncLayerWeight m_SyncLayerWeight = new CSyncLayerWeight();

	public Vector3 Velocity
	{
		get
		{
			return m_Velocity;
		}
	}

	public void Init(PlayerInfo playerInfo)
	{
		base.Init();
		Info = playerInfo;
		NewPlayer = Info.newPlayer;
		m_PlayerInfo = playerInfo;
		RoleId = Info.roleId;
		InsId = Info.insId;
		HP = Info.hp;
		MaxHp = Info.hpMax;
		EP = Info.ep;
		Name = Info.name;
		IsDownWaitSave = Info.isSecondHp;
		IsDie = Info.hp <= 0;
		PlayerAnimator.applyRootMotion = false;
		PlayerAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		IsPa = false;
		VehicleId = playerInfo.vehicleId;
		IsMale = playerInfo.sex;
		m_SanOpened = playerInfo.curStatus == 4;
		m_SanDestroyed = playerInfo.curStatus == 5;
		m_Yaw = playerInfo.yaw;
		Tanshen = playerInfo.isTanshen;
		if (playerInfo.isInBuildState)
		{
			ShowTuzhi();
		}
		else
		{
			HideTuzhi();
		}
		m_Landed = playerInfo.curStatus == 1;
		m_Moving = false;
		m_Velocity = Vector3.zero;
		if (base.InCar)
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
		else
		{
			SyncCurrentPos(playerInfo.pos);
			SetTargetRotation(playerInfo.orientation);
		}
		SetLayerWeight(FullBodyLayer, 0f);
		SetPosition(new Vector3(playerInfo.pos.x, playerInfo.pos.y, playerInfo.pos.z));
		SetInput(Info.input);
		PlayerAnimator.SetFloat(BasePlayerController.s_HorizontalInputHash, m_InputVector.x, 0f, 0f);
		PlayerAnimator.SetFloat(BasePlayerController.s_ForwardInputHash, m_InputVector.y, 0f, 0f);
		foreach (KeyValuePair<byte, int> item in playerInfo.animatiorInfo)
		{
			PlayerAnimation(item.Key, item.Value);
		}
		foreach (KeyValuePair<int, short> item2 in playerInfo.animatiorWeightInfo)
		{
			SetLayerWeight(item2.Key, MathUtils.Short2Float(item2.Value));
		}
		if (playerInfo.bagInfo.gun.gunId > 0)
		{
			AddGun(playerInfo.bagInfo.gun.gunId, playerInfo.bagInfo.gun);
			ChangeHandWeapon(playerInfo.bagInfo.gun.gunId);
			BaseGun currentGun = GetCurrentGun();
			if (currentGun != null)
			{
				currentGun.CurBulletNum3Rd = playerInfo.bagInfo.gun.bulletNumber;
				Battle.Ins.LoadOneRpgBulletFor3rd(currentGun);
			}
		}
		if (playerInfo.bagInfo.handWeapon > 0)
		{
			AddNearGun(playerInfo.bagInfo.handWeapon, -1);
			ChangeHandWeapon(playerInfo.bagInfo.handWeapon);
		}
		PutOnAllClothes(playerInfo.bagInfo.wears);
		CloseAllIK();
		if (m_LowModel == null)
		{
			PutOnLowModel();
		}
		m_Lod = -1;
		Battle.Ins.RegisOtherPlayerForCulling(this);
	}

	public override void Reset()
	{
		if (NameLabel != null)
		{
			UnityEngine.Object.Destroy(NameLabel.gameObject);
			NameLabel = null;
		}
		IsWatched = false;
		Culled = true;
		IsPa = false;
		HideTuzhi();
		base.Reset();
		m_Lod = -1;
		RemoveRoleIdinDic();
		Battle.Ins.UnRegisOtherPlayerForCulling(this);
	}

	public override void InitPrototyle()
	{
		base.InitPrototyle();
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		m_AttackedIk = base.gameObject.GetComponent<AttackedIK>();
		PlayerCollider.material = Battle.Ins.MinPhysicMaterial;
		PlayerCollider.isTrigger = false;
		List<Transform> list = new List<Transform>();
		list.Add(GetBodyPart(BodyPart.Head));
		list.Add(Utils.GetChildObjByName("Bip001 Neck", base.gameObject).transform);
		list.Add(Utils.GetChildObjByName("Bip001 Spine2", base.gameObject).transform);
		list.Add(Utils.GetChildObjByName("Bip001 Spine1", base.gameObject).transform);
		m_AttackedIk = base.gameObject.AddComponent<AttackedIK>();
		m_AttackedIk.Init(list);
	}

	protected override void Awake()
	{
		base.Awake();
		CullingIndex = -1;
		Culled = true;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		if (base.InCar && (bool)Battle.Ins)
		{
			Battle.Ins.SetOtherPlayerCullingPos(CullingIndex, PlayerTransform.position);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMove();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		m_AttackedIk.DoUpdate();
		if (IsWatched && (bool)Battle.Ins && (bool)Battle.Ins.SelfPlayer && Battle.Ins.SelfPlayer.IsDie)
		{
			Battle.Ins.SelfPlayer.SetPosition(PlayerTransform.position);
		}
	}

	public void SetInput(BaseInput input)
	{
		m_InputVector.Set(input.horizontalInput, input.verticalInput);
	}

	public void SyncCurrentPos(Vec3 pos)
	{
		if (!IsDie)
		{
			m_TargetPos.Set(pos.x, pos.y, pos.z);
			m_Moving = true;
			Battle.Ins.SetOtherPlayerCullingPos(CullingIndex, new Vector3(pos.x, pos.y, pos.z));
		}
	}

	public void SetCamearforwardPoint(Vec3 point)
	{
		Cameraforward.x = point.x;
		Cameraforward.y = point.y;
		Cameraforward.z = point.z;
	}

	private void UpdateMove()
	{
		if (m_Moving)
		{
			Vector3 position = PlayerTransform.position;
			Vector3 vector = Vector3.SmoothDamp(position, m_TargetPos, ref m_Velocity, 0.2f, float.MaxValue, Time.fixedDeltaTime);
			PlayerTransform.position = vector;
			m_Moving = m_TargetPos != vector;
			if (!m_Moving)
			{
				m_Velocity = Vector3.zero;
			}
		}
		if (m_Rotating)
		{
			Quaternion quaternion = Quaternion.Lerp(PlayerTransform.rotation, m_TargetRotation, 5f * Time.deltaTime);
			PlayerTransform.rotation = quaternion;
			m_Rotating = quaternion != m_TargetRotation;
		}
		Vector3 vector2 = PlayerTransform.InverseTransformVector(m_Velocity);
		SetAnimatorHorizontalValue(vector2.x);
		SetAnimatorForwardValue(vector2.z);
		SetAnimatorYValue(vector2.y);
	}

	public void SetPosition(Vector3 pos)
	{
		PlayerTransform.position = pos;
		Battle.Ins.SetOtherPlayerCullingPos(CullingIndex, pos);
	}

	public void SetAnimatorYawValue(float value)
	{
		m_Yaw = value;
		PlayerAnimator.SetFloat(BasePlayerController.s_YawHash, value, 0.01f, Time.deltaTime);
	}

	public void SetTargetRotation(ShortVec3 orientation)
	{
		m_TargetRotation = Quaternion.Euler(MathUtils.Short2Float(orientation.x), MathUtils.Short2Float(orientation.y), MathUtils.Short2Float(orientation.z));
		m_Rotating = true;
	}

	public void PlayAttackIk(Vector3 force, float multiple = 0.5f)
	{
		m_AttackedIk.Play(force.normalized * multiple);
	}

	public void AddGun(int id, BagGun gunInfo)
	{
		AddWeapon(id, new OtherGun(this, gunInfo.gunId, gunInfo.skinId, gunInfo.accessory, gunInfo.bulletNumber));
	}

	public void AddNearGun(int id, int skinid)
	{
		AddWeapon(id, new OtherNearWeapon(this, id, skinid));
	}

	public override void OnGetInCar(int vehicleId)
	{
		base.OnGetInCar(vehicleId);
		m_Moving = false;
		m_Velocity = Vector3.zero;
		m_Rotating = false;
	}

	protected override void OnDied()
	{
		HideNameLabel();
	}

	public void OnDestory()
	{
		RemoveRoleIdinDic();
	}

	public void RemoveRoleIdinDic()
	{
		if (Battle.Ins.NearOtherPlayersDic.ContainsKey(RoleId))
		{
			Battle.Ins.NearOtherPlayersDic.Remove(RoleId);
		}
		if (Battle.Ins.TeamPlayerDic.ContainsKey(RoleId))
		{
			Battle.Ins.TeamPlayerDic.Remove(RoleId);
		}
		if (Battle.Ins.OtherPlayersDic.ContainsKey(RoleId))
		{
			Battle.Ins.OtherPlayersDic.Remove(RoleId);
		}
		Battle.Ins.MapObjectDic.Remove(InsId);
	}

	private void OnDestroy()
	{
		CancelInvoke();
	}

	private void OnBecameVisible()
	{
		PlayerAnimator.enabled = true;
		foreach (KeyValuePair<int, int> item in Layer2aimHashDic)
		{
			PlayerAnimator.Play(item.Value, item.Key);
		}
	}

	private void OnBecameInvisible()
	{
		if (!(Battle.Ins == null) && !(Battle.Ins.SelfPlayer == null))
		{
			PlayerAnimator.enabled = false;
		}
	}

	private void ChangeLod(int level)
	{
		switch (level)
		{
		case 0:
			Skin.ShowAll();
			Skin.HideSkin(100);
			ShowAllWeapon();
			ShowDressing();
			ShowAllGunParts();
			PlayerAnimator.enabled = true;
			break;
		case 1:
			Skin.ShowAll();
			Skin.HideSkin(100);
			ShowAllWeapon();
			ShowDressing();
			HideAllGunParts();
			PlayerAnimator.enabled = true;
			break;
		case 2:
			Skin.ShowAll();
			Skin.HideSkin(100);
			ShowDressing();
			HideAllWeapon();
			PlayerAnimator.enabled = true;
			break;
		case 3:
			Skin.ShowAll();
			Skin.HideSkin(100);
			HideDressing();
			HideAllWeapon();
			PlayerAnimator.enabled = true;
			break;
		default:
			Skin.HideAll();
			if (m_LowModel != null)
			{
				Skin.ShowSkin(100);
			}
			HideAllWeapon();
			PlayerAnimator.enabled = true;
			break;
		}
	}

	private void ShowDressing()
	{
		Skin.ShowSkin(29);
		Skin.ShowSkin(26);
		Skin.ShowSkin(42);
		Skin.ShowSkin(23);
		Skin.ShowSkin(31);
		Skin.ShowSkin(117);
		Skin.ShowSkin(118);
		Skin.ShowSkin(92);
		Skin.ShowSkin(96);
		Skin.ShowSkin(95);
		Skin.ShowSkin(94);
	}

	private void HideDressing()
	{
		Skin.HideSkin(29);
		Skin.HideSkin(26);
		Skin.HideSkin(42);
		Skin.HideSkin(23);
		Skin.HideSkin(31);
		Skin.HideSkin(117);
		Skin.HideSkin(118);
		Skin.HideSkin(92);
		Skin.HideSkin(96);
		Skin.HideSkin(95);
		Skin.HideSkin(94);
	}

	public void UpdateLod()
	{
		if (PlayerTransform == null)
		{
			return;
		}
		Vector3 position = Battle.Ins.MainCamera.SelfTransform.position;
		float num = Battle.Ins.MainCamera.Fov / 51f;
		float num2 = Vector3.SqrMagnitude(position - PlayerTransform.position) * num * num;
		if (num2 < 225f)
		{
			if (m_Lod != 0)
			{
				m_Lod = 0;
				ChangeLod(m_Lod);
			}
		}
		else if (num2 < 1225f)
		{
			if (m_Lod != 1)
			{
				m_Lod = 1;
				ChangeLod(m_Lod);
			}
		}
		else if (num2 < 2500f)
		{
			if (m_Lod != 2)
			{
				m_Lod = 2;
				ChangeLod(m_Lod);
			}
		}
		else if (num2 < 16900f)
		{
			if (m_Lod != 3)
			{
				m_Lod = 3;
				ChangeLod(m_Lod);
			}
		}
		else if (m_Lod != 4)
		{
			m_Lod = 4;
			ChangeLod(m_Lod);
		}
	}

	public void PutOnLowModel()
	{
		ResMgr.Ins.CreateFromAB("role/roleLodskin01.ab", null, _003CPutOnLowModel_003Em__0);
	}

	protected override void OnChangeAnimatorState(int layer, string targetStateName, int stateHash)
	{
	}

	public override void SetAnimatorWeight(int layer, float weight)
	{
	}

	public void ShowNameLabel()
	{
		if (!(NameLabel == null))
		{
		}
	}

	public void HideNameLabel()
	{
		if (!(NameLabel == null))
		{
		}
	}

	[CompilerGenerated]
	private void _003CPutOnLowModel_003Em__0(GameObject o)
	{
		if (IsDie)
		{
			UnityEngine.Object.Destroy(o);
			return;
		}
		o.transform.localPosition = Vector3.zero;
		Skin.SetSkin(o, 100);
		if (m_Lod >= 3)
		{
			Skin.ShowSkin(100);
		}
		else
		{
			Skin.HideSkin(100);
		}
		m_LowModel = o;
		o.transform.SetParent(base.transform);
	}
}

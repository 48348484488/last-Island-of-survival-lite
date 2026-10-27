using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.LargeScene;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.CrossPlatformInput;
using cfg;
using gs.bag.scmsg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class PlayerController : BasePlayerController
{
	private sealed class MonsterControllerDis : IComparer<MonsterController>
	{
		public int Compare(MonsterController x, MonsterController y)
		{
			return x.DisToMySelf.CompareTo(y.DisToMySelf);
		}
	}

	[HideInInspector]
	public vThirdPersonCamera MainCamera;

	public bool FreeType;

	public bool HalfFreeType;

	public bool CanShoot = true;

	public bool Jumping;

	public bool InBuilding;

	public bool m_CanRush;

	public float Speed;

	public float WaitForRush;

	public float WaitForPaMove;

	public bool Swiming;

	public bool Climbing;

	public bool Skydiving;

	public bool InStartPlane;

	public float RotateSpeed = 10000f;

	public GameObject CurThrowWeapon;

	public Vector2 Input = Vector2.zero;

	public Vector2 LastJumpUpInput = Vector2.zero;

	private bool m_Grounded;

	public int LastWapInsId = -1;

	public bool AutoRun;

	public Transform JimiaoRightHandGuaDian;

	public CharacterSetSkin JimiaoSetSkin;

	public HoldIK JimiaoHoldIK;

	public Transform JimiaoCameraPoint;

	private bool m_CanOpenJimiao = true;

	private float m_EpAnimationSpeedChange = 1f;

	private float m_GunAnimationSpeedChange = 1f;

	private static readonly CSyncCameraLookPoint m_SyncLookPoint = new CSyncCameraLookPoint();

	private Vector3 m_LastCamearForward;

	private Rigidbody m_Rigidbody;

	public GameObject Wuqi_guadian002;

	public bool IsRedTeam = true;

	public bool IsRealDieBloodAir;

	public List<Collider> MyColliders;

	public float ShootingTime;

	public float AgainLoadBulletTime;

	public int MyMonsterNum;

	public HashSet<long> MyMonsterInsId = new HashSet<long>();

	public Light DayNightLight;

	public float YaoGanAngle;

	public long KillMeRoleId = -1L;

	private Coroutine m_SyncTick;

	private Coroutine m_HalfSecondTick;

	private Coroutine m_SyncSecondTick;

	private Coroutine m_Sync60Tick;

	private Coroutine m_Sync5Tick;

	private Vector3 m_LastPos = Vector3.zero;

	private float m_PreTime;

	private int m_stepAudioId = -1;

	private int _lasetFootSoundCfgId;

	public FSMSystem FSM;

	public FSMSystem FSMUpBody;

	private Vector3 m_TargetDirectionForward = Vector3.zero;

	private Vector3 m_TargetDirectionRight = Vector3.zero;

	private Vector2 m_oldInput;

	private float m_TargetAngleY;

	public float LastUpYapGanTargetAngleY;

	public float m_SpeedMultiple;

	public bool m_lastFrameFreeType;

	private Vector3 m_GroundNormal;

	private Vector3 targetDirection;

	public float GroundMinDistance = 0.2f;

	public float GroundMaxDistance = 0.2f;

	private float m_YSpeed;

	public float GroundDistance = 0.1f;

	public string GroundName;

	public string LastGroundName;

	public float ClimbAngle = 90f;

	private readonly RaycastHit[] m_GroundColliders = new RaycastHit[10];

	private Vector3 m_OldVelocity;

	private Vector3 m_OldEulerAngles;

	private Vector3 m_OldPos;

	private Vector2 m_OldInputVector;

	private float m_OldYaw;

	private static readonly CSyncVelocity m_SyncVelocity = new CSyncVelocity();

	private static readonly CSyncPlayerPos m_SyncPos = new CSyncPlayerPos();

	private static readonly CSyncOrientation m_SyncEulerAngles = new CSyncOrientation();

	private static readonly CSyncAnimator m_SyncAnimator = new CSyncAnimator();

	private static readonly CSyncLayerWeight m_SyncLayerWeight = new CSyncLayerWeight();

	private static readonly CSyncYaw m_SyncYaw = new CSyncYaw();

	private static readonly WaitForSeconds Wait02 = new WaitForSeconds(0.2f);

	private static readonly WaitForSeconds Wait05 = new WaitForSeconds(0.5f);

	private static readonly WaitForSeconds Wait2 = new WaitForSeconds(2f);

	private static readonly WaitForSeconds Wait60 = new WaitForSeconds(60f);

	private Vector3 m_LastSecondPos = new Vector3(2400f, 49f, -2600f);

	public VehicleMonitor NearCar;

	private readonly Collider[] m_NearCarsColliders = new Collider[15];

	public BasePlayerController CanSavedPeople;

	private readonly Collider[] m_NearPeopleColliders = new Collider[6];

	private float m_SavePeopleDis = 1.5f;

	private bool m_CanPlayrHitSound = true;

	private static readonly CSyncAnimatorSpeed cSyncAnimatorSpeed = new CSyncAnimatorSpeed();

	public float SpeedUpPercent = 1f;

	private CSyncFootTexture cSyncFootTexture = new CSyncFootTexture();

	private Collider[] m_NearGosColliders = new Collider[100];

	public Vector3 RobotBirthPos = Vector3.zero;

	private Ray m_ray;

	private OtherPlayerController m_CurAimedPlayer;

	public float ExtraAddHp;

	public Gun CurGun
	{
		get
		{
			return GunInHand as Gun;
		}
		set
		{
			GunInHand = value;
		}
	}

	public bool Grounded
	{
		get
		{
			return m_Grounded;
		}
		set
		{
			m_Grounded = value;
		}
	}

	public Vector3 GroundedNormal
	{
		get
		{
			return m_GroundNormal;
		}
		set
		{
			m_GroundNormal = value;
		}
	}

	public bool IsDriver
	{
		get
		{
			return NearCar != null && NearCar.IsDriver();
		}
	}

	public bool InBuildState
	{
		get
		{
			return FSMUpBody.CurrentState.ID == StateID.Build;
		}
	}

	public bool InPlantState
	{
		get
		{
			return FSMUpBody.CurrentState.ID == StateID.Plant;
		}
	}

	public bool IsJiMiao
	{
		get
		{
			return CurGun != null && CurGun.IsJiMiao;
		}
	}

	public bool IsJiMiaoHavejing
	{
		get
		{
			return CurGun != null && CurGun.IsJiMiao && CurGun.HaveJing;
		}
	}

	public bool CanOpenJimiao
	{
		get
		{
			return m_CanOpenJimiao;
		}
		set
		{
			m_CanOpenJimiao = value;
			if (!m_CanOpenJimiao && CurGun != null && CurGun.IsJiMiao)
			{
				CurGun.CloseJiMiao();
			}
		}
	}

	public float TargetAngleY { get; set; }

	public bool HaveGun
	{
		get
		{
			return false;
		}
	}

	public Rigidbody PlayerRigidbody
	{
		get
		{
			return m_Rigidbody;
		}
	}

	public void Init(PlayerInfo info)
	{
		base.Init();
		NewPlayer = info.newPlayer;
		ResMgr.Ins.CreateFromAB("role/rolehand.ab", null, _003CInit_003Em__0);
		base.gameObject.name = "Self!!!!!!!!!!!!!!!!!!!!!!!!!!!";
		m_PlayerInfo = info;
		InsId = info.insId;
		MainCamera = Battle.Ins.MainCamera;
		PlayerCollider.tag = "SelfCollider";
		Joystick.OnUpJoystick = (Action)Delegate.Combine(Joystick.OnUpJoystick, new Action(OnUpJoyStick));
		UnityEngine.Object.Destroy(Utils.GetChildObjByName("BigAimHelper", base.gameObject));
		if (info.groupId == 1)
		{
			IsRedTeam = true;
		}
		else
		{
			IsRedTeam = false;
		}
		MyColliders = GetComponentsInChildren<Collider>(true).vToList();
		VehicleId = info.vehicleId;
		SetHP(info.hp);
		SetPulmonary(100f);
		Vector3 position = new Vector3(info.pos.x, info.pos.y, info.pos.z);
		ResetSetPosition(position);
		m_LastSecondPos = new Vector3(info.pos.x, info.pos.y, info.pos.z);
		PlayerAnimator.applyRootMotion = true;
		PlayerAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		DisablePhysic();
		base.gameObject.SetLayerRecursively(BattleScMgr.SelfPlayerLayer);
		GameObject childObjByName = Utils.GetChildObjByName("Bip001 L Hand", base.gameObject);
		HandAttack handAttack = childObjByName.AddComponent<HandAttack>();
		handAttack.First = false;
		handAttack.InsId = InsId;
		childObjByName.gameObject.layer = BattleScMgr.GunLayer;
		GameObject childObjByName2 = Utils.GetChildObjByName("Bip001 R Hand", base.gameObject);
		Wuqi_guadian002 = Utils.GetChildObjByName("wuqi_guadian002", base.gameObject);
		HandAttack handAttack2 = childObjByName2.AddComponent<HandAttack>();
		handAttack2.First = false;
		handAttack2.InsId = InsId;
		childObjByName2.gameObject.layer = BattleScMgr.GunLayer;
		PlayerCollider.gameObject.layer = BattleScMgr.SelfPlayerColliderLayer;
		RoleId = Singleton<RoleMgr>.Ins.info.roleId;
		m_SanOpened = info.curStatus == 4;
		m_SanDestroyed = info.curStatus == 5;
		Tanshen = info.isTanshen;
		IsMale = info.sex;
		PutOnAllClothes(info.bagInfo.wears);
		InitFSM();
		InitWeapons();
		JimiaoCameraPoint = new GameObject().transform;
		SetJimiaoCameraPointKaijing();
		JimiaoCameraPoint.localScale = Vector3.one;
		PlayerTransform.eulerAngles = new Vector3(MathUtils.Short2Float(info.orientation.x), MathUtils.Short2Float(info.orientation.y), MathUtils.Short2Float(info.orientation.z));
		Battle.Ins.MainCamera.SetCameraTargetDirection(PlayerTransform.forward);
		m_SavePeopleDis += GetSkillValueIndex0(131) * 100f;
		InitSkill();
		foreach (KeyValuePair<int, short> item in Battle.Ins.SelfInfo.animatiorWeightInfo)
		{
			SetLayerWeight(item.Key, item.Value);
		}
		BattleEvent.OnSwitchState = (BattleEvent.OnswitchState)Delegate.Combine(BattleEvent.OnSwitchState, new BattleEvent.OnswitchState(OnSwitchState));
	}

	private void OnSwitchState(StateID from, StateID to, long insId)
	{
		if (insId == InsId && BattleEvent.OnSelfSwitchState != null)
		{
			BattleEvent.OnSelfSwitchState(from, to);
		}
	}

	public void ChangeHandAndBodyBones(bool tojimiaoHandBone)
	{
		Skin.ShowSkin(114);
		Skin.ShowSkin(118);
		Skin.ShowSkin(90);
		SwitchTypeClothToJimiao(90, tojimiaoHandBone);
		SwitchTypeClothToJimiao(114, tojimiaoHandBone);
		GameObject skinGameObject = Skin.GetSkinGameObject(118);
		SkinnedMeshRenderer componentInChildren = skinGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (tojimiaoHandBone)
		{
			componentInChildren.shadowCastingMode = ShadowCastingMode.Off;
			JimiaoSetSkin.SetBones(componentInChildren);
		}
		else
		{
			Skin.SetBones(componentInChildren);
			componentInChildren.shadowCastingMode = ShadowCastingMode.On;
		}
	}

	private void SwitchTypeClothToJimiao(int type, bool tojimiaoHandBone)
	{
		GameObject skinGameObject = Skin.GetSkinGameObject(type);
		if (!(skinGameObject == null))
		{
			Transform transform = skinGameObject.transform.Find(skinGameObject.name.Replace("(Clone)", "_jimiao"));
			Transform transform2 = skinGameObject.transform.Find(skinGameObject.name.Replace("(Clone)", string.Empty));
			if (transform == null)
			{
				Debug.LogError("upbody.name is wrong" + skinGameObject.name);
			}
			else if (tojimiaoHandBone)
			{
				transform.gameObject.SetActiveBetter(true);
				transform2.gameObject.SetActiveBetter(false);
				SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
				component.updateWhenOffscreen = true;
				component.shadowCastingMode = ShadowCastingMode.Off;
				JimiaoSetSkin.SetBones(component);
			}
			else
			{
				transform.gameObject.SetActiveBetter(false);
				transform2.gameObject.SetActiveBetter(true);
			}
		}
	}

	public void SetJimiaoCameraPointJimiao(Transform p)
	{
		JimiaoCameraPoint.parent = p;
		JimiaoCameraPoint.localRotation = Quaternion.identity;
	}

	public void SetJimiaoCameraPointKaijing()
	{
		JimiaoCameraPoint.parent = GetBodyPart(BodyPart.Head);
		JimiaoCameraPoint.localPosition = new Vector3(0f, -0.3f, -0.2f);
		JimiaoCameraPoint.localRotation = Quaternion.identity;
	}

	private void OnUpJoyStick()
	{
		if (FSM.CurrentState.ID == StateID.Jump || FSM.CurrentState.ID == StateID.Land)
		{
			LastJumpUpInput = Input;
		}
	}

	private void OnUseGun(int insId)
	{
	}

	private void InitSkill()
	{
		YanlongSkill();
	}

	public void ReBirth(PlayerInfo selfInfo)
	{
		Debug.LogError("ReBirth");
		DisablePhysic();
		base.SRebirth(selfInfo);
		Utils.TriggerEvent(BattleEvent.OnExitedBuildState);
		Reset();
		ResetBones();
		Battle.Ins.AimHelp.Disabled = false;
		Battle.Ins.MainCamera.SetMainTarget(base.transform, "Stand");
		Battle.Ins.SelfInfo = selfInfo;
		FreeType = false;
		IsDownWaitSave = false;
		SetHP(selfInfo.hp);
		SetEP(selfInfo.ep);
		SetPulmonary(100f);
		Battle.Ins.SelfInfo.curStatus = 5;
		AutoRun = false;
		VehicleId = selfInfo.vehicleId;
		Tanshen = selfInfo.isTanshen;
		ResetRigidbodyValue();
		PutOnAllClothes(selfInfo.bagInfo.wears);
		m_CurrentWeaponInsId = -1;
		UnityEngine.Object.Destroy(DollBones);
		PlayerAnimator.enabled = true;
		PlayerAnimator.applyRootMotion = true;
		Vector3 position = new Vector3(selfInfo.pos.x, selfInfo.pos.y, selfInfo.pos.z);
		ResetSetPosition(position);
		LargeSceneManager.Ins.ToSceneCell(position.x, position.y, position.z);
		PlayerTransform.eulerAngles = new Vector3(MathUtils.Short2Float(selfInfo.orientation.x), MathUtils.Short2Float(selfInfo.orientation.y), MathUtils.Short2Float(selfInfo.orientation.z));
		StartFsm();
		InitWeapons();
		Battle.Ins.MyBattlePanel.InitEquipInfo();
		Battle.Ins.MyBattlePanel.HideWatch();
		foreach (KeyValuePair<long, OtherPlayerController> item in Battle.Ins.OtherPlayersDic)
		{
			item.Value.IsWatched = false;
		}
		Battle.Ins.MainCamera.SetCameraTargetDirection(PlayerTransform.forward);
		Utils.TriggerEvent(RoleEvent.MoneyChangeDelegate);
	}

	public void RemoveInitItem()
	{
		if (CurGun != null)
		{
			CurGun.CloseJiMiao();
		}
		CurGun = null;
		GunInHand = null;
		m_CurrentWeaponInsId = -1;
		SetHaveBag(false);
		DestoryAllWeapon();
		Utils.TriggerEvent(BattlePackEvent.RefreshBagCapacity);
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.UpdateGunImages();
			Battle.Ins.MyBattlePanel.HideBulletClothIcon();
		}
	}

	public override void InitPrototyle()
	{
		base.InitPrototyle();
		BagEvent.DropGun = (Utils.IntDelegate)Delegate.Combine(BagEvent.DropGun, new Utils.IntDelegate(OnDropGunDelegate));
		BattlePackEvent.DropNearGunDelegate = (Utils.VoidDelegate)Delegate.Combine(BattlePackEvent.DropNearGunDelegate, new Utils.VoidDelegate(OnDropNearGunDelegate));
		BagEvent.AddOrRemoveBullet = (Utils.Int2Delegate)Delegate.Combine(BagEvent.AddOrRemoveBullet, new Utils.Int2Delegate(OnLoadBulletFinish));
		BagEvent.ShootBullet = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ShootBullet, new Utils.Int2Delegate(OnShootBullet));
		BattleEvent.OnSelfKilled = (Utils.LongDelegate)Delegate.Combine(BattleEvent.OnSelfKilled, new Utils.LongDelegate(OnSelfKilled));
		BagEvent.AddGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.AddGunPart, new Utils.Int2Delegate(OnAddGunPart));
		BagEvent.RemoveGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.RemoveGunPart, new Utils.Int2Delegate(OnRemovePart));
		m_Rigidbody = GetComponent<Rigidbody>();
	}

	private void OnShootBullet(int insId, int num)
	{
		Gun gun = GetWeapon(insId) as Gun;
		if (gun != null)
		{
			Utils.TriggerEvent(BattleEvent.OnCurLoadBulletNumChange, gun.CurLoadBulletNum);
		}
	}

	private void OnSelfKilled(long roleId)
	{
		KillMeRoleId = roleId;
	}

	public override void SRebirth(PlayerInfo info)
	{
		ReBirth(info);
	}

	public void ReciveNewPlayerInfo()
	{
		SingletonMono<AudioManager>.Ins.StopMusicBg();
		if (Battle.Ins.SelfInfo.curStatus == 1)
		{
			SingletonMono<AudioManager>.Ins.PlayMusicBg("Bgm_dengdai");
		}
		m_SanOpened = Battle.Ins.SelfInfo.curStatus == 4;
		IsDownWaitSave = Battle.Ins.SelfInfo.isSecondHp;
		StartFsm();
		foreach (KeyValuePair<byte, int> item in Battle.Ins.SelfInfo.animatiorInfo)
		{
			if (item.Value == 540309552)
			{
				FSM.SwitchState(StateID.Stand);
			}
			else if (item.Value == -2065082259)
			{
				FSM.SwitchState(StateID.Crouch);
			}
			else if (item.Value == 1245347906)
			{
				FSM.SwitchState(StateID.Pa);
			}
		}
		foreach (KeyValuePair<int, short> item2 in Battle.Ins.SelfInfo.animatiorWeightInfo)
		{
			SetLayerWeight(item2.Key, item2.Value);
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		try
		{
			m_SyncTick = Utils.StartConroutine(SyncTick());
			m_HalfSecondTick = Utils.StartConroutine(HalfSecondTick());
			m_SyncSecondTick = Utils.StartConroutine(SyncSecondTick());
			m_Sync60Tick = Utils.StartConroutine(Sync60Tick());
			m_Sync5Tick = Utils.StartConroutine(Sync5Tick());
		}
		catch (Exception)
		{
		}
	}

	private void OnDestroy()
	{
		Clear();
	}

	public void Clear()
	{
		FSM.Clear();
		FSMUpBody.Clear();
		BattlePackEvent.ChangeGunDelegate = (Utils.VoidDelegate)Delegate.Remove(BattlePackEvent.ChangeGunDelegate, new Utils.VoidDelegate(OnChangeGunDelegate));
		BattlePackEvent.DropGunDelegate = (Utils.IntDelegate)Delegate.Remove(BattlePackEvent.DropGunDelegate, new Utils.IntDelegate(OnDropGunDelegate));
		BattlePackEvent.DropNearGunDelegate = (Utils.VoidDelegate)Delegate.Remove(BattlePackEvent.DropNearGunDelegate, new Utils.VoidDelegate(OnDropNearGunDelegate));
		BattlePackEvent.PickGunDelegate = (BattlePackEvent.GunInfoDelegate)Delegate.Remove(BattlePackEvent.PickGunDelegate, new BattlePackEvent.GunInfoDelegate(OnPickGunDelegate));
		BattlePackEvent.PickNearGunDelegate = (Utils.IntDelegate)Delegate.Remove(BattlePackEvent.PickNearGunDelegate, new Utils.IntDelegate(OnPickNearGunDelegate));
		BattlePackEvent.LoadBulletFinishDelegate = (Utils.Int2Delegate)Delegate.Remove(BattlePackEvent.LoadBulletFinishDelegate, new Utils.Int2Delegate(OnLoadBulletFinish));
		BattlePackEvent.PickItemDelegate = (Utils.VoidDelegate)Delegate.Remove(BattlePackEvent.PickItemDelegate, new Utils.VoidDelegate(OnPickItem));
		BattleEvent.OnSelfKilled = (Utils.LongDelegate)Delegate.Remove(BattleEvent.OnSelfKilled, new Utils.LongDelegate(OnSelfKilled));
		BagEvent.AddGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.AddGunPart, new Utils.Int2Delegate(OnAddGunPart));
		BagEvent.RemoveGunPart = (Utils.Int2Delegate)Delegate.Combine(BagEvent.RemoveGunPart, new Utils.Int2Delegate(OnRemovePart));
		Utils.StopConroutine(m_SyncTick);
		Utils.StopConroutine(m_HalfSecondTick);
		Utils.StopConroutine(m_SyncSecondTick);
		Utils.StopConroutine(m_Sync60Tick);
		Utils.StopConroutine(m_Sync5Tick);
	}

	protected override void Update()
	{
		base.Update();
		ShootingTime -= Time.deltaTime;
		AgainLoadBulletTime -= Time.deltaTime;
		m_PreTime = Time.deltaTime;
		if (!IsDie && Battle.Ins.LoadFinish)
		{
			WaitForRush -= Time.deltaTime;
			if (WaitForRush <= 0f && !IsJiMiao)
			{
				WaitForRush = 0f;
				m_CanRush = true;
			}
			else
			{
				m_CanRush = false;
			}
			WaitForPaMove -= Time.deltaTime;
			if (WaitForPaMove <= 0f)
			{
				WaitForPaMove = 0f;
			}
			Input.x = CrossPlatformInputManager.GetAxis(KeyName.Horizontal);
			Input.y = CrossPlatformInputManager.GetAxis(KeyName.Vertical);
			CheckNeedIk();
			CheckCanOpenJimiao();
			SyncCameraForward();
			FSM.Update();
			FSMUpBody.Update();
			Speed = m_Rigidbody.velocity.magnitude;
			if (CurGun != null)
			{
				CurGun.Update();
			}
			AddDownForce();
			CheckGround();
			UpdateMyInputVectorForAimator();
			UpdateHalfFreeType();
			UpdateRigidBodyLock();
			UpdatePlayerColliderMaterial();
			CheckCanMoveUp();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (m_PreTime > 0.0333333f)
		{
			m_PreTime = 0.0333333f;
		}
		else if (m_PreTime < 0.0166666f)
		{
			m_PreTime = 0.0166666f;
		}
		Time.fixedDeltaTime = m_PreTime;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!IsDie && Battle.Ins.LoadFinish)
		{
			FSM.LateUpdate();
			FSMUpBody.LateUpdate();
			AddSurprise();
		}
	}

	private void CheckPlayStepAudio()
	{
		if (FSM.CurrentState.ID == StateID.Stand || FSM.CurrentState.ID == StateID.Crouch)
		{
			if ((double)base.InputVector.y >= 2.5)
			{
				if (_lasetFootSoundCfgId != 574)
				{
					PlayFootSound(574);
				}
			}
			else if (base.InputVector.y >= 0.1f)
			{
				if (_lasetFootSoundCfgId != 320)
				{
					PlayFootSound(320);
				}
			}
			else if ((base.InputVector.y <= 0f && base.InputVector.x != 0f) || base.InputVector.y < 0f)
			{
				if (_lasetFootSoundCfgId != 579)
				{
					PlayFootSound(579);
				}
			}
			else if (m_stepAudioId != -1)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(m_stepAudioId, true);
				m_stepAudioId = -1;
				_lasetFootSoundCfgId = -1;
			}
		}
		else if (m_stepAudioId != -1)
		{
			SingletonMono<AudioManager>.Ins.StopMusic(m_stepAudioId, true);
			m_stepAudioId = -1;
			_lasetFootSoundCfgId = -1;
		}
	}

	private void PlayFootSound(int cfgId)
	{
		SingletonMono<AudioManager>.Ins.StopMusic(m_stepAudioId);
		m_stepAudioId = SingletonMono<AudioManager>.Ins.Play2DLoop(cfgId);
		_lasetFootSoundCfgId = cfgId;
	}

	private void CheckNeedIk()
	{
		OpenAllIK();
		if (IsJiMiao)
		{
			CloseAllIK();
		}
		if (Input.sqrMagnitude < 0.001f || CrouchState.CrouchRush || AutoRun)
		{
			NeedAimIK = false;
			NeedHoldIK = false;
		}
		if (CurGun == null)
		{
			NeedHeadIK = false;
		}
	}

	private void InitFSM()
	{
		FSM = new FSMSystem(this);
		StandState state = new StandState();
		CrouchState state2 = new CrouchState();
		PaState state3 = new PaState();
		PaUpState state4 = new PaUpState();
		CrouchDownState state5 = new CrouchDownState();
		JumpState state6 = new JumpState();
		FallState state7 = new FallState();
		LandState state8 = new LandState();
		InCarState state9 = new InCarState();
		CrossState state10 = new CrossState();
		SwimState state11 = new SwimState();
		DownWaitSaveState state12 = new DownWaitSaveState();
		SavePeopleState state13 = new SavePeopleState();
		ClimbState state14 = new ClimbState();
		SitState state15 = new SitState();
		NullState state16 = new NullState();
		Debug.LogError("InitFSM");
		FSM.AddState(state);
		FSM.AddState(state2);
		FSM.AddState(state3);
		FSM.AddState(state4);
		FSM.AddState(state5);
		FSM.AddState(state6);
		FSM.AddState(state7);
		FSM.AddState(state8);
		FSM.AddState(state9);
		FSM.AddState(state10);
		FSM.AddState(state11);
		FSM.AddState(state12);
		FSM.AddState(state13);
		FSM.AddState(state14);
		FSM.AddState(state16);
		FSM.AddState(state15);
		FSMUpBody = new FSMSystem(this);
		AimState state17 = new AimState();
		ShootState state18 = new ShootState();
		ShouwuqiState state19 = new ShouwuqiState();
		NawuqiState state20 = new NawuqiState();
		HuanzidanState state21 = new HuanzidanState();
		ZhuangtianState state22 = new ZhuangtianState();
		AttackState state23 = new AttackState();
		ThrowWeaponState state24 = new ThrowWeaponState();
		HoldJinzhanWeaponState state25 = new HoldJinzhanWeaponState();
		UseItemState state26 = new UseItemState();
		BuildState state27 = new BuildState();
		CutPlantState state28 = new CutPlantState();
		PlantState state29 = new PlantState();
		FSMUpBody.AddState(state17);
		FSMUpBody.AddState(state18);
		FSMUpBody.AddState(state19);
		FSMUpBody.AddState(state20);
		FSMUpBody.AddState(state21);
		FSMUpBody.AddState(state22);
		FSMUpBody.AddState(state16);
		FSMUpBody.AddState(state23);
		FSMUpBody.AddState(state24);
		FSMUpBody.AddState(state25);
		FSMUpBody.AddState(state26);
		FSMUpBody.AddState(state27);
		FSMUpBody.AddState(state28);
		FSMUpBody.AddState(state29);
		StartFsm();
	}

	public void StartFsm()
	{
		if (Battle.Ins.SelfInfo.curStatus == 3 || Battle.Ins.SelfInfo.curStatus == 4)
		{
			FSM.StartFsmSystem(StateID.SkyDiving);
		}
		else if (Battle.Ins.SelfInfo.curStatus == 2)
		{
			FSM.StartFsmSystem(StateID.InStartPlane);
		}
		else if (Battle.Ins.SelfInfo.isSecondHp)
		{
			FSM.StartFsmSystem(StateID.DownWaitSave);
		}
		else
		{
			FSM.StartFsmSystem(StateID.Stand);
		}
		if (Battle.Ins.SelfInfo.isInBuildState)
		{
			FSMUpBody.StartFsmSystem(StateID.Build);
		}
		else
		{
			FSMUpBody.StartFsmSystem(StateID.NullStateID);
		}
	}

	public void EnableUpBodyFsm()
	{
		SetAnimatorWeight(UpperBodyLayer, 1f);
	}

	public void DisEnableUpBodyFsm()
	{
		SetAnimatorWeight(UpperBodyLayer, 0f);
	}

	public void EnableFullBodyMask()
	{
		SetAnimatorWeight(FullBodyLayer, 1f);
	}

	public void DisEnableFullBodyMask()
	{
		FastChangeAnimatorStates(FullBodyLayer, "Null");
		SetAnimatorWeight(FullBodyLayer, 0f);
	}

	public void AddGun(int insId)
	{
		if (!m_Weapons.ContainsKey(insId))
		{
			BagMgr.GunData gunDate = Singleton<BagMgr>.Ins.GetGunDate(insId);
			if (gunDate != null)
			{
				AddWeapon(insId, new Gun(Battle.Ins.SelfPlayer, Singleton<BagMgr>.Ins.GetGunDate(insId)));
			}
		}
	}

	public void AddNearGun(int insId)
	{
		BagItem bagItemQuickAngBagByInstanceId = Singleton<BagMgr>.Ins.GetBagItemQuickAngBagByInstanceId(insId);
		AddWeapon(insId, new NearWeapon(this, bagItemQuickAngBagByInstanceId.itemId));
	}

	public void AddThrowWeapon(int insId)
	{
		BagItem bagItemQuickAngBagByInstanceId = Singleton<BagMgr>.Ins.GetBagItemQuickAngBagByInstanceId(insId);
		AddWeapon(insId, new ThrowWeapon(this, bagItemQuickAngBagByInstanceId.itemId));
	}

	public Gun GetGunByInsId(int GetGunByInsId)
	{
		return GetWeapon(GetGunByInsId) as Gun;
	}

	public Weapon GetWeaponByInsId(int insId)
	{
		return GetWeapon(insId);
	}

	public NearWeapon GetCurNearweapon()
	{
		return GetCurrentWeapon() as NearWeapon;
	}

	public void CheckNoAdd(int insId)
	{
		if (insId > 0 && GetWeapon(insId) == null)
		{
			BagItem bagItemQuickAngBagByInstanceId = Singleton<BagMgr>.Ins.GetBagItemQuickAngBagByInstanceId(insId);
			ItemCfg itemCfg = ItemCfg.Get(bagItemQuickAngBagByInstanceId.itemId);
			if (itemCfg.type == 17 || itemCfg.type == 82)
			{
				AddNearGun(insId);
			}
			else if (itemCfg.type == 25)
			{
				AddThrowWeapon(insId);
			}
			else if (itemCfg.type == 13)
			{
				AddGun(insId);
			}
		}
	}

	public void PlayChangeWeapen(int insid, bool Check = true)
	{
		PlayChangeWeapenFromServer(insid, Check);
		Singleton<BagMgr>.Ins.PutToHand(insid);
	}

	public void PlayChangeWeapenFromServer(int insid, bool Check = true)
	{
		if (Swiming || insid == m_CurrentWeaponInsId)
		{
			return;
		}
		CheckNoAdd(insid);
		if (!CheckCanChangeWeapon() && Check)
		{
			return;
		}
		if (m_CurrentWeaponInsId != -1)
		{
			if (base.HaveThrowWeaponInHand)
			{
				ChangeHandWeaponFromServer(-1);
				return;
			}
			FSMUpBody.SwtichStateReStart(StateID.Shouwuqi, insid);
		}
		else if (insid != -1)
		{
			FSMUpBody.SwtichStateReStart(StateID.Nawuqi, insid);
		}
	}

	public override void ChangeHandWeapon(int insid)
	{
		ChangeHandWeaponFromServer(insid);
		Singleton<BagMgr>.Ins.PutToHand(insid);
	}

	public void ChangeHandWeaponFromServer(int insid)
	{
		if (insid == m_CurrentWeaponInsId)
		{
			return;
		}
		CheckNoAdd(insid);
		if (insid == -1 || GetWeapon(insid) != null)
		{
			if (CurGun != null)
			{
				CurGun.Enabled = false;
				CurGun.CloseJiMiao();
			}
			LastWapInsId = m_CurrentWeaponInsId;
			base.ChangeHandWeapon(insid);
			if (CurGun != null)
			{
				CurGun.Enabled = true;
				Battle.Ins.AimHelp.Disabled = false;
			}
			else
			{
				Battle.Ins.AimHelp.Disabled = true;
			}
			if (CurGun != null)
			{
				Utils.TriggerEvent(BattleEvent.OnChangeHandGun);
			}
		}
	}

	private void OnLoadBulletFinish(int indId, int num)
	{
		Gun gun = GetWeapon(indId) as Gun;
		if (gun != null)
		{
			gun.ChangeBullet(num);
		}
	}

	private void OnRemovePart(int weaponIndex, int partId)
	{
		RemoveGunPart(weaponIndex, partId);
	}

	private void OnAddGunPart(int weaponIndex, int partId)
	{
		AddGunPart(weaponIndex, partId);
	}

	private void OnPickNearGunDelegate(int id)
	{
		if (base.IsKongshou && FSM.CurrentState.ID != StateID.Swim && Climbing)
		{
		}
	}

	private void OnPickGunDelegate(BagGun gunInfo, int pos)
	{
	}

	private void OnDropGunDelegate(int pos)
	{
		if (CurGun != null)
		{
			CurGun.CloseJiMiao();
		}
		RemoveWeapon(pos);
		if (m_CurrentWeaponInsId == -1)
		{
			FSMUpBody.SwitchState(StateID.NullStateID);
		}
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.UpdateGunImages();
		}
	}

	private void OnDropNearGunDelegate()
	{
		if (m_CurrentWeaponInsId == -1)
		{
			FSMUpBody.SwitchState(StateID.NullStateID);
		}
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.UpdateGunImages();
		}
	}

	private void OnChangeGunDelegate()
	{
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.UpdateGunImages();
		}
	}

	public override void OnGetInCar(int vehicleId)
	{
		base.OnGetInCar(vehicleId);
		NearCar = Battle.Ins.GetVehicle(vehicleId);
		Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.InCar);
	}

	public override void OnGetOutCar()
	{
		base.OnGetOutCar();
		NearCar = null;
		if (BattleEvent.OnGetOutVehicle != null)
		{
			BattleEvent.OnGetOutVehicle();
		}
		Battle.Ins.SelfPlayer.FSM.SwitchState(StateID.Stand);
	}

	private void OnPickItem()
	{
		if ((FSM.CurrentState.ID == StateID.Stand || FSM.CurrentState.ID == StateID.Crouch) && FSMUpBody.CurrentState.ID == StateID.NullStateID)
		{
			PlayerAnimation(UpperBodyLayer, "PickItem.kongshou_zhan_pick01");
		}
	}

	public void FollowCamearRotation()
	{
		if (Battle.Ins.QuickLooking || !CheckNeedFollowCamearRotation())
		{
			return;
		}
		if (!base.InCar && !FreeType && !HalfFreeType)
		{
			m_TargetAngleY = Battle.Ins.MainCamera.SelfTransform.eulerAngles.y;
		}
		else if (IsJiMiao)
		{
			SetRotation(Battle.Ins.MainCamera.SelfTransform);
		}
		else
		{
			float orientation = MathUtils.GetOrientation(Input);
			float y = Battle.Ins.MainCamera.SelfTransform.eulerAngles.y;
			m_TargetAngleY = y + orientation;
			if (Input.y < 0f)
			{
				if (FSM.CurrentState.ID == StateID.Stand)
				{
					m_TargetAngleY -= 180f;
				}
				if (FSM.CurrentState.ID == StateID.Jump || FSM.CurrentState.ID == StateID.Fall || FSM.CurrentState.ID == StateID.Land)
				{
					m_TargetAngleY = y;
				}
			}
		}
		if (FSM.CurrentState.ID != StateID.Jump && FSM.CurrentState.ID != StateID.Land)
		{
			LastJumpUpInput = Input;
		}
		UpdatePlayerRotation();
	}

	public void UpdateHalfFreeType()
	{
		HalfFreeType = false;
		if (FSM.CurrentState.ID != StateID.Stand && FSM.CurrentState.ID != StateID.Jump && FSM.CurrentState.ID != StateID.Fall && FSM.CurrentState.ID != StateID.Land)
		{
			return;
		}
		if (FSMUpBody.CurrentState.ID == StateID.Aim)
		{
			if (ShootingTime < 0f && !IsJiMiao)
			{
				HalfFreeType = true;
			}
		}
		else if (FSMUpBody.CurrentState.ID == StateID.NullStateID || FSMUpBody.CurrentState.ID == StateID.HoldNearWeaponState)
		{
			HalfFreeType = true;
		}
	}

	private bool CheckNeedFollowCamearRotation()
	{
		if (IsJiMiao)
		{
			return true;
		}
		if (Input.x == 0f && Input.y == 0f && FSM.CurrentState.ID == StateID.SkyDiving && base.SanOpened)
		{
			return false;
		}
		return true;
	}

	private void UpdatePlayerRotation()
	{
		Vector3 eulerAngles = PlayerTransform.eulerAngles;
		if (m_TargetAngleY != eulerAngles.y)
		{
			if (Joystick.IsDown)
			{
				eulerAngles.y = Mathf.LerpAngle(eulerAngles.y, m_TargetAngleY, RotateSpeed * Time.fixedDeltaTime * 0.5f);
			}
			else
			{
				eulerAngles.y = Mathf.LerpAngle(eulerAngles.y, m_TargetAngleY, RotateSpeed * Time.fixedDeltaTime * 0.5f);
			}
			PlayerTransform.eulerAngles = eulerAngles;
		}
	}

	private void UpdateRigidBodyLock()
	{
		if (!IsDie && !base.InCar && FSM.CurrentState.ID != StateID.Swim)
		{
			if (m_Grounded && Input.magnitude < 0.05f && m_Rigidbody.velocity.sqrMagnitude < 0.05f && m_Rigidbody.angularVelocity.sqrMagnitude < 1f && !AutoRun)
			{
				LockRigidBodyXZ();
			}
			else
			{
				UnLockRigidBody();
			}
			if ((bool)NearCar && NearCar.GetSpeed() > 1f)
			{
				UnLockRigidBody();
			}
		}
	}

	private void UpdateMyInputVectorForAimator()
	{
		if (FreeType || HalfFreeType)
		{
			if (Input != Vector2.zero)
			{
				m_InputVector.x = 0f;
				if (HalfFreeType)
				{
					if (Input.y >= 0f)
					{
						m_InputVector.y = m_SpeedMultiple;
					}
					else
					{
						m_InputVector.y = 0f - m_SpeedMultiple;
					}
				}
				else
				{
					m_InputVector.y = m_SpeedMultiple;
				}
			}
			else
			{
				m_InputVector.x = 0f;
				m_InputVector.y = 0f;
			}
		}
		else
		{
			m_InputVector.x = Input.x * m_SpeedMultiple;
			m_InputVector.y = Input.y * m_SpeedMultiple;
		}
		if (AutoRun)
		{
			m_InputVector.x = 0f;
			m_InputVector.y = 3f;
			Input.y = 1f;
		}
	}

	private void UpdatePlayerColliderMaterial()
	{
		if ((Input.magnitude > 0.3f && ClimbAngle < 100f) || FSM.CurrentState.ID == StateID.Jump || FSM.CurrentState.ID == StateID.Fall)
		{
			PlayerCollider.material = Battle.Ins.MinPhysicMaterial;
		}
		else if ((double)Input.sqrMagnitude <= 0.0005)
		{
			PlayerCollider.material = Battle.Ins.MaxPhysicMaterial;
		}
		else if (ClimbAngle > 100f)
		{
			PlayerCollider.material = Battle.Ins.MidPhysicMaterial;
		}
		else
		{
			PlayerCollider.material = null;
		}
	}

	protected override void OnChangeAnimatorState(int layer, string targetStateName, int stateHash)
	{
		m_SyncAnimator.layer = (byte)layer;
		m_SyncAnimator.animationHash = stateHash;
		Client2Gs.Ins.Send(m_SyncAnimator);
		EnableLayer(layer);
		if (Battle.Ins.PlayTest)
		{
			Debug.LogError("layer: " + layer + "   " + targetStateName);
		}
	}

	private void EnableLayer(int layer)
	{
		if (layer == FullBodyLayer)
		{
			EnableFullBodyMask();
		}
		else if (layer == UpperBodyLayer)
		{
			EnableUpBodyFsm();
		}
	}

	private void CheckGround()
	{
		CheckGroundDistance();
		if (-0.06 < (double)GroundDistance && GroundDistance < 0.2f)
		{
			m_Grounded = true;
			return;
		}
		m_YSpeed = m_Rigidbody.velocity.y;
		m_Grounded = false;
	}

	public void SetGroundPos(Vector3 pos)
	{
		float y = Battle.Ins.GetGroundPos(pos).y;
		Vector3 position = new Vector3(pos.x, y, pos.z);
		Battle.Ins.SelfPlayer.SetPosition(position);
	}

	public void SetPosition(Vector3 position)
	{
		PlayerTransform.position = position;
	}

	public void ResetSetPosition(Vector3 position)
	{
		PlayerTransform.position = position;
		m_LastPos = position;
	}

	public void SetAnimatorYawValue(float value)
	{
		m_Yaw = value;
		PlayerAnimator.SetFloat(BasePlayerController.s_YawHash, value);
	}

	public virtual void FollowRotation(Transform referenceTransform)
	{
		Vector3 euler = new Vector3(base.transform.eulerAngles.x, referenceTransform.eulerAngles.y, base.transform.eulerAngles.z);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(euler), 5f * Time.fixedDeltaTime);
	}

	public virtual void SetRotation(Transform referenceTransform)
	{
		if (!Battle.Ins.QuickLooking || FSMUpBody.CurrentState.ID == StateID.Shoot)
		{
			Vector3 eulerAngles = PlayerTransform.eulerAngles;
			m_TargetAngleY = referenceTransform.eulerAngles.y;
			eulerAngles.y = m_TargetAngleY;
			PlayerTransform.eulerAngles = eulerAngles;
		}
	}

	private void CheckGroundDistance()
	{
		if (!PlayerCollider)
		{
			return;
		}
		GroundDistance = 1000f;
		InBuilding = false;
		RaycastHit hitInfo;
		if (!Physics.SphereCast(PlayerTransform.position + Vector3.up * 1f, 0.15f, Vector3.down, out hitInfo, 1000f, Battle.Ins.GroundLayer, QueryTriggerInteraction.Ignore))
		{
			return;
		}
		GroundName = hitInfo.collider.name;
		if (GroundName != LastGroundName)
		{
			Utils.TriggerEvent(BattleEvent.OnSceneChanged, LastGroundName, GroundName);
			LastGroundName = GroundName;
		}
		if (!(GroundName == "haidi") && !(GroundName == "tizi"))
		{
			m_GroundNormal = hitInfo.normal;
			if (Input.y < 0f)
			{
				ClimbAngle = Vector3.Angle(-base.transform.forward, m_GroundNormal);
			}
			else
			{
				ClimbAngle = Vector3.Angle(base.transform.forward, m_GroundNormal);
			}
			if (FSM != null && FSMUpBody != null && FSM.CurrentState != null && FSMUpBody.CurrentState != null && FSM.CurrentState.ID == StateID.Pa && FSMUpBody.CurrentState.ID != StateID.Shoot)
			{
				Vector3 lhs = Vector3.Cross(m_GroundNormal, base.transform.forward);
				Vector3 target = Vector3.Cross(lhs, m_GroundNormal);
				base.transform.forward = Vector3.MoveTowards(base.transform.forward, target, 1.5f * Time.deltaTime);
			}
			GroundDistance = Math.Min(LeftFoot.transform.position.y, RightFoot.transform.position.y) - hitInfo.point.y;
			if (hitInfo.collider.CompareTag("road"))
			{
				FootTexture = FootTextureType.Road;
			}
			else if (hitInfo.collider.CompareTag("builder"))
			{
				FootTexture = FootTextureType.Building;
				InBuilding = true;
			}
			else if (hitInfo.collider.CompareTag("Ground"))
			{
				FootTexture = FootTextureType.Ground;
			}
			else if (hitInfo.collider.CompareTag("Water"))
			{
				FootTexture = FootTextureType.Water;
			}
			else
			{
				FootTexture = FootTextureType.Road;
			}
		}
	}

	private void CheckUnderGround()
	{
		Ray ray = new Ray(PlayerTransform.position + Vector3.up * 1000f, Vector3.down);
		int num = Physics.RaycastNonAlloc(ray, m_GroundColliders, 2500f, Battle.Ins.CheckUnderGroundLayer);
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = m_GroundColliders[i];
			if (raycastHit.collider != null && raycastHit.point.y - PlayerTransform.position.y > 2.5f && PlayerTransform.position.y <= 10.5f)
			{
				PlayerTransform.position = raycastHit.point;
				FSM.SwitchState(StateID.Stand, raycastHit.point);
				UdpSession.Ins.Send("Under Ground Pos  " + raycastHit.point);
				break;
			}
		}
	}

	private bool CheckCanMoveUp()
	{
		if (ClimbAngle > 140f && (FSM.CurrentState.ID == StateID.Stand || FSM.CurrentState.ID == StateID.Crouch))
		{
			PlayerRigidbody.velocity = Vector3.zero;
			return false;
		}
		return true;
	}

	public void ChangeVelocity(float speed = 1.5f)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles.x = (eulerAngles.z = 0f);
		Vector3 vector = new Vector3(base.InputVector.x, 0f, base.InputVector.y);
		Vector3 vector2 = Quaternion.Euler(eulerAngles) * vector;
		PlayerRigidbody.velocity = new Vector3(vector2.x * speed, PlayerRigidbody.velocity.y, vector2.z * speed);
	}

	public void ChangeYVelocity(float ySpeed)
	{
		PlayerRigidbody.velocity = new Vector3(PlayerRigidbody.velocity.x, ySpeed, PlayerRigidbody.velocity.z);
	}

	protected override void OnHPChanged(int changNum, long roleId)
	{
		if (BattleEvent.OnSelfHpChange != null && roleId == RoleId)
		{
			BattleEvent.OnSelfHpChange(HP, changNum);
		}
	}

	protected override void OnEPChanged()
	{
		CheckAnimatorSpeedByEp();
		if (BattleEvent.OnEpChange != null)
		{
			BattleEvent.OnEpChange(EP);
		}
	}

	public void InitWeapons()
	{
		PlayChangeWeapen(Battle.Ins.SelfInfo.bagInfo.instanceId);
	}

	private void AddSurprise()
	{
		if (PlayerRigidbody.velocity.y > 10f)
		{
			PlayerRigidbody.velocity = new Vector3(0f, -10f, 0f);
			Debug.LogError("velocity.y+++++++" + PlayerRigidbody.velocity);
		}
		if (!Skydiving && (PlayerRigidbody.velocity.x > 10f || PlayerRigidbody.velocity.z > 10f))
		{
			PlayerRigidbody.velocity = new Vector3(0f, -10f, 0f);
			Debug.LogError("velocity.x+++++++");
		}
	}

	private void CheckCanOpenJimiao()
	{
		if (FSMUpBody != null && FSMUpBody.CurrentState != null && (FSMUpBody.CurrentState.ID == StateID.Aim || FSMUpBody.CurrentState.ID == StateID.Shoot))
		{
			CanOpenJimiao = true;
		}
		else
		{
			CanOpenJimiao = false;
		}
	}

	public RaycastHit CheckNearLowToTopTiZi()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(base.transform.position + Vector3.up * 0.5f - base.transform.forward * 0.5f, 0.2f, base.transform.forward, out hitInfo, 1f, 1) && hitInfo.collider.CompareTag("Tizi"))
		{
			return hitInfo;
		}
		return hitInfo;
	}

	public RaycastHit CheckNearTopToLowTiZi()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(PlayerTransform.position + Vector3.up * 1.5f + PlayerTransform.forward, 0.4f, Vector3.down, out hitInfo, 2.1f, 1) && hitInfo.collider.CompareTag("Tizi"))
		{
			return hitInfo;
		}
		return hitInfo;
	}

	private IEnumerator SyncTick()
	{
		while (true)
		{
			SyncMoveStatus();
			(FSM.GetState(StateID.Swim) as SwimState).CheckSwim();
			CheckPlayStepAudio();
			yield return Wait02;
		}
	}

	private IEnumerator SyncSecondTick()
	{
		while (true)
		{
			SendWalkDis();
			SendFootTexture();
			CheckUnderGround();
			yield return Wait2;
		}
	}

	private IEnumerator Sync60Tick()
	{
		while (true)
		{
			SendWalkDis();
			yield return Wait60;
		}
	}

	private IEnumerator Sync5Tick()
	{
		while (true)
		{
			CheckNeedControlMonster();
			yield return Utils.WaitForSeconds(3f);
		}
	}

	private void CheckNeedControlMonster()
	{
		List<MonsterController> list = new List<MonsterController>();
		foreach (KeyValuePair<long, MonsterController> item in SingletonMono<MonsterMgr>.Ins.MonsterDic)
		{
			if (item.Value != null)
			{
				list.Add(item.Value);
			}
		}
		list.Sort(new MonsterControllerDis());
		HashSet<long> hashSet = new HashSet<long>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].DisToMySelf < 200f && (!list[i].HasOwner || list[i].IsMyControl))
			{
				hashSet.Add(list[i].InsId);
			}
			if (hashSet.Count >= ConstsBs.MAX_CONTROLL_MONSTER_NUMBER)
			{
				break;
			}
		}
		foreach (long item2 in MyMonsterInsId)
		{
			if (!hashSet.Contains(item2))
			{
				SingletonMono<MonsterMgr>.Ins.SendUnControlMonsterMsg(item2);
			}
		}
		foreach (long item3 in hashSet)
		{
			if (!MyMonsterInsId.Contains(item3))
			{
				SingletonMono<MonsterMgr>.Ins.SendControlMonsterMsg(item3);
			}
		}
	}

	private void SyncMoveStatus()
	{
		if (!base.InCar && !InStartPlane)
		{
			Vector3 position = PlayerTransform.position;
			if (!MathUtils.RoughlyEquals(position, m_OldPos, 0.001f))
			{
				m_SyncPos.pos.x = position.x;
				m_SyncPos.pos.y = position.y;
				m_SyncPos.pos.z = position.z;
				Client2Gs.Ins.Send(m_SyncPos);
				m_OldPos = position;
			}
			Vector3 eulerAngles = PlayerTransform.eulerAngles;
			if (!MathUtils.RoughlyEquals(m_OldEulerAngles, eulerAngles, 1f))
			{
				m_SyncEulerAngles.orientation.x = MathUtils.Float2Short(eulerAngles.x);
				m_SyncEulerAngles.orientation.y = MathUtils.Float2Short(eulerAngles.y);
				m_SyncEulerAngles.orientation.z = MathUtils.Float2Short(eulerAngles.z);
				Client2Gs.Ins.Send(m_SyncEulerAngles);
				m_OldEulerAngles = eulerAngles;
			}
		}
		if (base.InCar && Mathf.Abs(m_OldYaw - m_Yaw) > 2f)
		{
			m_SyncYaw.yaw = m_Yaw;
			Client2Gs.Ins.Send(m_SyncYaw);
			m_OldYaw = m_Yaw;
		}
	}

	private void SyncCameraForward()
	{
		LookAtPosition = Battle.Ins.MainCamera.SelfTransform.position + Battle.Ins.MainCamera.SelfTransform.forward * 100f;
		if (!MathUtils.RoughlyEquals(Battle.Ins.MainCamera.SelfTransform.forward, m_LastCamearForward, 5f))
		{
			m_SyncLookPoint.point.x = Battle.Ins.MainCamera.SelfTransform.forward.x;
			m_SyncLookPoint.point.y = Battle.Ins.MainCamera.SelfTransform.forward.y;
			m_SyncLookPoint.point.z = Battle.Ins.MainCamera.SelfTransform.forward.z;
			Client2Gs.Ins.Send(m_SyncLookPoint);
			m_LastCamearForward = Battle.Ins.MainCamera.SelfTransform.forward;
		}
	}

	private void SendWalkDis()
	{
		if (Grounded && !MathUtils.RoughlyEquals(m_LastSecondPos, base.Pos, 1f))
		{
			m_LastSecondPos = base.Pos;
		}
	}

	private IEnumerator HalfSecondTick()
	{
		while (true)
		{
			CheckNearHaveCar();
			CheckNearSavedPeopleColliders();
			yield return Wait05;
		}
	}

	public override void SetAnimatorWeight(int layer, float weight)
	{
		if (!PlayerAnimator.GetLayerWeight(layer).Equals(weight))
		{
			base.SetAnimatorWeight(layer, weight);
			m_SyncLayerWeight.layer = (byte)layer;
			m_SyncLayerWeight.value = MathUtils.Float2Short(weight);
			Client2Gs.Ins.Send(m_SyncLayerWeight);
		}
	}

	private void CheckNearHaveCar()
	{
		if (base.InCar)
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(PlayerTransform.position, 2.5f, m_NearCarsColliders, 1 << BattleScMgr.CarLayer, QueryTriggerInteraction.Collide);
		if (num > 0)
		{
			float num2 = float.MaxValue;
			GameObject gameObject = null;
			for (int i = 0; i < num; i++)
			{
				Collider collider = m_NearCarsColliders[i];
				float num3 = Vector3.Distance(PlayerTransform.position, collider.transform.position);
				if (num3 < num2 && collider != null && !collider.GetComponentInParent<VehicleMonitor>().IsBroken())
				{
					num2 = num3;
					gameObject = collider.gameObject;
				}
			}
			if (gameObject != null)
			{
				NearCar = gameObject.GetComponentInParent<VehicleMonitor>();
				if (BattleEvent.OnNearCar != null && !Skydiving)
				{
					BattleEvent.OnNearCar(NearCar);
				}
			}
		}
		else if (BattleEvent.OnFarCar != null)
		{
			NearCar = null;
			BattleEvent.OnFarCar();
		}
	}

	private void CheckNearSavedPeopleColliders()
	{
		CanSavedPeople = null;
		int num = Physics.OverlapSphereNonAlloc(PlayerTransform.position, m_SavePeopleDis, m_NearPeopleColliders, 1 << LayerMask.NameToLayer("OtherPlayerCollider"), QueryTriggerInteraction.Ignore);
		if (num <= 0)
		{
			return;
		}
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			OtherPlayerController componentInParent = m_NearPeopleColliders[i].GetComponentInParent<OtherPlayerController>();
			if (componentInParent != null && Singleton<TeamScMgr>.Ins.IsInMyTeam(componentInParent.RoleId))
			{
				float num3 = Vector3.Distance(PlayerTransform.position, componentInParent.transform.position);
				if (num3 < num2 && componentInParent.IsDownWaitSave)
				{
					num2 = num3;
					CanSavedPeople = componentInParent;
				}
			}
		}
	}

	protected override void OnDied()
	{
		FSMUpBody.SwitchState(StateID.NullStateID);
		FSM.SwitchState(StateID.NullStateID);
		if (IsMale)
		{
			SingletonMono<AudioManager>.Ins.Play2D(188);
		}
		else
		{
			SingletonMono<AudioManager>.Ins.Play2D(173);
		}
		Battle.Ins.MainCamera.ChangeState("Die");
	}

	public void SetZaijvRigidbodyValue()
	{
		m_Rigidbody.useGravity = false;
		m_Rigidbody.detectCollisions = false;
		PlayerCollider.enabled = false;
		m_Rigidbody.isKinematic = true;
		LockRigidBodyXYZ();
	}

	public override void PlayHitEffect(SHitPlayer msg)
	{
		if (m_CanPlayrHitSound)
		{
			m_CanPlayrHitSound = false;
			int num = -1;
			num = ((!IsMale) ? Utils.Random(179, 182) : Utils.Random(176, 179));
			float duration = Utils.Random(1f, 3f);
			SoundControll.PlaySound(num);
			Vector3 arg = MathUtils.Short2Float(new Vector3(msg.forward.x, msg.forward.y, msg.forward.z));
			Utils.TriggerEvent(BattleEvent.OnHited, arg);
			SingletonMono<TimerManager>.Ins.AddTimer(RoleId + "CanPlayHitSound", duration, _003CPlayHitEffect_003Em__1);
		}
		base.PlayHitEffect(msg);
	}

	public void ChangeYPos(float ySpeed)
	{
		PlayerTransform.SetPositionY(PlayerTransform.position.y + ySpeed);
	}

	public override void SetAnimatorSpeed(float speed)
	{
		if (PlayerAnimator.speed != speed)
		{
			base.SetAnimatorSpeed(speed);
			cSyncAnimatorSpeed.speed = speed;
			Client2Gs.Ins.Send(cSyncAnimatorSpeed);
		}
	}

	public void AddAnimatorSpeed(float percent)
	{
		percent *= 0.01f;
		SpeedUpPercent += percent;
		SetAnimatorSpeed(PlayerAnimator.speed + percent);
	}

	public void ReduceAnimatorSpeed(float percent)
	{
		percent *= 0.01f;
		SpeedUpPercent -= percent;
		SetAnimatorSpeed(PlayerAnimator.speed - percent);
	}

	private void UpdateAnimatorSpeed()
	{
		float animatorSpeed = m_EpAnimationSpeedChange * m_GunAnimationSpeedChange;
		SetAnimatorSpeed(animatorSpeed);
	}

	public void SetEpAnimatorSpeed(float percent)
	{
		if (m_EpAnimationSpeedChange != percent)
		{
			m_EpAnimationSpeedChange = percent;
			UpdateAnimatorSpeed();
		}
	}

	public void SetGunAnimatorSpeed(float percent)
	{
		if (m_GunAnimationSpeedChange != percent)
		{
			m_GunAnimationSpeedChange = percent;
			UpdateAnimatorSpeed();
		}
	}

	private void CheckAnimatorSpeedByEp()
	{
		if (EP > 0)
		{
			List<EPPropertyCfg> allList = EPPropertyCfg.GetAllList();
			for (int num = allList.Count - 1; num > 0; num--)
			{
				if (EP > allList[num].maxValue)
				{
					SetEpAnimatorSpeed(1f + allList[num].addSpeed * 0.01f);
					break;
				}
			}
		}
		else
		{
			SetEpAnimatorSpeed(1f);
		}
	}

	public float RunSpeedUpSkill(int skillType)
	{
		List<int> list = new List<int>();
		foreach (int item in list)
		{
			CharacterSkillCfg characterSkillCfg = CharacterSkillCfg.Get(item);
			if (characterSkillCfg.skilltype == skillType)
			{
				AddAnimatorSpeed(characterSkillCfg.skillvalues[0]);
				return characterSkillCfg.skillvalues[0];
			}
		}
		return 0f;
	}

	public float GetSkillValueIndex0(int skillType)
	{
		List<int> list = new List<int>();
		foreach (int item in list)
		{
			CharacterSkillCfg characterSkillCfg = CharacterSkillCfg.Get(item);
			if (characterSkillCfg.skilltype == skillType)
			{
				return characterSkillCfg.skillvalues[0] * 0.01f;
			}
		}
		return 0f;
	}

	public void SetHVMax(float max)
	{
		if (base.InputVector.x > max)
		{
			base.InputVector = new Vector3(max, base.InputVector.y, base.InputVector.y);
		}
		if (base.InputVector.y > max)
		{
			base.InputVector = new Vector3(base.InputVector.x, base.InputVector.y, max);
		}
		SetAnimatorHorizontalValue(base.InputVector.x);
		SetAnimatorForwardValue(base.InputVector.y);
	}

	private void AddDownForce()
	{
		if (!IsDie && (FSM.CurrentState.ID == StateID.Stand || FSM.CurrentState.ID == StateID.Crouch))
		{
			if (!InBuilding)
			{
				PlayerRigidbody.AddForce(Physics.gravity * 10f);
			}
			else
			{
				PlayerRigidbody.AddForce(Physics.gravity * 10f);
			}
		}
	}

	public bool CheckCanFire()
	{
		if (!CheckCommonDoCondition() || Swiming || IsDriver || (base.InCar && !Tanshen) || base.Pos.y < 0f || FSM.CurrentState.ID == StateID.Cross || !CanShoot || InBuildState)
		{
			return false;
		}
		return true;
	}

	public bool CheckCanUseItem()
	{
		if (IsDie || IsDownWaitSave || Climbing || FSMUpBody.CurrentState.ID == StateID.Shouwuqi || Swiming || (IsDriver && NearCar != null && NearCar.GetSpeed() > 1f))
		{
			return false;
		}
		return true;
	}

	public bool CheckCanEnterBuildState()
	{
		if ((FSM.CurrentState.ID == StateID.Stand || FSM.CurrentState.ID == StateID.Crouch) && (FSMUpBody.CurrentState.ID == StateID.NullStateID || FSMUpBody.CurrentState.ID == StateID.Aim || FSMUpBody.CurrentState.ID == StateID.HoldNearWeaponState))
		{
			return true;
		}
		if (InBuildState)
		{
			return true;
		}
		return false;
	}

	public bool CheckCanChangeWeapon()
	{
		if (IsDie || IsDownWaitSave || Swiming || IsDriver || Climbing || FSM.CurrentState.ID == StateID.Attack)
		{
			return false;
		}
		return true;
	}

	public bool CheckCommonDoCondition()
	{
		if (IsDie || IsDownWaitSave || FSMUpBody.CurrentState.ID == StateID.Shouwuqi || FSMUpBody.CurrentState.ID == StateID.Nawuqi || Climbing)
		{
			return false;
		}
		return true;
	}

	public void SendFootTexture()
	{
		if (FootTexture != LastFootTexture)
		{
			cSyncFootTexture.textureType = (byte)FootTexture;
			Client2Gs.Ins.Send(cSyncFootTexture);
			LastFootTexture = FootTexture;
		}
	}

	public void DisablePhysic()
	{
		if (m_Rigidbody != null)
		{
			m_Rigidbody.useGravity = false;
			m_Rigidbody.isKinematic = true;
		}
	}

	public void EnablePhysic()
	{
		if (m_Rigidbody != null)
		{
			m_Rigidbody.useGravity = true;
			m_Rigidbody.isKinematic = false;
		}
	}

	public void LockRigidBodyXZ()
	{
		m_Rigidbody.constraints = (RigidbodyConstraints)122;
	}

	public void LockRigidBodyXYZ()
	{
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void UnLockRigidBody()
	{
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	public void ResetRigidbodyValue()
	{
		if (!(m_Rigidbody == null))
		{
			m_Rigidbody.useGravity = true;
			m_Rigidbody.detectCollisions = true;
			PlayerCollider.enabled = true;
			m_Rigidbody.isKinematic = false;
			UnLockRigidBody();
		}
	}

	public bool CheckCanPaUp()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(HeadTop.position, 0.1f, Vector3.up, out hitInfo, 1.3f, 524289))
		{
			return false;
		}
		return true;
	}

	public bool CheckCanCrouchUp()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(RootBone.position, 0.1f, Vector3.up, out hitInfo, 1.3f, 524289))
		{
			return false;
		}
		return true;
	}

	public bool CheckCanPaToStand()
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(HeadTop.position, 0.1f, Vector3.up, out hitInfo, 1.8f, 524289))
		{
			return false;
		}
		return true;
	}

	private Vector3 FindPosForRobot()
	{
		RobotBirthPos = Vector3.zero;
		int num = Physics.OverlapSphereNonAlloc(PlayerTransform.position, 35f, m_NearGosColliders, (1 << BattleScMgr.DefaultLayer) | BattleScMgr.WaterLayer);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (m_NearGosColliders[i].gameObject.layer == BattleScMgr.WaterLayer)
				{
					return Vector3.zero;
				}
				if (m_NearGosColliders[i].CompareTag("tree"))
				{
					Vector3 lhs = m_NearGosColliders[i].transform.position - PlayerTransform.position;
					float num2 = Vector3.Dot(lhs, PlayerTransform.forward);
					if (num2 > 0f)
					{
						RobotBirthPos = m_NearGosColliders[i].transform.position + lhs.normalized;
						return RobotBirthPos;
					}
				}
			}
		}
		return Vector3.zero;
	}

	private void ShowAimedName()
	{
		m_ray.origin = Battle.Ins.MainCamera.GetTargetLookAtTransform().position;
		m_ray.direction = Battle.Ins.MainCamera.GetBaseDirection();
		RaycastHit hitInfo;
		if (Physics.Raycast(m_ray, out hitInfo, 700f, 1 << BattleScMgr.OtherPlayerColliderLayer))
		{
			Collider collider = hitInfo.collider;
			OtherPlayerController componentInParent = collider.GetComponentInParent<OtherPlayerController>();
			if (componentInParent != m_CurAimedPlayer)
			{
				if (m_CurAimedPlayer != null)
				{
					m_CurAimedPlayer.HideNameLabel();
				}
				m_CurAimedPlayer = componentInParent;
				if (!Physics.Linecast(Battle.Ins.MainCamera.SelfTransform.position, componentInParent.HeadBub.position, 1 << BattleScMgr.SceneLayerMask))
				{
					m_CurAimedPlayer.ShowNameLabel();
					return;
				}
				m_CurAimedPlayer.HideNameLabel();
				m_CurAimedPlayer = null;
			}
		}
		else if (m_CurAimedPlayer != null)
		{
			m_CurAimedPlayer.HideNameLabel();
			m_CurAimedPlayer = null;
		}
	}

	public float MolongSkill()
	{
		float skillValueIndex = GetSkillValueIndex0(1001);
		if (HP >= ConstsBs.HpPlayer)
		{
			return skillValueIndex;
		}
		return 0f;
	}

	private void YanlongSkill()
	{
		ExtraAddHp += GetSkillValueIndex0(1000);
	}

	public override void ClothChanged()
	{
		base.ClothChanged();
		if (IsJiMiao)
		{
			CurGun.CloseJiMiao();
		}
	}

	public void SetAutoRun()
	{
		AutoRun = true;
	}

	public void CloseAutoRun()
	{
		AutoRun = false;
	}

	[CompilerGenerated]
	private void _003CInit_003Em__0(GameObject o)
	{
		RoleHandAnimator = o.GetComponent<Animator>();
		JimiaoRightHandGuaDian = Utils.GetChildObjByName("wuqi_guadian", o).transform;
		JimiaoSetSkin = o.GetComponent<CharacterSetSkin>();
		JimiaoHoldIK = o.AddComponent<HoldIK>();
		JimiaoHoldIK.Init(Utils.GetChildObjByName("Bip001 L UpperArm", o).transform, Utils.GetChildObjByName("Bip001 L ForeTwist", o).transform, Utils.GetChildObjByName("Bip001 L Forearm", o).transform, Utils.GetChildObjByName("Bip001 L Hand", o).transform);
	}

	[CompilerGenerated]
	private void _003CPlayHitEffect_003Em__1()
	{
		m_CanPlayrHitSound = true;
	}
}

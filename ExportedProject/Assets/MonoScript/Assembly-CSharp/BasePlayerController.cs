using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public abstract class BasePlayerController : MapObject
{
	public enum FootTextureType
	{
		Grass = 0,
		Building = 1,
		Ground = 2,
		Road = 3,
		Water = 5
	}

	public enum BodyPart
	{
		LeftArmHigh = 0,
		RightArmHigh = 1,
		LeftArmLow = 2,
		RightArmLow = 3,
		LeftLegHigh = 4,
		RightLegHigh = 5,
		LeftLegLow = 6,
		RightLegLow = 7,
		Body = 8,
		Head = 9,
		HeadTop = 10,
		Root = 11,
		LeftFoot = 12,
		RightFoot = 13,
		RightHand = 14,
		LeftHand = 15,
		Neck = 16,
		HeadBub = 17,
		Count = 18
	}

	[CompilerGenerated]
	private sealed class _003CPlayerDie_003Ec__AnonStorey2
	{
		internal SPlayerDie msg;

		internal BasePlayerController _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!_0024this.IsDie)
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			o.gameObject.SetLayerRecursively(BattleScMgr.OtherPlayerColliderLayer);
			o.gameObject.name = "Bip001";
			if (_0024this.DollBones != null)
			{
				UnityEngine.Object.Destroy(_0024this.DollBones);
			}
			_0024this.DollBones = o.gameObject;
			Transform[] componentsInChildren = o.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Transform value;
				if (componentsInChildren[i] != null && _0024this.m_NameToBone.TryGetValue(componentsInChildren[i].name, out value) && value != null)
				{
					componentsInChildren[i].localPosition = value.localPosition;
					componentsInChildren[i].localRotation = value.localRotation;
				}
			}
			if (_0024this.PlayerAnimator != null)
			{
				_0024this.PlayerAnimator.enabled = false;
			}
			if (_0024this.Skin != null)
			{
				_0024this.Skin.SetBones(_0024this.DollBones.transform, componentsInChildren);
			}
			Singleton<BattleScMgr>.Ins.AddForceDie(new Vector3(msg.forward.x, msg.forward.y, msg.forward.z), msg.bodyPart, _0024this.DollBones);
			_0024this.PlayerCollider.enabled = false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_PutOnCloth_003Ec__AnonStorey3
	{
		internal ItemCfg clothCfg;

		internal BasePlayerController _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!_0024this.m_clothes.Contains(clothCfg.id))
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			GameObject childObjByName = Utils.GetChildObjByName("Bip001", o);
			if (childObjByName != null)
			{
				UnityEngine.Object.Destroy(childObjByName);
			}
			SkinnedMeshRenderer componentInChildren = o.GetComponentInChildren<SkinnedMeshRenderer>();
			MeshRenderer componentInChildren2 = o.GetComponentInChildren<MeshRenderer>();
			MaterialLevel componentInChildren3 = o.GetComponentInChildren<MaterialLevel>();
			if (componentInChildren3 != null && componentInChildren != null)
			{
				componentInChildren.material = componentInChildren3.Low;
			}
			if (componentInChildren3 != null && componentInChildren2 != null)
			{
				componentInChildren2.material = componentInChildren3.Low;
			}
			if (string.IsNullOrEmpty(clothCfg.root))
			{
				if (_0024this.RoleId != Singleton<RoleMgr>.Ins.info.roleId)
				{
					if (componentInChildren != null)
					{
						componentInChildren.quality = SkinQuality.Bone1;
					}
				}
				else if (componentInChildren != null)
				{
					componentInChildren.gameObject.layer = 9;
				}
				_0024this.Skin.SetSkin(o, clothCfg.childType);
			}
			else
			{
				_0024this.Skin.SetSkinAttach(clothCfg.childType, clothCfg.root, o);
			}
			if (clothCfg.childType == 22)
			{
				if (_0024this.m_HaveTaoZhuang)
				{
					o.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
				}
			}
			else if (clothCfg.childType == 33)
			{
				_0024this.Skin.HideSkin(16);
			}
			else if (clothCfg.childType == 16)
			{
				if (_0024this.Skin.ContainSkin(33))
				{
					_0024this.Skin.HideSkin(16);
				}
			}
			else if (clothCfg.childType == 23)
			{
				_0024this.SetHaveBag(true);
			}
			else if (clothCfg.childType == 44)
			{
				_0024this.Skin.HideAllNoHideParent();
				_0024this.m_HaveJili = true;
				_0024this.Skin.ShowSkin(44);
				_0024this.Skin.ShowSkin(30);
				_0024this.Skin.ShowSkin(23);
				_0024this.Skin.ShowSkin(31);
			}
			if (_0024this.RoleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				_0024this.Skin.OpenUpdateWhenOffscreen(clothCfg.childType);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_PutOnCloth_003Ec__AnonStorey4
	{
		internal ItemCfg beidaiCfg;

		internal _003C_PutOnCloth_003Ec__AnonStorey3 _003C_003Ef__ref_00243;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!_003C_003Ef__ref_00243._0024this.m_clothes.Contains(_003C_003Ef__ref_00243.clothCfg.id))
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			_003C_003Ef__ref_00243._0024this.Skin.SetSkin(o, beidaiCfg.childType);
			if (_003C_003Ef__ref_00243._0024this.RoleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				_003C_003Ef__ref_00243._0024this.Skin.OpenUpdateWhenOffscreen(beidaiCfg.childType);
			}
		}
	}

	public Transform PlayerTransform;

	[SerializeField]
	public Animator PlayerAnimator;

	public Animator RoleHandAnimator;

	[SerializeField]
	public CapsuleCollider PlayerCollider;

	[SerializeField]
	protected Transform[] m_BodyParts;

	[SerializeField]
	protected SkinnedMeshRenderer m_SkinnedMeshRenderer;

	public Transform LeftFoot;

	public Transform RightFoot;

	public Transform HeadTop;

	public Transform HeadBub;

	public Transform HeadNameLabel;

	public Transform RootBone;

	public Transform RightHand;

	public Transform LeftHand;

	public Transform NeckBone;

	public Transform RightJianbang;

	public Transform RightHandGuaDian;

	[SerializeField]
	protected Transform m_GuaDianL1;

	[SerializeField]
	protected Transform m_GuaDianL2;

	[SerializeField]
	protected Transform m_GuaDianR1;

	[SerializeField]
	protected Transform m_GuaDianR2;

	[SerializeField]
	protected Transform m_LeftHandGuaDian;

	[SerializeField]
	protected Transform m_ShouQiangGuaDian;

	[SerializeField]
	protected Transform m_BagGuaDian;

	[SerializeField]
	protected Transform m_NearWeaponGuaDian;

	[NonSerialized]
	public long RoleId;

	[NonSerialized]
	public int HP = ConstsBs.HpPlayer;

	[NonSerialized]
	public int EP;

	[NonSerialized]
	public int Pulmonary = 100;

	[NonSerialized]
	public bool IsDie;

	[NonSerialized]
	public bool IsDownWaitSave;

	[NonSerialized]
	public string Name = string.Empty;

	[NonSerialized]
	public bool IsMale = true;

	[NonSerialized]
	public RectTransform NameLabel;

	public SoundControll SoundControll;

	protected PlayerInfo m_PlayerInfo;

	public bool NewPlayer;

	public int VipLevel;

	public GameObject TuzhiGo;

	[NonSerialized]
	public int VehicleId = -1;

	protected GameObject m_San;

	protected GameObject m_SanBag;

	protected bool m_SanOpened;

	protected bool m_SanDestroyed;

	protected bool m_Landed;

	public bool Tanshen;

	public FootTextureType FootTexture = FootTextureType.Road;

	public FootTextureType LastFootTexture = FootTextureType.Ground;

	private Coroutine m_TickThree;

	private Coroutine m_HalfScondTick;

	private Transform[] m_RootBones;

	private GameObject m_RootBone;

	private Dictionary<string, Transform> m_NameToBone = new Dictionary<string, Transform>();

	public GameObject DollBones;

	[SerializeField]
	public HoldIK m_HoldIK;

	[SerializeField]
	public HoldIK m_RightHandHoldIK;

	[SerializeField]
	protected HeadTrackIK m_HeadIK;

	[SerializeField]
	protected AimingIK m_AimingIK;

	public bool NeedHeadIK;

	public bool NeedAimIK;

	public bool NeedHoldIK;

	[NonSerialized]
	public Vector3 LookAtPosition;

	protected Vector2 m_InputVector;

	public float m_Yaw;

	protected static int s_HorizontalInputHash = Animator.StringToHash("Horizontal Input");

	protected static int s_ForwardInputHash = Animator.StringToHash("Forward Input");

	protected static int s_YInputHash = Animator.StringToHash("YInput");

	protected static int s_YawHash = Animator.StringToHash("Yaw");

	protected static Dictionary<string, int> s_StateNamesHash = new Dictionary<string, int>();

	private static string[] s_LayerNames;

	public int BaseLayer;

	public int UpperBodyLayer;

	public int FullBodyLayer;

	public int LeftArmLayer;

	public int RightArmLayer;

	private readonly Dictionary<int, string> m_Layer2StateName = new Dictionary<int, string>();

	public Dictionary<int, int> Layer2aimHashDic = new Dictionary<int, int>();

	[SerializeField]
	public CharacterSetSkin Skin;

	protected HashSet<int> m_clothes = new HashSet<int>();

	protected bool m_haveBag;

	protected bool m_Visible;

	private bool m_HaveJili;

	private bool m_HaveTaoZhuang;

	protected Dictionary<int, Weapon> m_Weapons = new Dictionary<int, Weapon>();

	protected int m_CurrentWeaponInsId = -1;

	protected bool m_ShowAllWeapon = true;

	protected bool m_ShowOnlyCurrentWeapon;

	protected bool m_HideAllParts;

	[NonSerialized]
	public BaseGun GunInHand;

	private List<int> m_AlwaysShowGunIdList = new List<int> { 2008, 2009 };

	private static readonly WaitForSeconds Wait03 = new WaitForSeconds(3f);

	public GameObject WudiEffect;

	private static readonly WaitForSeconds Wait05 = new WaitForSeconds(0.5f);

	public PlayerInfo PlayerInfo
	{
		get
		{
			return m_PlayerInfo;
		}
	}

	public Vector3 NameLabelPos
	{
		get
		{
			if (HeadNameLabel != null)
			{
				return Battle.Ins.MainCamera.Camera.WorldToScreenPoint(HeadNameLabel.position);
			}
			return Vector3.zero;
		}
	}

	public bool InCar
	{
		get
		{
			return VehicleId > 0;
		}
	}

	public bool SanOpened
	{
		get
		{
			return m_SanOpened;
		}
		set
		{
			m_SanOpened = value;
		}
	}

	public bool SanDestroyed
	{
		get
		{
			return m_SanDestroyed;
		}
		set
		{
			m_SanDestroyed = value;
		}
	}

	public bool Landed
	{
		get
		{
			return m_Landed;
		}
	}

	public Vector3 Pos
	{
		get
		{
			return PlayerTransform.position;
		}
	}

	public int GetHp
	{
		get
		{
			return HP;
		}
	}

	public Vector2 InputVector
	{
		get
		{
			return m_InputVector;
		}
		set
		{
			m_InputVector = value;
		}
	}

	public bool IsKongshou
	{
		get
		{
			return m_CurrentWeaponInsId == -1;
		}
	}

	public bool HaveGunInHand
	{
		get
		{
			return GunInHand != null;
		}
	}

	public bool HaveNearWeaponInHand
	{
		get
		{
			return GetCurrentWeapon() is NearWeapon;
		}
	}

	public bool HaveThrowWeaponInHand
	{
		get
		{
			return GetCurrentWeapon() is ThrowWeapon;
		}
	}

	public int CurrentWeaponInsId
	{
		get
		{
			return m_CurrentWeaponInsId;
		}
	}

	public virtual void Init()
	{
		m_HaveJili = false;
		SaveBones();
		m_TickThree = Utils.StartConroutine(TickThree());
		m_HalfScondTick = Utils.StartConroutine(HalfSecondTick());
	}

	public virtual void Reset()
	{
		m_Landed = false;
		FootTexture = FootTextureType.Road;
		LastFootTexture = FootTextureType.Ground;
		m_clothes.Clear();
		m_HaveJili = false;
		m_haveBag = false;
		Skin.DestroyAllSkin();
		DestoryAllWeapon();
		m_CurrentWeaponInsId = -1;
		m_ShowAllWeapon = true;
		m_ShowOnlyCurrentWeapon = false;
		m_HideAllParts = false;
		GunInHand = null;
	}

	public void DestoryAllWeapon()
	{
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			weapon.Value.Destroy();
		}
		m_Weapons.Clear();
	}

	public virtual void InitPrototyle()
	{
		PlayerTransform = base.transform;
		PlayerAnimator = GetComponent<Animator>();
		m_SkinnedMeshRenderer = base.gameObject.AddComponent<SkinnedMeshRenderer>();
		Bounds localBounds = new Bounds(new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 1f));
		m_SkinnedMeshRenderer.localBounds = localBounds;
		PlayerCollider = GameObject.Find(base.gameObject.name + "/Collider").GetComponent<CapsuleCollider>();
		Skin = GetComponent<CharacterSetSkin>();
		SoundControll = GetComponent<SoundControll>();
		m_AimingIK = GetComponent<AimingIK>();
		m_HeadIK = GetComponent<HeadTrackIK>();
		m_HoldIK = GetComponent<HoldIK>();
		InitLayerIndex();
		InitBodyParts();
		InitGuaDian();
		InitAimationStates();
		InitIK();
	}

	protected void InitGuaDian()
	{
		m_GuaDianL1 = Utils.GetChildObjByName("wuqi_guadian_L_1", base.gameObject).transform;
		m_GuaDianL2 = Utils.GetChildObjByName("wuqi_guadian_L_2", base.gameObject).transform;
		m_GuaDianR1 = Utils.GetChildObjByName("wuqi_guadian_R_1", base.gameObject).transform;
		m_GuaDianR2 = Utils.GetChildObjByName("wuqi_guadian_R_2", base.gameObject).transform;
		m_BagGuaDian = Utils.GetChildObjByName("tongyong_bag", base.gameObject).transform;
		RightHandGuaDian = Utils.GetChildObjByName("wuqi_guadian", base.gameObject).transform;
		m_LeftHandGuaDian = Utils.GetChildObjByName("wuqi_guadian002", base.gameObject).transform;
		m_ShouQiangGuaDian = Utils.GetChildObjByName("shouqiang_guadian", base.gameObject).transform;
		m_NearWeaponGuaDian = Utils.GetChildObjByName("jinzhan_guadian", base.gameObject).transform;
	}

	private void InitBodyParts()
	{
		m_BodyParts = new Transform[18];
		m_BodyParts[9] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 Head").transform;
		m_BodyParts[8] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2").transform;
		m_BodyParts[0] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm").transform;
		m_BodyParts[1] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm").transform;
		m_BodyParts[2] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm").transform;
		m_BodyParts[3] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm").transform;
		m_BodyParts[5] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 R Thigh").transform;
		m_BodyParts[7] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 R Thigh/Bip001 R Calf").transform;
		m_BodyParts[6] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 L Thigh/Bip001 L Calf").transform;
		m_BodyParts[4] = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 L Thigh").transform;
		NeckBone = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck").transform;
		m_BodyParts[16] = NeckBone;
		RightJianbang = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle").transform;
		LeftFoot = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 L Thigh/Bip001 L Calf/Bip001 L Foot/Bip001 L Toe0").transform;
		m_BodyParts[12] = LeftFoot;
		RightFoot = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Pelvis/Bip001 R Thigh/Bip001 R Calf/Bip001 R Foot/Bip001 R Toe0").transform;
		m_BodyParts[13] = RightFoot;
		HeadTop = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 Head/Head").transform;
		m_BodyParts[10] = HeadTop;
		HeadBub = GameObject.Find(base.gameObject.name + "/Bip001/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 Head/Bip001 HeadNub").transform;
		m_BodyParts[17] = HeadTop;
		RootBone = GameObject.Find(base.gameObject.name + "/Bip001").transform;
		m_BodyParts[11] = RootBone;
		RightHand = GetChildObjByName("Bip001 R Hand").transform;
		LeftHand = GetChildObjByName("Bip001 L Hand").transform;
		HeadNameLabel = GetChildObjByName("HeadNameLabel").transform;
		m_BodyParts[14] = RightHand;
		m_BodyParts[15] = LeftHand;
	}

	public void UpdateCapsule()
	{
		Vector3 vector = (RightFoot.position + LeftFoot.position) * 0.5f;
		PlayerCollider.transform.rotation = Quaternion.FromToRotation(Vector3.up, HeadTop.position - vector);
		PlayerCollider.height = (HeadTop.position - vector).magnitude + 0.1f;
		PlayerCollider.center = Vector3.zero;
		PlayerCollider.transform.localPosition = (base.transform.InverseTransformPoint(HeadTop.position) + base.transform.InverseTransformPoint(vector)) * 0.5f;
		PlayerCollider.direction = 1;
	}

	public Transform GetBodyPart(BodyPart bodyPart)
	{
		return m_BodyParts[(int)bodyPart];
	}

	protected GameObject GetChildObjByName(string childName)
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == childName)
			{
				return componentsInChildren[i].gameObject;
			}
		}
		return null;
	}

	public virtual void SetAnimatorSpeed(float speed)
	{
		PlayerAnimator.speed = speed;
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void LateUpdate()
	{
		UpdateIK();
	}

	public void SetHP(int num)
	{
		int hP = HP;
		HP = num;
		OnHPChanged(HP - hP, RoleId);
	}

	private void CheckSanBag()
	{
		if (m_San != null && m_San.activeSelf)
		{
			Skin.HideSkin(23);
			Skin.HideSkin(31);
		}
		if (m_SanBag != null && m_SanBag.activeSelf)
		{
			Skin.HideSkin(23);
			Skin.HideSkin(31);
		}
	}

	public void SetHPStatus(bool isSecond)
	{
		if (isSecond)
		{
			IsDownWaitSave = true;
		}
		else
		{
			IsDownWaitSave = false;
		}
	}

	protected virtual void OnHPChanged(int changNum, long roleId)
	{
	}

	public void SetEP(int num)
	{
		EP = num;
		OnEPChanged();
	}

	protected virtual void OnEPChanged()
	{
	}

	public void SaveBones()
	{
		m_RootBone = Utils.GetChildObjByName("Bip001", base.gameObject);
		m_RootBones = m_RootBone.GetComponentsInChildren<Transform>(true);
		Transform[] rootBones = m_RootBones;
		foreach (Transform transform in rootBones)
		{
			if (!m_NameToBone.ContainsKey(transform.name))
			{
				m_NameToBone.Add(transform.name, transform);
			}
		}
	}

	public void ResetBones()
	{
		Skin.SetBones(m_RootBone.transform, m_RootBones);
	}

	public void PlayerDie(SPlayerDie msg)
	{
		_003CPlayerDie_003Ec__AnonStorey2 _003CPlayerDie_003Ec__AnonStorey = new _003CPlayerDie_003Ec__AnonStorey2();
		_003CPlayerDie_003Ec__AnonStorey.msg = msg;
		_003CPlayerDie_003Ec__AnonStorey._0024this = this;
		SetHP(0);
		IsDie = true;
		CloseAllIK();
		EffectInfo[] componentsInChildren = base.gameObject.GetComponentsInChildren<EffectInfo>();
		EffectInfo[] array = componentsInChildren;
		foreach (EffectInfo effectInfo in array)
		{
			effectInfo.gameObject.SetActive(false);
		}
		if (_003CPlayerDie_003Ec__AnonStorey.msg.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			Battle.Ins.SelfPlayer.PlayerRigidbody.useGravity = false;
			Battle.Ins.SelfPlayer.LockRigidBodyXYZ();
			Battle.Ins.AimHelp.Disabled = true;
			Utils.TriggerEvent(BattleEvent.onSelfDie, base.transform.position);
		}
		ResMgr.Ins.CreateFromAB("role/bip001.ab", null, _003CPlayerDie_003Ec__AnonStorey._003C_003Em__0);
		OnDied();
	}

	protected virtual void OnDied()
	{
	}

	public virtual void PlayHitEffect(SHitPlayer msg)
	{
		SingletonMono<EffectMgr>.Ins.PlayEffectAtPos("effect/dk_jianxue_xiao.ab", GetBodyPart((BodyPart)msg.bodyPart).transform, MathUtils.Short2Float(new Vector3(msg.hitPos.x, msg.hitPos.y, msg.hitPos.z)), MathUtils.Short2Float(new Vector3(msg.forward.x, msg.forward.y, msg.forward.z)));
	}

	public void SetPulmonary(float num)
	{
		Pulmonary = (int)num;
		Pulmonary = Mathf.Clamp(Pulmonary, 0, 100);
	}

	public void CloseAllIK()
	{
		NeedHeadIK = false;
		NeedAimIK = false;
		NeedHoldIK = false;
	}

	public void OpenAllIK()
	{
		NeedHeadIK = true;
		NeedAimIK = true;
		NeedHoldIK = true;
	}

	private void UpdateIK()
	{
		if (NeedHeadIK)
		{
			m_HeadIK.LookAt(LookAtPosition, 0.5f, 0.5f);
		}
		if (RoleId == Singleton<RoleMgr>.Ins.info.roleId && GunInHand != null && GunInHand.WeaponObject != null)
		{
			if (NeedAimIK && (bool)GunInHand.GunInfo.CameraPos)
			{
				m_AimingIK.SetAimPosition(LookAtPosition, GunInHand.GunInfo.CameraPos);
			}
			if (NeedHoldIK)
			{
				m_HoldIK.SetIKWeight(1f);
				m_HoldIK.SetIKPosition(GunInHand.GunInfo.LeftHandPos.position);
			}
		}
	}

	public virtual void SetAnimatorWeight(int layer, float weight)
	{
		PlayerAnimator.SetLayerWeight(layer, weight);
	}

	public bool ChangeAnimatorStates(int layer, string targetStateName, bool allowReplay = false)
	{
		if (string.IsNullOrEmpty(targetStateName))
		{
			return false;
		}
		if (CompareAndSetLayerStateName(layer, targetStateName) && !allowReplay)
		{
			return false;
		}
		int stateNameHash = GetStateNameHash(s_LayerNames[layer] + targetStateName);
		if (PlayerAnimator != null)
		{
			PlayerAnimator.CrossFade(stateNameHash, 0.2f, layer);
		}
		OnChangeAnimatorState(layer, targetStateName, stateNameHash);
		return true;
	}

	public bool FastChangeAnimatorStates(int layer, string targetStateName, bool allowReplay = false)
	{
		if (string.IsNullOrEmpty(targetStateName))
		{
			return false;
		}
		if (CompareAndSetLayerStateName(layer, targetStateName) && !allowReplay)
		{
			return false;
		}
		int stateNameHash = GetStateNameHash(s_LayerNames[layer] + targetStateName);
		if (PlayerAnimator != null)
		{
			PlayerAnimator.Play(stateNameHash, layer, 0f);
		}
		OnChangeAnimatorState(layer, targetStateName, stateNameHash);
		return true;
	}

	public bool FixedChangeAnimatorStates(int layer, string targetStateName, float time = 0.25f, bool allowReplay = false)
	{
		if (string.IsNullOrEmpty(targetStateName))
		{
			return false;
		}
		if (CompareAndSetLayerStateName(layer, targetStateName) && !allowReplay)
		{
			return false;
		}
		int stateNameHash = GetStateNameHash(s_LayerNames[layer] + targetStateName);
		if (PlayerAnimator != null)
		{
			PlayerAnimator.CrossFadeInFixedTime(stateNameHash, time, layer);
		}
		OnChangeAnimatorState(layer, targetStateName, stateNameHash);
		return true;
	}

	protected virtual void OnChangeAnimatorState(int layer, string targetStateName, int stateHash)
	{
	}

	private void InitAimationStates()
	{
		if (s_LayerNames == null)
		{
			s_LayerNames = new string[PlayerAnimator.layerCount];
			int i = 0;
			for (int num = s_LayerNames.Length; i < num; i++)
			{
				s_LayerNames[i] = PlayerAnimator.GetLayerName(i) + ".";
			}
		}
	}

	private void InitIK()
	{
		m_HoldIK = base.gameObject.AddComponent<HoldIK>();
		m_RightHandHoldIK = base.gameObject.AddComponent<HoldIK>();
		m_HoldIK.Init(GetChildObjByName("Bip001 L UpperArm").transform, GetChildObjByName("Bip001 L ForeTwist").transform, GetChildObjByName("Bip001 L Forearm").transform, GetChildObjByName("Bip001 L Hand").transform);
		m_RightHandHoldIK.Init(GetChildObjByName("Bip001 R UpperArm").transform, GetChildObjByName("Bip001 R ForeTwist").transform, GetChildObjByName("Bip001 R Forearm").transform, GetChildObjByName("Bip001 R Hand").transform);
		m_HeadIK = base.gameObject.AddComponent<HeadTrackIK>();
		m_HeadIK.Init(PlayerTransform, GetChildObjByName("Bip001 Head").transform, new List<Transform>
		{
			GetChildObjByName("Bip001 Spine").transform,
			GetChildObjByName("Bip001 Spine1").transform,
			GetChildObjByName("Bip001 Spine2").transform,
			GetChildObjByName("Bip001 Spine").transform
		});
		m_AimingIK = base.gameObject.AddComponent<AimingIK>();
		m_AimingIK.Init(GetChildObjByName("Bip001 R UpperArm").transform, GetChildObjByName("Bip001 R Hand").transform);
	}

	private bool CompareAndSetLayerStateName(int layer, string stateName)
	{
		string value;
		if (m_Layer2StateName.TryGetValue(layer, out value))
		{
			if (stateName == value)
			{
				return true;
			}
			m_Layer2StateName[layer] = stateName;
			return false;
		}
		m_Layer2StateName.Add(layer, stateName);
		return false;
	}

	protected int GetStateNameHash(string stateName)
	{
		int value;
		if (!s_StateNamesHash.TryGetValue(stateName, out value))
		{
			value = Animator.StringToHash(stateName);
			s_StateNamesHash.Add(stateName, value);
		}
		return value;
	}

	public void SetAnimatorHorizontalValue(float value)
	{
		PlayerAnimator.SetFloat(s_HorizontalInputHash, value, 0.12f, Time.deltaTime);
	}

	public void SetAnimatorForwardValue(float value, float time = 0.12f)
	{
		PlayerAnimator.SetFloat(s_ForwardInputHash, value, time, Time.deltaTime);
		if (RoleHandAnimator != null)
		{
			RoleHandAnimator.SetFloat(s_ForwardInputHash, value, time, Time.deltaTime);
		}
	}

	public float GetAnimatorForwardValue()
	{
		return PlayerAnimator.GetFloat(s_ForwardInputHash);
	}

	public void SetAnimatorYValue(float value)
	{
		PlayerAnimator.SetFloat(s_YInputHash, value, 0.05f, Time.deltaTime);
	}

	public void SetLayerWeight(int layer, float value)
	{
		PlayerAnimator.SetLayerWeight(layer, value);
	}

	public void PlayerAnimation(int layer, string aimName)
	{
		int stateNameHash = GetStateNameHash(s_LayerNames[layer] + aimName);
		PlayerAnimation(layer, stateNameHash);
	}

	public void PlayerAnimation(int layer, int aniHash)
	{
		Layer2aimHashDic[layer] = aniHash;
		if (PlayerAnimator.enabled)
		{
			PlayerAnimator.CrossFade(aniHash, 0.3f, layer, 0f);
		}
	}

	public bool IsPlayFinish(string aniName)
	{
		return IsUpBodyLayerPlayFinish(aniName) || IsBaseLayerPlayFinish(aniName) || IsFullLayerPlayFinish(aniName);
	}

	public bool IsUpBodyLayerPlayFinish(string aniName)
	{
		AnimatorStateInfo currentAnimatorStateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(UpperBodyLayer);
		return currentAnimatorStateInfo.IsName(aniName) && currentAnimatorStateInfo.normalizedTime >= 1f;
	}

	public bool IsBaseLayerPlayFinish(string aniName)
	{
		AnimatorStateInfo currentAnimatorStateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(BaseLayer);
		return currentAnimatorStateInfo.IsName(aniName) && currentAnimatorStateInfo.normalizedTime >= 1f;
	}

	public bool IsFullLayerPlayFinish(string aniName)
	{
		AnimatorStateInfo currentAnimatorStateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(FullBodyLayer);
		return currentAnimatorStateInfo.IsName(aniName) && currentAnimatorStateInfo.normalizedTime >= 1f;
	}

	private void InitLayerIndex()
	{
		BaseLayer = PlayerAnimator.GetLayerIndex("Base Layer");
		UpperBodyLayer = PlayerAnimator.GetLayerIndex("UpperBody Layer");
		FullBodyLayer = PlayerAnimator.GetLayerIndex("FullBody Layer");
		LeftArmLayer = PlayerAnimator.GetLayerIndex("LeftArm Layer");
		RightArmLayer = PlayerAnimator.GetLayerIndex("RightArm Layer");
	}

	public ItemCfg GetClothType(int type)
	{
		foreach (int clothe in m_clothes)
		{
			ItemCfg itemCfg = ItemCfg.Get(clothe);
			if (itemCfg == null)
			{
				Debug.LogError(clothe);
				return null;
			}
			if (itemCfg.type == type)
			{
				return itemCfg;
			}
		}
		return null;
	}

	public int GetClothHp(int clothId)
	{
		return 0;
	}

	public void PutOnAllClothes(HashSet<int> clothesSet)
	{
		m_clothes = new HashSet<int>(clothesSet);
		if (m_clothes.Contains(49))
		{
			m_HaveJili = true;
		}
		foreach (int clothe in m_clothes)
		{
			ItemCfg itemCfg = ItemCfg.Get(clothe);
			if (itemCfg != null)
			{
				_PutOnCloth(clothe, itemCfg);
			}
		}
	}

	public void PutOffAllClothes()
	{
		int[] array = new int[25];
		m_clothes.CopyTo(array);
		for (int i = 0; i < array.Length; i++)
		{
			PutOffCloth(array[i]);
		}
	}

	public void PutOnCloth(int itemId)
	{
		if (m_clothes.Contains(itemId))
		{
			return;
		}
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg == null)
		{
			return;
		}
		int num = -1;
		foreach (int clothe in m_clothes)
		{
			ItemCfg itemCfg2 = ItemCfg.Get(clothe);
			if (itemCfg2 != null && itemCfg2.childType == itemCfg.childType)
			{
				num = clothe;
				break;
			}
		}
		if (num >= 0)
		{
			PutOffCloth(num);
		}
		m_clothes.Add(itemId);
		ClothChanged();
		_PutOnCloth(itemId, itemCfg);
	}

	public virtual void ClothChanged()
	{
	}

	public void PutOffCloth(int itemId)
	{
		if (!m_clothes.Remove(itemId))
		{
			return;
		}
		ClothChanged();
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		GameObject gameObject = Skin.RemoveCombineSkin(itemCfg.childType);
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		if (itemCfg.childType == 23)
		{
			gameObject = Skin.RemoveCombineSkin(31);
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			SetHaveBag(false);
		}
		else if (itemCfg.childType == 33)
		{
			if (!m_HaveJili)
			{
				Skin.ShowSkin(16);
			}
		}
		else if (itemCfg.childType == 44)
		{
			m_HaveJili = false;
			PutOnAllClothes(m_clothes);
			if (Skin.ContainSkin(33))
			{
				Skin.HideSkin(16);
			}
		}
	}

	public void OnClothHpChanged(int itemId, int hp)
	{
		if (!m_clothes.Contains(itemId))
		{
		}
	}

	private void _PutOnCloth(int clothId, ItemCfg clothCfg)
	{
		_003C_PutOnCloth_003Ec__AnonStorey3 _003C_PutOnCloth_003Ec__AnonStorey = new _003C_PutOnCloth_003Ec__AnonStorey3();
		_003C_PutOnCloth_003Ec__AnonStorey.clothCfg = clothCfg;
		_003C_PutOnCloth_003Ec__AnonStorey._0024this = this;
		if (m_HaveJili && clothId != 49 && _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType != 23)
		{
			return;
		}
		if (clothId == 49)
		{
			m_HaveJili = true;
		}
		if (_003C_PutOnCloth_003Ec__AnonStorey.clothCfg == null)
		{
			Debug.LogError(string.Format("[BasePlayerController]unknown cloth id:{0} when _PutOnCloth.", clothId));
			return;
		}
		if (_003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType == 41 || _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType == 47 || _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType == 46 || _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType == 45)
		{
			m_HaveTaoZhuang = true;
		}
		ResMgr.Ins.CreateFromAB((!m_PlayerInfo.sex) ? _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.womanModelPath : _003C_PutOnCloth_003Ec__AnonStorey.clothCfg.modelPath, null, _003C_PutOnCloth_003Ec__AnonStorey._003C_003Em__0);
		if (_003C_PutOnCloth_003Ec__AnonStorey.clothCfg.childType == 23)
		{
			_003C_PutOnCloth_003Ec__AnonStorey4 _003C_PutOnCloth_003Ec__AnonStorey2 = new _003C_PutOnCloth_003Ec__AnonStorey4();
			_003C_PutOnCloth_003Ec__AnonStorey2._003C_003Ef__ref_00243 = _003C_PutOnCloth_003Ec__AnonStorey;
			_003C_PutOnCloth_003Ec__AnonStorey2.beidaiCfg = ItemCfg.Get(clothId + 1);
			if (_003C_PutOnCloth_003Ec__AnonStorey2.beidaiCfg != null && _003C_PutOnCloth_003Ec__AnonStorey2.beidaiCfg.childType == 31)
			{
				ResMgr.Ins.CreateFromAB((!m_PlayerInfo.sex) ? _003C_PutOnCloth_003Ec__AnonStorey2.beidaiCfg.womanModelPath : _003C_PutOnCloth_003Ec__AnonStorey2.beidaiCfg.modelPath, null, _003C_PutOnCloth_003Ec__AnonStorey2._003C_003Em__0);
			}
		}
	}

	protected void SetHaveBag(bool haveBag)
	{
		if (m_haveBag != haveBag)
		{
			m_haveBag = haveBag;
		}
	}

	public int GetCurrentWeaponInsId()
	{
		return m_CurrentWeaponInsId;
	}

	public Weapon GetCurrentWeapon()
	{
		return GetWeapon(m_CurrentWeaponInsId);
	}

	public BaseGun GetCurrentGun()
	{
		return GetGun(m_CurrentWeaponInsId);
	}

	public Weapon GetWeapon(int insId)
	{
		Weapon value;
		m_Weapons.TryGetValue(insId, out value);
		return value;
	}

	public BaseGun GetGun(int pos)
	{
		return GetWeapon(pos) as BaseGun;
	}

	public void AddWeapon(int insId, Weapon weapon)
	{
		if (!m_Weapons.ContainsKey(insId))
		{
			m_Weapons.Add(insId, weapon);
			weapon.InsId = insId;
			UpdateWeaponVisible(insId, weapon);
		}
	}

	public void RemoveWeapon(int insId)
	{
		Weapon weapon = GetWeapon(insId);
		if (weapon != null)
		{
			m_Weapons.Remove(insId);
			weapon.Destroy();
		}
		if (m_CurrentWeaponInsId == insId)
		{
			GunInHand = null;
		}
	}

	public virtual void ChangeHandWeapon(int insId)
	{
		int currentWeaponInsId = m_CurrentWeaponInsId;
		m_CurrentWeaponInsId = insId;
		HideAllWeapon();
		GuaWuqiInHand(m_CurrentWeaponInsId);
		GunInHand = GetGun(m_CurrentWeaponInsId);
		if (GunInHand != null)
		{
			GunInHand.Visible = true;
		}
	}

	public void HideWeapon(int insId)
	{
		Weapon weapon = GetWeapon(insId);
		if (weapon != null)
		{
			weapon.Visible = false;
		}
	}

	public void AddGunPart(int insId, int partId)
	{
		BaseGun gun = GetGun(insId);
		if (gun != null)
		{
			gun.AddPart(partId);
		}
	}

	public void RemoveGunPart(int insId, int partId)
	{
		BaseGun gun = GetGun(insId);
		if (gun != null)
		{
			gun.RemovePart(partId);
		}
	}

	protected Transform GetWuqiTransform(int insId)
	{
		Weapon weapon = GetWeapon(insId);
		return (weapon == null || !(weapon.WeaponObject != null)) ? null : weapon.WeaponObject.transform;
	}

	public void GuaWuqiInHand(int InsId)
	{
		Transform wuqiTransform = GetWuqiTransform(InsId);
		if (!(wuqiTransform == null))
		{
			wuqiTransform.localEulerAngles = Vector3.zero;
			wuqiTransform.localPosition = Vector3.zero;
			if (InsId == m_CurrentWeaponInsId)
			{
				wuqiTransform.SetParent(RightHandGuaDian, false);
				wuqiTransform.gameObject.SetActiveBetter(true);
			}
		}
	}

	public void HideWeaponExceptCurrent()
	{
		m_ShowAllWeapon = false;
		m_ShowOnlyCurrentWeapon = true;
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			if (weapon.Key != m_CurrentWeaponInsId)
			{
				weapon.Value.Visible = false;
			}
		}
	}

	public void ShowAllWeapon()
	{
		m_ShowAllWeapon = true;
		m_ShowOnlyCurrentWeapon = false;
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			weapon.Value.Visible = true;
			if (weapon.Value is BaseGun)
			{
				(weapon.Value as BaseGun).PartVisible = !m_HideAllParts;
			}
		}
	}

	public void HideAllWeapon()
	{
		m_ShowAllWeapon = false;
		m_ShowOnlyCurrentWeapon = false;
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			if (!(weapon.Value is BaseGun) || !m_AlwaysShowGunIdList.Contains((weapon.Value as BaseGun).GunCfg.id))
			{
				weapon.Value.Visible = false;
			}
		}
	}

	public void ShowAllGunParts()
	{
		if (!m_HideAllParts)
		{
			return;
		}
		m_HideAllParts = false;
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			if (weapon.Value is BaseGun)
			{
				(weapon.Value as BaseGun).PartVisible = true;
			}
		}
	}

	public void HideAllGunParts()
	{
		if (m_HideAllParts)
		{
			return;
		}
		m_HideAllParts = true;
		foreach (KeyValuePair<int, Weapon> weapon in m_Weapons)
		{
			if (weapon.Value is BaseGun)
			{
				(weapon.Value as BaseGun).PartVisible = false;
			}
		}
	}

	private void UpdateWeaponVisible(int insId, Weapon weapon)
	{
		weapon.Visible = m_ShowAllWeapon || (m_ShowOnlyCurrentWeapon && m_CurrentWeaponInsId == insId);
	}

	public virtual void OnGetInCar(int vehicleId)
	{
		PlayerCollider.enabled = false;
		VehicleId = vehicleId;
	}

	public virtual void OnGetOutCar()
	{
		PlayerCollider.enabled = true;
		VehicleId = 0;
	}

	protected IEnumerator TickThree()
	{
		while (true)
		{
			yield return Wait03;
			if (m_SkinnedMeshRenderer != null && !m_SkinnedMeshRenderer.isVisible && RoleId != Singleton<RoleMgr>.Ins.info.roleId && PlayerAnimator != null && !Battle.Ins.NearOtherPlayersDic.ContainsKey(RoleId))
			{
				PlayerAnimator.enabled = false;
			}
		}
	}

	public virtual void SRebirth(PlayerInfo info)
	{
		EffectInfo[] componentsInChildren = base.gameObject.GetComponentsInChildren<EffectInfo>();
		EffectInfo[] array = componentsInChildren;
		foreach (EffectInfo effectInfo in array)
		{
			effectInfo.gameObject.SetActive(false);
		}
		IsDie = false;
	}

	private void LoadWuDiEffect()
	{
		if (WudiEffect == null)
		{
			ResMgr.Ins.CreateFromAB("effect/wudi_01.ab", null, _003CLoadWuDiEffect_003Em__0);
		}
	}

	private IEnumerator HalfSecondTick()
	{
		while (true)
		{
			CheckSanBag();
			yield return Wait05;
		}
	}

	private void OnDestroy()
	{
		Utils.StopConroutine(m_TickThree);
		Utils.StopConroutine(m_HalfScondTick);
		StopAllCoroutines();
	}

	public void ShowTuzhi()
	{
		if (TuzhiGo == null)
		{
			TuzhiGo = UnityEngine.Object.Instantiate(Battle.Ins.TuZhiGo, RightHandGuaDian);
			TuzhiGo.transform.localPosition = Vector3.zero;
		}
		TuzhiGo.SetActive(true);
	}

	public void HideTuzhi()
	{
		if (TuzhiGo != null)
		{
			TuzhiGo.SetActive(false);
		}
	}

	[CompilerGenerated]
	private void _003CLoadWuDiEffect_003Em__0(GameObject go)
	{
		if (IsDie)
		{
			UnityEngine.Object.Destroy(go);
			return;
		}
		WudiEffect = go;
		WudiEffect.transform.SetParent(RootBone.transform);
		WudiEffect.transform.localPosition = Vector3.zero;
		WudiEffect.transform.localEulerAngles = Vector3.zero;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.battle.monster.scmsg;
using gs.battle.scmsg;

public class MonsterController : MapObject
{
	[CompilerGenerated]
	private sealed class _003CPutOnCloth_003Ec__AnonStorey3
	{
		internal ItemCfg clothCfg;

		internal MonsterController _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (_0024this.gameObject == null)
			{
				return;
			}
			GameObject childObjByName = Utils.GetChildObjByName("Bip001", o);
			if (childObjByName != null)
			{
				UnityEngine.Object.Destroy(childObjByName);
			}
			SkinnedMeshRenderer componentInChildren = o.GetComponentInChildren<SkinnedMeshRenderer>();
			_0024this.m_renders.Add(componentInChildren);
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
				if (componentInChildren != null)
				{
					componentInChildren.quality = SkinQuality.Bone1;
				}
				_0024this.Skin.SetSkin(o, clothCfg.childType);
			}
			else
			{
				_0024this.Skin.SetSkinAttach(clothCfg.childType, clothCfg.root, o);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CHumanMonsterDie_003Ec__AnonStorey4
	{
		internal SMonsterDie msg;

		internal MonsterController _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!_0024this.IsDie)
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			o.gameObject.SetLayerRecursively(BattleScMgr.OtherPlayerColliderLayer);
			o.gameObject.name = "Bip001";
			Transform[] componentsInChildren = o.GetComponentsInChildren<Transform>(true);
			if (_0024this.MyAnimator != null)
			{
				_0024this.MyAnimator.enabled = false;
			}
			if (_0024this.Skin != null)
			{
				_0024this.Skin.SetBones(o.gameObject.transform, componentsInChildren);
			}
			Singleton<BattleScMgr>.Ins.AddForceDie(new Vector3(msg.forward.x, msg.forward.y, msg.forward.z), msg.bodyPart, o.gameObject);
			if ((bool)_0024this.MyCapsuleCollider)
			{
				_0024this.MyCapsuleCollider.gameObject.SetActive(false);
			}
		}
	}

	private static readonly CSyncMonsterPos cSyncMonsterPos = new CSyncMonsterPos();

	private static readonly CSyncMonsterVelocity cSyncMonsterVeloctity = new CSyncMonsterVelocity();

	private static readonly CSyncMonsterAnimator cSyncMonsterAnimator = new CSyncMonsterAnimator();

	private static readonly CSyncMonsterOrientation cSyncMonsterOrientation = new CSyncMonsterOrientation();

	private static int s_HorizontalInputHash = Animator.StringToHash("Horizontal Input");

	private static int s_ForwardInputHash = Animator.StringToHash("Forward Input");

	public Animator MyAnimator;

	private Coroutine m_syncTick;

	private Coroutine m_Tick2;

	private Coroutine m_Tick1;

	public GameObject Target;

	private GameObject m_lastTarget;

	public BasePlayerController TargetController;

	public MonsterInfo MyMonsterInfo;

	public Rigidbody Rig;

	public Vector3 BirthPos;

	public float FreeMoveRange;

	public float DefenceRange;

	public float FollowRange;

	public float AttackRange;

	public float DisToBirthPos;

	public float DisToTarget;

	public float DisToMySelf = float.MaxValue;

	public float walkSpeed;

	public float runSpeed;

	public float MoveSpeed;

	public float turnSpeed;

	public MonsterCfg MyCfg;

	public HumanMonsterCfg HumanCfg;

	public GunCfg MyGunCfg;

	public GunInfo MyGunInfo;

	public bool IsDie;

	public FSMSystem FSM;

	public int Id;

	public bool IsMyControl;

	public bool CanMove;

	public bool HasOwner;

	public CharacterSetSkin Skin;

	public Transform RightHandGuaDian;

	public float NextAttackTime;

	public GameObject EyeGo;

	public Transform MyCapsuleCollider;

	public float RevengeTime;

	public bool NeedAttackBack;

	public bool NeedAttackShooter;

	private List<Renderer> m_renders;

	private static Collider[] m_Players = new Collider[20];

	private static Collider[] m_BackAttackDisPlayers = new Collider[30];

	private ActiveAttackStrategy m_activeAttackStrategy;

	private AttackBackStrategy m_attackBackStrategy;

	private AttackShooterStrategy m_attackShooterStrategy;

	private static readonly WaitForSeconds Wait02 = new WaitForSeconds(0.2f);

	private Vector3 m_OldEulerAngles;

	private Vector3 m_OldPos;

	private bool m_Moving;

	private Vector3 m_TargetPos;

	private Vector3 m_Velocity;

	private bool m_Rotating;

	private Quaternion m_TargetRotation;

	public Dictionary<int, int> Layer2aimHashDic = new Dictionary<int, int>();

	private readonly Dictionary<int, string> m_Layer2StateName = new Dictionary<int, string>();

	protected static Dictionary<string, int> s_StateNamesHash = new Dictionary<string, int>();

	private string[] s_LayerNames;

	private int m_Lod = -1;

	private GameObject m_LowModel;

	protected override void Awake()
	{
		base.Awake();
		m_renders = new List<Renderer>();
		MyAnimator = GetComponent<Animator>();
		MyCapsuleCollider = base.transform.Find("Collider");
		InitAimationStates();
		EyeGo = new GameObject("eye");
		EyeGo.transform.SetParent(base.transform);
		EyeGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);
	}

	public void Init(MonsterInfo monsterInfo)
	{
		MyMonsterInfo = monsterInfo;
		InsId = monsterInfo.instanceId;
		MyCfg = MonsterCfg.Get(monsterInfo.id);
		base.gameObject.name = "Monster " + InsId;
		HumanCfg = HumanMonsterCfg.Get(monsterInfo.id);
		if (HumanCfg != null)
		{
			RightHandGuaDian = Utils.GetChildObjByName("wuqi_guadian", base.gameObject).transform;
			PutOnAllCloth();
			Skin = GetComponent<CharacterSetSkin>();
			LoadWeapon();
			PlayHaveGunAim(false);
			base.gameObject.AddComponent<CommonControllerFunc>();
			if (m_LowModel == null)
			{
				PutOnLowModel();
			}
			m_Lod = -1;
		}
		else
		{
			m_renders = GetComponentsInChildren<Renderer>(true).vToList();
		}
		Id = monsterInfo.id;
		base.transform.position = new Vector3(MyMonsterInfo.pos.x, MyMonsterInfo.pos.y, MyMonsterInfo.pos.z);
		m_Tick2 = Battle.Ins.StartCoroutine(Tick2());
		Invoke("SetGroundPos", 3f);
		Hp = MyMonsterInfo.hp;
		BirthPos = base.transform.position;
	}

	private void PutOnAllCloth()
	{
		foreach (int skinId in HumanCfg.skinIds)
		{
			ItemCfg itemCfg = ItemCfg.Get(skinId);
			if (itemCfg != null)
			{
				PutOnCloth(skinId, itemCfg);
			}
		}
	}

	private void LoadWeapon()
	{
		if (MyCfg.weaponId > 0)
		{
			MyGunCfg = GunCfg.Get(MyCfg.weaponId);
			ItemCfg itemCfg = ItemCfg.Get(MyCfg.weaponId);
			ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadWeapon_003Em__0);
		}
	}

	public void PlayHaveGunAim(bool needSync = true)
	{
		if (MyGunCfg == null)
		{
			return;
		}
		if (MyGunCfg.gunAimType == 5 || MyGunCfg.gunAimType == 3 || MyGunCfg.gunAimType == 6)
		{
			if (needSync)
			{
				ChangeAnimatorStates("Stand.jvjiqiang.f");
			}
			else
			{
				PlayerAnimation(0, "Stand.jvjiqiang.f");
			}
		}
		else if (MyGunCfg.gunAimType == 8)
		{
			if (needSync)
			{
				ChangeAnimatorStates("Stand.rpg.f");
			}
			else
			{
				PlayerAnimation(0, "Stand.rpg.f");
			}
		}
		else if (MyGunCfg.gunAimType == 4)
		{
			if (needSync)
			{
				ChangeAnimatorStates("Stand.sandanqiang.f");
			}
			else
			{
				PlayerAnimation(0, "Stand.sandanqiang.f");
			}
		}
		else if (MyGunCfg.gunAimType == 1)
		{
			if (HumanCfg.isMan)
			{
				if (needSync)
				{
					ChangeAnimatorStates("Stand.shouqiang.f");
				}
				else
				{
					PlayerAnimation(0, "Stand.shouqiang.f");
				}
			}
			else if (needSync)
			{
				ChangeAnimatorStates("Stand.shouqiangWomen.f");
			}
			else
			{
				PlayerAnimation(0, "Stand.shouqiangWomen.f");
			}
		}
		else if (needSync)
		{
			ChangeAnimatorStates("Stand.chongfengqiang.f");
		}
		else
		{
			PlayerAnimation(0, "Stand.chongfengqiang.f");
		}
	}

	public string PlayHumanMonsterFree()
	{
		if (MyGunCfg != null)
		{
			if (MyGunCfg.gunAimType == 5 || MyGunCfg.gunAimType == 3 || MyGunCfg.gunAimType == 6)
			{
				ChangeAnimatorStates("monsterFree.jujiqiang_zhan_free02", 1);
				return "jujiqiang_zhan_free02";
			}
			if (MyGunCfg.gunAimType == 8)
			{
				ChangeAnimatorStates("monsterFree.huojiantong_zhan_free02", 1);
				return "huojiantong_zhan_free02";
			}
			if (MyGunCfg.gunAimType == 4)
			{
				ChangeAnimatorStates("monsterFree.sandanqiang_zhan_free02", 1);
				return "sandanqiang_zhan_free02";
			}
			if (MyGunCfg.gunAimType == 1)
			{
				ChangeAnimatorStates("monsterFree.shouqiang_zhan_free02", 1);
				return "shouqiang_zhan_free02";
			}
			ChangeAnimatorStates("monsterFree.chongfengqiang_zhan_free02", 1);
			return "chongfengqiang_zhan_free02";
		}
		return string.Empty;
	}

	public int PlaySound(int id, float volumePercent)
	{
		SoundCfg soundCfg = SoundCfg.Get(id);
		if (soundCfg == null)
		{
			Debug.LogError("音效id不存在 id===" + id);
			return -1;
		}
		return SingletonMono<AudioManager>.Ins.Play(soundCfg.path, base.transform.position, false, (float)soundCfg.volume * volumePercent * 0.01f, true, 0f, soundCfg.radius);
	}

	private void PutOnCloth(int clothId, ItemCfg clothCfg)
	{
		_003CPutOnCloth_003Ec__AnonStorey3 _003CPutOnCloth_003Ec__AnonStorey = new _003CPutOnCloth_003Ec__AnonStorey3();
		_003CPutOnCloth_003Ec__AnonStorey.clothCfg = clothCfg;
		_003CPutOnCloth_003Ec__AnonStorey._0024this = this;
		if (HumanCfg != null)
		{
			ResMgr.Ins.CreateFromAB((!HumanCfg.isMan) ? _003CPutOnCloth_003Ec__AnonStorey.clothCfg.womanModelPath : _003CPutOnCloth_003Ec__AnonStorey.clothCfg.modelPath, null, _003CPutOnCloth_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private void SetGroundPos()
	{
		BirthPos = Battle.Ins.GetGroundPos(new Vector3(MyMonsterInfo.birthpos.x, MyMonsterInfo.birthpos.y + 1f, MyMonsterInfo.birthpos.z));
		base.transform.position = Battle.Ins.GetGroundPos(new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z));
	}

	public void StartController()
	{
		AttackRange = MyCfg.attackRange;
		FreeMoveRange = MyCfg.freeMoveRange;
		DefenceRange = MyCfg.defenceRange;
		FollowRange = MyCfg.followRange;
		walkSpeed = MyCfg.walkSpeed;
		runSpeed = MyCfg.runSpeed;
		if (HumanCfg != null)
		{
			turnSpeed = 15f;
		}
		else
		{
			turnSpeed = 2.5f;
		}
		if (Rig == null)
		{
			Rig = base.gameObject.AddComponent<Rigidbody>();
			Rig.mass = 1000f;
			Rig.constraints = (RigidbodyConstraints)80;
			if (!MyCfg.canMove)
			{
				Rig.isKinematic = true;
			}
		}
		InitFSM();
		m_syncTick = Battle.Ins.StartCoroutine(SyncTick());
		m_Tick1 = Battle.Ins.StartCoroutine(Tick1());
		IsMyControl = true;
		FindOneAttackTarget();
		Battle.Ins.SelfPlayer.MyMonsterNum++;
		Battle.Ins.SelfPlayer.MyMonsterInsId.Add(InsId);
		BattleEvent.OnShoot = (Utils.LongDelegate)Delegate.Combine(BattleEvent.OnShoot, new Utils.LongDelegate(OnShoot));
	}

	private void OnShoot(long shooterInsId)
	{
		if (!IsMyControl || !(Target == null))
		{
			return;
		}
		BasePlayerController basePlayerController = Battle.Ins.MapObjectDic[shooterInsId] as BasePlayerController;
		if (!(basePlayerController != null))
		{
			return;
		}
		int rangePlayer = GetRangePlayer(MyCfg.zhianDis);
		for (int i = 0; i < rangePlayer; i++)
		{
			if ((bool)(m_Players[i] = basePlayerController.PlayerCollider))
			{
				SetTarget(m_Players[i].gameObject);
				DoAttacShooter();
				break;
			}
		}
	}

	public void StopController()
	{
		Battle.Ins.SelfPlayer.MyMonsterNum--;
		Battle.Ins.SelfPlayer.MyMonsterInsId.Remove(InsId);
		IsMyControl = false;
		if (m_syncTick != null)
		{
			Battle.Ins.StopCoroutine(m_syncTick);
		}
		if (m_Tick1 != null)
		{
			Battle.Ins.StopCoroutine(m_Tick1);
		}
		BattleEvent.OnShoot = (Utils.LongDelegate)Delegate.Remove(BattleEvent.OnShoot, new Utils.LongDelegate(OnShoot));
		UnityEngine.Object.Destroy(Rig);
		if (FSM != null)
		{
			FSM.Clear();
			FSM = null;
		}
	}

	public void FindOneAttackTarget()
	{
		if ((bool)Target || !IsMyControl || MyCfg.type != 2)
		{
			return;
		}
		int num = UpdateDefenceRangePlayer();
		float num2 = float.MaxValue;
		GameObject target = null;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			float num3 = Vector3.Distance(base.transform.position, m_Players[i].transform.position);
			BasePlayerController componentInParent = m_Players[i].gameObject.GetComponentInParent<BasePlayerController>();
			if (num3 < num2 && CanSeeObj(m_Players[i].gameObject) && (bool)componentInParent && !componentInParent.IsDie)
			{
				target = m_Players[i].gameObject;
				num2 = num3;
			}
		}
		SetTarget(target);
	}

	public int UpdateDefenceRangePlayer()
	{
		return Physics.OverlapSphereNonAlloc(base.transform.position, DefenceRange, m_Players, (1 << BattleScMgr.SelfPlayerColliderLayer) | (1 << BattleScMgr.OtherPlayerColliderLayer), QueryTriggerInteraction.Ignore);
	}

	public int GetRangePlayer(int dis)
	{
		return Physics.OverlapSphereNonAlloc(base.transform.position, dis, m_Players, (1 << BattleScMgr.SelfPlayerColliderLayer) | (1 << BattleScMgr.OtherPlayerColliderLayer), QueryTriggerInteraction.Ignore);
	}

	public void SetTarget(GameObject t)
	{
		Target = t;
		if (m_lastTarget != Target)
		{
			TargetChanged();
			m_lastTarget = Target;
			if ((bool)t)
			{
				TargetController = t.GetComponentInParent<BasePlayerController>();
			}
			else
			{
				TargetController = null;
			}
		}
	}

	public int UpdateNearPlayer()
	{
		if (MyCfg.backAttackDis > 0)
		{
			return Physics.OverlapSphereNonAlloc(base.transform.position, MyCfg.backAttackDis, m_BackAttackDisPlayers, (1 << BattleScMgr.SelfPlayerColliderLayer) | (1 << BattleScMgr.OtherPlayerColliderLayer), QueryTriggerInteraction.Ignore);
		}
		return 0;
	}

	public void TargetChanged()
	{
		NeedAttackBack = false;
		NeedAttackShooter = false;
		SetEnterAttackStrategy(m_activeAttackStrategy);
		ResetRevengeTime();
	}

	public void BeHit(long fromInsId)
	{
		if (!IsMyControl || !(Target == null))
		{
			return;
		}
		if (MyCfg.backAttackDis > 0)
		{
			BackAttack(fromInsId);
			if (Target == null || FSM.CurrentState.ID == StateID.MonsterFreeMove || Target == null || FSM.CurrentState.ID == StateID.MonsterGoBitrh)
			{
				DoEscape();
			}
		}
		else if (MyCfg.type == 3 || FSM.CurrentState.ID == StateID.MonsterFreeMove || FSM.CurrentState.ID == StateID.MonsterGoBitrh)
		{
			DoEscape();
		}
	}

	public void BackAttack(long fromInsId)
	{
		int num = UpdateNearPlayer();
		for (int i = 0; i < num; i++)
		{
			BasePlayerController componentInParent = m_BackAttackDisPlayers[i].GetComponentInParent<BasePlayerController>();
			if (componentInParent.InsId == fromInsId)
			{
				SetTarget(componentInParent.gameObject);
			}
		}
		if (Target != null)
		{
			DoAttackBack();
		}
	}

	public void DoEscape()
	{
		MonsterFreeMoveState monsterFreeMoveState = FSM.GetState(StateID.MonsterFreeMove) as MonsterFreeMoveState;
		monsterFreeMoveState.SetForceRun(4f);
	}

	private void DoAttackBack()
	{
		UpdateTargetDis();
		NeedAttackBack = true;
		SetEnterAttackStrategy(m_attackBackStrategy);
		FSM.SwitchState(StateID.MonsterDefence);
	}

	public void InitEnterAttackStrategy()
	{
		m_activeAttackStrategy = new ActiveAttackStrategy();
		m_attackBackStrategy = new AttackBackStrategy();
		m_attackShooterStrategy = new AttackShooterStrategy();
	}

	private void DoAttacShooter()
	{
		UpdateTargetDis();
		NeedAttackShooter = true;
		SetEnterAttackStrategy(m_attackShooterStrategy);
		FSM.SwitchState(StateID.MonsterDefence);
	}

	private void SetEnterAttackStrategy(IEnterAttackStrategy enterAttackStrategy)
	{
		MonsterDefenceState monsterDefenceState = FSM.GetState(StateID.MonsterDefence) as MonsterDefenceState;
		if (monsterDefenceState != null)
		{
			monsterDefenceState.SetEnterAttackStrategy(enterAttackStrategy);
		}
	}

	public void ResetRevengeTime()
	{
		RevengeTime = MyCfg.revengeTime;
	}

	public void UpdateRevengeTime()
	{
		if (DisToTarget > DefenceRange || !CanSeeTarget())
		{
			RevengeTime -= Time.deltaTime;
			if (RevengeTime <= 0f)
			{
				SetTarget(null);
			}
		}
	}

	private void InitFSM()
	{
		if (FSM == null)
		{
			FSM = new FSMSystem(this);
			MonsterFreeMoveState state = new MonsterFreeMoveState();
			MonsterGoBirthState state2 = new MonsterGoBirthState();
			MonsterAttackState state3 = new MonsterAttackState();
			MonsterDefenceState state4 = new MonsterDefenceState();
			FSM.AddState(state);
			FSM.AddState(state3);
			FSM.AddState(state2);
			FSM.AddState(state4);
			InitEnterAttackStrategy();
		}
		StartFsm();
	}

	public void StartFsm()
	{
		FSM.StartFsmSystem(StateID.MonsterFreeMove);
	}

	public void Update()
	{
		if (IsMyControl)
		{
			FSM.Update();
			FSM.LateUpdate();
			UpateMyMove();
			UpdateRevengeTime();
		}
		if (NextAttackTime > 0f)
		{
			NextAttackTime -= Time.deltaTime;
		}
	}

	public void UpateMyMove()
	{
		if (IsDie)
		{
			return;
		}
		if (m_Rotating)
		{
			if (HumanCfg != null)
			{
				m_TargetRotation.eulerAngles = new Vector3(0f, m_TargetRotation.eulerAngles.y, 0f);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, m_TargetRotation, turnSpeed * Time.deltaTime);
			m_Rotating = base.transform.rotation != m_TargetRotation;
		}
		if (CanMove && MyCfg.canMove)
		{
			if (HumanCfg == null)
			{
				AnimalMove();
			}
			else
			{
				HumanMove();
			}
		}
	}

	private void AnimalMove()
	{
		if (IsPlayCurAnimationName("run") || IsPlayCurAnimationName("walk"))
		{
			Rig.MovePosition(base.transform.position + base.transform.forward * Time.deltaTime * MoveSpeed);
		}
	}

	private void HumanMove()
	{
		Rig.MovePosition(base.transform.position + base.transform.forward * Time.deltaTime * MoveSpeed);
		SetAnimatorForwardValue(MoveSpeed * 0.5f);
	}

	public void PlayWalk()
	{
		if (MyCfg.canMove)
		{
			MoveSpeed = walkSpeed;
			if (MyGunCfg == null)
			{
				ChangeAnimatorStates("walk");
			}
			else
			{
				SetAnimatorForwardValue(1f);
			}
			CanMove = true;
		}
	}

	public void PlayRun()
	{
		if (MyCfg.canMove)
		{
			MoveSpeed = runSpeed;
			if (MyGunCfg == null)
			{
				ChangeAnimatorStates("run");
			}
			else
			{
				SetAnimatorForwardValue(1f);
			}
			CanMove = true;
		}
	}

	public void Stop()
	{
		if (Rig != null)
		{
			Rig.velocity = Vector3.zero;
		}
		SetAnimatorForwardValue(0f);
		m_TargetPos = base.transform.position;
		CanMove = false;
	}

	private IEnumerator SyncTick()
	{
		while (!IsDie)
		{
			UpdateBirthDis();
			UpdateTargetDis();
			yield return Wait02;
		}
	}

	private IEnumerator Tick2()
	{
		while (!IsDie)
		{
			if (IsMyControl)
			{
				FindOneAttackTarget();
			}
			UpdateDisToMySelf();
			yield return Utils.WaitForSeconds(2f);
		}
	}

	private IEnumerator Tick1()
	{
		while (!IsDie)
		{
			if (IsMyControl)
			{
				ControlSync();
			}
			yield return Utils.WaitForSeconds(1f);
		}
	}

	private void UpdateBirthDis()
	{
		DisToBirthPos = Vector3.Distance(base.transform.position, BirthPos);
	}

	private void UpdateDisToMySelf()
	{
		if ((bool)Battle.Ins.SelfPlayer)
		{
			DisToMySelf = Vector3.Distance(base.transform.position, Battle.Ins.SelfPlayer.Pos);
		}
	}

	private void UpdateTargetDis()
	{
		if ((bool)Target)
		{
			DisToTarget = Vector3.Distance(base.transform.position, Target.transform.position);
		}
		else
		{
			DisToTarget = 2.1474836E+09f;
		}
	}

	public void SetTargetRotation(Vector3 orientation)
	{
		m_TargetRotation = Quaternion.Euler(0f, orientation.y, 0f);
		m_Rotating = true;
	}

	public void SetTargetRotation(Quaternion q)
	{
		m_TargetRotation = q;
		m_Rotating = true;
	}

	public void SetTargetPos(Vector3 pos)
	{
		m_TargetPos.Set(pos.x, pos.y, pos.z);
		SetTargetRotation(Quaternion.LookRotation(pos - base.transform.position, Vector3.up));
	}

	public bool IsOnGround()
	{
		float awayGroundDistance = Battle.Ins.GetAwayGroundDistance(new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z));
		if (Mathf.Abs(awayGroundDistance) < 1f)
		{
			return true;
		}
		return false;
	}

	public bool CanSeeTarget()
	{
		if ((bool)TargetController)
		{
			RaycastHit hitInfo;
			if (Physics.Linecast(EyeGo.transform.position, TargetController.HeadBub.position, out hitInfo, -1, QueryTriggerInteraction.Ignore) && hitInfo.collider.gameObject.layer != BattleScMgr.SelfPlayerColliderLayer && hitInfo.collider.gameObject.layer != BattleScMgr.MonsterLayer && hitInfo.collider.gameObject.layer != BattleScMgr.OtherPlayerColliderLayer)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool CanSeeObj(GameObject t)
	{
		if ((bool)t)
		{
			RaycastHit hitInfo;
			if (Physics.Linecast(EyeGo.transform.position, t.transform.position + Vector3.up * 1.6f, out hitInfo, -1, QueryTriggerInteraction.Ignore) && hitInfo.collider.gameObject.layer != BattleScMgr.SelfPlayerColliderLayer && hitInfo.collider.gameObject.layer != BattleScMgr.MonsterLayer && hitInfo.collider.gameObject.layer != BattleScMgr.OtherPlayerColliderLayer)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void SetAnimatorHorizontalValue(float value)
	{
		MyAnimator.SetFloat(s_HorizontalInputHash, value, 0.12f, Time.deltaTime);
	}

	public void SetAnimatorForwardValue(float value, float time = 0f)
	{
		if (HumanCfg != null)
		{
			MyAnimator.SetFloat(s_ForwardInputHash, value, time, Time.deltaTime);
		}
	}

	public void Die(SMonsterDie msg)
	{
		if (FSM != null)
		{
			FSM.Stop();
		}
		Stop();
		SingletonMono<AudioManager>.Ins.Play(MyCfg.dieSound, base.transform.position);
		if (HumanCfg != null)
		{
			HumanMonsterDie(msg);
		}
		else
		{
			PlayerAnimation(0, "dieing");
		}
		if (IsMyControl)
		{
			if (m_syncTick != null)
			{
				Battle.Ins.StopCoroutine(m_syncTick);
			}
			UnityEngine.Object.Destroy(Rig);
			if (FSM != null)
			{
				FSM.Clear();
				FSM = null;
			}
			IsMyControl = false;
			SingletonMono<MonsterMgr>.Ins.SendUnControlMonsterMsg(InsId);
		}
		IsDie = true;
		Invoke("Remove", 10f);
	}

	public void HumanMonsterDie(SMonsterDie msg)
	{
		_003CHumanMonsterDie_003Ec__AnonStorey4 _003CHumanMonsterDie_003Ec__AnonStorey = new _003CHumanMonsterDie_003Ec__AnonStorey4();
		_003CHumanMonsterDie_003Ec__AnonStorey.msg = msg;
		_003CHumanMonsterDie_003Ec__AnonStorey._0024this = this;
		IsDie = true;
		EffectInfo[] componentsInChildren = base.gameObject.GetComponentsInChildren<EffectInfo>();
		EffectInfo[] array = componentsInChildren;
		foreach (EffectInfo effectInfo in array)
		{
			effectInfo.gameObject.SetActive(false);
		}
		ResMgr.Ins.CreateFromAB("role/bip001.ab", null, _003CHumanMonsterDie_003Ec__AnonStorey._003C_003Em__0);
	}

	public void ChangeNoOwner()
	{
		Stop();
		if (!IsDie)
		{
			PlayerAnimation(0, "idle");
		}
		PlayHaveGunAim(false);
	}

	public void Remove()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		CancelInvoke("SetGroundPos");
		if (m_syncTick != null)
		{
			Battle.Ins.StopCoroutine(m_syncTick);
		}
		if (m_Tick1 != null)
		{
			Battle.Ins.StopCoroutine(m_Tick1);
		}
		if (m_Tick2 != null)
		{
			Battle.Ins.StopCoroutine(m_Tick2);
		}
		SingletonMono<MonsterMgr>.Ins.MonsterDic.Remove(InsId);
	}

	public void FixedUpdate()
	{
		if (!IsMyControl)
		{
			UpdateMsgMove();
		}
		else
		{
			UpateMyMove();
		}
	}

	private void UpdateMsgMove()
	{
		if (m_Moving)
		{
			Vector3 vector = Vector3.SmoothDamp(base.transform.position, m_TargetPos, ref m_Velocity, 0.2f, float.MaxValue, Time.fixedDeltaTime);
			base.transform.position = vector;
			m_Moving = m_TargetPos != vector;
			if (!m_Moving)
			{
				m_Velocity = Vector3.zero;
			}
			SetAnimatorForwardValue(m_Velocity.magnitude);
		}
		if (m_Rotating)
		{
			Quaternion quaternion = Quaternion.Lerp(base.transform.rotation, m_TargetRotation, 5f * Time.deltaTime);
			base.transform.rotation = quaternion;
			m_Rotating = quaternion != m_TargetRotation;
		}
	}

	public void SetTargetPosFromMsg(Vec3 pos)
	{
		if (!IsDie)
		{
			m_TargetPos.Set(pos.x, pos.y, pos.z);
			m_Moving = true;
		}
	}

	public void SetTargetRotationFromMsg(ShortVec3 orientation)
	{
		m_TargetRotation = Quaternion.Euler(MathUtils.Short2Float(orientation.x), MathUtils.Short2Float(orientation.y), MathUtils.Short2Float(orientation.z));
		m_Rotating = true;
	}

	public void PlayerAnimation(int layer, int aniHash)
	{
		Layer2aimHashDic[layer] = aniHash;
		if (MyAnimator.enabled)
		{
			MyAnimator.CrossFade(aniHash, 0.3f, layer);
		}
	}

	public void PlayerAnimation(int layer, string aimName)
	{
		int stateNameHash = GetStateNameHash(s_LayerNames[layer] + aimName);
		PlayerAnimation(layer, stateNameHash);
	}

	private void OnBecameVisible()
	{
		MyAnimator.enabled = true;
		foreach (KeyValuePair<int, int> item in Layer2aimHashDic)
		{
			MyAnimator.Play(item.Value, item.Key);
		}
	}

	private void OnBecameInvisible()
	{
		MyAnimator.enabled = false;
	}

	public void ControlSync()
	{
		Vector3 position = base.transform.position;
		if (!MathUtils.RoughlyEquals(position, m_OldPos, 0.001f))
		{
			cSyncMonsterPos.pos.x = position.x;
			cSyncMonsterPos.pos.y = position.y;
			cSyncMonsterPos.pos.z = position.z;
			cSyncMonsterPos.instanceId = InsId;
			Client2Gs.Ins.Send(cSyncMonsterPos);
			m_OldPos = position;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		if (!MathUtils.RoughlyEquals(m_OldEulerAngles, eulerAngles, 1f))
		{
			cSyncMonsterOrientation.orientation.x = MathUtils.Float2Short(eulerAngles.x);
			cSyncMonsterOrientation.orientation.y = MathUtils.Float2Short(eulerAngles.y);
			cSyncMonsterOrientation.orientation.z = MathUtils.Float2Short(eulerAngles.z);
			cSyncMonsterOrientation.instanceId = InsId;
			Client2Gs.Ins.Send(cSyncMonsterOrientation);
			m_OldEulerAngles = eulerAngles;
		}
	}

	private void OnChangeAnimatorState(string targetStateName, int stateHash, int layerIndex)
	{
		cSyncMonsterAnimator.layer = (byte)layerIndex;
		cSyncMonsterAnimator.animationHash = stateHash;
		cSyncMonsterAnimator.instanceId = InsId;
		Client2Gs.Ins.Send(cSyncMonsterAnimator);
		if (Battle.Ins.PlayTest)
		{
			Debug.LogError("Monsterlayer:  + " + targetStateName);
		}
	}

	private bool CompareAndSetLayerStateName(string stateName, int layerIndex)
	{
		string value;
		if (m_Layer2StateName.TryGetValue(layerIndex, out value))
		{
			if (stateName == value)
			{
				return true;
			}
			m_Layer2StateName[layerIndex] = stateName;
			return false;
		}
		m_Layer2StateName.Add(layerIndex, stateName);
		return false;
	}

	public bool ChangeAnimatorStates(string targetStateName, int layerIndex = 0, bool allowReplay = false)
	{
		if (string.IsNullOrEmpty(targetStateName))
		{
			return false;
		}
		if (CompareAndSetLayerStateName(targetStateName, layerIndex) && !allowReplay)
		{
			return false;
		}
		int stateNameHash = GetStateNameHash(s_LayerNames[layerIndex] + targetStateName);
		if (MyAnimator != null)
		{
			MyAnimator.CrossFade(stateNameHash, 0.1f);
		}
		OnChangeAnimatorState(targetStateName, stateNameHash, layerIndex);
		return true;
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

	private void InitAimationStates()
	{
		if (s_LayerNames == null)
		{
			s_LayerNames = new string[MyAnimator.layerCount];
			int i = 0;
			for (int layerCount = MyAnimator.layerCount; i < layerCount; i++)
			{
				s_LayerNames[i] = MyAnimator.GetLayerName(i) + ".";
			}
		}
	}

	public bool IsPlayFinish(string aniName)
	{
		return IsUpBodyLayerPlayFinish(aniName) || IsBaseLayerPlayFinish(aniName);
	}

	public bool IsUpBodyLayerPlayFinish(string aniName)
	{
		if (MyAnimator.layerCount <= 1)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = MyAnimator.GetCurrentAnimatorStateInfo(1);
		return currentAnimatorStateInfo.IsName(aniName) && currentAnimatorStateInfo.normalizedTime >= 1f;
	}

	public bool IsBaseLayerPlayFinish(string aniName)
	{
		AnimatorStateInfo currentAnimatorStateInfo = MyAnimator.GetCurrentAnimatorStateInfo(0);
		return currentAnimatorStateInfo.IsName(aniName) && currentAnimatorStateInfo.normalizedTime >= 1f;
	}

	public bool IsPlayCurAnimationName(string name, int layer = 0)
	{
		return MyAnimator.GetCurrentAnimatorStateInfo(layer).IsName(name);
	}

	public void UpdateLod()
	{
		Vector3 position = Battle.Ins.MainCamera.SelfTransform.position;
		float num = Battle.Ins.MainCamera.Fov / 51f;
		float num2 = Vector3.SqrMagnitude(position - base.transform.position) * num * num;
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
		else if (num2 < 4900f)
		{
			if (m_Lod != 3)
			{
				m_Lod = 3;
				ChangeLod(m_Lod);
			}
		}
		else if (num2 < 14400f)
		{
			if (m_Lod != 4)
			{
				m_Lod = 4;
				ChangeLod(m_Lod);
			}
		}
		else if (m_Lod != 5)
		{
			m_Lod = 5;
			ChangeLod(m_Lod);
		}
	}

	public void PutOnLowModel()
	{
		ResMgr.Ins.CreateFromAB("role/roleLodskin01.ab", null, _003CPutOnLowModel_003Em__1);
	}

	private void ChangeLod(int level)
	{
		if (HumanCfg != null)
		{
			switch (level)
			{
			case 0:
				Skin.ShowAll();
				Skin.HideSkin(100);
				ShowDressing();
				MyAnimator.enabled = true;
				break;
			case 1:
				Skin.ShowAll();
				Skin.HideSkin(100);
				ShowAllWeapon();
				ShowDressing();
				MyAnimator.enabled = true;
				break;
			case 2:
				Skin.ShowAll();
				Skin.HideSkin(100);
				ShowAllWeapon();
				ShowDressing();
				MyAnimator.enabled = true;
				break;
			case 3:
				Skin.ShowAll();
				Skin.HideSkin(100);
				HideDressing();
				ShowAllWeapon();
				MyAnimator.enabled = true;
				break;
			case 4:
				Skin.HideAll();
				if (m_LowModel != null)
				{
					Skin.ShowSkin(100);
				}
				HideAllWeapon();
				MyAnimator.enabled = true;
				break;
			default:
				Skin.HideAll();
				if (m_LowModel != null)
				{
					Skin.ShowSkin(100);
				}
				HideAllWeapon();
				MyAnimator.enabled = false;
				break;
			}
		}
		else if (level > 4)
		{
			HideAllRenders();
			MyAnimator.enabled = false;
		}
		else if (level > 3)
		{
			ShowAllRenders();
			MyAnimator.enabled = true;
		}
		else
		{
			ShowAllRenders();
			MyAnimator.enabled = true;
		}
	}

	private void HideAllWeapon()
	{
		if (MyGunInfo != null)
		{
			MyGunInfo.gameObject.SetActiveBetter(false);
		}
	}

	private void ShowAllWeapon()
	{
		if (MyGunInfo != null)
		{
			MyGunInfo.gameObject.SetActiveBetter(true);
		}
	}

	private void HideAllRenders()
	{
		foreach (Renderer render in m_renders)
		{
			render.enabled = false;
		}
	}

	private void ShowAllRenders()
	{
		foreach (Renderer render in m_renders)
		{
			render.enabled = true;
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

	[CompilerGenerated]
	private void _003CLoadWeapon_003Em__0(GameObject go)
	{
		go.transform.SetParent(RightHandGuaDian);
		go.transform.localPosition = Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		MyGunInfo = go.GetComponent<GunInfo>();
	}

	[CompilerGenerated]
	private void _003CPutOnLowModel_003Em__1(GameObject o)
	{
		if (IsDie || Skin == null)
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

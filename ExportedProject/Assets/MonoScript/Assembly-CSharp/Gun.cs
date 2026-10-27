using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using cfg;
using gs.battle.scmsg;

public class Gun : BaseGun
{
	[CompilerGenerated]
	private sealed class _003COpenJiMiao_003Ec__AnonStorey3
	{
		internal Transform hand;

		internal Gun _0024this;

		internal void _003C_003Em__0(object[] go)
		{
			if (_0024this.IsJiMiao)
			{
				_0024this.GunTransform.gameObject.SetActiveBetter(true);
				_0024this.GunTransform.rotation = Quaternion.FromToRotation(_0024this.GunInfo.CameraPos.forward, _0024this.m_cameraTransform.forward) * _0024this.GunTransform.rotation;
				_0024this.GunTransform.rotation = Quaternion.FromToRotation(_0024this.GunInfo.CameraPos.right, _0024this.m_cameraTransform.right) * _0024this.GunTransform.rotation;
				Vector3 vector = _0024this.GunInfo.CameraPos.position - _0024this.GunTransform.position;
				_0024this.GunTransform.position = _0024this.m_cameraTransform.position - vector;
				hand.rotation = Quaternion.FromToRotation(_0024this.GunInfo.CameraPos.forward, _0024this.m_cameraTransform.forward) * hand.rotation;
				hand.rotation = Quaternion.FromToRotation(_0024this.GunInfo.CameraPos.right, _0024this.m_cameraTransform.right) * hand.rotation;
				vector = hand.position - _0024this.m_Player.JimiaoRightHandGuaDian.transform.position;
				hand.position = vector + _0024this.GunTransform.position;
				_0024this.GunTransform.SetParent(_0024this.m_Player.JimiaoRightHandGuaDian.transform, true);
				_0024this.GunTransform.localScale = Vector3.one;
				_0024this.GunTransform.localEulerAngles = Vector3.zero;
				_0024this.GunTransform.localPosition = Vector3.zero;
			}
		}
	}

	[NonSerialized]
	public int AimRadiusMax;

	[NonSerialized]
	public int AimRadiusMin;

	[NonSerialized]
	public float AimRadius;

	[NonSerialized]
	public int AimPointX;

	[NonSerialized]
	public int AimPointY;

	[NonSerialized]
	public int RandRadiusMax;

	[NonSerialized]
	public int RandRadiusMin;

	[NonSerialized]
	public int RandRadius;

	[NonSerialized]
	public int RandPosX;

	[NonSerialized]
	public int RandPosY;

	public const float CameraDefaultFov = 51f;

	public FireType CurFireType;

	private float AimRadiusShrinkSpeed;

	private float AimRadiusShootAddSpeed;

	private float AnimationSpeedChange = 1f;

	private Vector2 CameraCenterChange = Vector2.zero;

	private float[] m_Factors;

	private readonly vThirdPersonCamera m_Camera;

	private readonly Transform m_cameraTransform;

	private readonly PlayerController m_Player;

	public Transform GunTransform;

	private Animator m_Animator;

	private bool m_Enabled;

	private int m_lianfaNum = -1;

	private bool m_autoShooting;

	private float m_coolRemainTime;

	private bool m_isJiMiao;

	private bool m_isKaijing;

	private float m_BulletYSpeed;

	private bool m_CanCameraBack;

	private float jingMoveSpeed = 20f;

	private float jingStayTimeMin = 0.1f;

	private float jingStayTimeMax = 0.3f;

	private Vector2 jingPos;

	private Vector2 jingTarget;

	private Vector2 jingTarget2;

	private Vector2 jingPosOffet;

	private float jingStayReminTime;

	private int jingMoveStep;

	private Vector2 jingVelocity = Vector2.zero;

	private Coroutine m_kaijingCoroutine;

	private Renderer[] GunMeshs;

	private Vector2 m_ShootBulletScreenPoint = Vector2.zero;

	private static readonly CShoot cShoot = new CShoot();

	public static int ExtraBulletNumBySkill = 0;

	private static float maxAngleOffset = Mathf.Atan2(1f, 10f) * 57.29578f;

	public int CurLoadBulletNum
	{
		get
		{
			return Singleton<BagMgr>.Ins.GetGunDate(InsId).CurBulletNum;
		}
	}

	public bool CanAddBullet
	{
		get
		{
			return CurLoadBulletNum < BulletMax;
		}
	}

	public bool IsJiMiao
	{
		get
		{
			return m_isJiMiao;
		}
	}

	public bool IsKaijing
	{
		get
		{
			return m_isKaijing;
		}
	}

	public bool Enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			if (m_Enabled != value)
			{
				m_Enabled = value;
				if (m_Enabled)
				{
					OnEnable();
				}
				else
				{
					OnDisable();
				}
			}
		}
	}

	public bool IsQuiet
	{
		get
		{
			return GetBoolFactor(35);
		}
	}

	public bool IsNoFireEffect
	{
		get
		{
			return GetBoolFactor(36) || GetBoolFactor(35);
		}
	}

	public int ChangeBulletMode
	{
		get
		{
			return (int)m_Factors[38];
		}
	}

	public Vector2 JingPos
	{
		get
		{
			return jingPos + jingPosOffet;
		}
	}

	public Gun(PlayerController player, BagMgr.GunData gunDate)
		: base(player, gunDate.Id, gunDate.SkinId, gunDate.PartsId)
	{
		InsId = gunDate.InsId;
		m_Factors = gunDate.Factors;
		AddPropByLevel(gunDate.Id);
		ExtraBulletSkill();
		UpdateBulletMax();
		m_Player = player;
		m_Camera = Battle.Ins.MainCamera;
		m_cameraTransform = m_Camera.transform;
		ChangeBullet(gunDate.CurBulletNum);
		OnEnable();
		if (m_gunCfg.canAuto)
		{
			SetFireType(FireType.Auto);
		}
		BagEvent.ChangeGunFactors = (Utils.IntDelegate)Delegate.Combine(BagEvent.ChangeGunFactors, new Utils.IntDelegate(OnChangeGunFactors));
	}

	private void OnChangeGunFactors(int insId)
	{
		if (insId == InsId)
		{
			m_Factors = Singleton<BagMgr>.Ins.GetGunDate(InsId).Factors;
		}
	}

	private void AddPropByLevel(int gunid)
	{
		int key = 1;
		GunLevelCfg gunLevelCfg = GunLevelCfg.Get(key);
		if (gunLevelCfg != null)
		{
			int i = 0;
			for (int count = gunLevelCfg.idsProp.Count; i < count; i++)
			{
				m_Factors[gunLevelCfg.idsProp[i]] += gunLevelCfg.valuesProp[i];
			}
		}
	}

	public override void SetWeaponGameObject(GameObject gunGo)
	{
		base.SetWeaponGameObject(gunGo);
		if (m_gunCfg.gunType == 8 && CurLoadBulletNum > 0)
		{
			GameObject getOneRpgBullet = Battle.Ins.GetOneRpgBullet;
			getOneRpgBullet.transform.SetParent(GunInfo.Muzzle, false);
		}
		GunTransform = WeaponObject.transform;
		m_Animator = WeaponObject.GetComponent<Animator>();
	}

	public float GetFactor(int index)
	{
		return m_Factors[index];
	}

	public int GetIntFactor(int index)
	{
		return (int)m_Factors[index];
	}

	public bool GetBoolFactor(int index)
	{
		return m_Factors[index] > 0f;
	}

	public float GetProp(int index)
	{
		return GetFactor(index + 47);
	}

	public int GetInitialBulletMax()
	{
		return GetInitialBulletMax(m_gunCfg);
	}

	public static int GetInitialBulletMax(GunCfg gunCfg)
	{
		return (int)gunCfg.factors[34] + ExtraBulletNumBySkill;
	}

	public void SetFireType(FireType fireType)
	{
		CurFireType = fireType;
		m_autoShooting = false;
		m_lianfaNum = -1;
	}

	public void StartShoot()
	{
		if (CurLoadBulletNum <= 0 && !Singleton<BattleScMgr>.Ins.NeedNewPlayerboot)
		{
			SingletonMono<AudioManager>.Ins.Play2D(157);
			return;
		}
		if (CurFireType == FireType.Auto)
		{
			m_autoShooting = true;
		}
		else if (CurFireType == FireType.LianFa)
		{
			m_lianfaNum = m_gunCfg.lianfaBulletNum;
		}
		if (!(m_coolRemainTime > 0f))
		{
			DoOneShoot();
		}
	}

	public void StopShoot()
	{
		m_autoShooting = false;
	}

	public Vector3 GetAimPointWorldPos(Vector2 screenPoint)
	{
		Ray ray = m_Camera.ScreenPointToRay(screenPoint + new Vector2(m_Camera.Camera.pixelWidth, m_Camera.Camera.pixelHeight) * 0.5f);
		ray.origin += ray.direction * 1.6f;
		Vector3 point = ray.GetPoint(10000f);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10000f, 1847363585) && hitInfo.collider != null)
		{
			return hitInfo.point;
		}
		return point;
	}

	public void ChangeBullet(int bulletNum)
	{
		m_lianfaNum = -1;
		Utils.TriggerEvent(BattleEvent.OnCurLoadBulletNumChange, CurLoadBulletNum);
	}

	public bool HasBullet()
	{
		return CurLoadBulletNum > 0;
	}

	public void PlayAnim(string name)
	{
		if ((bool)m_Animator)
		{
			m_Animator.Play(name);
		}
	}

	private void ShowGunShadow()
	{
		if (GunMeshs == null && WeaponObject != null)
		{
			GunMeshs = WeaponObject.GetComponentsInChildren<Renderer>(true);
		}
		Renderer[] gunMeshs = GunMeshs;
		foreach (Renderer renderer in gunMeshs)
		{
			if (renderer != null)
			{
				renderer.shadowCastingMode = ShadowCastingMode.On;
			}
		}
	}

	private void HideGunShadow()
	{
		if (GunMeshs == null && WeaponObject != null)
		{
			GunMeshs = WeaponObject.GetComponentsInChildren<Renderer>(true);
		}
		Renderer[] gunMeshs = GunMeshs;
		foreach (Renderer renderer in gunMeshs)
		{
			if (renderer != null)
			{
				renderer.shadowCastingMode = ShadowCastingMode.Off;
			}
		}
	}

	public void OpenJiMiao()
	{
		if (m_Player.CanOpenJimiao && !m_Player.IsDie && !m_isJiMiao)
		{
			m_isJiMiao = true;
			jingPos = Vector2.zero;
			jingPosOffet = Vector2.zero;
			m_Camera.SetLockedTarget(m_Player.JimiaoCameraPoint, "jimiao");
			m_Camera.ChangeState(m_Player.JimiaoCameraPoint, "jimiao");
			HideGunShadow();
			Vector3 aimPointWorldPos = GetAimPointWorldPos(Vector2.zero);
			if (Vector3.Distance(aimPointWorldPos, m_cameraTransform.position) > 20f)
			{
				m_Camera.SetLockAtPoint(aimPointWorldPos);
			}
			ItemCfg partCfg = GetPartCfg(14);
			if (partCfg == null)
			{
				_003COpenJiMiao_003Ec__AnonStorey3 _003COpenJiMiao_003Ec__AnonStorey = new _003COpenJiMiao_003Ec__AnonStorey3();
				_003COpenJiMiao_003Ec__AnonStorey._0024this = this;
				m_Player.SetJimiaoCameraPointJimiao(m_Player.RootBone);
				m_Player.Skin.HideAllNoHideParent();
				m_Player.ChangeHandAndBodyBones(true);
				_003COpenJiMiao_003Ec__AnonStorey.hand = m_Player.RoleHandAnimator.transform;
				_003COpenJiMiao_003Ec__AnonStorey.hand.SetParent(m_cameraTransform, true);
				_003COpenJiMiao_003Ec__AnonStorey.hand.localPosition = Vector3.zero;
				_003COpenJiMiao_003Ec__AnonStorey.hand.localRotation = Quaternion.identity;
				_003COpenJiMiao_003Ec__AnonStorey.hand.localScale = Vector3.one;
				_003COpenJiMiao_003Ec__AnonStorey.hand.gameObject.SetActiveBetter(true);
				GunTransform.gameObject.SetActiveBetter(false);
				m_Player.RoleHandAnimator.Play("Aim." + base.GunCfg.aimZhan, m_Player.RoleHandAnimator.GetLayerIndex("UpperBody Layer"), 0f);
				DelayInvoker.DelayInvoke("jimiao" + Time.time, 0.1f, _003COpenJiMiao_003Ec__AnonStorey._003C_003Em__0);
				m_Camera.CameraStateList.FindState("jimiao").fov = 51f;
				Utils.TriggerEvent(BattleEvent.OnOpenJiMiao);
			}
			else
			{
				m_Player.SetJimiaoCameraPointKaijing();
				m_Player.Skin.HideAll();
				m_Player.HideAllWeapon();
				Utils.TriggerEvent(BattleEvent.OnOpenKaiJing);
				m_isKaijing = true;
				m_Camera.CameraStateList.FindState("jimiao").fov = 51f / partCfg.extras[1];
			}
		}
	}

	public void CloseJiMiao()
	{
		try
		{
			if (m_isJiMiao && !(GunTransform == null))
			{
				ShowGunShadow();
				m_isJiMiao = false;
				m_isKaijing = false;
				m_Player.Skin.ShowAll();
				m_Player.Skin.ShowAllNoHideParent();
				m_Camera.Unlock();
				m_Player.ChangeHandAndBodyBones(false);
				Vector3 aimPointWorldPos = GetAimPointWorldPos(Vector2.zero);
				if (Vector3.Distance(aimPointWorldPos, m_cameraTransform.position) > 20f)
				{
					m_Camera.SetLockAtPoint(aimPointWorldPos);
				}
				m_Player.ShowAllWeapon();
				m_Player.HideWeaponExceptCurrent();
				GunTransform.SetParent(Battle.Ins.SelfPlayer.RightHandGuaDian.transform, false);
				GunTransform.localPosition = Vector3.zero;
				GunTransform.localRotation = Quaternion.identity;
				GunTransform.localScale = Vector3.one;
				if (m_kaijingCoroutine != null)
				{
					Battle.StopConroutine(m_kaijingCoroutine);
					m_kaijingCoroutine = null;
				}
				Utils.TriggerEvent(BattleEvent.OnCloseKaiJing);
				Utils.TriggerEvent(BattleEvent.OnCloseJiMiao);
				m_Player.SetJimiaoCameraPointKaijing();
			}
		}
		catch (Exception)
		{
		}
	}

	private void CameraUp(float x, float y)
	{
		m_CanCameraBack = false;
		ChangeCamera(x, y);
	}

	private void ChangeCamera(float x, float y)
	{
		Vector3 point = new Vector3(x + (float)Utils.ScreenWidth * 0.5f, y + (float)Utils.ScreenHeight * 0.5f);
		Ray ray = m_Camera.ScreenPointToRay(point);
		m_Camera.SetCameraOffsetDirection(ray.direction);
		Vector2 vector = new Vector2((float)AimRadiusMax * m_Factors[44], (float)AimRadiusMax * m_Factors[45]) * 51f / m_Camera.Fov;
		m_Camera.offsetAngles.x = MathUtils.ClampBetween(m_Camera.offsetAngles.x, 0f, 0f - vector.y);
		m_Camera.offsetAngles.y = MathUtils.ClampBetween(m_Camera.offsetAngles.y, 0f, vector.x);
	}

	private void CameraBack()
	{
		float num = AimRadiusShrinkSpeed * m_Factors[46] * 0.1f;
		m_Camera.offsetAngles = Vector3.Slerp(m_Camera.offsetAngles, Vector3.zero, num * Time.deltaTime);
	}

	private void RandShootBulletScreenPoint(float angleMin, float angleMax, int randRadius)
	{
		int num = UnityEngine.Random.Range(0, randRadius);
		float num2 = UnityEngine.Random.Range(angleMin, angleMax);
		m_ShootBulletScreenPoint.x = (int)(Math.Cos(num2) * (double)num) + RandPosX;
		m_ShootBulletScreenPoint.y = (int)(Math.Sin(num2) * (double)num) + RandPosY;
	}

	private void DoOneShoot()
	{
		m_lianfaNum--;
		Utils.TriggerEvent(BattleEvent.OnShoot, m_Player.InsId);
		m_Player.FSMUpBody.SwtichStateReStart(StateID.Shoot);
		ShootOnce();
		if (AimRadius > (float)AimRadiusMax)
		{
			AimRadius = AimRadiusMax;
		}
		AimRadius += AimRadiusShootAddSpeed;
		switch (CurFireType)
		{
		case FireType.Danfa:
			m_coolRemainTime = m_Factors[53] * 0.001f;
			break;
		case FireType.LianFa:
			m_coolRemainTime = (float)m_gunCfg.lianfaBulletInterval * 0.001f;
			break;
		case FireType.Auto:
			m_coolRemainTime = m_Factors[54] * 0.001f;
			break;
		}
		CameraUp(CameraCenterChange.x, CameraCenterChange.y);
		Singleton<BagMgr>.Ins.ShootBullet(InsId, 1);
	}

	private void ShootOnce()
	{
		if (m_gunCfg.effects.Count > 0)
		{
			EffectCfg effectCfg = ((!IsNoFireEffect) ? EffectCfg.Get(m_gunCfg.effects[0]) : EffectCfg.Get(m_gunCfg.effects[1]));
			if (m_isJiMiao)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffect(effectCfg.path, GunInfo.Muzzle);
			}
			else
			{
				SingletonMono<EffectMgr>.Ins.PlayEffect(effectCfg.path, GunInfo.Muzzle);
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(16).path, GunTransform.position, Battle.Ins.MainCamera.transform.forward);
			}
		}
		if (m_gunCfg.audios.Count > 0)
		{
			Battle.Ins.SelfPlayer.SoundControll.PlaySound(m_gunCfg.audios[IsQuiet ? 2 : 0]);
		}
		Battle.StartConroutine(PlayShellDownSound());
		Singleton<BagMgr>.Ins.ReduceDurability();
		if (IsJiMiao && GetPartCfg(14) != null)
		{
			m_ShootBulletScreenPoint.x = JingPos.x;
			m_ShootBulletScreenPoint.y = JingPos.y;
			PlayJingAni();
		}
		if (m_gunCfg.sandanBulletNum > 0)
		{
			float num = 360f / (float)m_gunCfg.sandanBulletNum;
			float num2 = UnityEngine.Random.Range(0f, num);
			int i = 0;
			for (int sandanBulletNum = m_gunCfg.sandanBulletNum; i < sandanBulletNum; i++)
			{
				float num3 = UnityEngine.Random.Range(0f, AimRadius);
				float num4 = num * (float)i + num2;
				m_ShootBulletScreenPoint.x = (int)(Math.Cos(num4) * (double)num3);
				m_ShootBulletScreenPoint.y = (int)(Math.Sin(num4) * (double)num3);
				Vector3 aimPointWorldPos = GetAimPointWorldPos(m_ShootBulletScreenPoint);
				if (i == 0)
				{
					cShoot.pos.x = aimPointWorldPos.x;
					cShoot.pos.y = aimPointWorldPos.y;
					cShoot.pos.z = aimPointWorldPos.z;
				}
				FireOneBullet(aimPointWorldPos);
			}
		}
		else
		{
			float num5 = UnityEngine.Random.Range(0, RandRadius);
			float num6 = UnityEngine.Random.Range(0, 360);
			m_ShootBulletScreenPoint.x = (int)(Math.Cos(num6) * (double)num5) + RandPosX;
			m_ShootBulletScreenPoint.y = (int)(Math.Sin(num6) * (double)num5) + RandPosY;
			Vector3 aimPointWorldPos2 = GetAimPointWorldPos(m_ShootBulletScreenPoint);
			cShoot.pos.x = aimPointWorldPos2.x;
			cShoot.pos.y = aimPointWorldPos2.y;
			cShoot.pos.z = aimPointWorldPos2.z;
			FireOneBullet(aimPointWorldPos2);
		}
		Client2Gs.Ins.Send(cShoot);
	}

	private void FireOneBullet(Vector3 targetPos)
	{
		BulletControl bulletControl;
		if (base.GunCfg.gunType == 8)
		{
			bulletControl = GetRpgBullet();
			bulletControl.GetComponent<AutoRecycle>().enabled = true;
			bulletControl.IsRpgBullet = true;
		}
		else
		{
			bulletControl = Battle.Ins.BulletPool.Get();
			bulletControl.IsRpgBullet = false;
		}
		bulletControl.ItemId = Singleton<BagMgr>.Ins.GetCurrentGunBulletItemId(InsId);
		Rigidbody bulletRigidbody = bulletControl.BulletRigidbody;
		bulletRigidbody.isKinematic = false;
		bulletRigidbody.velocity = Vector3.zero;
		bulletControl.gameObject.SetActive(true);
		Vector3 vector = ((!m_isJiMiao) ? GunInfo.Muzzle.position : (m_cameraTransform.position + m_cameraTransform.forward * 0.2f));
		Vector3 normalized = (targetPos - vector).normalized;
		Vector3 vel = normalized * m_Factors[37];
		if (m_gunCfg.bulletDown)
		{
			Vector3 vector2 = Vector3.Cross(normalized, Vector3.Cross(Vector3.up, normalized));
			vel += vector2 * m_BulletYSpeed;
		}
		bulletControl.Fire(m_gunCfg.id, Battle.Ins.SelfPlayer.InsId, vector, normalized, vel);
	}

	private BulletControl GetRpgBullet()
	{
		Transform transform = GunInfo.Muzzle.transform.Find("rpgzidan");
		if (transform != null)
		{
			transform.GetComponent<Collider>().enabled = true;
			BulletControl component = transform.GetComponent<BulletControl>();
			component.enabled = true;
			transform.GetComponent<Rigidbody>().WakeUp();
			transform.Find("EF_rpg_02").gameObject.SetActive(true);
			transform.parent = null;
			return component;
		}
		Debug.LogError("can not find rpg zidan");
		return null;
	}

	private IEnumerator PlayShellDownSound()
	{
		yield return new WaitForSeconds(0.4f);
		try
		{
			if (base.GunCfg != null)
			{
				if (base.GunCfg.gunType == 4)
				{
					SingletonMono<AudioManager>.Ins.Play2D(426);
				}
				else
				{
					SingletonMono<AudioManager>.Ins.Play2D(134);
				}
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void Update()
	{
		if (GunTransform == null || m_gunCfg == null || !m_Enabled)
		{
			return;
		}
		CaulateFactors();
		if (AimRadius > (float)AimRadiusMin)
		{
			AimRadius -= AimRadiusShrinkSpeed * Time.deltaTime;
		}
		if (AimRadius < (float)AimRadiusMin)
		{
			AimRadius = AimRadiusMin;
		}
		if (m_coolRemainTime > 0f)
		{
			m_coolRemainTime -= Time.deltaTime;
			if (m_coolRemainTime < 0f)
			{
				m_coolRemainTime = 0f;
			}
		}
		if (m_isJiMiao)
		{
			if (Physics.Linecast(m_cameraTransform.position, GunInfo.Muzzle.position, 1 << BattleScMgr.DefaultLayer))
			{
				if (!Singleton<BattleScMgr>.Ins.NeedNewPlayerboot)
				{
					CloseJiMiao();
				}
			}
			else
			{
				UpdateJingMove();
			}
		}
		if (m_CanCameraBack)
		{
			CameraBack();
		}
		if ((m_autoShooting || m_lianfaNum > 0) && HasBullet())
		{
			m_CanCameraBack = false;
			if (m_coolRemainTime <= 0f)
			{
				DoOneShoot();
			}
		}
		else
		{
			m_CanCameraBack = true;
		}
		m_Player.SetGunAnimatorSpeed(AnimationSpeedChange);
	}

	private void OnEnable()
	{
		if (m_gunCfg != null)
		{
			CaulateFactors();
			AimRadius = GetIntFactor(0);
		}
		m_isJiMiao = false;
		m_autoShooting = false;
		m_lianfaNum = -1;
		m_isKaijing = false;
		m_CanCameraBack = false;
	}

	private void OnDisable()
	{
		CloseJiMiao();
		AnimationSpeedChange = 1f;
		m_Player.SetGunAnimatorSpeed(1f);
	}

	private void CaulateFactors()
	{
		StateID iD = m_Player.FSM.CurrentState.ID;
		float num = 0f;
		switch (iD)
		{
		case StateID.Jump:
		case StateID.Fall:
			AimRadiusMax = (int)m_Factors[1];
			num = m_Factors[6];
			AimRadiusShrinkSpeed = m_Factors[10];
			AimRadiusShootAddSpeed = m_Factors[15];
			break;
		case StateID.Stand:
			AimRadiusMax = (int)m_Factors[2];
			num = m_Factors[7];
			AimRadiusShrinkSpeed = m_Factors[11];
			AimRadiusShootAddSpeed = m_Factors[16];
			AnimationSpeedChange = m_gunCfg.moveSpeedFactors[0];
			break;
		case StateID.Crouch:
			AimRadiusMax = (int)m_Factors[3];
			num = m_Factors[8];
			AimRadiusShrinkSpeed = m_Factors[12];
			AimRadiusShootAddSpeed = m_Factors[17];
			AnimationSpeedChange = m_gunCfg.moveSpeedFactors[1];
			break;
		case StateID.Pa:
			AimRadiusMax = (int)m_Factors[4];
			num = m_Factors[9];
			AimRadiusShrinkSpeed = m_Factors[13];
			AimRadiusShootAddSpeed = m_Factors[18];
			AnimationSpeedChange = m_gunCfg.moveSpeedFactors[2];
			break;
		default:
			AimRadiusMax = (int)m_Factors[2];
			num = m_Factors[7];
			AimRadiusShrinkSpeed = m_Factors[11];
			AimRadiusShootAddSpeed = m_Factors[16];
			AnimationSpeedChange = 1f;
			break;
		}
		AimRadiusMax = Mathf.RoundToInt((float)AimRadiusMax * (1f + m_Player.Speed * m_Factors[5]));
		AimRadiusMin = Mathf.RoundToInt(num * (1f + m_Player.Speed * m_Factors[5]));
		if (AimRadiusMin >= AimRadiusMax)
		{
			AimRadiusMin = AimRadiusMax - 1;
		}
		if (AimRadius < (float)AimRadiusMin)
		{
			AimRadius = AimRadiusMin;
		}
		AimRadiusShootAddSpeed *= 1f + m_Player.Speed * m_Factors[14];
		RandRadiusMax = (int)m_Factors[19];
		RandRadiusMin = (int)m_Factors[20];
		RandPosX = Mathf.RoundToInt(Mathf.Min(m_Factors[21], Mathf.Max(m_Factors[22], (AimRadius - (float)AimRadiusMin) * Utils.Random(m_Factors[25], m_Factors[26]))));
		RandPosY = Mathf.RoundToInt(Mathf.Min(m_Factors[23], Mathf.Max(m_Factors[24], (AimRadius - (float)AimRadiusMin) * m_Factors[27])));
		RandRadius = Mathf.RoundToInt(Mathf.Max(RandRadiusMin, Mathf.Min(RandRadiusMax, (AimRadius - (float)AimRadiusMin) / (float)(AimRadiusMax - AimRadiusMin) * (float)(RandRadiusMax - RandRadiusMin) * m_Factors[29] + (float)RandRadiusMin)));
		CameraCenterChange.x = AimRadiusShootAddSpeed * Utils.Random(m_Factors[40], m_Factors[41]) * 51f / m_Camera.Fov;
		CameraCenterChange.y = AimRadiusShootAddSpeed * Utils.Random(m_Factors[42], m_Factors[43]) * 51f / m_Camera.Fov;
	}

	private void UpdateJingMove()
	{
		if (jingStayReminTime >= 0f)
		{
			jingStayReminTime -= Time.deltaTime;
			if (jingStayReminTime <= 0f)
			{
				jingMoveStep = 0;
				jingPos = Vector2.zero;
				float num = Utils.Random(0, AimRadiusMin);
				float f = Utils.Random(0f, (float)Math.PI * 2f);
				jingTarget = new Vector2(num * Mathf.Cos(f), num * Mathf.Sin(f));
				jingTarget.y = jingTarget.y / (float)AimRadiusMin * (float)AimRadiusMin;
				jingMoveSpeed = jingTarget.magnitude / Utils.Random(0.8f, 1.5f);
				jingVelocity = jingTarget.normalized * jingMoveSpeed;
				jingTarget2 = -jingTarget * Utils.Random(0.3f, 0.8f);
			}
		}
		else if (jingMoveStep == 0)
		{
			jingPos += jingVelocity * Time.deltaTime;
			if (MathUtils.isTargetArrived(Vector2.zero, jingTarget, jingPos))
			{
				jingPos = jingTarget;
				jingMoveStep = 1;
				jingVelocity = (jingTarget2 - jingTarget).normalized * jingVelocity.magnitude;
			}
		}
		else if (jingMoveStep == 1)
		{
			jingPos += jingVelocity * Time.deltaTime;
			if (MathUtils.isTargetArrived(jingTarget, jingTarget2, jingPos))
			{
				jingPos = jingTarget2;
				jingMoveStep = 2;
				jingVelocity = (-jingTarget2).normalized * jingVelocity.magnitude;
			}
		}
		else if (jingMoveStep == 2)
		{
			jingPos += jingVelocity * Time.deltaTime;
			if (MathUtils.isTargetArrived(jingTarget2, Vector2.zero, jingPos))
			{
				jingPos = Vector2.zero;
				jingStayReminTime = Utils.Random(jingStayTimeMin, jingStayTimeMax);
			}
		}
	}

	protected override void OnAddPart(int partId)
	{
		base.OnAddPart(partId);
		UpdateBulletMax();
		m_BulletYSpeed = (float)m_gunCfg.ZeroPointDistance / m_Factors[37] * 9.81f * 0.5f;
	}

	protected override void OnRemovePart(int partId)
	{
		base.OnRemovePart(partId);
		UpdateBulletMax();
		m_BulletYSpeed = (float)m_gunCfg.ZeroPointDistance / m_Factors[37] * 9.81f * 0.5f;
	}

	private void UpdateBulletMax()
	{
		BulletMax = GetIntFactor(34) + ExtraBulletNumBySkill;
	}

	private void ExtraBulletSkill()
	{
		float num = Battle.Ins.SelfPlayer.GetSkillValueIndex0(11) * 100f;
		ExtraBulletNumBySkill = (int)num;
	}

	private void PlayJingAni()
	{
		if (m_kaijingCoroutine != null)
		{
			Battle.StopConroutine(m_kaijingCoroutine);
		}
		if (m_gunCfg.needLashuan)
		{
			m_kaijingCoroutine = Battle.StartConroutine(PlayJujiJingAim());
		}
		else
		{
			m_kaijingCoroutine = Battle.StartConroutine(PlayNormalJingAnim());
		}
	}

	private IEnumerator PlayJujiJingAim()
	{
		yield return new WaitForSeconds(0.01f);
		jingPosOffet.y += 100f;
		jingPosOffet.x = Utils.Random(-30f, 30f);
		Battle.Ins.JingUI.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		Battle.Ins.MainCamera.shakeDistance = 0.3f;
		yield return new WaitForSeconds(0.2f);
		float scale = 1.2f;
		while (true)
		{
			jingPosOffet = Vector2.MoveTowards(jingPosOffet, Vector3.zero, Time.deltaTime * 400f);
			scale = Mathf.Lerp(scale, 1f, Time.deltaTime * 5f);
			Battle.Ins.MainCamera.shakeDistance = Mathf.Lerp(Battle.Ins.MainCamera.shakeDistance, 0f, Time.deltaTime * 5f);
			Battle.Ins.JingUI.transform.localScale = new Vector3(scale, scale, scale);
			if (jingPosOffet.sqrMagnitude < 0.01f)
			{
				break;
			}
			yield return null;
		}
		Battle.Ins.JingUI.transform.localScale = Vector3.one;
	}

	private IEnumerator PlayNormalJingAnim()
	{
		yield break;
	}

	public static void Shoot(int gunId, long shooterInsId, Vector3 shootPosition, Vector3 dir, int includeLayerMask, int excludeLayerMask, int hitProb = 10000)
	{
		dir = Vector3.Normalize(dir);
		GunCfg gunCfg = GunCfg.Get(gunId);
		int hitLayerMask = (0x6E1C8801 | includeLayerMask) & ~excludeLayerMask;
		float num = gunCfg.factors[50];
		if (gunCfg.sandanBulletNum > 0)
		{
			float num2 = 360f / (float)gunCfg.sandanBulletNum;
			float num3 = UnityEngine.Random.Range(0f, num2);
			Vector3 axis = Vector3.Cross(Vector3.up, dir);
			int i = 0;
			for (int sandanBulletNum = gunCfg.sandanBulletNum; i < sandanBulletNum; i++)
			{
				Vector3 vector = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, maxAngleOffset), axis) * dir;
				vector = Quaternion.AngleAxis(num3 + (float)i * num2, dir) * vector;
				BulletControl bulletControl = Battle.Ins.BulletPool.Get();
				bulletControl.Fire(gunId, shooterInsId, shootPosition, vector, vector * num, hitLayerMask, Utils.Random(0, 10000) >= hitProb);
			}
		}
		else
		{
			BulletControl bulletControl2 = Battle.Ins.BulletPool.Get();
			bulletControl2.Fire(gunId, shooterInsId, shootPosition, dir, dir * num, hitLayerMask, Utils.Random(0, 10000) >= hitProb);
		}
	}
}

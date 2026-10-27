using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class TurretGameObject : MonoBehaviour
{
	private bool _isOpen;

	private int _playerLayer;

	private long _turrentInstanceId;

	private int _turrentItemId;

	private GameObject _target;

	private BasePlayerController _targetBasePlayerController;

	private long _targetInstanceId;

	private float _checkDistance = 10f;

	private float _fleeDistance = 10f;

	private float _checkRange = 10f;

	private float _fireRange = 10f;

	private int _turretType;

	private readonly WaitForSeconds _waitFor01 = new WaitForSeconds(0.1f);

	public Transform HorizontalRotate;

	public Transform VerticalRotate;

	public Transform Muzzle;

	public Transform shootbulletPos;

	public Transform turretAnim;

	private float _bulletSpeed = 50f;

	private Vector3 offectVector3;

	private float _coolRemainTime;

	private float _coolRemainTimeCfg;

	private float _time01 = 0.1f;

	private string aniStr = "fire";

	private string _effectStr = "effect/EF_zidongpaotai_fire.ab";

	private Transform _infraredRayTransform;

	private Transform _infraredRayTransformScale;

	private Vector3 _defaultInfraredRayLength = new Vector3(1f, 1f, 10f);

	private Vector3 _rayScale = new Vector3(1f, 1f, 10f);

	private float _fireProbability = 10000f;

	private TurretCfg _turretCfg;

	private Coroutine _autoCor;

	private Coroutine _shotgunCor;

	private BasePlayerController.BodyPart _bodyPart = BasePlayerController.BodyPart.Body;

	private float _refreshTargeTime;

	private float _defaultRefreshTargePartTime;

	private float _angle;

	private Vector3 _horizontalRotate = Vector3.zero;

	private readonly Collider[] _doorCollider = new Collider[32];

	private Vector3 dir;

	private Vector3 dirAll;

	private float _randomEulerAngle;

	private float _randomEulerAngleTime;

	private static readonly Color DebugColor = Color.green;

	private float _changeTargetTime = 10f;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
	}

	private void RefreshRayTransform()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(Muzzle.position, Muzzle.transform.right, out hitInfo, _checkRange, -1 & ~(1 << LayerMask.NameToLayer("Window")), QueryTriggerInteraction.Ignore))
		{
			float newZ = Vector3.Distance(Muzzle.transform.position, hitInfo.collider.transform.position);
			_rayScale.Set(_infraredRayTransformScale.transform.localScale.x, _infraredRayTransformScale.transform.localScale.y, newZ);
			_infraredRayTransformScale.transform.localScale = _rayScale;
		}
		else
		{
			_infraredRayTransformScale.transform.localScale = _defaultInfraredRayLength;
		}
	}

	private void Start()
	{
		SShoot.handler = (SShoot.Handler)Delegate.Combine(SShoot.handler, new SShoot.Handler(SShootHanlde));
		HorizontalRotate = GetTransform(base.transform, "zuoyouzhuandong");
		VerticalRotate = GetTransform(base.transform, "shangxiazhuandong");
		Muzzle = GetTransform(base.transform, "muzzle");
		shootbulletPos = GetTransform(base.transform, "shootbulletpos");
		turretAnim = GetTransform(base.transform, "fire");
		_infraredRayTransform = GetTransform(base.transform, "EF_hongwaixian_01");
		_infraredRayTransformScale = GetTransform(base.transform, "lashenwo_01");
		_playerLayer = (1 << LayerMask.NameToLayer("OtherPlayerCollider")) | (1 << LayerMask.NameToLayer("Enemy"));
		if ((bool)VerticalRotate)
		{
			offectVector3 = Muzzle.transform.position - VerticalRotate.transform.position;
		}
	}

	private void SShootHanlde(SShoot msg)
	{
		if (msg.insId == _turrentInstanceId)
		{
			FireOther();
		}
	}

	public void SetData(long instanceId, int itemId)
	{
		_turrentItemId = itemId;
		_turretCfg = TurretCfg.Get(itemId);
		_defaultRefreshTargePartTime = _turretCfg.defaultRefreshTargePartTime;
		_checkRange = _turretCfg.checkRange;
		_fireRange = _turretCfg.fireRange;
		_fireProbability = _turretCfg.fireProbability;
		_turrentInstanceId = instanceId;
		_turretType = _turretCfg.type;
		aniStr = _turretCfg.fireAnimation;
		_effectStr = _turretCfg.fireEffect;
		if (_turretType == 2)
		{
			if (_autoCor == null)
			{
				_autoCor = StartCoroutine(CheckAotuTurrent());
			}
		}
		else if (_turretType == 1 && _shotgunCor == null)
		{
			_shotgunCor = StartCoroutine(CheckShotgunTurrent());
		}
		GunCfg gunCfg = GunCfg.Get(Singleton<TurretMgr>.Ins.GetTurretGunId(_turretCfg));
		_coolRemainTimeCfg = gunCfg.factors[53] * 0.001f;
		_bulletSpeed = gunCfg.factors[50];
		ResertCoolRemainTime();
	}

	protected void OnEnable()
	{
		if (_autoCor == null && _turretType == 2)
		{
			_autoCor = StartCoroutine(CheckAotuTurrent());
		}
		if (_shotgunCor == null && _turretType == 1)
		{
			_shotgunCor = StartCoroutine(CheckShotgunTurrent());
		}
	}

	protected void OnDisable()
	{
		if (_shotgunCor != null)
		{
			StopCoroutine(_shotgunCor);
		}
		if (_autoCor != null)
		{
			StopCoroutine(_autoCor);
		}
		_shotgunCor = null;
		_autoCor = null;
	}

	public void SetSwitch(bool isOpen)
	{
		_isOpen = isOpen;
	}

	private void ResertCoolRemainTime()
	{
		_coolRemainTime = _coolRemainTimeCfg;
	}

	public void RefreshTargetBody()
	{
		_refreshTargeTime -= Time.deltaTime;
		if (_refreshTargeTime <= 0f)
		{
			if ((bool)_target && (bool)_targetBasePlayerController)
			{
				BasePlayerController.BodyPart bodyPart = (BasePlayerController.BodyPart)UnityEngine.Random.Range(0, 16);
				_target = _targetBasePlayerController.GetBodyPart(bodyPart).gameObject;
			}
			_refreshTargeTime = _defaultRefreshTargePartTime;
		}
	}

	public void SetTarget(long targetInstanceId)
	{
		_targetInstanceId = targetInstanceId;
		_targetBasePlayerController = Battle.Ins.GetPlayer(targetInstanceId);
		if ((bool)_targetBasePlayerController)
		{
			_target = _targetBasePlayerController.GetBodyPart(BasePlayerController.BodyPart.Body).gameObject;
		}
	}

	public void UpdateTarget()
	{
		if (_targetInstanceId > 0)
		{
			_targetBasePlayerController = Battle.Ins.GetPlayer(_targetInstanceId);
			if ((bool)_targetBasePlayerController)
			{
				_target = _targetBasePlayerController.GetBodyPart(BasePlayerController.BodyPart.Body).gameObject;
			}
			else
			{
				Debug.LogError("_targetBasePlayerController is null");
			}
		}
	}

	public void CancelTarget()
	{
		_target = null;
		_targetBasePlayerController = null;
		_targetInstanceId = 0L;
	}

	private void AdjustAngle()
	{
		if ((bool)_target)
		{
			Vector3 vector = _target.transform.position - offectVector3;
			Vector3 vector2 = vector - HorizontalRotate.transform.position;
			Vector3 vector3 = vector2;
			vector3.y = 0f;
			_horizontalRotate = vector3;
			HorizontalRotate.transform.right = Vector3.Lerp(HorizontalRotate.transform.right, vector3, Time.deltaTime * cfg.Consts.TURRET_ROTATE_SPEED);
			Vector3 normalized = (vector - VerticalRotate.transform.position).normalized;
			if (normalized.y > 0f)
			{
				_angle = Vector3.Angle(new Vector3(normalized.x, 0f, normalized.z), normalized);
			}
			else
			{
				_angle = 0f - Vector3.Angle(new Vector3(normalized.x, 0f, normalized.z), normalized);
			}
			float z = VerticalRotate.transform.localEulerAngles.z;
			if (Mathf.Abs(z - _angle) <= 10f)
			{
				VerticalRotate.transform.localEulerAngles = new Vector3(0f, 0f, _angle);
			}
			else
			{
				VerticalRotate.transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, 0f, z), new Vector3(0f, 0f, _angle), Time.time);
			}
		}
	}

	private void FireBullet()
	{
		Vector3 position = shootbulletPos.transform.position;
		Gun.Shoot(_turrentItemId, _turrentInstanceId, position, shootbulletPos.transform.right, 1 << BattleScMgr.SelfPlayerLayer, 0);
		Singleton<TurretMgr>.Ins.Shoot(_turrentInstanceId);
	}

	private void FireOther()
	{
		if ((bool)turretAnim)
		{
			PlayFireAnim();
		}
		if ((bool)shootbulletPos)
		{
			SingletonMono<EffectMgr>.Ins.PlayEffectAtPos(_effectStr, shootbulletPos, Vector3.zero);
		}
		float num = Vector3.Distance(Battle.Ins.SelfPlayer.Pos, base.gameObject.transform.position);
		GunCfg gunCfg = GunCfg.Get(_turrentItemId);
		PlaySound(gunCfg.audios[(!(num < 100f)) ? 1 : 0], Singleton<BattleScMgr>.Ins.GetAudioPercent(1, num));
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

	private void Fire()
	{
		if (Singleton<TurretMgr>.Ins.ManagementHasBullet(_turrentInstanceId))
		{
			if ((bool)turretAnim)
			{
				PlayFireAnim();
			}
			if ((bool)shootbulletPos)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtPos(_effectStr, shootbulletPos, Vector3.zero);
			}
			FireBullet();
			Singleton<TurretMgr>.Ins.UseBullet(_turrentInstanceId);
		}
	}

	private void PlayFireAnim()
	{
		Animator component = turretAnim.GetComponent<Animator>();
		component.CrossFade(aniStr, 0.1f, 0);
		component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
	}

	private IEnumerator CheckAotuTurrent()
	{
		while (true)
		{
			if (!Singleton<TurretMgr>.Ins.ManagementHasBullet(_turrentInstanceId))
			{
				yield return _waitFor01;
			}
			Physics.OverlapSphereNonAlloc(base.transform.position, _checkRange, _doorCollider, _playerLayer);
			RefreshHatred();
			if ((bool)_target && (bool)_targetBasePlayerController)
			{
				if (!Singleton<TurretMgr>.Ins.IsContainRoleId(_turrentInstanceId, _targetBasePlayerController.RoleId))
				{
					Singleton<TurretMgr>.Ins.CancelTurretTargetRole(_turrentInstanceId);
				}
			}
			else
			{
				long hatredRoleId = Singleton<TurretMgr>.Ins.GetHatredRoleId(_turrentInstanceId);
				if (hatredRoleId > 0)
				{
					Singleton<TurretMgr>.Ins.TurretTargetRole(_turrentInstanceId, hatredRoleId);
				}
			}
			yield return _waitFor01;
		}
	}

	private void RefreshHatred()
	{
		List<long> list = new List<long>();
		for (int i = 0; i < _doorCollider.Length; i++)
		{
			if (!_doorCollider[i])
			{
				continue;
			}
			Transform parent = _doorCollider[i].gameObject.transform.parent;
			BasePlayerController component = parent.GetComponent<BasePlayerController>();
			if ((bool)component && !component.IsDie && Singleton<TurretMgr>.Ins.IsFire(_turrentInstanceId, component.RoleId))
			{
				Transform bodyPart = component.GetBodyPart(BasePlayerController.BodyPart.Body);
				if (IsWatch(Muzzle, bodyPart, _checkRange))
				{
					list.Add(component.RoleId);
				}
			}
		}
		Singleton<TurretMgr>.Ins.RefreshHatredList(_turrentInstanceId, list);
	}

	public void AttackTurret(long roleId)
	{
		long turretAttackRole = Singleton<TurretMgr>.Ins.GetTurretAttackRole(_turrentInstanceId);
		if (turretAttackRole > 0)
		{
			return;
		}
		BasePlayerController player = Battle.Ins.GetPlayer(roleId);
		if ((bool)player && !player.IsDie && Singleton<TurretMgr>.Ins.IsFire(_turrentInstanceId, roleId))
		{
			Transform bodyPart = player.GetBodyPart(BasePlayerController.BodyPart.Body);
			if (IsWatch(Muzzle, bodyPart, _fireRange))
			{
				Singleton<TurretMgr>.Ins.CancelTurretTargetRole(_turrentInstanceId);
				Singleton<TurretMgr>.Ins.SetTurretAttackRole(_turrentInstanceId, roleId);
			}
		}
	}

	private IEnumerator CheckShotgunTurrent()
	{
		while (true)
		{
			if (!Muzzle)
			{
				yield return _waitFor01;
			}
			RaycastHit hit;
			if (Physics.Raycast(Muzzle.transform.position, Muzzle.transform.right, out hit, _checkRange, _playerLayer))
			{
				Transform parent = hit.collider.gameObject.transform.parent;
				BasePlayerController component = parent.GetComponent<BasePlayerController>();
				if ((bool)_target)
				{
					if ((bool)component)
					{
						if (component.RoleId != _targetInstanceId)
						{
							if (Singleton<TurretMgr>.Ins.IsFire(_turrentInstanceId, component.RoleId))
							{
								Transform bodyPart = component.GetBodyPart(BasePlayerController.BodyPart.Body);
								if (IsWatch(Muzzle, bodyPart, _checkRange))
								{
									Singleton<TurretMgr>.Ins.TurretTargetRole(_turrentInstanceId, component.RoleId);
								}
							}
						}
						else
						{
							Transform bodyPart2 = component.GetBodyPart(BasePlayerController.BodyPart.Body);
							if (!IsWatch(Muzzle, bodyPart2, _checkRange))
							{
								Singleton<TurretMgr>.Ins.CancelTurretTargetRole(_turrentInstanceId);
							}
						}
					}
				}
				else if ((bool)component && Singleton<TurretMgr>.Ins.IsFire(_turrentInstanceId, component.RoleId))
				{
					Transform bodyPart3 = component.GetBodyPart(BasePlayerController.BodyPart.Body);
					if (IsWatch(Muzzle, bodyPart3, _checkRange))
					{
						Singleton<TurretMgr>.Ins.TurretTargetRole(_turrentInstanceId, component.RoleId);
					}
				}
			}
			else if ((bool)_target)
			{
				Singleton<TurretMgr>.Ins.CancelTurretTargetRole(_turrentInstanceId);
			}
			yield return _waitFor01;
		}
	}

	public bool IsWatch(Transform muzzleTransform, Transform targetWatch, float range)
	{
		dir = targetWatch.position - muzzleTransform.position;
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(Muzzle.position, dir, out hitInfo, range, -1 & ~(1 << LayerMask.NameToLayer("Window")), QueryTriggerInteraction.Ignore);
		Debug.DrawRay(Muzzle.position, dir, DebugColor, 1f);
		if (flag)
		{
			int layer = hitInfo.collider.gameObject.layer;
			if (layer != 8 && layer != 23 && layer != LayerMask.NameToLayer("OtherPlayerCollider") && layer != LayerMask.NameToLayer("Enemy"))
			{
				return false;
			}
			if (layer == LayerMask.NameToLayer("OtherPlayerCollider") || layer == LayerMask.NameToLayer("Enemy"))
			{
				Transform parent = hitInfo.collider.gameObject.transform.parent;
				BasePlayerController component = parent.GetComponent<BasePlayerController>();
				if (component != null)
				{
					Transform bodyPart = component.GetBodyPart(BasePlayerController.BodyPart.Body);
					if (bodyPart == targetWatch)
					{
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}

	public bool IsWatchRaycastAll(Transform muzzleTransform, Transform targetWatch)
	{
		dirAll = targetWatch.position - muzzleTransform.position;
		RaycastHit[] array = Physics.RaycastAll(Muzzle.position, dirAll, _checkRange);
		if (array.Length <= 0)
		{
			return false;
		}
		bool result = true;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (base.transform.isChild(raycastHit.transform))
			{
				continue;
			}
			int layer = raycastHit.collider.gameObject.layer;
			if (layer != 8 && layer != 23 && layer != 11 && layer != LayerMask.NameToLayer("OtherPlayerCollider") && layer != LayerMask.NameToLayer("Enemy"))
			{
				result = false;
			}
			if (layer != LayerMask.NameToLayer("OtherPlayerCollider") && layer != LayerMask.NameToLayer("Enemy") && layer != LayerMask.NameToLayer("Player"))
			{
				continue;
			}
			Transform parent = raycastHit.collider.gameObject.transform.parent;
			BasePlayerController component = parent.GetComponent<BasePlayerController>();
			if (component != null)
			{
				Transform bodyPart = component.GetBodyPart(BasePlayerController.BodyPart.Body);
				if (bodyPart == targetWatch)
				{
					return result;
				}
			}
		}
		return false;
	}

	private IEnumerator CheckCancelTurrent()
	{
		while (true)
		{
			RaycastHit hit;
			if (Physics.Raycast(Muzzle.transform.position, Muzzle.transform.right, out hit) && (bool)_target)
			{
				int layer = hit.collider.gameObject.layer;
				if (layer != 8 && layer != LayerMask.NameToLayer("OtherPlayerCollider") && layer != LayerMask.NameToLayer("Enemy"))
				{
					Singleton<TurretMgr>.Ins.CancelTurretTargetRole(_turrentInstanceId);
				}
			}
			yield return _waitFor01;
		}
	}

	private void Update()
	{
		if (!_isOpen || !Singleton<TurretMgr>.Ins.IsHasBullet(_turrentInstanceId))
		{
			if ((bool)_infraredRayTransformScale)
			{
				_infraredRayTransformScale.transform.localScale = new Vector3(1f, 1f, 0f);
			}
			return;
		}
		if ((bool)_infraredRayTransformScale)
		{
			RefreshRayTransform();
		}
		if (_turretType == 2)
		{
			AdjustAngle();
		}
		if ((bool)_target)
		{
			_coolRemainTime -= Time.deltaTime;
			if (_coolRemainTime <= 0f)
			{
				Fire();
				_coolRemainTime = _coolRemainTimeCfg;
			}
		}
		_time01 -= Time.deltaTime;
		if (_time01 <= 0f)
		{
			_time01 = 0.1f;
			if (_targetInstanceId > 0 && !_target)
			{
				UpdateTarget();
			}
		}
		if (_turretType == 2)
		{
			AdjustAngle();
			RandomPatrol();
		}
		RefreshTargetBody();
		AutoChangeTarget();
	}

	private void RandomPatrol()
	{
		SetPatrolTime();
		SetPatrolAngle();
	}

	private void SetPatrolTime()
	{
		if (Singleton<TurretMgr>.Ins.IsTurretManager(_turrentInstanceId) && !_target)
		{
			_randomEulerAngleTime -= Time.deltaTime;
			if (_randomEulerAngleTime < 0f)
			{
				_randomEulerAngle = UnityEngine.Random.Range(0, 360);
				_randomEulerAngleTime = 5f;
				Singleton<TurretMgr>.Ins.SynchronizedTurretPatrolAngle(_turrentInstanceId, _randomEulerAngle);
			}
		}
	}

	private void SetPatrolAngle()
	{
		if (!_target)
		{
			float angle = Singleton<TurretMgr>.Ins.GetAngle(_turrentInstanceId);
			Vector3 localEulerAngles = HorizontalRotate.transform.localEulerAngles;
			HorizontalRotate.transform.localEulerAngles = Vector3.Lerp(localEulerAngles, new Vector3(localEulerAngles.x, angle, localEulerAngles.z), Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		SShoot.handler = (SShoot.Handler)Delegate.Remove(SShoot.handler, new SShoot.Handler(SShootHanlde));
		Utils.TriggerEvent(TurrentEvent.DestoryTurrentDelegate, _turrentInstanceId);
	}

	private Transform GetTransform(Transform root, string name)
	{
		Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == name)
			{
				return transform;
			}
		}
		return null;
	}

	private void AutoChangeTarget()
	{
		_changeTargetTime -= Time.deltaTime;
		if (_changeTargetTime <= 0f)
		{
			Singleton<TurretMgr>.Ins.ClearTurretAttackRole(_turrentInstanceId);
			_changeTargetTime = 10f;
		}
	}
}

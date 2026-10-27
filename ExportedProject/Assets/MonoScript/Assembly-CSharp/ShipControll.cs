using System;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class ShipControll : MonoBehaviour, IVehicle
{
	internal enum SoundId
	{
		inWaterSoundId = 0,
		startEngineSoundId = 1,
		startEngineWithoutFuelSoundId = 2,
		engineIdleSoundId = 3,
		onLandEngineSoundId = 4,
		turnSoundId = 5,
		engineSoundId = 6,
		hitHardCoreSoundId = 7,
		hitSoftCoreSoundId = 8,
		onFireSoundId = 9,
		explosionSoundId = 10
	}

	private float _mEngineForce = 2000f;

	private Transform SelfTransform;

	private Rigidbody _mRigidbody;

	private Transform[] _mAttachPoints;

	private Transform _smokeBurnEffectParent;

	private Transform _mSmokeEffect;

	private Transform _mBurnEffect;

	private Transform _mExplosionEffect;

	private bool _effectVisible = true;

	private Transform _mSplashEffect;

	private Vector3 _mSmokeEffectPos;

	private Vector3 _mBurnEffectPos;

	private Vector3 _mExplosionEffectPos;

	private Vector3 _mSplashEffectPos;

	private Transform[] _mGlasses;

	private Transform _mCenter;

	private Transform _mTail;

	internal float _mHorizontalInput;

	internal float _mVerticalInput;

	private float _mMaxSpeed = 80f;

	private float _mSpeed;

	private float _mWaterLevel = 2f;

	private readonly float WaterResistance = 0.5f;

	private float _mRotateSpeed = 20f;

	private Vector2 _mHMove = Vector2.zero;

	private Vector2 hTilt = Vector2.zero;

	private Vector3 _mBouyanceForce;

	private bool _mIsInWater = true;

	private bool _mIsTailOnGround;

	private float _mAddBumpForceTime;

	internal bool _mNeedSync;

	internal bool _mHasInput;

	private bool _mEngineRunning;

	private int _mShipId;

	private int _mHp;

	private float _mFuel;

	private float _mFuelConsume;

	private float _mLastReportTime;

	private float _mLastSyncEngineTime;

	private float _mInWaterLength = 0.3f;

	private ShipCfg _mShipCfg;

	private CCauseDamgeToVehicle cCauseDamgeToVehicle = new CCauseDamgeToVehicle();

	private CSyncEngineSound cSyncEngineSound = new CSyncEngineSound();

	private CCauseDamgeByVehicle cCauseDamgeByVehicle = new CCauseDamgeByVehicle();

	private CSyncSound cSyncSound = new CSyncSound();

	private CStopVehicleSound cStopVehicleSound = new CStopVehicleSound();

	private float _mLastSpeed;

	private Vector3 _mForwardDirection;

	private VehicleMonitor vehicleMonitor;

	internal void PlaySound(SoundId soundId, bool loop = false, bool playMoreThanOne = false, float pitch = 1f)
	{
		SoundCfg soundCfg = SoundCfg.Get(_mShipCfg.soundIds[(int)soundId]);
		float minDist = ((soundId == SoundId.inWaterSoundId) ? 1 : 10);
		SingletonMono<AudioManager>.Ins.PlayOnTarget(soundCfg.path, base.gameObject, Vector3.zero, playMoreThanOne, (float)soundCfg.volume * 0.01f, pitch, minDist, soundCfg.radius, loop);
		if (!_mNeedSync)
		{
			return;
		}
		switch (soundId)
		{
		case SoundId.engineSoundId:
			if (Time.time - _mLastSyncEngineTime < 2f)
			{
				return;
			}
			_mLastSyncEngineTime = Time.time;
			break;
		case SoundId.inWaterSoundId:
			return;
		}
		cSyncSound.soundId = soundCfg.id;
		cSyncSound.id = _mShipId;
		cSyncSound.loop = loop;
		cSyncSound.minDist = minDist;
		cSyncSound.maxDist = soundCfg.radius;
		cSyncSound.playMoreThanOne = playMoreThanOne;
		cSyncSound.volume = (float)soundCfg.volume * 0.01f;
		cSyncSound.pitch = pitch;
		Client2Gs.Ins.Send(cSyncSound);
	}

	internal bool IsSoundPlaying(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(_mShipCfg.soundIds[(int)soundId]);
		return SingletonMono<AudioManager>.Ins.IsSoundPlaying(soundCfg.path, base.gameObject);
	}

	internal void StopPlaySound(SoundId soundId)
	{
		if (IsSoundPlaying(soundId))
		{
			SoundCfg soundCfg = SoundCfg.Get(_mShipCfg.soundIds[(int)soundId]);
			SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
			if (soundId == SoundId.engineIdleSoundId && _mNeedSync)
			{
				cStopVehicleSound.vehicleId = _mShipId;
				cStopVehicleSound.soundId = SoundCfg.Get(_mShipCfg.soundIds[(int)soundId]).id;
				Client2Gs.Ins.Send(cStopVehicleSound);
			}
		}
	}

	public void PlayHornSound()
	{
	}

	public void Init(ShipInfo shipInfo, ShipCfg shipCfg, SVehicleInfo sVehicleInfo)
	{
		SelfTransform = base.transform;
		_mShipCfg = shipCfg;
		_mRigidbody = GetComponent<Rigidbody>();
		_mRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		_mRigidbody.mass = _mShipCfg.mass;
		_mEngineForce = _mShipCfg.engineTorque;
		_mMaxSpeed = _mShipCfg.maxSpeed;
		_mRotateSpeed = _mShipCfg.turnSpeed;
		_mFuelConsume = _mShipCfg.fuelConsume;
		_mGlasses = shipInfo.glasses;
		_mSmokeEffectPos = shipInfo.smokeEffectPos;
		_mBurnEffectPos = shipInfo.burnEffectPos;
		_mExplosionEffectPos = shipInfo.explodeEffectPos;
		_mSplashEffectPos = shipInfo.splashEffectPos;
		_mAttachPoints = shipInfo.seats;
		_mCenter = shipInfo.turnCenter;
		_mTail = shipInfo.tail;
		_mHp = sVehicleInfo.vehicleInfo.hp;
		_mFuel = sVehicleInfo.vehicleInfo.fuel;
		_mShipId = sVehicleInfo.vehicleInfo.id;
		_mBouyanceForce = Vector3.up * Mathf.Abs(_mRigidbody.mass * Physics.gravity.y);
		OnHpChange(false);
		PlaySound(SoundId.inWaterSoundId, true);
		_smokeBurnEffectParent = new GameObject("__effects__").transform;
		_smokeBurnEffectParent.parent = SelfTransform;
		_smokeBurnEffectParent.localPosition = Vector3.zero;
		_smokeBurnEffectParent.localScale = Vector3.one;
		_smokeBurnEffectParent.localRotation = Quaternion.identity;
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_smoke.ab", OnSmokeEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_ranshao.ab", OnBurnEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_baozha.ab", OnExplosionEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/chuanlang_01.ab", OnSplashEffectLoaded);
	}

	private void OnSmokeEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mSmokeEffect = effectInfo.transform;
			_mSmokeEffect.SetParent(_smokeBurnEffectParent);
			_mSmokeEffect.localRotation = Quaternion.identity;
			_mSmokeEffect.SetLocalPosition(_mSmokeEffectPos.x, _mSmokeEffectPos.y, _mSmokeEffectPos.z);
			_mSmokeEffect.gameObject.SetActiveBetter(_effectVisible);
			OnHpChange(false);
		}
	}

	private void OnBurnEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mBurnEffect = effectInfo.transform;
			_mBurnEffect.SetParent(_smokeBurnEffectParent);
			_mBurnEffect.localRotation = Quaternion.identity;
			_mBurnEffect.SetLocalPosition(_mBurnEffectPos.x, _mBurnEffectPos.y, _mBurnEffectPos.z);
			_mBurnEffect.gameObject.SetActiveBetter(_effectVisible);
			OnHpChange(false);
		}
	}

	private void OnExplosionEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mExplosionEffect = effectInfo.transform;
			_mExplosionEffect.SetParent(SelfTransform);
			_mExplosionEffect.localRotation = Quaternion.identity;
			_mExplosionEffect.SetLocalPosition(_mExplosionEffectPos.x, _mExplosionEffectPos.y, _mExplosionEffectPos.z);
			OnHpChange(false);
		}
	}

	private void OnSplashEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mSplashEffect = effectInfo.transform;
			_mSplashEffect.SetParent(_smokeBurnEffectParent);
			_mSplashEffect.localRotation = Quaternion.identity;
			_mSplashEffect.SetLocalPosition(_mSplashEffectPos.x, _mSplashEffectPos.y, _mSplashEffectPos.z);
			_mSplashEffect.gameObject.SetActiveBetter(_effectVisible);
		}
	}

	public void SetAllEffectVisible(bool visible)
	{
		if (_effectVisible != visible)
		{
			_effectVisible = visible;
			_smokeBurnEffectParent.gameObject.SetActive(_effectVisible);
		}
	}

	private void FixedUpdate()
	{
		if (_mNeedSync)
		{
			Engine();
		}
		else
		{
			if (vehicleMonitor == null)
			{
				vehicleMonitor = GetComponent<VehicleMonitor>();
			}
			if (vehicleMonitor != null && _mSplashEffect != null)
			{
				_mSplashEffect.gameObject.SetActive(vehicleMonitor.Get3rdSpeed() >= 10f);
			}
		}
		Sound();
	}

	private void Sound()
	{
		if (!_mNeedSync || !(_mFuel > 0f) || IsBroken() || !_mEngineRunning)
		{
			return;
		}
		if (_mIsInWater)
		{
			PlaySound(SoundId.engineSoundId);
			if (Mathf.Abs(_mHorizontalInput) > 0.5f)
			{
				PlaySound(SoundId.turnSoundId);
			}
		}
		if (Mathf.Abs(_mVerticalInput) > 0.1f && base.transform.position.y >= _mWaterLevel)
		{
			PlaySound(SoundId.onLandEngineSoundId);
		}
	}

	private void Engine()
	{
		_mSpeed = Mathf.Max(Mathf.Sqrt(_mRigidbody.velocity.x * _mRigidbody.velocity.x + _mRigidbody.velocity.z * _mRigidbody.velocity.z), Mathf.Abs(_mRigidbody.velocity.y)) * 3.6f;
		if (_mLastSpeed < 60f && _mSpeed >= 60f)
		{
			Battle.Ins.MainCamera.ChangeState("DriveShipHighSpeed", true, 1f);
		}
		else if (_mLastSpeed >= 60f && _mSpeed < 60f)
		{
			Battle.Ins.MainCamera.ChangeState("DriveShip", true, 1f);
		}
		_mLastSpeed = _mSpeed;
		_mWaterLevel = Battle.Ins.GetWaterLevel(_mTail.position);
		_mIsInWater = _mTail.position.y <= _mWaterLevel;
		if (_mIsInWater)
		{
			_mFuel -= _mFuelConsume * Time.fixedDeltaTime;
			if (_mFuel < 0f)
			{
				_mFuel = 0f;
			}
			_mRigidbody.AddForce(_mBouyanceForce);
			if (_mTail.position.y < _mWaterLevel)
			{
				_mRigidbody.AddForce(_mBouyanceForce * Mathf.Clamp((_mWaterLevel - _mTail.position.y) * 0.5f, 0f, 1f));
			}
			if (_mSpeed > 0.1f)
			{
				_mRigidbody.AddForce(-_mRigidbody.velocity * WaterResistance * _mRigidbody.mass);
			}
		}
		if (_mSplashEffect != null)
		{
			_mSplashEffect.gameObject.SetActive(_mSpeed >= 10f);
		}
		if (Mathf.Abs(_mVerticalInput) > 0.01f && CanAcceptInput())
		{
			float num = _mEngineForce * _mVerticalInput;
			if (_mSpeed <= _mMaxSpeed)
			{
				_mForwardDirection = base.transform.forward;
				_mForwardDirection.y = 0f;
				_mRigidbody.AddForce(_mForwardDirection * num);
			}
		}
		float value = 0f;
		float num2 = 0f;
		if (_mHMove.x > 0f)
		{
			value = 0f - Time.fixedDeltaTime;
		}
		else if (_mHMove.x < 0f)
		{
			value = Time.fixedDeltaTime;
		}
		if (Mathf.Abs(_mHorizontalInput) > 0.1f)
		{
			value = _mHorizontalInput;
		}
		num2 = ((!(_mSpeed >= 60f)) ? (-2f * Time.fixedDeltaTime) : Time.fixedDeltaTime);
		_mHMove.x = Mathf.Clamp(value, -1f, 1f);
		_mHMove.y += Mathf.Clamp(num2, -1f, 1f);
		_mHMove.y = Mathf.Clamp01(_mHMove.y);
		hTilt.x = Mathf.Lerp(hTilt.x, _mHMove.x * 20f, Time.fixedDeltaTime);
		hTilt.y = Mathf.Lerp(hTilt.y, _mHMove.y * 7f, Time.fixedDeltaTime);
		if (Mathf.Abs(_mHorizontalInput) > 0.01f && CanAcceptInput())
		{
			base.transform.RotateAround(_mCenter.position, Vector3.up, _mHorizontalInput * _mRotateSpeed * Time.deltaTime);
		}
		if (CanAcceptInput())
		{
			_mRigidbody.transform.localRotation = Quaternion.Euler(0f - hTilt.y, _mRigidbody.transform.localEulerAngles.y, 0f - hTilt.x);
		}
		if (_mSpeed >= _mMaxSpeed * 0.6f && Time.time - _mAddBumpForceTime > 5f && base.transform.InverseTransformDirection(_mRigidbody.velocity).z > 0f)
		{
			_mAddBumpForceTime = Time.time;
			float num3 = _mRigidbody.mass * 1f;
			Vector3 vector = Vector3.RotateTowards(base.transform.forward, base.transform.up, (float)Math.PI / 3f, 1f);
			_mRigidbody.AddForce(vector * num3, ForceMode.Impulse);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == BattleScMgr.BulletLayer)
		{
			return;
		}
		ContactPoint[] contacts = collision.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (contactPoint.thisCollider.gameObject == _mTail.gameObject && (collision.gameObject.layer == BattleScMgr.DefaultLayer || collision.gameObject.layer == BattleScMgr.CarTriggerLayer))
			{
				_mIsTailOnGround = true;
				Debug.LogError("Tail hit default layer or other vehicle");
				break;
			}
		}
		if (collision.contacts.Length < 1 || collision.relativeVelocity.magnitude < 1f)
		{
			return;
		}
		bool flag = collision.gameObject.layer == BattleScMgr.OtherPlayerColliderLayer || collision.gameObject.layer == BattleScMgr.PlayerLayer;
		if (_mRigidbody.velocity.sqrMagnitude > 0.1f && _mNeedSync)
		{
			OnHit(_mSpeed);
			if (flag)
			{
				PlaySound(SoundId.hitSoftCoreSoundId);
			}
			else
			{
				PlaySound(SoundId.hitHardCoreSoundId);
			}
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		ContactPoint[] contacts = collisionInfo.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (contactPoint.thisCollider.gameObject == _mTail.gameObject)
			{
				_mIsTailOnGround = false;
				break;
			}
		}
	}

	private bool CanAcceptInput()
	{
		return _mIsInWater && !IsBroken() && _mFuel > 0f;
	}

	public float GetFuel()
	{
		return _mFuel;
	}

	public void SetFuel(float fuel)
	{
		_mFuel = fuel;
	}

	public int GetSpeed()
	{
		return (int)_mSpeed;
	}

	public void SyncEngineSound(float volume)
	{
	}

	public void KillOrStartEngine()
	{
		if (!_mEngineRunning)
		{
			StartEngine();
		}
		else
		{
			StopEngine();
		}
	}

	private void StartEngine()
	{
		_mEngineRunning = true;
		if (_mFuel > 0f)
		{
			PlaySound(SoundId.engineIdleSoundId, true);
			PlaySound(SoundId.startEngineSoundId);
		}
		else
		{
			PlaySound(SoundId.startEngineWithoutFuelSoundId);
		}
	}

	private void StopEngine()
	{
		_mEngineRunning = false;
		StopPlaySound(SoundId.engineIdleSoundId);
		StopPlaySound(SoundId.engineSoundId);
	}

	public int GetEngineSoundId()
	{
		return SoundCfg.Get(6).id;
	}

	public void OnDamage(float damage, Vector3 pos)
	{
		if (_mHp > 0)
		{
			ReduceHp((int)damage);
			cCauseDamgeToVehicle.damage = (int)damage;
			cCauseDamgeToVehicle.vehicleId = _mShipId;
			Client2Gs.Ins.Send(cCauseDamgeToVehicle);
		}
	}

	public Transform[] GetAttachPoints()
	{
		return _mAttachPoints;
	}

	public bool IsBroken()
	{
		return _mHp <= 0;
	}

	public float GetMaxFuel()
	{
		return _mShipCfg.maxFuel;
	}

	public void SetNeedSync(bool needSync)
	{
		_mNeedSync = needSync;
	}

	public void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		StopPlaySound(SoundId.engineSoundId);
		StopPlaySound(SoundId.engineIdleSoundId);
	}

	public void SetHp(int hp)
	{
		_mHp = hp;
		OnHpChange();
	}

	private void OnHpChange(bool showExplosion = true)
	{
		float num = (float)_mHp / (float)_mShipCfg.maxHp;
		if (_mSmokeEffect != null)
		{
			_mSmokeEffect.gameObject.SetActive(num <= 0.3f);
		}
		if (_mBurnEffect != null)
		{
			_mBurnEffect.gameObject.SetActive(num <= 0.1f);
		}
		if (_mHp > 0)
		{
			return;
		}
		ReplaceTexture[] componentsInChildren = GetComponentsInChildren<ReplaceTexture>();
		ReplaceTexture[] array = componentsInChildren;
		foreach (ReplaceTexture replaceTexture in array)
		{
			MeshRenderer component = replaceTexture.GetComponent<MeshRenderer>();
			if (component != null && replaceTexture.texture2Replace != null)
			{
				component.material.mainTexture = replaceTexture.texture2Replace;
			}
		}
		Transform[] mGlasses = _mGlasses;
		foreach (Transform transform in mGlasses)
		{
			transform.gameObject.SetActive(false);
		}
		if (showExplosion)
		{
			if (_mExplosionEffect != null)
			{
				_mExplosionEffect.gameObject.SetActive(true);
			}
			_mRigidbody.AddExplosionForce(10f, base.transform.position, 1f, 0f, ForceMode.VelocityChange);
			StopPlaySound(SoundId.engineIdleSoundId);
			PlaySound(SoundId.explosionSoundId);
		}
	}

	public int GetHp()
	{
		return _mHp;
	}

	public int GetMaxHp()
	{
		return _mShipCfg.maxHp;
	}

	private void ReduceHp(int damage)
	{
		if (_mHp > 0)
		{
			_mHp -= damage;
			if (_mHp < 0)
			{
				_mHp = 0;
			}
			OnHpChange();
			if (_mHp <= 0)
			{
				DoExplosion();
			}
		}
	}

	private void DoExplosion()
	{
		if (vehicleMonitor == null)
		{
			vehicleMonitor = GetComponent<VehicleMonitor>();
		}
		if (vehicleMonitor != null)
		{
			vehicleMonitor.OnExplosion();
		}
	}

	public void OnHit(float relativeSpeed)
	{
		int num = (int)(_mShipCfg.hitShipDropHpRate1 * relativeSpeed + _mShipCfg.hitShipDropHpRate1);
		int num2 = (int)((_mShipCfg.hitShipPlayerDropHpRate1 * relativeSpeed + _mShipCfg.hitShipPlayerDropHpRate1) * (float)ConstsBs.HpPlayer * 0.0001f);
		if (num > 0)
		{
			ReduceHp(num);
		}
		cCauseDamgeByVehicle.vehicleId = _mShipId;
		cCauseDamgeByVehicle.vehicleHp = _mHp;
		cCauseDamgeByVehicle.playerId2Damage.Clear();
		if (num2 > 0)
		{
			Transform[] mAttachPoints = _mAttachPoints;
			foreach (Transform transform in mAttachPoints)
			{
				if (transform != null)
				{
					BasePlayerController componentInChildren = transform.GetComponentInChildren<BasePlayerController>();
					if (componentInChildren != null)
					{
						cCauseDamgeByVehicle.playerId2Damage[componentInChildren.RoleId] = num2;
					}
				}
			}
		}
		if (num > 0 || num2 > 0)
		{
			Client2Gs.Ins.Send(cCauseDamgeByVehicle);
		}
	}
}

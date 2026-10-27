using System.Collections;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class HelicopterController : MonoBehaviour, IVehicle
{
	internal enum SoundId
	{
		startEngineSoundId = 0,
		startEngineWithoutFuelSoundId = 1,
		engineIdleSoundId = 2,
		engineLowSpeedSoundId = 3,
		engineHighSpeedSoundId = 4,
		landSoundId = 5,
		hitHardCoreSoundId = 6,
		hitSoftCoreSoundId = 7,
		rubSoundId = 8,
		fallHintSoundId = 9,
		fallSoundId = 10
	}

	private Rigidbody rigidBody;

	private Transform SelfTransform;

	private HeliRotorController mainRotorController;

	private HeliRotorController subRotorController;

	private float TiltForce = 10000f;

	private float TurnTiltSpeed = 30f;

	private float ForwardForce = 20000f;

	private float ForwardTiltSpeed = 20f;

	private float TurnSpeed = 30f;

	private float EffectiveHeight = 200f;

	private int planeId;

	private float _engineForce;

	private Transform _mTurnCenter;

	internal bool needSync;

	private int hp;

	private float _mLastReportTime;

	private PlaneCfg planeCfg;

	private Transform[] attachPoints;

	private Transform _smokeBurnEffectParent;

	private Transform _mSmokeEffect;

	private Transform _mBurnEffect;

	private Transform _mExplosionEffect;

	private Transform _mFlyEffect;

	private bool _effectVisible = true;

	private Vector3 _mSmokeEffectPos;

	private Vector3 _mBurnEffectPos;

	private Vector3 _mExplosionEffectPos;

	private Vector3 _mFlyEffectPos;

	private Transform[] _mGlasses;

	private Transform _mUndercarriage;

	private Ray _mRay;

	private int _mGroundLayer;

	private float _mCurrentHeight;

	private readonly float VolumeChangeSpeed = 3f;

	private Coroutine _mStopEngineSoundCoroutine;

	private Vector2 hMove = Vector2.zero;

	private Vector2 hTilt = Vector2.zero;

	private bool IsOnGround;

	internal float leftHorizontalInput;

	internal float leftVerticalInput;

	internal float rightHorizontalInput;

	internal float rightVeticalInput;

	internal bool hasInput;

	private float fuel = 100f;

	private float fuelConsume = 0.1f;

	private bool engineRunning;

	private CCauseDamgeToVehicle cCauseDamgeToVehicle = new CCauseDamgeToVehicle();

	private CSyncEngineSound cSyncEngineSound = new CSyncEngineSound();

	private CCauseDamgeByVehicle cCauseDamgeByVehicle = new CCauseDamgeByVehicle();

	private CSyncSound cSyncSound = new CSyncSound();

	private CStopVehicleSound cStopVehicleSound = new CStopVehicleSound();

	private float _mLastSyncEngineTime;

	private float _mMaxHeight;

	private readonly float ApplyBalanceEngineForceHeight = 10f;

	private float ChangeSpeedRate = 0.2f;

	private float MaxChangeSpeedRate = 0.4f;

	private float _mCurrentChangeSpeedRate;

	private readonly float WaitEngineStartTime = 1f;

	private float _mStartEngineTime;

	private float _mBalanceEngineForce;

	private Vector3 _mTileDirection;

	private bool _mSwingToLeft = true;

	private readonly float MAX_DELTA_X = 5f;

	private readonly float MINIAL_INPUT_VALUE = 0.05f;

	private bool _mStartSwing;

	private float _mSwingSpeed = 2f;

	private float _mWaterLevel = -1000f;

	private readonly float InWaterMaxChangeSpeedRate = 0.02f;

	private bool _mInWater;

	private float _mLowSpeedSoundVolume;

	private float _mHighSpeedSoundVolume;

	private float _mCurrentForwardTiltSpeed;

	private float _mCurrentTurnTiltSpeed;

	public float EngineForce
	{
		get
		{
			return _engineForce;
		}
		set
		{
			mainRotorController.RotarSpeed = value * 80f;
			subRotorController.RotarSpeed = value * 40f;
			_engineForce = value;
		}
	}

	internal void PlaySound(SoundId soundId, bool loop = false, bool playMoreThanOne = false, float volume = 1f, float pitch = 1f)
	{
		SoundCfg soundCfg = SoundCfg.Get(planeCfg.soundIds[(int)soundId]);
		SingletonMono<AudioManager>.Ins.PlayOnTarget(soundCfg.path, base.gameObject, Vector3.zero, playMoreThanOne, volume * (float)soundCfg.volume * 0.01f, pitch, 10f, soundCfg.radius, loop);
		if (!needSync)
		{
			return;
		}
		switch (soundId)
		{
		case SoundId.engineLowSpeedSoundId:
		case SoundId.engineHighSpeedSoundId:
			if (Time.time - _mLastSyncEngineTime < 2f)
			{
				return;
			}
			_mLastSyncEngineTime = Time.time;
			break;
		case SoundId.fallHintSoundId:
			return;
		}
		cSyncSound.soundId = soundCfg.id;
		cSyncSound.id = planeId;
		cSyncSound.loop = loop;
		cSyncSound.minDist = 40f;
		cSyncSound.maxDist = soundCfg.radius;
		cSyncSound.playMoreThanOne = playMoreThanOne;
		cSyncSound.volume = volume * (float)soundCfg.volume * 0.01f;
		cSyncSound.pitch = pitch;
		Client2Gs.Ins.Send(cSyncSound);
	}

	internal bool IsSoundPlaying(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(planeCfg.soundIds[(int)soundId]);
		return SingletonMono<AudioManager>.Ins.IsSoundPlaying(soundCfg.path, base.gameObject);
	}

	public void PlayHornSound()
	{
	}

	internal void StopPlaySound(SoundId soundId)
	{
		if (IsSoundPlaying(soundId))
		{
			SoundCfg soundCfg = SoundCfg.Get(planeCfg.soundIds[(int)soundId]);
			SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
			if ((soundId == SoundId.fallHintSoundId || soundId == SoundId.engineIdleSoundId || soundId == SoundId.engineHighSpeedSoundId || soundId == SoundId.engineLowSpeedSoundId) && needSync)
			{
				cStopVehicleSound.vehicleId = planeId;
				cStopVehicleSound.soundId = SoundCfg.Get(planeCfg.soundIds[(int)soundId]).id;
				Client2Gs.Ins.Send(cStopVehicleSound);
			}
		}
	}

	internal void StopPlaySound(int soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(soundId);
		if (soundCfg != null)
		{
			SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
		}
	}

	private void Awake()
	{
		_mRay.direction = Vector3.down;
		_mGroundLayer = LayerMask.GetMask("Default", "Water");
		SelfTransform = base.transform;
	}

	private void FixedUpdate()
	{
		if (needSync)
		{
			_mWaterLevel = Battle.Ins.GetWaterLevel(base.transform.position);
			if (base.transform.position.y + rigidBody.centerOfMass.y <= _mWaterLevel)
			{
				_mInWater = true;
				rigidBody.AddForce(-Vector3.up * rigidBody.mass * Physics.gravity.y * 1.05f);
				float b = Mathf.Clamp(Mathf.Abs(rigidBody.velocity.y) * 0.1f, 0f, MaxChangeSpeedRate);
				_mCurrentChangeSpeedRate = Mathf.Lerp(_mCurrentChangeSpeedRate, b, Time.fixedDeltaTime);
				rigidBody.AddForce(-rigidBody.velocity * _mCurrentChangeSpeedRate, ForceMode.VelocityChange);
			}
			if (!_mInWater && hasInput && !IsBroken())
			{
				Inputs();
				LiftProcess();
				TiltProcess();
				MoveProcess();
			}
		}
		if ((!needSync || _mInWater || !hasInput || IsBroken()) && EngineForce > 0.1f)
		{
			EngineForce = Mathf.Lerp(EngineForce, 0f, Time.fixedDeltaTime * 3f);
		}
		ReportEngineSound();
		UpdateFlyEffect();
		Sound();
	}

	private void ReportEngineSound()
	{
		if (needSync && engineRunning && Time.time - _mLastReportTime > 1f)
		{
			_mLastReportTime = Time.time;
			cSyncEngineSound.id = planeId;
			cSyncEngineSound.volume = EngineForce;
			Client2Gs.Ins.Send(cSyncEngineSound);
		}
	}

	public void Init(PlaneInfo planeInfo, PlaneCfg planeCfg, SVehicleInfo sVehicleInfo)
	{
		this.planeCfg = planeCfg;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.drag = 1f;
		rigidBody.angularDrag = 4f;
		rigidBody.mass = planeCfg.mass;
		ChangeSpeedRate = planeCfg.upChangeSpeedRate;
		MaxChangeSpeedRate = planeCfg.maxUpChangeSpeedRate;
		_mBalanceEngineForce = Mathf.Abs(Physics.gravity.y * planeCfg.mass);
		TiltForce = planeCfg.tiltForce;
		TurnTiltSpeed = planeCfg.tiltSpeed;
		ForwardForce = planeCfg.forwardForce;
		ForwardTiltSpeed = planeCfg.forwardSpeed;
		TurnSpeed = planeCfg.turnSpeed;
		_mMaxHeight = planeCfg.maxFlyHeight;
		fuelConsume = planeCfg.fuelConsume;
		planeId = sVehicleInfo.vehicleInfo.id;
		hp = sVehicleInfo.vehicleInfo.hp;
		fuel = sVehicleInfo.vehicleInfo.fuel;
		_mTurnCenter = planeInfo.turnCenter;
		attachPoints = planeInfo.seats;
		if (planeInfo.heliRotorModel != null)
		{
			mainRotorController = planeInfo.heliRotorModel.gameObject.AddComponent<HeliRotorController>();
			mainRotorController.RotateAxis = HeliRotorController.Axis.Y;
		}
		if (planeInfo.subHeliRotorModel != null)
		{
			subRotorController = planeInfo.subHeliRotorModel.gameObject.AddComponent<HeliRotorController>();
			subRotorController.RotateAxis = HeliRotorController.Axis.X;
			subRotorController._inputChangeRotateSpeed = true;
		}
		_mGlasses = planeInfo.glasses;
		_mSmokeEffectPos = planeInfo.smokeEffectPos;
		_mBurnEffectPos = planeInfo.burnEffectPos;
		_mExplosionEffectPos = planeInfo.explodeEffectPos;
		_mFlyEffectPos = planeInfo.flyRotorEffectPos;
		_mUndercarriage = planeInfo.undercarriages;
		OnHpChange(false);
		UpdateFlyEffect();
		_smokeBurnEffectParent = new GameObject("__effects__").transform;
		_smokeBurnEffectParent.parent = SelfTransform;
		_smokeBurnEffectParent.localPosition = Vector3.zero;
		_smokeBurnEffectParent.localScale = Vector3.one;
		_smokeBurnEffectParent.localRotation = Quaternion.identity;
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_smoke.ab", OnSmokeEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_ranshao.ab", OnBurnEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_baozha.ab", OnExplosionEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/luoxuanjiang.ab", OnFlyEffectLoaded);
	}

	public void SetAllEffectVisible(bool visible)
	{
		if (_effectVisible != visible)
		{
			_effectVisible = visible;
			_smokeBurnEffectParent.gameObject.SetActive(_effectVisible);
		}
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
			_mBurnEffect.SetLocalPosition(_mBurnEffectPos.x, _mBurnEffectPos.y, _mBurnEffectPos.z);
			_mBurnEffect.localRotation = Quaternion.identity;
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

	private void OnFlyEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mFlyEffect = effectInfo.transform;
			_mFlyEffect.SetParent(_smokeBurnEffectParent);
			_mFlyEffect.transform.localRotation = Quaternion.identity;
			_mFlyEffect.SetLocalPosition(_mFlyEffectPos.x, _mFlyEffectPos.y, _mFlyEffectPos.z);
			_mFlyEffect.gameObject.SetActiveBetter(_effectVisible);
		}
	}

	private bool HasPassenger()
	{
		Transform[] array = attachPoints;
		foreach (Transform transform in array)
		{
			if (transform.childCount > 0)
			{
				return true;
			}
		}
		return false;
	}

	private void Sound()
	{
		if (!needSync)
		{
			return;
		}
		if (fuel > 0f && !IsBroken() && !IsOnGround && engineRunning && !_mInWater)
		{
			if (GetSpeed() >= 50)
			{
				_mHighSpeedSoundVolume = Mathf.Lerp(_mHighSpeedSoundVolume, 1f, VolumeChangeSpeed * Time.fixedDeltaTime);
				_mLowSpeedSoundVolume = Mathf.Lerp(_mLowSpeedSoundVolume, 0f, VolumeChangeSpeed * Time.fixedDeltaTime);
			}
			else
			{
				_mHighSpeedSoundVolume = Mathf.Lerp(_mHighSpeedSoundVolume, 0f, VolumeChangeSpeed * Time.fixedDeltaTime);
				_mLowSpeedSoundVolume = Mathf.Lerp(_mLowSpeedSoundVolume, 1f, VolumeChangeSpeed * Time.fixedDeltaTime);
			}
		}
		else
		{
			_mHighSpeedSoundVolume = Mathf.Lerp(_mHighSpeedSoundVolume, 0f, VolumeChangeSpeed * Time.fixedDeltaTime);
			_mLowSpeedSoundVolume = Mathf.Lerp(_mLowSpeedSoundVolume, 0f, VolumeChangeSpeed * Time.fixedDeltaTime);
		}
		if (fuel <= 0f || IsBroken() || _mInWater)
		{
			StopPlaySound(SoundId.engineIdleSoundId);
		}
		if (_mHighSpeedSoundVolume > 0.05f && engineRunning && fuel > 0f && hp > 0)
		{
			PlaySound(SoundId.engineHighSpeedSoundId, true, false, _mHighSpeedSoundVolume);
		}
		else
		{
			StopPlaySound(SoundId.engineHighSpeedSoundId);
		}
		if (_mLowSpeedSoundVolume > 0.05f && engineRunning && fuel > 0f && hp > 0)
		{
			PlaySound(SoundId.engineLowSpeedSoundId, true, false, _mLowSpeedSoundVolume);
		}
		else
		{
			StopPlaySound(SoundId.engineLowSpeedSoundId);
		}
		if (!IsOnGround && (attachPoints[0].childCount == 0 || fuel <= 0f) && HasPassenger())
		{
			PlaySound(SoundId.fallHintSoundId, true);
		}
		else
		{
			StopPlaySound(SoundId.fallHintSoundId);
		}
	}

	private void MoveProcess()
	{
		if (!IsOnGround)
		{
			Vector3 forward = SelfTransform.forward;
			forward.y = 0f;
			rigidBody.AddForce(forward * hMove.y * ForwardForce);
		}
	}

	private void LiftProcess()
	{
		if (ApplyEngineForce())
		{
			rigidBody.AddForce(Vector3.up * EngineForce);
		}
	}

	private void TiltProcess()
	{
		if (!IsOnGround)
		{
			if (!_mStartSwing)
			{
				hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltSpeed, Time.deltaTime);
			}
			else
			{
				hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * MAX_DELTA_X, Time.deltaTime);
			}
			hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltSpeed, Time.deltaTime);
			rigidBody.transform.localRotation = Quaternion.Euler(hTilt.y, rigidBody.transform.localEulerAngles.y, 0f - hTilt.x);
			_mTileDirection = SelfTransform.right;
			_mTileDirection.y = 0f;
			rigidBody.AddForce(_mTileDirection * TiltForce * leftHorizontalInput);
		}
	}

	private bool ApplyEngineForce()
	{
		if (HasPassenger() && Time.time - _mStartEngineTime >= WaitEngineStartTime)
		{
			return true;
		}
		return false;
	}

	private void Inputs()
	{
		if (engineRunning)
		{
			fuel -= fuelConsume * Time.fixedDeltaTime;
			if (fuel < 0f)
			{
				fuel = 0f;
			}
		}
		float y = 0f;
		float x = 0f;
		if (hMove.y > 0f)
		{
			y = 0f - Time.fixedDeltaTime;
		}
		else if (hMove.y < 0f)
		{
			y = Time.fixedDeltaTime;
		}
		if (hMove.x > 0.1f)
		{
			x = 0f - Time.fixedDeltaTime;
		}
		else if (hMove.x < -0.1f)
		{
			x = Time.fixedDeltaTime;
		}
		if (rightVeticalInput > 0f && fuel > 0f)
		{
			if (_mStartEngineTime < 1f)
			{
				_mStartEngineTime = Time.time;
			}
			EngineForce = _mBalanceEngineForce;
			if (ApplyEngineForce())
			{
				float num = Mathf.Clamp(Mathf.Abs(leftVerticalInput) * 5f, 1f, 2f);
				if (_mCurrentHeight < ApplyBalanceEngineForceHeight)
				{
					_mCurrentChangeSpeedRate = Mathf.Lerp(_mCurrentChangeSpeedRate, MaxChangeSpeedRate, Time.fixedDeltaTime);
					rigidBody.AddForce(Vector3.up * _mCurrentChangeSpeedRate * rightVeticalInput * num, ForceMode.VelocityChange);
				}
				else if (_mCurrentHeight >= ApplyBalanceEngineForceHeight && rigidBody.transform.position.y < EffectiveHeight)
				{
					rigidBody.AddForce(Vector3.up * ChangeSpeedRate * rightVeticalInput * num, ForceMode.VelocityChange);
				}
			}
		}
		else if (rightVeticalInput < 0f || fuel <= 0f)
		{
			if (fuel > 0f)
			{
				rigidBody.AddForce(Vector3.up * ChangeSpeedRate * rightVeticalInput, ForceMode.VelocityChange);
			}
			else
			{
				EngineForce = Mathf.Lerp(EngineForce, 0f, Time.fixedDeltaTime * 3f);
			}
			if (EngineForce < 0f)
			{
				EngineForce = 0f;
			}
		}
		if (IsOnGround)
		{
			return;
		}
		if (Mathf.Abs(leftHorizontalInput) > 0.1f)
		{
			x = leftHorizontalInput;
		}
		if (Mathf.Abs(leftVerticalInput) > 0.1f)
		{
			y = leftVerticalInput;
		}
		if (Mathf.Abs(rightHorizontalInput) > 0.1f && EngineForce > 5f)
		{
			base.transform.RotateAround(_mTurnCenter.position, Vector3.up, rightHorizontalInput * Time.deltaTime * TurnSpeed);
		}
		if (_mCurrentHeight >= 10f && EngineForce > 0f)
		{
			if (Mathf.Abs(leftHorizontalInput) < MINIAL_INPUT_VALUE && Mathf.Abs(leftVerticalInput) < MINIAL_INPUT_VALUE && Mathf.Abs(rightHorizontalInput) < MINIAL_INPUT_VALUE && Mathf.Abs(rightVeticalInput) < MINIAL_INPUT_VALUE)
			{
				if (_mStartSwing)
				{
					if (_mSwingToLeft)
					{
						hMove.x = Mathf.Lerp(hMove.x, -1f, _mSwingSpeed * Time.fixedDeltaTime);
						if (hMove.x < 0f && Mathf.Abs(hMove.x + 1f) < 0.05f)
						{
							_mSwingToLeft = false;
						}
					}
					else
					{
						hMove.x = Mathf.Lerp(hMove.x, 1f, _mSwingSpeed * Time.fixedDeltaTime);
						if (hMove.x > 0f && Mathf.Abs(hMove.x - 1f) < 0.05f)
						{
							_mSwingToLeft = true;
						}
					}
				}
				else if (Mathf.Abs(hTilt.x) < 1f && Mathf.Abs(hTilt.y) < 1f)
				{
					_mStartSwing = true;
					_mSwingToLeft = true;
					hMove = Vector2.zero;
				}
			}
			else
			{
				_mStartSwing = false;
			}
		}
		else
		{
			_mStartSwing = false;
		}
		if (!_mStartSwing)
		{
			hMove.x = x;
			hMove.y = y;
		}
		hMove.x = Mathf.Clamp(hMove.x, -1f, 1f);
		hMove.y = Mathf.Clamp(hMove.y, -1f, 1f);
	}

	private void UpdateFlyEffect()
	{
		if (_mFlyEffect != null)
		{
			_mFlyEffect.gameObject.SetActive(mainRotorController.RotarSpeed > 10f);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == BattleScMgr.BulletLayer || col.gameObject.layer == BattleScMgr.CarForBulletLayer || col.gameObject.layer == BattleScMgr.GunLayer)
		{
			return;
		}
		bool flag = col.gameObject.layer == BattleScMgr.OtherPlayerColliderLayer;
		if (rigidBody.velocity.magnitude > 1f)
		{
			if (flag)
			{
				PlaySound(SoundId.hitSoftCoreSoundId);
				return;
			}
			PlaySound(SoundId.hitHardCoreSoundId);
			OnHit(GetSpeed(), false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == BattleScMgr.BulletLayer)
		{
			return;
		}
		if (collision.gameObject.layer == BattleScMgr.DefaultLayer)
		{
			IsOnGround = true;
		}
		if (collision.contacts.Length < 1 || collision.relativeVelocity.magnitude < 1f)
		{
			return;
		}
		float relativeSpeed = collision.relativeVelocity.magnitude * 3.6f;
		if (collision.gameObject.layer == BattleScMgr.CarTriggerLayer)
		{
			VehicleMonitor componentInParent = collision.gameObject.GetComponentInParent<VehicleMonitor>();
			if (componentInParent != null)
			{
				componentInParent.OnHit(relativeSpeed);
			}
		}
		bool flag = collision.gameObject.layer == BattleScMgr.OtherPlayerColliderLayer;
		if (!(rigidBody.velocity.sqrMagnitude > 0.1f) || !needSync)
		{
			return;
		}
		bool flag2 = false;
		ContactPoint[] contacts = collision.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (contactPoint.thisCollider.gameObject == _mUndercarriage.gameObject)
			{
				flag2 = true;
				break;
			}
		}
		OnHit(relativeSpeed, flag2);
		if (flag2)
		{
			if (rigidBody.velocity.x > 10f || rigidBody.velocity.z > 10f)
			{
				PlaySound(SoundId.rubSoundId);
			}
			else
			{
				PlaySound(SoundId.landSoundId);
			}
		}
		if (rigidBody.velocity.magnitude > 1f)
		{
			if (!flag && !flag2)
			{
				PlaySound(SoundId.hitHardCoreSoundId);
			}
			if (flag)
			{
				PlaySound(SoundId.hitSoftCoreSoundId);
			}
		}
		if (hp <= 0)
		{
			PlaySound(SoundId.fallSoundId);
		}
	}

	private void Update()
	{
		if (needSync && engineRunning)
		{
			_mRay.origin = base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(_mRay, out hitInfo, 1000f, _mGroundLayer))
			{
				EffectiveHeight = hitInfo.point.y + _mMaxHeight;
				IsOnGround = hitInfo.distance < 0.8f;
				_mCurrentHeight = hitInfo.distance;
			}
		}
	}

	private void OnHit(float relativeSpeed, bool landUseUndercarriage)
	{
		int num = 0;
		int num2 = 0;
		if (!landUseUndercarriage)
		{
			num = (int)(planeCfg.hitPlaneDropHpRate1 * relativeSpeed + planeCfg.hitPlaneDropHpRate2);
			num2 = (int)((planeCfg.hitPlanePlayerDropHpRate1 * relativeSpeed + planeCfg.hitPlanePlayerDropHpRate2) * (float)ConstsBs.HpPlayer * 0.0001f);
		}
		else
		{
			num = (int)(planeCfg.undercarriagHitDropHpRate1 * relativeSpeed + planeCfg.undercarriagHitDropHpRate2);
			num2 = (int)((planeCfg.undercarriagHitPlayerDropHpRate1 * relativeSpeed + planeCfg.undercarriagHitPlayerDropHpRate2) * (float)ConstsBs.HpPlayer * 0.0001f);
		}
		if (num > 0)
		{
			ReduceHp(num);
		}
		cCauseDamgeByVehicle.vehicleId = planeId;
		cCauseDamgeByVehicle.vehicleHp = hp;
		cCauseDamgeByVehicle.playerId2Damage.Clear();
		if (num2 > 0)
		{
			Transform[] array = attachPoints;
			foreach (Transform transform in array)
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
		if (num > 0 || cCauseDamgeByVehicle.playerId2Damage.Count > 0)
		{
			Client2Gs.Ins.Send(cCauseDamgeByVehicle);
		}
	}

	public void OnHit(float relativeSpeed)
	{
		OnHit(relativeSpeed, false);
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.layer == BattleScMgr.DefaultLayer)
		{
			IsOnGround = false;
		}
	}

	public float GetFuel()
	{
		return fuel;
	}

	public void SetFuel(float fuel)
	{
		this.fuel = fuel;
	}

	public int GetHp()
	{
		return hp;
	}

	public int GetMaxHp()
	{
		return planeCfg.maxHp;
	}

	public int GetSpeed()
	{
		float b = (int)(Mathf.Sqrt(rigidBody.velocity.x * rigidBody.velocity.x + rigidBody.velocity.z * rigidBody.velocity.z) * 3.6f);
		float a = Mathf.Abs(rigidBody.velocity.y * 3.6f);
		return (int)Mathf.Max(a, b);
	}

	public void SyncEngineSound(float volume)
	{
		mainRotorController.RotarSpeed = volume * 80f;
		subRotorController.RotarSpeed = volume * 40f;
	}

	public void KillOrStartEngine()
	{
		if (!engineRunning)
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
		if (_mStopEngineSoundCoroutine != null)
		{
			StopCoroutine(_mStopEngineSoundCoroutine);
		}
		engineRunning = true;
		if (fuel > 0f)
		{
			EngineForce = 5f;
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
		_mStartEngineTime = 0f;
		engineRunning = false;
		cSyncEngineSound.id = planeId;
		cSyncEngineSound.volume = 0f;
		Client2Gs.Ins.Send(cSyncEngineSound);
		_mStopEngineSoundCoroutine = StartCoroutine(StopEngineSound());
	}

	private IEnumerator StopEngineSound()
	{
		int number = 5;
		float volume = 1f;
		while (number > 0)
		{
			volume -= 0.2f;
			PlaySound(SoundId.engineIdleSoundId, true, false, volume);
			number--;
			yield return new WaitForSeconds(0.8f);
		}
		StopPlaySound(SoundId.engineIdleSoundId);
	}

	public int GetEngineSoundId()
	{
		return SoundCfg.Get(4).id;
	}

	public void OnDamage(float damage, Vector3 pos)
	{
		if (hp > 0)
		{
			ReduceHp((int)damage);
			cCauseDamgeToVehicle.damage = (int)damage;
			cCauseDamgeToVehicle.vehicleId = planeId;
			Client2Gs.Ins.Send(cCauseDamgeToVehicle);
		}
	}

	private void ReduceHp(int damage)
	{
		if (hp > 0)
		{
			hp -= damage;
			if (hp < 0)
			{
				hp = 0;
			}
			OnHpChange();
			if (hp <= 0)
			{
				DoExplosion();
			}
		}
	}

	private void DoExplosion()
	{
		VehicleMonitor component = GetComponent<VehicleMonitor>();
		component.OnExplosion();
	}

	private void OnHpChange(bool showExplosion = true)
	{
		float num = (float)hp / (float)planeCfg.maxHp;
		if (_mSmokeEffect != null)
		{
			_mSmokeEffect.gameObject.SetActive(num <= 0.3f);
		}
		if (_mBurnEffect != null)
		{
			_mBurnEffect.gameObject.SetActive(num <= 0.1f);
		}
		if (hp > 0)
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
			rigidBody.AddExplosionForce(10f, base.transform.position, 1f, 0f, ForceMode.VelocityChange);
			StopPlaySound(SoundId.fallHintSoundId);
			StopPlaySound(SoundId.engineIdleSoundId);
		}
	}

	public void SetNeedSync(bool needSync)
	{
		this.needSync = needSync;
	}

	public void SetHp(int hp)
	{
		this.hp = hp;
		OnHpChange();
	}

	public Transform[] GetAttachPoints()
	{
		return attachPoints;
	}

	public bool IsBroken()
	{
		return hp <= 0;
	}

	public float GetMaxFuel()
	{
		return planeCfg.maxFuel;
	}

	public void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		StopPlaySound(sStopVehicleSound.soundId);
	}

	public float GetHeight()
	{
		if (needSync)
		{
			return _mCurrentHeight;
		}
		_mRay.origin = base.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(_mRay, out hitInfo, 1000f, _mGroundLayer))
		{
			return hitInfo.distance;
		}
		return 0f;
	}

	public float GetMaxHeigth()
	{
		return _mMaxHeight;
	}
}

using System.Collections;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

[RequireComponent(typeof(Rigidbody))]
public class TwoWheelMotoController : MonoBehaviour, IVehicle
{
	internal enum SoundId
	{
		startEngineSoundId = 0,
		startEngineWithoutFuelSoundId = 1,
		engineIdleSoundId = 2,
		engineLowSpeedSoundId = 3,
		engineNormalSpeedSoundId = 4,
		engineHighSpeedSoundId = 5,
		engineTurboSoundId = 6,
		engineStopSoundId = 7,
		onHardRoadSoundId = 8,
		onMudSoundId = 9,
		onSoftRoadSoundId = 10,
		onGrassRoadSoundId = 11,
		onRockRoadSoundId = 12,
		onWaterSoundId = 13,
		brakeOnHighSpeedSoundId = 14,
		brakeOnLowSpeedSoundId = 15,
		rollOverSoundId = 16,
		hitPlayerSoundId = 17,
		crushPlayerSoundId = 18,
		hitCarSoundId = 19,
		hitWallSoundId = 20,
		hitGroundSoundId = 21,
		hitWaterSoundId = 22,
		onFireSoundId = 23,
		explosionSoundId = 24,
		explosionWithBarrierSoundId = 25,
		hornSoundId = 26,
		glassBrokenSoundId = 27,
		tireBrokenSoundId = 28,
		tireSkipSoundId = 29
	}

	internal bool hasInput;

	[HideInInspector]
	public float gasInput;

	[HideInInspector]
	public float brakeInput;

	[HideInInspector]
	public float steerInput;

	[HideInInspector]
	public float nitrogenInput;

	private Rigidbody _mRigidbody;

	private Transform SelfTransform;

	internal bool sleepingRigid;

	public Transform[] attachPoints;

	public Transform leftHandAttachPoint;

	public Transform rightHandAttachPoint;

	private TwoWheelMotoCfg _mTwoWheelMotoCfg;

	private int carId;

	private float hp;

	private float fuel;

	private float fuelConsume;

	private float extraFuelConsume;

	private float extraEngineRate;

	private Transform frontWheelModel;

	private Transform rearWheelModel;

	private WheelCollider frontWheelCollider;

	private WheelCollider rearWheelCollider;

	private Transform frontWheelTransform;

	private Transform rearWheelTransform;

	private Transform _smokeBurnEffectParent;

	private Transform _mSmokeEffect;

	private Transform _mBurnEffect;

	private Transform _mExplosionEffect;

	private Transform _mNitrogenEffect;

	private float SteerAngle = 80f;

	private float highspeedsteerAngle = 15f;

	private float highspeedsteerAngleAtspeed = 100f;

	private float engineTorque = 3000f;

	private float brakeTorque = 2500f;

	private float maxspeed = 220f;

	private float maxSpeedWithNitrogen = 250f;

	private bool _effectVisible = true;

	private Vector3 _mSmokeEffectPos;

	private Vector3 _mBurnEffectPos;

	private Vector3 _mExplosionEffectPos;

	private Vector3 _mNitrogenEffectPos;

	private readonly float OnFireHpRate = 0.1f;

	private readonly float SmokeHpRate = 0.3f;

	private float minimumCollisionForce = 1f;

	private static readonly CSyncSound cSyncSound = new CSyncSound();

	private static readonly CCauseDamgeToVehicle cCauseDamgeToVehicle = new CCauseDamgeToVehicle();

	private static readonly CCauseDamgeByVehicle cCauseDamgeByVehicle = new CCauseDamgeByVehicle();

	private static readonly CStopVehicleSound cStopVehicleSound = new CStopVehicleSound();

	private static readonly CVehicleHitPlayer cVehicleHitPlayer = new CVehicleHitPlayer();

	private static readonly CSyncCarWheelRotation cSyncCarWheelRotation = new CSyncCarWheelRotation();

	private float _mSyncWheelTime;

	private bool _mStopIdleSound = true;

	private float _mReportEngineSoundTime;

	internal bool needSync;

	private float engineVolumeFromSync;

	private float engineVolume;

	private float _mSyncBrakeSoundTime;

	private float _mTriggerSoundTime;

	private float m_HitTimeLimit;

	private float _mWaterLevel = -1000f;

	private readonly float MaxChangeSpeedRate = 0.02f;

	private float _mCurrentChangeSpeedRate;

	private bool engineRunning;

	private float _mFrontYRotation;

	private float _mTailYRotation;

	private VehicleMonitor _mVihicleMonitor;

	private float fuelInput;

	private int direction = 1;

	private Transform handTransform;

	public Transform COM;

	private bool changingGear;

	private float gearShiftRate = 10f;

	private float[] gearSpeed;

	private int currentGear;

	private int totalGears = 6;

	private float speed;

	private float shiftGearTime;

	private Transform chassis;

	private float chassisVerticalLean = 4f;

	private float chassisHorizontalLean = 20f;

	private float horizontalLean;

	private float verticalLean;

	private float MaxEngineRPM = 6000f;

	private float MinEngineRPM = 1000f;

	private float EngineRPM;

	private float defsteerAngle;

	private bool reversing;

	private Vector3 handVector3 = default(Vector3);

	private Quaternion quaternion = Quaternion.Euler(-120f, 0f, 0f);

	private Quaternion quaternion90 = Quaternion.Euler(-90f, 0f, 0f);

	private Vector3 axis;

	private float wheelRotation;

	private Vector3 vector3;

	private float _mSyncFrontWheelRotation;

	private float _mSyncRearWheelRotation;

	private float wheelRotationToSync;

	private float _mRpm;

	private int _totalGears
	{
		get
		{
			return totalGears - 1;
		}
	}

	internal float _gasInput
	{
		get
		{
			if (fuelInput <= 0.25f)
			{
				return 0f;
			}
			return (direction != 1) ? Mathf.Clamp01(brakeInput) : Mathf.Clamp01(gasInput);
		}
		set
		{
			gasInput = value;
		}
	}

	internal float _brakeInput
	{
		get
		{
			return (direction != 1) ? Mathf.Clamp01(gasInput) : Mathf.Clamp01(brakeInput);
		}
		set
		{
			brakeInput = value;
		}
	}

	internal float _nitrogenInput
	{
		get
		{
			return nitrogenInput;
		}
		set
		{
			nitrogenInput = value;
			if (nitrogenInput > 0f)
			{
				gasInput = 1f;
			}
		}
	}

	public float FrontWheelColliderSteerAngle
	{
		get
		{
			return frontWheelCollider.steerAngle;
		}
	}

	private void Awake()
	{
		SelfTransform = base.transform;
	}

	public void Init(TwoWheelMotoInfo twoWheelMotoInfo, TwoWheelMotoCfg twoWheelMotoCfg, SVehicleInfo sVehicleInfo)
	{
		_mTwoWheelMotoCfg = twoWheelMotoCfg;
		_mRigidbody = GetComponent<Rigidbody>();
		_mRigidbody.drag = 0.05f;
		_mRigidbody.angularDrag = 0.25f;
		_mRigidbody.mass = twoWheelMotoCfg.mass;
		totalGears = twoWheelMotoCfg.gearNum;
		shiftGearTime = twoWheelMotoCfg.switchGearTime;
		_mRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
		COM = twoWheelMotoInfo.com;
		chassis = twoWheelMotoInfo.chassis;
		COM.transform.localPosition = new Vector3(_mTwoWheelMotoCfg.comX, _mTwoWheelMotoCfg.comY, _mTwoWheelMotoCfg.comZ);
		carId = sVehicleInfo.vehicleInfo.id;
		hp = sVehicleInfo.vehicleInfo.hp;
		fuel = sVehicleInfo.vehicleInfo.fuel;
		fuelConsume = twoWheelMotoCfg.fuelConsume;
		extraFuelConsume = twoWheelMotoCfg.extraFuelConsume;
		extraEngineRate = twoWheelMotoCfg.extraEngineTorqueRate;
		attachPoints = twoWheelMotoInfo.seats;
		leftHandAttachPoint = twoWheelMotoInfo.leftHandAttachPoint;
		rightHandAttachPoint = twoWheelMotoInfo.rightHandAttachPoint;
		frontWheelCollider = twoWheelMotoInfo.frontWheelCollider;
		frontWheelModel = twoWheelMotoInfo.frontWheelModel;
		rearWheelCollider = twoWheelMotoInfo.rearWheelCollider;
		rearWheelModel = twoWheelMotoInfo.rearWheelModel;
		frontWheelTransform = frontWheelCollider.transform;
		rearWheelTransform = rearWheelCollider.transform;
		_mSmokeEffectPos = twoWheelMotoInfo.smokeEffectPos;
		_mBurnEffectPos = twoWheelMotoInfo.burnEffectPos;
		_mExplosionEffectPos = twoWheelMotoInfo.explodeEffectPos;
		_mNitrogenEffectPos = twoWheelMotoInfo.nitrogenEffectPos;
		handTransform = twoWheelMotoInfo.handTransform;
		SteerAngle = twoWheelMotoCfg.steerAngle;
		defsteerAngle = SteerAngle;
		highspeedsteerAngle = twoWheelMotoCfg.highspeedsteerAngle;
		highspeedsteerAngleAtspeed = twoWheelMotoCfg.highspeedsteerAngleAtspeed;
		engineTorque = twoWheelMotoCfg.engineTorque;
		brakeTorque = twoWheelMotoCfg.brakeTorque;
		maxspeed = twoWheelMotoCfg.maxSpeed;
		maxSpeedWithNitrogen = twoWheelMotoCfg.maxSpeedWithNitrogen;
		InitGearSpeed();
		_smokeBurnEffectParent = new GameObject("__effects__").transform;
		_smokeBurnEffectParent.parent = SelfTransform;
		_smokeBurnEffectParent.localPosition = Vector3.zero;
		_smokeBurnEffectParent.localScale = Vector3.one;
		_smokeBurnEffectParent.localRotation = Quaternion.identity;
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_smoke.ab", OnSmokeEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_ranshao.ab", OnBurnEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_baozha.ab", OnExplosionEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_qiliu_01.ab", OnNitrogenEffectLoaded);
		OnHpChange(false);
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

	private void OnNitrogenEffectLoaded(EffectInfo effectInfo)
	{
		if ((bool)this)
		{
			_mNitrogenEffect = effectInfo.transform;
			_mNitrogenEffect.SetParent(SelfTransform);
			_mNitrogenEffect.localRotation = Quaternion.identity;
			_mNitrogenEffect.SetLocalPosition(_mNitrogenEffectPos.x, _mNitrogenEffectPos.y, _mNitrogenEffectPos.z);
		}
	}

	internal void PlayNitrogenSound()
	{
		if (fuel > 0f)
		{
			PlaySound(SoundId.engineTurboSoundId);
		}
	}

	internal void PlaySound(SoundId soundId, bool loop = false, bool playMoreThanOne = false, float volume = 1f, float pitch = 1f, bool syncSound = true)
	{
		SoundCfg soundCfg = SoundCfg.Get(_mTwoWheelMotoCfg.soundIds[(int)soundId]);
		SingletonMono<AudioManager>.Ins.PlayOnTarget(soundCfg.path, base.gameObject, Vector3.zero, playMoreThanOne, volume * (float)soundCfg.volume * 0.01f, pitch, 10f, soundCfg.radius, loop);
		if (!syncSound || !needSync)
		{
			return;
		}
		if (soundId == SoundId.brakeOnHighSpeedSoundId || soundId == SoundId.brakeOnLowSpeedSoundId)
		{
			if (Time.time - _mSyncBrakeSoundTime < 3f)
			{
				return;
			}
			_mSyncBrakeSoundTime = Time.time;
		}
		cSyncSound.soundId = soundCfg.id;
		cSyncSound.id = carId;
		cSyncSound.loop = loop;
		cSyncSound.minDist = 10f;
		cSyncSound.maxDist = soundCfg.radius;
		cSyncSound.playMoreThanOne = playMoreThanOne;
		cSyncSound.volume = volume;
		cSyncSound.pitch = pitch;
		Client2Gs.Ins.Send(cSyncSound);
	}

	internal void StopPlaySound(SoundId soundId)
	{
		if (IsSoundPlaying(soundId))
		{
			SoundCfg soundCfg = SoundCfg.Get(_mTwoWheelMotoCfg.soundIds[(int)soundId]);
			SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
		}
		if (soundId == SoundId.engineIdleSoundId && needSync)
		{
			cStopVehicleSound.vehicleId = carId;
			cStopVehicleSound.soundId = SoundCfg.Get(_mTwoWheelMotoCfg.soundIds[(int)soundId]).id;
			Client2Gs.Ins.Send(cStopVehicleSound);
		}
	}

	internal bool IsSoundPlaying(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(_mTwoWheelMotoCfg.soundIds[(int)soundId]);
		return SingletonMono<AudioManager>.Ins.IsSoundPlaying(soundCfg.path, base.gameObject);
	}

	private void OnHpChange(bool showExplosion = true)
	{
		float num = hp / (float)_mTwoWheelMotoCfg.maxHp;
		if (_mSmokeEffect != null)
		{
			_mSmokeEffect.gameObject.SetActive(num <= SmokeHpRate);
		}
		if (_mBurnEffect != null)
		{
			_mBurnEffect.gameObject.SetActive(num <= OnFireHpRate);
		}
		if (num <= OnFireHpRate)
		{
			PlaySound(SoundId.onFireSoundId, true, false, 1f, 1f, false);
		}
		if (!(hp <= 0f))
		{
			return;
		}
		ReplaceTexture[] componentsInChildren = GetComponentsInChildren<ReplaceTexture>();
		if (componentsInChildren != null)
		{
			ReplaceTexture[] array = componentsInChildren;
			foreach (ReplaceTexture replaceTexture in array)
			{
				MeshRenderer component = replaceTexture.GetComponent<MeshRenderer>();
				if (component != null && replaceTexture.texture2Replace != null)
				{
					component.material.mainTexture = replaceTexture.texture2Replace;
				}
			}
		}
		if (showExplosion)
		{
			if (_mExplosionEffect != null)
			{
				_mExplosionEffect.gameObject.SetActive(true);
			}
			_mRigidbody.AddExplosionForce(10f, base.transform.position, 1f, 0f, ForceMode.VelocityChange);
			StopPlaySound(SoundId.engineIdleSoundId);
			StopPlaySound(SoundId.engineHighSpeedSoundId);
			PlaySound(SoundId.explosionSoundId, false, false, 1f, 1f, false);
		}
	}

	private void Update()
	{
		if (!IsBroken())
		{
			WheelAlign();
			if (needSync)
			{
				Lean();
			}
		}
		if (m_HitTimeLimit > 0f)
		{
			m_HitTimeLimit -= Time.deltaTime;
		}
		Sound();
	}

	private void FixedUpdate()
	{
		if (needSync)
		{
			if (Battle.Ins != null)
			{
				_mWaterLevel = Battle.Ins.GetWaterLevel(base.transform.position);
			}
			if (base.transform.position.y + _mRigidbody.centerOfMass.y <= _mWaterLevel)
			{
				_mRigidbody.AddForce(-Vector3.up * _mRigidbody.mass * Physics.gravity.y * 1.01f);
				float b = Mathf.Clamp(Mathf.Abs(_mRigidbody.velocity.y) * 0.1f, 0f, MaxChangeSpeedRate);
				_mCurrentChangeSpeedRate = Mathf.Lerp(_mCurrentChangeSpeedRate, b, Time.fixedDeltaTime);
				_mRigidbody.AddForce(-_mRigidbody.velocity * _mCurrentChangeSpeedRate, ForceMode.VelocityChange);
			}
			SyncWheelRmpAndRotation();
			if (_mNitrogenEffect != null)
			{
				if (fuel > 0f && nitrogenInput > 0f)
				{
					_mNitrogenEffect.gameObject.SetActive(true);
				}
				else
				{
					_mNitrogenEffect.gameObject.SetActive(false);
				}
			}
		}
		if (!IsBroken())
		{
			if (needSync)
			{
				Engine();
				ApplySteering();
				Braking();
				ShiftGears();
			}
		}
		else if (_mRigidbody.velocity.magnitude > 0.1f)
		{
			ApplyBrakeTorque(frontWheelCollider, brakeTorque);
			ApplyBrakeTorque(rearWheelCollider, brakeTorque);
		}
	}

	private void Engine()
	{
		float num = 0f;
		if (nitrogenInput > 0f)
		{
			num += extraFuelConsume * Time.fixedDeltaTime;
		}
		if (Mathf.Abs(gasInput) > 0.1f)
		{
			num += Mathf.Abs(gasInput) * fuelConsume * Time.fixedDeltaTime;
		}
		fuel -= num;
		if (fuel < 0f)
		{
			fuel = 0f;
		}
		speed = _mRigidbody.velocity.magnitude * 3.6f;
		SelfTransform.eulerAngles = new Vector3(SelfTransform.eulerAngles.x, SelfTransform.eulerAngles.y, 0f);
		if (gasInput < 0f && (SelfTransform.InverseTransformDirection(_mRigidbody.velocity).z <= 0f || speed < 1f))
		{
			reversing = true;
			direction = -1;
		}
		else
		{
			reversing = false;
			direction = 1;
		}
		SteerAngle = Mathf.Lerp(defsteerAngle, highspeedsteerAngle, speed / highspeedsteerAngleAtspeed);
		frontWheelCollider.steerAngle = SteerAngle * steerInput;
		EngineRPM = Mathf.Clamp((Mathf.Abs(frontWheelCollider.rpm + rearWheelCollider.rpm) * gearShiftRate + MinEngineRPM) / (float)(currentGear + 1), MinEngineRPM, MaxEngineRPM);
		if (OverTorque())
		{
			rearWheelCollider.motorTorque = 0f;
		}
		else if (!reversing && !changingGear)
		{
			if (nitrogenInput <= 0f)
			{
				rearWheelCollider.motorTorque = engineTorque * Mathf.Clamp(gasInput, 0f, 1f);
			}
			else
			{
				rearWheelCollider.motorTorque = engineTorque * extraEngineRate;
			}
		}
		else if (reversing)
		{
			if (speed < 15f)
			{
				rearWheelCollider.motorTorque = engineTorque * gasInput;
			}
			else
			{
				rearWheelCollider.motorTorque = 0f;
			}
		}
	}

	private void Lean()
	{
		verticalLean = Mathf.Clamp(Mathf.Lerp(verticalLean, SelfTransform.InverseTransformDirection(_mRigidbody.angularVelocity).x * chassisVerticalLean, Time.deltaTime * 5f), -10f, 10f);
		WheelHit hit;
		frontWheelCollider.GetGroundHit(out hit);
		float num = Mathf.Clamp(hit.sidewaysSlip, -1f, 1f);
		num = ((!(SelfTransform.InverseTransformDirection(_mRigidbody.velocity).z > 0f)) ? 1f : (-1f));
		horizontalLean = Mathf.Clamp(Mathf.Lerp(horizontalLean, SelfTransform.InverseTransformDirection(_mRigidbody.angularVelocity).y * num * chassisHorizontalLean, Time.deltaTime * 3f), -50f, 50f);
		Quaternion localRotation = Quaternion.Euler(verticalLean, chassis.localRotation.y + _mRigidbody.angularVelocity.z, horizontalLean);
		chassis.localRotation = localRotation;
	}

	public void ShiftGears()
	{
		if (currentGear < _totalGears && !changingGear && EngineRPM > MaxEngineRPM - 500f && rearWheelCollider.rpm >= 0f)
		{
			StartCoroutine("ChangingGear", currentGear + 1);
		}
		if (currentGear <= 0 || !(EngineRPM < MinEngineRPM + 500f) || changingGear)
		{
			return;
		}
		for (int i = 0; i < gearSpeed.Length; i++)
		{
			if (speed > gearSpeed[i])
			{
				StartCoroutine("ChangingGear", i);
			}
		}
	}

	private IEnumerator ChangingGear(int gear)
	{
		changingGear = true;
		yield return new WaitForSeconds(shiftGearTime);
		changingGear = false;
		currentGear = gear;
	}

	private bool OverTorque()
	{
		if ((nitrogenInput <= 0f && speed > maxspeed) || (nitrogenInput > 0f && speed > maxSpeedWithNitrogen) || !engineRunning || fuel <= 0f)
		{
			return true;
		}
		return false;
	}

	private void Braking()
	{
		if (Mathf.Abs(gasInput) <= 0.1f)
		{
			frontWheelCollider.brakeTorque = brakeTorque * 0.2f;
			rearWheelCollider.brakeTorque = brakeTorque * 0.2f;
		}
		else if (gasInput < 0f && !reversing)
		{
			frontWheelCollider.brakeTorque = brakeTorque * (Mathf.Abs(gasInput) * 0.2f);
			rearWheelCollider.brakeTorque = brakeTorque * Mathf.Abs(gasInput);
		}
		else
		{
			frontWheelCollider.brakeTorque = 0f;
			rearWheelCollider.brakeTorque = 0f;
		}
	}

	private void ApplySteering()
	{
		float num = ((steerInput >= 0f) ? 1 : (-1));
		float value = SteerAngle * steerInput * steerInput * num;
		frontWheelCollider.steerAngle = Mathf.Clamp(value, 0f - SteerAngle, SteerAngle);
		axis = quaternion * Vector3.forward;
		handTransform.localRotation = Quaternion.AngleAxis(frontWheelCollider.steerAngle, axis) * quaternion90;
	}

	private void ApplyBrakeTorque(WheelCollider wc, float brake)
	{
		wc.brakeTorque = brake;
	}

	private void SyncWheelRmpAndRotation()
	{
		if (Time.time - _mSyncWheelTime > 1f && engineRunning && (Mathf.Abs(frontWheelCollider.transform.localEulerAngles.y - _mFrontYRotation) > 2f || Mathf.Abs(rearWheelCollider.transform.localEulerAngles.y - _mTailYRotation) > 2f || Mathf.Abs(rearWheelCollider.rpm - _mRpm) > 2f))
		{
			_mSyncWheelTime = Time.time;
			cSyncCarWheelRotation.vehicleId = carId;
			_mFrontYRotation = (cSyncCarWheelRotation.frontYRatation = frontWheelCollider.transform.localEulerAngles.y);
			_mTailYRotation = (cSyncCarWheelRotation.tailYRatation = rearWheelCollider.transform.localEulerAngles.y);
			_mRpm = (cSyncCarWheelRotation.rpm = rearWheelCollider.rpm);
			Client2Gs.Ins.Send(cSyncCarWheelRotation);
		}
	}

	private void StopWheelRmp()
	{
		_mSyncWheelTime = Time.time;
		cSyncCarWheelRotation.vehicleId = carId;
		_mFrontYRotation = (cSyncCarWheelRotation.frontYRatation = frontWheelCollider.transform.localEulerAngles.y);
		_mTailYRotation = (cSyncCarWheelRotation.tailYRatation = rearWheelCollider.transform.localEulerAngles.y);
		_mRpm = (cSyncCarWheelRotation.rpm = 0f);
		Client2Gs.Ins.Send(cSyncCarWheelRotation);
	}

	public float GetFuel()
	{
		return fuel;
	}

	public void SetFuel(float fuel)
	{
		this.fuel = fuel;
	}

	public int GetSpeed()
	{
		return (int)speed;
	}

	public void SyncEngineSound(float volume)
	{
	}

	public void KillOrStartEngine()
	{
		engineRunning = !engineRunning;
		if (engineRunning)
		{
			StartEngine();
		}
		else
		{
			StopEngine();
		}
	}

	private void StopEngine()
	{
		fuelInput = 0f;
		StopWheelRmp();
		ForceStopPlaySound(SoundId.engineIdleSoundId);
		ForceStopPlaySound(SoundId.engineHighSpeedSoundId);
		ForceStopPlaySound(SoundId.engineLowSpeedSoundId);
		_mNitrogenEffect.gameObject.SetActive(false);
		if (fuel > 0f)
		{
			PlaySound(SoundId.engineStopSoundId);
		}
	}

	private void PlayStartEngineSound()
	{
		if (fuel > 0f)
		{
			PlaySound(SoundId.startEngineSoundId);
		}
		else
		{
			PlaySound(SoundId.startEngineWithoutFuelSoundId);
		}
	}

	private void PlayIdleSound()
	{
		if (fuel > 0f)
		{
			PlaySound(SoundId.engineIdleSoundId, true);
		}
	}

	private void StartEngine()
	{
		fuelInput = 1f;
		PlayStartEngineSound();
		if (fuel > 0f)
		{
			PlayIdleSound();
		}
	}

	public int GetEngineSoundId()
	{
		return 1;
	}

	private void ReduceHp(int damage)
	{
		if (hp > 0f)
		{
			hp -= damage;
			if (hp < 0f)
			{
				hp = 0f;
			}
			OnHpChange();
			if (hp <= 0f)
			{
				DoExplosion();
			}
		}
	}

	private void DoExplosion()
	{
		if (_mVihicleMonitor == null)
		{
			_mVihicleMonitor = GetComponent<VehicleMonitor>();
		}
		_mVihicleMonitor.OnExplosion();
	}

	private void Sound()
	{
		if (speed >= 50f && Mathf.Abs(steerInput) >= 0.8f)
		{
			PlaySound(SoundId.tireSkipSoundId);
		}
		if (_brakeInput >= 0.8f)
		{
			SoundId soundId = ((!(speed >= 40f)) ? SoundId.brakeOnLowSpeedSoundId : SoundId.brakeOnHighSpeedSoundId);
			PlaySound(soundId);
		}
		if (fuel > 0f)
		{
			if (needSync)
			{
				if (engineRunning)
				{
					engineVolume = Mathf.Clamp01(speed / maxspeed);
					if (engineVolume > 0.1f)
					{
						PlaySound(SoundId.engineHighSpeedSoundId, true, false, engineVolume, 1f, false);
					}
				}
				return;
			}
			if (_mVihicleMonitor == null)
			{
				_mVihicleMonitor = GetComponent<VehicleMonitor>();
			}
			if (attachPoints == null || attachPoints[0].childCount <= 0)
			{
				return;
			}
			PlaySound(SoundId.engineIdleSoundId, true);
			engineVolumeFromSync = Mathf.Clamp01(1.5f * _mVihicleMonitor.Get3rdSpeed() / maxspeed);
			if (engineVolumeFromSync > 0.1f)
			{
				PlaySound(SoundId.engineHighSpeedSoundId, true, false, engineVolumeFromSync, 1f, false);
			}
			if (Time.time - _mTriggerSoundTime > 3f)
			{
				_mTriggerSoundTime = Time.time;
				if (BattleEvent.vehicleSoundEvent != null)
				{
					BattleEvent.vehicleSoundEvent(base.gameObject, carId, GetEngineSoundId());
				}
			}
		}
		else
		{
			StopPlaySound(SoundId.engineIdleSoundId);
			StopPlaySound(SoundId.engineHighSpeedSoundId);
		}
	}

	public void OnDamage(float damage, Vector3 pos)
	{
		if (hp > 0f)
		{
			ReduceHp((int)damage);
			cCauseDamgeToVehicle.damage = (int)damage;
			cCauseDamgeToVehicle.vehicleId = carId;
			Client2Gs.Ins.Send(cCauseDamgeToVehicle);
		}
	}

	public Transform[] GetAttachPoints()
	{
		return attachPoints;
	}

	public bool IsBroken()
	{
		return hp <= 0f;
	}

	public float GetMaxFuel()
	{
		return _mTwoWheelMotoCfg.maxFuel;
	}

	public void SetNeedSync(bool needSync)
	{
		this.needSync = needSync;
	}

	private void ForceStopPlaySound(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(_mTwoWheelMotoCfg.soundIds[(int)soundId]);
		SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
	}

	public void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		_mStopIdleSound = true;
		ForceStopPlaySound(SoundId.engineIdleSoundId);
		ForceStopPlaySound(SoundId.engineHighSpeedSoundId);
		ForceStopPlaySound(SoundId.engineLowSpeedSoundId);
	}

	public void SetHp(int hp)
	{
		this.hp = hp;
	}

	public int GetHp()
	{
		return (int)hp;
	}

	public int GetMaxHp()
	{
		return _mTwoWheelMotoCfg.maxHp;
	}

	public void OnHit(float relativeSpeed)
	{
		if (m_HitTimeLimit > 0f)
		{
			return;
		}
		m_HitTimeLimit = 1.5f;
		int num = (int)(_mTwoWheelMotoCfg.hitCarDropHpRate1 * relativeSpeed + _mTwoWheelMotoCfg.hitCarDropHpRate2);
		int num2 = (int)((_mTwoWheelMotoCfg.hitCarPlayerDropHpRate1 * relativeSpeed + _mTwoWheelMotoCfg.hitCarPlayerDropHpRate2) * (float)ConstsBs.HpPlayer * 0.0001f);
		if (num > 0)
		{
			ReduceHp(num);
		}
		cCauseDamgeByVehicle.vehicleId = carId;
		cCauseDamgeByVehicle.vehicleHp = (int)hp;
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
		if (num > 0 || num2 > 0)
		{
			Client2Gs.Ins.Send(cCauseDamgeByVehicle);
		}
	}

	public void PlayHornSound()
	{
		PlaySound(SoundId.hornSoundId);
	}

	public void SetAllEffectVisible(bool visible)
	{
		if (_effectVisible != visible)
		{
			_effectVisible = visible;
			_smokeBurnEffectParent.gameObject.SetActive(_effectVisible);
		}
	}

	public void EnableWheelCollidar()
	{
		frontWheelCollider.enabled = true;
		rearWheelCollider.enabled = true;
	}

	public void DiableWheelCollidar()
	{
		frontWheelCollider.enabled = false;
		rearWheelCollider.enabled = false;
	}

	public void SyncWheelRotation(SSyncCarWheelRotation sSyncCarWheelRotation)
	{
		if (!needSync)
		{
			_mSyncFrontWheelRotation = sSyncCarWheelRotation.frontYRatation;
			_mSyncRearWheelRotation = sSyncCarWheelRotation.tailYRatation;
		}
	}

	public void WheelAlign()
	{
		if (needSync)
		{
			Vector3 vector = frontWheelTransform.TransformPoint(frontWheelCollider.center);
			RaycastHit hitInfo;
			if (Physics.Raycast(vector, -frontWheelTransform.up, out hitInfo, frontWheelCollider.suspensionDistance + frontWheelCollider.radius) && !hitInfo.transform.IsChildOf(SelfTransform) && !hitInfo.collider.isTrigger)
			{
				frontWheelModel.position = hitInfo.point + frontWheelTransform.up * frontWheelCollider.radius;
			}
			else
			{
				frontWheelModel.position = Vector3.Lerp(frontWheelModel.position, vector - frontWheelTransform.up * frontWheelCollider.suspensionDistance, Time.deltaTime * 10f);
			}
			wheelRotation += frontWheelCollider.rpm * 6f * Time.deltaTime;
			frontWheelModel.rotation = frontWheelTransform.rotation * Quaternion.Euler(wheelRotation, frontWheelCollider.steerAngle, frontWheelTransform.rotation.z);
			rearWheelModel.rotation = rearWheelTransform.rotation * Quaternion.Euler(wheelRotation, rearWheelCollider.transform.rotation.y, rearWheelTransform.rotation.z);
		}
		else
		{
			float currentSpeed = GetCurrentSpeed();
			if (currentSpeed > 0f)
			{
				wheelRotationToSync += currentSpeed * 12f * Time.deltaTime;
				frontWheelModel.rotation = frontWheelTransform.transform.rotation * Quaternion.Euler(wheelRotationToSync, _mSyncFrontWheelRotation, 0f);
				rearWheelModel.rotation = rearWheelTransform.transform.rotation * Quaternion.Euler(wheelRotationToSync, _mSyncRearWheelRotation, 0f);
			}
		}
	}

	private void InitGearSpeed()
	{
		if (gearSpeed == null)
		{
			gearSpeed = new float[totalGears];
		}
		for (int i = 0; i < totalGears; i++)
		{
			gearSpeed[i] = Mathf.Lerp(0f, maxspeed, (float)(i + 1) / (float)totalGears);
		}
	}

	internal float GetCurrentSpeed()
	{
		if (_mVihicleMonitor == null)
		{
			_mVihicleMonitor = GetComponent<VehicleMonitor>();
		}
		if (_mVihicleMonitor != null)
		{
			return _mVihicleMonitor.Get3rdSpeed();
		}
		return 0f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == BattleScMgr.BulletLayer || other.gameObject.layer == BattleScMgr.CarForBulletLayer || other.gameObject.layer == BattleScMgr.GunLayer)
		{
			return;
		}
		float num = speed;
		BasePlayerController componentInParent = other.gameObject.GetComponentInParent<BasePlayerController>();
		if (componentInParent != null && other.gameObject.layer == BattleScMgr.OtherPlayerColliderLayer && !IsPassenger(other.gameObject))
		{
			long roleId = componentInParent.RoleId;
			int num2 = (int)(_mTwoWheelMotoCfg.hitByCarPlayerDropHpRate1 * num + _mTwoWheelMotoCfg.hitByCarPlayerDropHpRate2);
			if (num2 > 0 && roleId > 0)
			{
				cCauseDamgeByVehicle.vehicleId = carId;
				cCauseDamgeByVehicle.vehicleHp = (int)hp;
				cCauseDamgeByVehicle.playerId2Damage.Clear();
				cCauseDamgeByVehicle.playerId2Damage[roleId] = num2;
				Client2Gs.Ins.Send(cCauseDamgeByVehicle);
				cVehicleHitPlayer.roleId = componentInParent.RoleId;
				cVehicleHitPlayer.vehicleId = carId;
				cVehicleHitPlayer.speed = num;
				cVehicleHitPlayer.forward.x = MathUtils.Float2Short(_mRigidbody.velocity.x);
				cVehicleHitPlayer.forward.y = MathUtils.Float2Short(_mRigidbody.velocity.y);
				cVehicleHitPlayer.forward.z = MathUtils.Float2Short(_mRigidbody.velocity.z);
				Client2Gs.Ins.Send(cVehicleHitPlayer);
			}
		}
		if (other.gameObject.layer == BattleScMgr.CarTriggerLayer)
		{
			VehicleMonitor componentInParent2 = other.gameObject.GetComponentInParent<VehicleMonitor>();
			if (componentInParent2 != null)
			{
				componentInParent2.OnHit(num);
			}
		}
		if (needSync && componentInParent == null)
		{
			if (other.gameObject.CompareTag("Ground"))
			{
				if (_mRigidbody.velocity.y > 3f)
				{
					OnHit(num);
				}
			}
			else
			{
				OnHit(num);
			}
		}
		bool flag = other.gameObject.layer == BattleScMgr.SelfPlayerColliderLayer || other.gameObject.layer == BattleScMgr.OtherPlayerColliderLayer;
		bool flag2 = other.gameObject.layer == BattleScMgr.CarTriggerLayer;
		if (_mRigidbody.velocity.magnitude > 1f)
		{
			if (flag && !IsPassenger(other.gameObject))
			{
				PlaySound(SoundId.hitPlayerSoundId);
			}
			else if (flag2)
			{
				PlaySound(SoundId.hitCarSoundId);
			}
			else
			{
				PlaySound(SoundId.hitWallSoundId);
			}
		}
	}

	private bool IsPassenger(GameObject go)
	{
		if (_mVihicleMonitor == null)
		{
			_mVihicleMonitor = GetComponent<VehicleMonitor>();
		}
		return _mVihicleMonitor.IsPassenger(go);
	}
}

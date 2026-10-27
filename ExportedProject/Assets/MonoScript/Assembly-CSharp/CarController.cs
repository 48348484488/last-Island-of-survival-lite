using System;
using System.Collections;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour, IVehicle
{
	public enum WheelType
	{
		FWD = 0,
		RWD = 1,
		AWD = 2
	}

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

	private Rigidbody rigid;

	private Transform SelfTransform;

	internal bool sleepingRigid;

	public Transform FrontLeftWheelTransform;

	public Transform FrontRightWheelTransform;

	public Transform RearLeftWheelTransform;

	public Transform RearRightWheelTransform;

	public CarWheelCollider FrontLeftWheelCollider;

	public CarWheelCollider FrontRightWheelCollider;

	public CarWheelCollider RearLeftWheelCollider;

	public CarWheelCollider RearRightWheelCollider;

	public Transform SteeringWheel;

	public WheelType _wheelTypeChoise = WheelType.RWD;

	public bool engineRunning;

	public bool semiAutomaticGear;

	public AnimationCurve[] engineTorqueCurve;

	public float[] gearSpeed;

	public float engineTorque = 3000f;

	public float brakeTorque = 2500f;

	public float maxEngineRPM = 7000f;

	public float minEngineRPM = 1000f;

	[Range(0.75f, 2f)]
	public float engineInertia = 1f;

	public float steerAngle = 80f;

	public float highspeedsteerAngle = 15f;

	public float highspeedsteerAngleAtspeed = 100f;

	public float speed;

	public float defMaxSpeed;

	public float maxspeed = 220f;

	public float normalMaxSpeed = 220f;

	public float maxSpeedWithNitrogen = 250f;

	private float orgSteerAngle;

	private float fuelInput;

	public int currentGear;

	public int totalGears = 6;

	public float gearShiftingDelay = 0.35f;

	public bool changingGear;

	public int direction = 1;

	public bool autoGenerateGearCurves = true;

	public bool autoGenerateTargetSpeedsForChangingGear = true;

	[HideInInspector]
	public float gasInput;

	[HideInInspector]
	public float brakeInput;

	[HideInInspector]
	public float steerInput;

	[HideInInspector]
	public float clutchInput;

	[HideInInspector]
	private float nitrogenInput;

	[HideInInspector]
	public bool cutGas;

	[HideInInspector]
	public float idleInput;

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

	private Transform _smokeBurnEffectParent;

	private Transform _mSmokeEffect;

	private Transform _mBurnEffect;

	private Transform _mExplosionEffect;

	private Transform _mNitrogenEffect;

	private bool _effectVisible = true;

	private Vector3 _mSmokeEffectPos;

	private Vector3 _mBurnEffectPos;

	private Vector3 _mExplosionEffectPos;

	private Vector3 _mNitrogenEffectPos;

	private Transform[] _mGlasses;

	private readonly float OnFireHpRate = 0.1f;

	private readonly float SmokeHpRate = 0.3f;

	private float _mWaterLevel = -1000f;

	private readonly float MaxChangeSpeedRate = 0.02f;

	private float _mCurrentChangeSpeedRate;

	private float _mRpm;

	private float _mFrontYRotation;

	private float _mTailYRotation;

	private VehicleMonitor _mVihicleMonitor;

	internal float engineRPM;

	internal float rawEngineRPM;

	public bool ABS = true;

	public bool TCS = true;

	public bool ESP = true;

	[Range(0.05f, 0.5f)]
	public float ABSThreshold = 0.35f;

	[Range(0.05f, 0.5f)]
	public float TCSThreshold = 0.25f;

	[Range(0f, 1f)]
	public float TCSStrength = 1f;

	[Range(0.05f, 0.5f)]
	public float ESPThreshold = 0.25f;

	[Range(0.1f, 1f)]
	public float ESPStrength = 0.5f;

	public bool overSteering;

	public bool underSteering;

	public float frontSlip;

	public float rearSlip;

	private float fuelConsume;

	private float extraFuelConsume;

	private float extraEngineRate;

	public Transform[] attachPoints;

	public Transform[] tanshenPoints;

	public Transform leftHandAttachPoint;

	public Transform rightHandAttachPoint;

	private CarCfg carCfg;

	internal int carId;

	internal bool hasInput;

	internal bool needSync;

	private float engineVolumeFromSync;

	private float engineVolume;

	private int hp;

	internal const int FRONT_LEFT_WHEEL = 0;

	internal const int FRONT_RIGHT_WHEEL = 1;

	internal const int REAR_LEFT_WHEEL = 2;

	internal const int REAR_RIGHT_WHEEL = 3;

	private float _mSyncBrakeSoundTime;

	private float _mTriggerSoundTime;

	private float m_HitTimeLimit = 1.5f;

	private float fuel;

	public bool automaticGear
	{
		get
		{
			return true;
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
			if (!changingGear && !cutGas)
			{
				return (direction != 1) ? Mathf.Clamp01(brakeInput) : Mathf.Clamp01(gasInput);
			}
			return 0f;
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

	internal void PlaySound(SoundId soundId, bool loop = false, bool playMoreThanOne = false, float volume = 1f, float pitch = 1f, bool syncSound = true)
	{
		SoundCfg soundCfg = SoundCfg.Get(carCfg.soundIds[(int)soundId]);
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

	private void Awake()
	{
		SelfTransform = base.transform;
		GameObject childObjByName = Utils.GetChildObjByName("boli", base.gameObject);
		if (childObjByName != null)
		{
			childObjByName.SetActive(false);
		}
	}

	public void PlayHornSound()
	{
		PlaySound(SoundId.hornSoundId);
	}

	internal void StopPlaySound(SoundId soundId)
	{
		if (IsSoundPlaying(soundId))
		{
			SoundCfg soundCfg = SoundCfg.Get(carCfg.soundIds[(int)soundId]);
			SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
		}
		if (soundId == SoundId.engineIdleSoundId && needSync)
		{
			cStopVehicleSound.vehicleId = carId;
			cStopVehicleSound.soundId = SoundCfg.Get(carCfg.soundIds[(int)soundId]).id;
			Client2Gs.Ins.Send(cStopVehicleSound);
		}
	}

	internal bool IsSoundPlaying(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(carCfg.soundIds[(int)soundId]);
		return SingletonMono<AudioManager>.Ins.IsSoundPlaying(soundCfg.path, base.gameObject);
	}

	public void Init(CarInfo carInfo, CarCfg carCfg, SVehicleInfo sVehicleInfo)
	{
		rigid = GetComponent<Rigidbody>();
		rigid.drag = 0.05f;
		rigid.angularDrag = 0.25f;
		rigid.mass = carCfg.mass;
		rigid.centerOfMass = new Vector3(carCfg.comX, carCfg.comY, carCfg.comZ);
		carId = sVehicleInfo.vehicleInfo.id;
		this.carCfg = carCfg;
		hp = sVehicleInfo.vehicleInfo.hp;
		fuel = sVehicleInfo.vehicleInfo.fuel;
		fuelConsume = carCfg.fuelConsume;
		extraFuelConsume = carCfg.extraFuelConsume;
		extraEngineRate = carCfg.extraEngineTorqueRate;
		attachPoints = carInfo.seats;
		tanshenPoints = carInfo.Tanshen;
		leftHandAttachPoint = carInfo.leftHandAttachPoint;
		rightHandAttachPoint = carInfo.rightHandAttachPoint;
		FrontLeftWheelTransform = carInfo.frontLeftWheelModel;
		FrontRightWheelTransform = carInfo.frontRightWheelModel;
		RearLeftWheelTransform = carInfo.rearLeftWheelModel;
		RearRightWheelTransform = carInfo.rearRightWheelModel;
		if (carInfo.frontLeftWheelModel != null && carInfo.frontLeftWheelCollider != null)
		{
			FrontLeftWheelCollider = carInfo.frontLeftWheelCollider.gameObject.AddComponent<CarWheelCollider>();
			FrontLeftWheelCollider.wheelCollider = carInfo.frontLeftWheelCollider;
			FrontLeftWheelCollider.wheelModel = carInfo.frontLeftWheelModel;
			FrontLeftWheelCollider.Init(0);
		}
		if (carInfo.frontRightWheelModel != null && carInfo.frontRightWheelCollider != null)
		{
			FrontRightWheelCollider = carInfo.frontRightWheelCollider.gameObject.AddComponent<CarWheelCollider>();
			FrontRightWheelCollider.wheelCollider = carInfo.frontRightWheelCollider;
			FrontRightWheelCollider.wheelModel = carInfo.frontRightWheelModel;
			FrontRightWheelCollider.Init(1);
		}
		if (carInfo.rearLeftWheelModel != null && carInfo.rearLeftWheelCollider != null)
		{
			RearLeftWheelCollider = carInfo.rearLeftWheelCollider.gameObject.AddComponent<CarWheelCollider>();
			RearLeftWheelCollider.wheelCollider = carInfo.rearLeftWheelCollider;
			RearLeftWheelCollider.wheelModel = carInfo.rearLeftWheelModel;
			RearLeftWheelCollider.Init(2);
		}
		if (carInfo.rearRightWheelModel != null && carInfo.rearRightWheelCollider != null)
		{
			RearRightWheelCollider = carInfo.rearRightWheelCollider.gameObject.AddComponent<CarWheelCollider>();
			RearRightWheelCollider.wheelCollider = carInfo.rearRightWheelCollider;
			RearRightWheelCollider.wheelModel = carInfo.rearRightWheelModel;
			RearRightWheelCollider.Init(3);
		}
		_mGlasses = carInfo.glasses;
		_mSmokeEffectPos = carInfo.smokeEffectPos;
		_mBurnEffectPos = carInfo.burnEffectPos;
		_mExplosionEffectPos = carInfo.explodeEffectPos;
		_mNitrogenEffectPos = carInfo.nitrogenEffectPos;
		OnHpChange(false);
		steerAngle = carCfg.steerAngle;
		highspeedsteerAngle = carCfg.highspeedsteerAngle;
		highspeedsteerAngleAtspeed = carCfg.highspeedsteerAngleAtspeed;
		totalGears = carCfg.gearNum;
		gearShiftingDelay = carCfg.switchGearTime;
		engineTorque = carCfg.engineTorque;
		brakeTorque = carCfg.brakeTorque;
		normalMaxSpeed = (maxspeed = carCfg.maxSpeed);
		maxSpeedWithNitrogen = carCfg.maxSpeedWithNitrogen;
		if (carCfg.driveType == 2)
		{
			_wheelTypeChoise = WheelType.AWD;
		}
		else if (carCfg.driveType == 1)
		{
			_wheelTypeChoise = WheelType.RWD;
		}
		else if (carCfg.driveType == 0)
		{
			_wheelTypeChoise = WheelType.FWD;
		}
		orgSteerAngle = steerAngle;
		TorqueCurve();
		_smokeBurnEffectParent = new GameObject("__effects__").transform;
		_smokeBurnEffectParent.parent = SelfTransform;
		_smokeBurnEffectParent.localPosition = Vector3.zero;
		_smokeBurnEffectParent.localScale = Vector3.one;
		_smokeBurnEffectParent.localRotation = Quaternion.identity;
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_smoke.ab", OnSmokeEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/zaiju_ranshao.ab", OnBurnEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_baozha.ab", OnExplosionEffectLoaded);
		SingletonMono<EffectMgr>.Ins.LoadEffect("effect/qiche_qiliu_01.ab", OnNitrogenEffectLoaded);
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

	public void SetAllEffectVisible(bool visible)
	{
		if (_effectVisible != visible)
		{
			_effectVisible = visible;
			_smokeBurnEffectParent.gameObject.SetActive(_effectVisible);
		}
	}

	private void OnEnable()
	{
		changingGear = false;
	}

	public void EnableWheelCollidar()
	{
		FrontLeftWheelCollider.wheelCollider.enabled = true;
		FrontRightWheelCollider.wheelCollider.enabled = true;
		RearLeftWheelCollider.wheelCollider.enabled = true;
		RearRightWheelCollider.wheelCollider.enabled = true;
	}

	public void DiableWheelCollidar()
	{
		FrontLeftWheelCollider.wheelCollider.enabled = false;
		FrontRightWheelCollider.wheelCollider.enabled = false;
		RearLeftWheelCollider.wheelCollider.enabled = false;
		RearRightWheelCollider.wheelCollider.enabled = false;
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
		StopPlaySound(SoundId.engineIdleSoundId);
		if (fuel > 0f)
		{
			PlaySound(SoundId.engineStopSoundId);
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

	private void Update()
	{
		if (!IsBroken())
		{
			GearBox();
			Clutch();
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
			if (base.transform.position.y + rigid.centerOfMass.y <= _mWaterLevel)
			{
				rigid.AddForce(-Vector3.up * rigid.mass * Physics.gravity.y * 1.01f);
				float b = Mathf.Clamp(Mathf.Abs(rigid.velocity.y) * 0.1f, 0f, MaxChangeSpeedRate);
				_mCurrentChangeSpeedRate = Mathf.Lerp(_mCurrentChangeSpeedRate, b, Time.fixedDeltaTime);
				rigid.AddForce(-rigid.velocity * _mCurrentChangeSpeedRate, ForceMode.VelocityChange);
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
				RevLimiter();
				ApplySteering();
				if (ESP)
				{
					ESPCheck(rigid.angularVelocity.y, FrontLeftWheelCollider.steerAngle);
				}
			}
			Braking();
		}
		else if (rigid.velocity.magnitude > 0.1f)
		{
			ApplyBrakeTorque(FrontLeftWheelCollider, brakeTorque);
			ApplyBrakeTorque(FrontRightWheelCollider, brakeTorque);
			ApplyBrakeTorque(RearLeftWheelCollider, brakeTorque);
			ApplyBrakeTorque(RearRightWheelCollider, brakeTorque);
		}
	}

	private void SyncWheelRmpAndRotation()
	{
		if (Time.time - _mSyncWheelTime > 1f && engineRunning && (Mathf.Abs(FrontLeftWheelCollider.transform.localEulerAngles.y - _mFrontYRotation) > 2f || Mathf.Abs(RearLeftWheelCollider.transform.localEulerAngles.y - _mTailYRotation) > 2f || Mathf.Abs(FrontLeftWheelCollider.rpm - _mRpm) > 2f))
		{
			_mSyncWheelTime = Time.time;
			cSyncCarWheelRotation.vehicleId = carId;
			_mFrontYRotation = (cSyncCarWheelRotation.frontYRatation = FrontLeftWheelCollider.transform.localEulerAngles.y);
			_mTailYRotation = (cSyncCarWheelRotation.tailYRatation = RearLeftWheelCollider.transform.localEulerAngles.y);
			_mRpm = (cSyncCarWheelRotation.rpm = FrontLeftWheelCollider.rpm);
			Client2Gs.Ins.Send(cSyncCarWheelRotation);
		}
	}

	private void StopWheelRmp()
	{
		_mSyncWheelTime = Time.time;
		cSyncCarWheelRotation.vehicleId = carId;
		_mFrontYRotation = (cSyncCarWheelRotation.frontYRatation = FrontLeftWheelCollider.transform.localEulerAngles.y);
		_mTailYRotation = (cSyncCarWheelRotation.tailYRatation = RearLeftWheelCollider.transform.localEulerAngles.y);
		_mRpm = (cSyncCarWheelRotation.rpm = 0f);
		Client2Gs.Ins.Send(cSyncCarWheelRotation);
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
				engineVolume = Mathf.Clamp01(engineRPM / maxEngineRPM);
				if (engineVolume > 0.1f)
				{
					PlaySound(SoundId.engineHighSpeedSoundId, true, false, engineVolume, engineVolume, false);
				}
				return;
			}
			if (_mVihicleMonitor == null)
			{
				_mVihicleMonitor = GetComponent<VehicleMonitor>();
			}
			engineVolumeFromSync = Mathf.Clamp01(1.5f * _mVihicleMonitor.Get3rdSpeed() / normalMaxSpeed);
			if (engineVolumeFromSync > 0.1f)
			{
				PlaySound(SoundId.engineHighSpeedSoundId, true, false, engineVolumeFromSync, 1f, false);
			}
			if (attachPoints == null || attachPoints[0].childCount <= 0)
			{
				return;
			}
			PlaySound(SoundId.engineIdleSoundId, true);
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
		speed = rigid.velocity.magnitude * 3.6f;
		steerAngle = Mathf.Lerp(orgSteerAngle, highspeedsteerAngle, speed / highspeedsteerAngleAtspeed);
		if ((bool)SteeringWheel)
		{
			SteeringWheel.transform.rotation = base.transform.rotation * Quaternion.Euler(20f, 0f, FrontLeftWheelCollider.steerAngle * -6f);
		}
		if (rigid.velocity.magnitude < 0.01f && Mathf.Abs(steerInput) < 0.01f && Mathf.Abs(_gasInput) < 0.01f && Mathf.Abs(rigid.angularVelocity.magnitude) < 0.01f)
		{
			sleepingRigid = true;
		}
		else
		{
			sleepingRigid = false;
		}
		rawEngineRPM = Mathf.Clamp(Mathf.MoveTowards(rawEngineRPM, maxEngineRPM * 1.1f * Mathf.Clamp01(Mathf.Lerp(0f, 1f, (1f - clutchInput) * ((RearLeftWheelCollider.wheelRPMToSpeed + RearRightWheelCollider.wheelRPMToSpeed) * (float)direction / 2f / gearSpeed[currentGear])) + (_gasInput * clutchInput + idleInput)), engineInertia * 100f), 0f, maxEngineRPM * 1.1f);
		rawEngineRPM *= fuelInput;
		engineRPM = Mathf.Lerp(engineRPM, rawEngineRPM, Mathf.Lerp(Time.fixedDeltaTime * 5f, Time.fixedDeltaTime * 50f, rawEngineRPM / maxEngineRPM));
		switch (_wheelTypeChoise)
		{
		case WheelType.FWD:
			ApplyMotorTorque(FrontLeftWheelCollider, engineTorque);
			ApplyMotorTorque(FrontRightWheelCollider, engineTorque);
			break;
		case WheelType.RWD:
			ApplyMotorTorque(RearLeftWheelCollider, engineTorque);
			ApplyMotorTorque(RearRightWheelCollider, engineTorque);
			break;
		case WheelType.AWD:
			ApplyMotorTorque(FrontLeftWheelCollider, engineTorque / 2f);
			ApplyMotorTorque(FrontRightWheelCollider, engineTorque / 2f);
			ApplyMotorTorque(RearLeftWheelCollider, engineTorque / 2f);
			ApplyMotorTorque(RearRightWheelCollider, engineTorque / 2f);
			break;
		}
	}

	private void ApplyMotorTorque(CarWheelCollider wc, float torque)
	{
		if (TCS)
		{
			WheelHit hit;
			wc.wheelCollider.GetGroundHit(out hit);
			if (Mathf.Abs(wc.rpm) >= 100f)
			{
				torque = ((!(hit.forwardSlip > TCSThreshold)) ? (torque + Mathf.Clamp(torque * hit.forwardSlip * TCSStrength, 0f - engineTorque, 0f)) : (torque - Mathf.Clamp(torque * hit.forwardSlip * TCSStrength, 0f, engineTorque)));
			}
		}
		if (OverTorque())
		{
			wc.wheelCollider.motorTorque = 0f;
			return;
		}
		float num = 0f;
		if (nitrogenInput <= 0f)
		{
			num = torque * (1f - clutchInput) * _gasInput * (engineTorqueCurve[currentGear].Evaluate(wc.wheelRPMToSpeed * (float)direction) * (float)direction);
		}
		else
		{
			float num2 = Mathf.Clamp(1f - 1.2f * speed / maxspeed, 0.3f, 1f);
			if (speed >= maxspeed * 0.85f)
			{
				num2 = 1f;
			}
			num = torque * extraEngineRate * num2;
		}
		wc.wheelCollider.motorTorque = num;
	}

	private void ESPCheck(float velocity, float steering)
	{
		WheelHit hit;
		FrontLeftWheelCollider.wheelCollider.GetGroundHit(out hit);
		WheelHit hit2;
		FrontRightWheelCollider.wheelCollider.GetGroundHit(out hit2);
		frontSlip = hit.sidewaysSlip + hit2.sidewaysSlip;
		WheelHit hit3;
		RearLeftWheelCollider.wheelCollider.GetGroundHit(out hit3);
		WheelHit hit4;
		RearRightWheelCollider.wheelCollider.GetGroundHit(out hit4);
		rearSlip = hit3.sidewaysSlip + hit4.sidewaysSlip;
		if (Mathf.Abs(frontSlip) >= ESPThreshold)
		{
			overSteering = true;
		}
		else
		{
			overSteering = false;
		}
		if (Mathf.Abs(rearSlip) >= ESPThreshold && !overSteering)
		{
			underSteering = true;
		}
		else
		{
			underSteering = false;
		}
		if (!(Mathf.Abs(frontSlip) < ESPThreshold) && !(Math.Abs(rearSlip) < ESPThreshold))
		{
			if (underSteering)
			{
				ApplyBrakeTorque(RearLeftWheelCollider, brakeTorque * ESPStrength * Mathf.Clamp(frontSlip, 0f, float.PositiveInfinity));
				ApplyBrakeTorque(RearRightWheelCollider, brakeTorque * ESPStrength * Mathf.Clamp(0f - frontSlip, 0f, float.PositiveInfinity));
			}
			if (overSteering)
			{
				ApplyBrakeTorque(FrontLeftWheelCollider, brakeTorque * ESPStrength * Mathf.Clamp(0f - rearSlip, 0f, float.PositiveInfinity));
				ApplyBrakeTorque(FrontRightWheelCollider, brakeTorque * ESPStrength * Mathf.Clamp(rearSlip, 0f, float.PositiveInfinity));
			}
		}
	}

	private void ApplyBrakeTorque(CarWheelCollider wc, float brake)
	{
		if (ABS)
		{
			WheelHit hit;
			wc.wheelCollider.GetGroundHit(out hit);
			if (Mathf.Abs(hit.forwardSlip) * Mathf.Clamp01(brake) >= ABSThreshold)
			{
				brake = 0f;
			}
		}
		wc.wheelCollider.brakeTorque = brake;
	}

	private void ApplySteering()
	{
		float num = ((steerInput >= 0f) ? 1 : (-1));
		float value = steerAngle * steerInput * steerInput * num;
		FrontLeftWheelCollider.wheelCollider.steerAngle = Mathf.Clamp(value, 0f - steerAngle, steerAngle);
		FrontRightWheelCollider.wheelCollider.steerAngle = Mathf.Clamp(value, 0f - steerAngle, steerAngle);
	}

	private void Braking()
	{
		if (Mathf.Abs(_brakeInput) > 0.01f)
		{
			ApplyBrakeTorque(FrontLeftWheelCollider, brakeTorque * Mathf.Clamp(_brakeInput, 0f, 1f));
			ApplyBrakeTorque(FrontRightWheelCollider, brakeTorque * Mathf.Clamp(_brakeInput, 0f, 1f));
			ApplyBrakeTorque(RearLeftWheelCollider, brakeTorque * Mathf.Clamp(_brakeInput, 0f, 1f) / 2f);
			ApplyBrakeTorque(RearRightWheelCollider, brakeTorque * Mathf.Clamp(_brakeInput, 0f, 1f) / 2f);
		}
		else
		{
			float brake = ((!(Mathf.Abs(_gasInput) < 0.01f)) ? 0f : (brakeTorque * 0.2f));
			ApplyBrakeTorque(FrontLeftWheelCollider, brake);
			ApplyBrakeTorque(FrontRightWheelCollider, brake);
			ApplyBrakeTorque(RearLeftWheelCollider, brake);
			ApplyBrakeTorque(RearRightWheelCollider, brake);
		}
	}

	private void Clutch()
	{
		if (engineRunning)
		{
			idleInput = Mathf.Lerp(1f, 0f, engineRPM / minEngineRPM);
		}
		else
		{
			idleInput = 0f;
		}
		if (speed <= 10f && !cutGas)
		{
			clutchInput = Mathf.Lerp(clutchInput, Mathf.Lerp(1f, Mathf.Lerp(0.2f, 0f, (RearLeftWheelCollider.wheelRPMToSpeed + RearRightWheelCollider.wheelRPMToSpeed) / 2f / 10f), Mathf.Abs(_gasInput)), Time.deltaTime * 50f);
		}
		else if (!cutGas)
		{
			if (changingGear)
			{
				clutchInput = Mathf.Lerp(clutchInput, 1f, Time.deltaTime * 10f);
			}
			else
			{
				clutchInput = Mathf.Lerp(clutchInput, 0f, Time.deltaTime * 10f);
			}
		}
		clutchInput = Mathf.Clamp01(clutchInput);
	}

	private void GearBox()
	{
		float z = base.transform.InverseTransformDirection(rigid.velocity).z;
		if (((brakeInput > 0.9f && z < 1f) || z < -1f) && !changingGear && direction != -1)
		{
			StartCoroutine("ChangingGear", -1);
		}
		else if (brakeInput < 0.1f && z > -1f && direction == -1 && !changingGear)
		{
			StartCoroutine("ChangingGear", 0);
		}
		if (currentGear < totalGears - 1 && !changingGear && speed >= gearSpeed[currentGear] * 0.7f && FrontLeftWheelCollider.rpm > 0f)
		{
			StartCoroutine("ChangingGear", currentGear + 1);
		}
		if (currentGear > 0 && !changingGear && speed < gearSpeed[currentGear - 1] * 0.5f && direction != -1)
		{
			StartCoroutine("ChangingGear", currentGear - 1);
		}
	}

	internal IEnumerator ChangingGear(int gear)
	{
		changingGear = true;
		yield return new WaitForSeconds(gearShiftingDelay);
		if (gear == -1)
		{
			currentGear = 0;
			direction = -1;
		}
		else
		{
			currentGear = gear;
			direction = 1;
		}
		changingGear = false;
	}

	private void RevLimiter()
	{
		if (engineRPM >= maxEngineRPM * 1.05f)
		{
			cutGas = true;
		}
		else if (engineRPM < maxEngineRPM)
		{
			cutGas = false;
		}
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
			int num2 = (int)(carCfg.hitByCarPlayerDropHpRate1 * num + carCfg.hitByCarPlayerDropHpRate2);
			if (num2 > 0 && roleId > 0)
			{
				cCauseDamgeByVehicle.vehicleId = carId;
				cCauseDamgeByVehicle.vehicleHp = hp;
				cCauseDamgeByVehicle.playerId2Damage.Clear();
				cCauseDamgeByVehicle.playerId2Damage[roleId] = num2;
				Client2Gs.Ins.Send(cCauseDamgeByVehicle);
				cVehicleHitPlayer.roleId = componentInParent.RoleId;
				cVehicleHitPlayer.vehicleId = carId;
				cVehicleHitPlayer.speed = num;
				cVehicleHitPlayer.forward.x = MathUtils.Float2Short(rigid.velocity.x);
				cVehicleHitPlayer.forward.y = MathUtils.Float2Short(rigid.velocity.y);
				cVehicleHitPlayer.forward.z = MathUtils.Float2Short(rigid.velocity.z);
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
				if (rigid.velocity.y > 3f)
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
		if (rigid.velocity.magnitude > 1f)
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

	public void OnHit(float relativeSpeed)
	{
		if (m_HitTimeLimit > 0f)
		{
			return;
		}
		m_HitTimeLimit = 1.5f;
		int num = (int)(carCfg.hitCarDropHpRate1 * relativeSpeed + carCfg.hitCarDropHpRate2);
		int num2 = (int)((carCfg.hitCarPlayerDropHpRate1 * relativeSpeed + carCfg.hitCarPlayerDropHpRate2) * (float)ConstsBs.HpPlayer * 0.0001f);
		if (num > 0)
		{
			ReduceHp(num);
		}
		cCauseDamgeByVehicle.vehicleId = carId;
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
			Transform[] array2 = tanshenPoints;
			foreach (Transform transform2 in array2)
			{
				if (transform2 != null)
				{
					BasePlayerController componentInChildren2 = transform2.GetComponentInChildren<BasePlayerController>();
					if (componentInChildren2 != null)
					{
						cCauseDamgeByVehicle.playerId2Damage[componentInChildren2.RoleId] = num2;
					}
				}
			}
		}
		if (num > 0 || num2 > 0)
		{
			Client2Gs.Ins.Send(cCauseDamgeByVehicle);
		}
	}

	internal void PlayNitrogenSound()
	{
		if (fuel > 0f)
		{
			PlaySound(SoundId.engineTurboSoundId);
		}
	}

	private bool OverTorque()
	{
		if ((nitrogenInput <= 0f && speed > maxspeed) || (nitrogenInput > 0f && speed > maxSpeedWithNitrogen) || !engineRunning || fuel <= 0f)
		{
			return true;
		}
		return false;
	}

	public void TorqueCurve()
	{
		maxspeed = ((!(nitrogenInput > 0f)) ? normalMaxSpeed : maxSpeedWithNitrogen);
		if (defMaxSpeed == maxspeed)
		{
			return;
		}
		if (totalGears < 1)
		{
			Debug.LogError("You are trying to set your vehicle gear to 0 or below! Why you trying to do this???");
			totalGears = 1;
			return;
		}
		if (engineTorqueCurve == null)
		{
			engineTorqueCurve = new AnimationCurve[totalGears];
		}
		currentGear = 0;
		for (int i = 0; i < engineTorqueCurve.Length; i++)
		{
			if (engineTorqueCurve[i] == null)
			{
				engineTorqueCurve[i] = new AnimationCurve(new Keyframe(0f, 1f));
				continue;
			}
			for (int j = 0; j < engineTorqueCurve[i].length; j++)
			{
				engineTorqueCurve[i].RemoveKey(j);
			}
		}
		if (autoGenerateTargetSpeedsForChangingGear && gearSpeed == null)
		{
			gearSpeed = new float[totalGears];
		}
		for (int k = 0; k < totalGears; k++)
		{
			if (autoGenerateTargetSpeedsForChangingGear)
			{
				gearSpeed[k] = Mathf.Lerp(0f, maxspeed, (float)(k + 1) / (float)totalGears);
			}
			if (k != 0)
			{
				engineTorqueCurve[k].MoveKey(0, new Keyframe(0f, Mathf.Lerp(0.25f, 0f, (float)(k + 1) / (float)totalGears)));
				engineTorqueCurve[k].AddKey(Mathf.Lerp(0f, maxspeed / 1f, (float)k / (float)totalGears), Mathf.Lerp(1f, 0.25f, (float)k / (float)totalGears));
				engineTorqueCurve[k].AddKey(gearSpeed[k], 0.1f);
				engineTorqueCurve[k].AddKey(gearSpeed[k] * 2f, -3f);
				engineTorqueCurve[k].postWrapMode = WrapMode.Once;
			}
			else
			{
				engineTorqueCurve[k].MoveKey(0, new Keyframe(0f, 1f));
				engineTorqueCurve[k].AddKey(gearSpeed[k] / 3f, 1f);
				engineTorqueCurve[k].AddKey(gearSpeed[k], 0f);
				engineTorqueCurve[k].postWrapMode = WrapMode.Once;
			}
			defMaxSpeed = maxspeed;
		}
	}

	private void OnDestroy()
	{
		SingletonMono<AudioManager>.Ins.DestroyAttachedSound(base.gameObject);
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

	public int GetEngineSoundId()
	{
		SoundCfg soundCfg = SoundCfg.Get(carCfg.soundIds[5]);
		return soundCfg.id;
	}

	public void OnDamage(float damage, Vector3 pos)
	{
		if (hp > 0)
		{
			ReduceHp((int)damage);
			cCauseDamgeToVehicle.damage = (int)damage;
			cCauseDamgeToVehicle.vehicleId = carId;
			Client2Gs.Ins.Send(cCauseDamgeToVehicle);
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

	private void OnHpChange(bool showExplosion = true)
	{
		float num = (float)hp / (float)carCfg.maxHp;
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
			rigid.AddExplosionForce(10f, base.transform.position, 1f, 0f, ForceMode.VelocityChange);
			StopPlaySound(SoundId.engineIdleSoundId);
			StopPlaySound(SoundId.engineHighSpeedSoundId);
			PlaySound(SoundId.explosionSoundId, false, false, 1f, 1f, false);
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

	public Transform[] GetTanshenPoints()
	{
		return tanshenPoints;
	}

	public bool IsBroken()
	{
		return hp <= 0;
	}

	public float GetMaxFuel()
	{
		return carCfg.maxFuel;
	}

	public int GetHp()
	{
		return hp;
	}

	public int GetMaxHp()
	{
		return carCfg.maxHp;
	}

	public void SyncWheelRotation(SSyncCarWheelRotation sSyncCarWheelRotation)
	{
		if (!needSync)
		{
			if (FrontLeftWheelCollider != null)
			{
				FrontLeftWheelCollider.OnSSyncCarWheelRotation(sSyncCarWheelRotation);
			}
			if (FrontRightWheelCollider != null)
			{
				FrontRightWheelCollider.OnSSyncCarWheelRotation(sSyncCarWheelRotation);
			}
			if (RearLeftWheelCollider != null)
			{
				RearLeftWheelCollider.OnSSyncCarWheelRotation(sSyncCarWheelRotation);
			}
			if (RearRightWheelCollider != null)
			{
				RearRightWheelCollider.OnSSyncCarWheelRotation(sSyncCarWheelRotation);
			}
		}
	}

	public void SyncEngineSound(float volume)
	{
		engineVolumeFromSync = volume;
		_mStopIdleSound = false;
	}

	private void ForceStopPlaySound(SoundId soundId)
	{
		SoundCfg soundCfg = SoundCfg.Get(carCfg.soundIds[(int)soundId]);
		SingletonMono<AudioManager>.Ins.StopPlayOnTarget(soundCfg.path, base.gameObject);
	}

	public void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		_mStopIdleSound = true;
		ForceStopPlaySound(SoundId.engineIdleSoundId);
		ForceStopPlaySound(SoundId.engineHighSpeedSoundId);
	}
}

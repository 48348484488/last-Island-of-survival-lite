using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using SC.LargeScene;
using SC.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;
using cfg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class Battle : MonoBehaviour
{
	private sealed class VehicleLoader
	{
		private int type;

		private int vehicleId;

		private VehicleCfg vehicleCfg;

		private bool replaceOld;

		public VehicleLoader(int type, int vehicleId, bool replaceOld = false)
		{
			this.type = type;
			this.vehicleId = vehicleId;
			this.replaceOld = replaceOld;
		}

		public void Load()
		{
			vehicleCfg = VehicleCfg.Get(type);
			if (vehicleCfg != null)
			{
				ResMgr.Ins.CreateFromAB(vehicleCfg.abPath, null, OnVehicleLoaded);
			}
		}

		private void InitVehicle(GameObject go, SVehicleInfo sVehicleInfo)
		{
			Transform transform = go.transform;
			transform.SetParent(Ins.m_VehiclesParent);
			transform.position = new Vector3(sVehicleInfo.vehicleInfo.pos.x, sVehicleInfo.vehicleInfo.pos.y, sVehicleInfo.vehicleInfo.pos.z);
			transform.eulerAngles = new Vector3(sVehicleInfo.vehicleInfo.orientation.x, sVehicleInfo.vehicleInfo.orientation.y, sVehicleInfo.vehicleInfo.orientation.z);
			if (vehicleCfg.vehicleType == 0)
			{
				CarController carController = go.AddComponent<CarController>();
				CarCfg carCfg = CarCfg.Get(type);
				CarInfo component = go.GetComponent<CarInfo>();
				carController.Init(component, carCfg, sVehicleInfo);
			}
			else if (vehicleCfg.vehicleType == 2)
			{
				HelicopterController helicopterController = go.AddComponent<HelicopterController>();
				PlaneCfg planeCfg = PlaneCfg.Get(type);
				PlaneInfo component2 = go.GetComponent<PlaneInfo>();
				helicopterController.Init(component2, planeCfg, sVehicleInfo);
			}
			else if (vehicleCfg.vehicleType == 1)
			{
				ShipControll shipControll = go.AddComponent<ShipControll>();
				ShipCfg shipCfg = ShipCfg.Get(type);
				ShipInfo component3 = go.GetComponent<ShipInfo>();
				shipControll.Init(component3, shipCfg, sVehicleInfo);
			}
			else if (vehicleCfg.vehicleType == 3)
			{
				TwoWheelMotoController twoWheelMotoController = go.AddComponent<TwoWheelMotoController>();
				TwoWheelMotoCfg twoWheelMotoCfg = TwoWheelMotoCfg.Get(type);
				TwoWheelMotoInfo component4 = go.GetComponent<TwoWheelMotoInfo>();
				twoWheelMotoController.Init(component4, twoWheelMotoCfg, sVehicleInfo);
			}
			VehicleMonitor vehicleMonitor = go.AddComponent<VehicleMonitor>();
			Ins.battleObjectId2VehicleMonitor.Add(sVehicleInfo.vehicleInfo.id, vehicleMonitor);
			vehicleMonitor.Init(sVehicleInfo);
		}

		private void OnVehicleLoaded(GameObject go)
		{
			if (replaceOld)
			{
				VehicleMonitor value = null;
				if (Ins.battleObjectId2VehicleMonitor.TryGetValue(vehicleId, out value))
				{
					SVehicleInfo sVehicleInfo = value.GetSVehicleInfo();
					CleanVehicleMonitor(value);
					UnityEngine.Object.DestroyImmediate(value.gameObject);
					Ins.battleObjectId2VehicleMonitor.Remove(vehicleId);
					InitVehicle(go, sVehicleInfo);
				}
				else
				{
					UnityEngine.Object.Destroy(go);
				}
			}
			else
			{
				SVehicleInfo value2 = null;
				if (Ins.battleObjectId2VehicleInfo.TryGetValue(vehicleId, out value2) && !Ins.battleObjectId2VehicleMonitor.ContainsKey(value2.vehicleInfo.id))
				{
					InitVehicle(go, value2);
				}
				else
				{
					UnityEngine.Object.Destroy(go);
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003COnSceneChanged_003Ec__AnonStorey7
	{
		internal SceneSoundCfg sceneSoundCfg;

		internal SoundCfg soundCfg;

		internal Battle _0024this;

		internal void _003C_003Em__0(object[] go)
		{
			_0024this.SpaceSceneSound(sceneSoundCfg, soundCfg);
		}
	}

	[CompilerGenerated]
	private sealed class _003CSpaceSceneSound_003Ec__AnonStorey8
	{
		internal SceneSoundCfg sceneSoundCfg;

		internal SoundCfg soundCfg;

		internal void _003C_003Em__0()
		{
			if ((SingletonMono<DayNightSystem>.Ins.IsNight && sceneSoundCfg.dayType == 1) || (!SingletonMono<DayNightSystem>.Ins.IsNight && sceneSoundCfg.dayType == 0))
			{
				SingletonMono<AudioManager>.Ins.Play2D(soundCfg.id);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003COnSThrowGrenade_003Ec__AnonStorey9
	{
		internal SThrowGrenade msg;

		internal Battle _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (!_0024this.MapObjectDic.ContainsKey(msg.grenadeInsId))
			{
				_0024this.MapObjectDic.Add(msg.grenadeInsId, o.GetComponent<MapObject>());
				o.GetComponent<Rigidbody>().useGravity = false;
				o.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
	}

	public bool PlayTest;

	public bool LoadFinish;

	public bool QuickLooking;

	public PlayerController SelfPlayer;

	public GameObject SelfGameObject;

	public vThirdPersonCamera MainCamera;

	public ObjectPool<BulletControl> BulletPool;

	public ObjectPool<Bullet3rdControl> Bullet3rdPool;

	public ObjectPool<BulletControl> RpgBulletPool;

	public ObjectPool<Decal> DecalPool;

	private GameObject m_RolePlayerModel;

	private ObjectPool<OtherPlayerController> OtherRolePlayerPool;

	private GameObject m_Bullet;

	private GameObject m_Bullet3rd;

	private GameObject m_RpgBullet;

	private GameObject m_Decal;

	public Dictionary<long, OtherPlayerController> OtherPlayersDic = new Dictionary<long, OtherPlayerController>();

	public Dictionary<long, OtherPlayerController> NearOtherPlayersDic = new Dictionary<long, OtherPlayerController>();

	public int GroundLayer;

	public int CheckUnderGroundLayer;

	private Transform m_BulletParent;

	private Transform m_RpgBulletParent;

	private Transform m_DecalParent;

	private Transform m_OtherPlayerPoolParent;

	private Transform m_VehiclesParent;

	private Transform m_MineParent;

	private Transform m_plantParent;

	public GameObject JingUI;

	public BattlePanel MyBattlePanel;

	private static SToxicGasZone sToxicGasZone;

	private static SSafeZone sSafeZone;

	private bool isToxicGasShrink;

	private float gasShrinkStartTime;

	private float gasShrinkEndTime;

	private Transform ToxicgasTrans;

	private float shrinkTotalTime;

	private float waitForShrinkTotalTime;

	private float shrinkBeginRadius;

	private float shrinkEndRadius;

	private float shrinkBeginX;

	private float shrinkBeginY;

	public Dictionary<int, GameObject> BattleObjectId2GameObject = new Dictionary<int, GameObject>();

	public Dictionary<long, OtherPlayerController> TeamPlayerDic = new Dictionary<long, OtherPlayerController>();

	private readonly Dictionary<int, SVehicleInfo> battleObjectId2VehicleInfo = new Dictionary<int, SVehicleInfo>();

	private readonly Dictionary<int, VehicleMonitor> battleObjectId2VehicleMonitor = new Dictionary<int, VehicleMonitor>();

	private static readonly CGetInVehicle cGetInVehicle = new CGetInVehicle();

	private static readonly CGetOutVehicle cGetOutVehicle = new CGetOutVehicle();

	private static readonly CSwitchSeat cSwitchSeat = new CSwitchSeat();

	private static readonly CMakeMark cMakeMark = new CMakeMark();

	private static readonly CRemoveMark cRemoveMark = new CRemoveMark();

	public Camera UICamrea;

	private bool hasMark;

	private Vec3 mark;

	private FPSCounter m_fps;

	public static Battle Ins;

	public Dictionary<int, TreeInfo> TreesDic = new Dictionary<int, TreeInfo>();

	public Dictionary<int, TreeDetail> _treeIdToTreeDetail = new Dictionary<int, TreeDetail>();

	public Dictionary<long, Mine> MineDic = new Dictionary<long, Mine>();

	public Dictionary<long, STrashCan> LajitongDic = new Dictionary<long, STrashCan>();

	public Dictionary<long, STrashCan> XiangziDic = new Dictionary<long, STrashCan>();

	public Dictionary<long, PlantInfo> PlantDic = new Dictionary<long, PlantInfo>();

	public Dictionary<long, MapObject> MapObjectDic = new Dictionary<long, MapObject>();

	public Dictionary<int, ObjectPool<PlantInfo>> PlantPoolDic = new Dictionary<int, ObjectPool<PlantInfo>>();

	public Dictionary<int, ObjectPool<Mine>> MinePoolDic = new Dictionary<int, ObjectPool<Mine>>();

	public List<int> CanHitTreeWapId = new List<int>();

	public List<int> CanHitMineWapId = new List<int>();

	public Dictionary<string, List<int>> SceneName2SoundIds = new Dictionary<string, List<int>>();

	public Light MainLight;

	public long ControlAiID = -1L;

	public OtherPlayerController Other;

	public AimHelper AimHelp;

	private Vector3 safeZoneCenter;

	private Vector3 gasZoneCenter;

	private Vector3 m_GasCenterMoveSpeed;

	private float m_GasStartTime;

	public PhysicMaterial MinPhysicMaterial;

	public PhysicMaterial MidPhysicMaterial;

	public PhysicMaterial MaxPhysicMaterial;

	public GameObject StartPlane;

	public GameObject TuZhiGo;

	public Vector3 StartPos;

	public PlayerInfo SelfInfo;

	private WaitForSeconds m_ZeroOne = new WaitForSeconds(0.1f);

	private Material ToxicgasMaterial;

	private Vector2 m_500tietuTiling = new Vector2(20f, 10f);

	private Vector2 m_500nodeTiling = new Vector2(2f, 1f);

	private Vector2 m_1500tietuTiling = new Vector2(30f, 3f);

	private Vector2 m_1500nodeTiling = new Vector2(6f, 1f);

	private float m_LastGasRadius;

	public float WaterSurfaceHeight;

	private float WaterDeep;

	private bool m_lastCameraInWater;

	private readonly CSyncSound m_SyncSoundMsg = new CSyncSound();

	private int _mGasolineId;

	private bool _mIsAddGasoline;

	private float _mUseGasolineTime;

	private string _mGasolineName = string.Empty;

	private VehicleMonitor _mVehicleMonitor;

	private Coroutine _mCoroutine;

	private int m_LastLookedIndex = -1;

	private static readonly float[] OtherPlayerDistancesArray = new float[2] { 50f, 600f };

	private static readonly float[] VehicleDistancesArray = new float[2] { 50f, 600f };

	private CullingGroup s_OtherPlayerCullingGroup;

	private CullingGroup s_VehicleCullingGroup;

	private static readonly BoundingSphere[] s_OtherPlayerBoundingSphereArray = new BoundingSphere[256];

	private static readonly BoundingSphere[] s_VehicleBoundingSphereArray = new BoundingSphere[128];

	private static readonly OtherPlayerController[] s_OtherPlayersForCulling = new OtherPlayerController[256];

	private static readonly VehicleMonitor[] s_VehiclesForCulling = new VehicleMonitor[128];

	private int m_OtherPlayerForCullingCount;

	private int m_VehicleCountForCulling;

	private bool m_SendCullingMsg;

	private static readonly CAddRolesToCanSee cAddRolesToCanSee = new CAddRolesToCanSee();

	private static readonly CRemoveRolesFromCanSee cRemoveRolesFromCanSee = new CRemoveRolesFromCanSee();

	private static readonly CAddVehiclesToCanSee cAddVehiclesToCanSee = new CAddVehiclesToCanSee();

	private static readonly CRemoveVehiclesFromCanSee cRemoveVehiclesFromCanSee = new CRemoveVehiclesFromCanSee();

	[CompilerGenerated]
	private static ObjectPool<OtherPlayerController>.DestroyObject<OtherPlayerController> _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static ObjectPool<Decal>.DestroyObject<Decal> _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static ObjectPool<Decal>.RecycleObject<Decal> _003C_003Ef__am_0024cache2;

	[CompilerGenerated]
	private static ObjectPool<BulletControl>.DestroyObject<BulletControl> _003C_003Ef__am_0024cache3;

	[CompilerGenerated]
	private static ObjectPool<Bullet3rdControl>.DestroyObject<Bullet3rdControl> _003C_003Ef__am_0024cache4;

	[CompilerGenerated]
	private static ObjectPool<BulletControl>.DestroyObject<BulletControl> _003C_003Ef__am_0024cache5;

	public GameObject GetOneRpgBullet
	{
		get
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_RpgBullet.gameObject, Vector3.zero, Quaternion.identity);
			gameObject.name = "rpgzidan";
			gameObject.transform.Find("EF_rpg_02").gameObject.SetActive(false);
			gameObject.GetComponent<Collider>().enabled = true;
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			component.isKinematic = true;
			gameObject.SetActive(true);
			return gameObject;
		}
	}

	private void Awake()
	{
		if (PlayTest)
		{
			PlayTest = false;
		}
		s_OtherPlayerCullingGroup = new CullingGroup();
		s_VehicleCullingGroup = new CullingGroup();
		m_fps = GameObject.Find("FPS").GetComponent<FPSCounter>();
		MainLight = GameObject.Find("Main Light").GetComponent<Light>();
		Ins = this;
		LoadFinish = false;
		Init();
		base.gameObject.AddComponent<LargeSceneManager>();
		QuickLooking = false;
		MainCamera = Camera.main.GetComponent<vThirdPersonCamera>();
		StartConroutine(SingletonMono<PlantMgr>.Ins.DownTime());
		TeamScEvent.TeammateCutDownAction = (Action<long>)Delegate.Combine(TeamScEvent.TeammateCutDownAction, new Action<long>(_003CAwake_003Em__0));
		foreach (TreeCfg all in TreeCfg.GetAllList())
		{
			foreach (CutTreeToolInfo cutTreeToolInfo in all.cutTreeToolInfos)
			{
				CanHitTreeWapId.Add(cutTreeToolInfo.cutTreeToolItemId);
			}
		}
		foreach (MineCfg all2 in MineCfg.GetAllList())
		{
			foreach (MineToolInfo mineToolInfo in all2.mineToolInfos)
			{
				CanHitMineWapId.Add(mineToolInfo.toolItemId);
			}
		}
		SingletonMono<BuildManager>.Ins.BuildPartParent = new GameObject("BuildPartParent");
		QualitySettings.SetQualityLevel(SettingMgr.QualityLevel);
		PerformanceUtils.SetShadow(SettingMgr.ShadowQuality);
		PerformanceUtils.SetRender(SettingMgr.RenderQuality);
		PerformanceUtils.SetFrame(SettingMgr.FrameRateQuality);
		PerformanceUtils.SetResolution(SettingMgr.ResolutionLevel);
	}

	public void Init()
	{
		BattleEvent.OnLoadBattleSceneFinish = (BattleEvent.OnloadBattleSceneFinish)Delegate.Combine(BattleEvent.OnLoadBattleSceneFinish, new BattleEvent.OnloadBattleSceneFinish(OnSLoadBattleSceneFinish));
		SceneEvent.InitLoadSceneFinish = (Utils.VoidDelegate)Delegate.Combine(SceneEvent.InitLoadSceneFinish, new Utils.VoidDelegate(OnLoadSceneFinish));
		BagEvent.PutEquipByItemId2Battle = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutEquipByItemId2Battle, new Utils.IntDelegate(OnSelfPickWear));
		BagEvent.PutSkinByItemId2Battle = (Utils.IntDelegate)Delegate.Combine(BagEvent.PutSkinByItemId2Battle, new Utils.IntDelegate(OnSelfPickWear));
		BagEvent.RemoveEquipByItemId2Battle = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveEquipByItemId2Battle, new Utils.IntDelegate(OnSelfDropWear));
		BagEvent.RemoveSkinByItemId2Battle = (Utils.IntDelegate)Delegate.Combine(BagEvent.RemoveSkinByItemId2Battle, new Utils.IntDelegate(OnSelfDropWear));
		BagEvent.IsShowEquipBoolDelegate = (Utils.BoolDelegate)Delegate.Combine(BagEvent.IsShowEquipBoolDelegate, new Utils.BoolDelegate(OnIsShowEquipBool));
		BattlePackEvent.EquipHpDelegate = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.EquipHpDelegate, new Utils.Int2Delegate(OnSelfEquipHpDelegate));
		SPlayerInfo.handler = (SPlayerInfo.Handler)Delegate.Combine(SPlayerInfo.handler, new SPlayerInfo.Handler(OnSplayerInfo));
		SPlayerDisappear.handler = (SPlayerDisappear.Handler)Delegate.Combine(SPlayerDisappear.handler, new SPlayerDisappear.Handler(OnSPlayerDisappear));
		SPlayEffectAtPos.handler = (SPlayEffectAtPos.Handler)Delegate.Combine(SPlayEffectAtPos.handler, new SPlayEffectAtPos.Handler(OnSPlayEffectAtPos));
		SAttachEffect.handler = (SAttachEffect.Handler)Delegate.Combine(SAttachEffect.handler, new SAttachEffect.Handler(OnSAttachEffect));
		SHitPlayer.handler = (SHitPlayer.Handler)Delegate.Combine(SHitPlayer.handler, new SHitPlayer.Handler(OnSHitPlayer));
		SEnterBuildState.handler = (SEnterBuildState.Handler)Delegate.Combine(SEnterBuildState.handler, new SEnterBuildState.Handler(OnSEnterBuildState));
		SExitBuildState.handler = (SExitBuildState.Handler)Delegate.Combine(SExitBuildState.handler, new SExitBuildState.Handler(OnSExitBuildState));
		SLevelFlyBattlePlane.handler = (SLevelFlyBattlePlane.Handler)Delegate.Combine(SLevelFlyBattlePlane.handler, new SLevelFlyBattlePlane.Handler(OnLeavelFlyBattlePlane));
		SSyncFootTexture.handler = (SSyncFootTexture.Handler)Delegate.Combine(SSyncFootTexture.handler, new SSyncFootTexture.Handler(OnSyncFootTexture));
		SSyncPlayerPos.handler = (SSyncPlayerPos.Handler)Delegate.Combine(SSyncPlayerPos.handler, new SSyncPlayerPos.Handler(OnSSyncPlayerPos));
		SSyncOrientation.handler = (SSyncOrientation.Handler)Delegate.Combine(SSyncOrientation.handler, new SSyncOrientation.Handler(OnSyncOrientation));
		SSyncVelocity.handler = (SSyncVelocity.Handler)Delegate.Combine(SSyncVelocity.handler, new SSyncVelocity.Handler(OnSyncVelocity));
		SSyncAnimator.handler = (SSyncAnimator.Handler)Delegate.Combine(SSyncAnimator.handler, new SSyncAnimator.Handler(OnSyncAnimator));
		SSyncLayerWeight.handler = (SSyncLayerWeight.Handler)Delegate.Combine(SSyncLayerWeight.handler, new SSyncLayerWeight.Handler(OnSSyncLayerWeight));
		SSyncAnimatorSpeed.handler = (SSyncAnimatorSpeed.Handler)Delegate.Combine(SSyncAnimatorSpeed.handler, new SSyncAnimatorSpeed.Handler(OnSyncAnimatorSpeed));
		SSyncYaw.handler = (SSyncYaw.Handler)Delegate.Combine(SSyncYaw.handler, new SSyncYaw.Handler(OnSyncYaw));
		SServerPullback.handler = (SServerPullback.Handler)Delegate.Combine(SServerPullback.handler, new SServerPullback.Handler(OnSServerPullBack));
		SHpChange.handler = (SHpChange.Handler)Delegate.Combine(SHpChange.handler, new SHpChange.Handler(OnSHpChange));
		SNewBeeStatusChange.handler = (SNewBeeStatusChange.Handler)Delegate.Combine(SNewBeeStatusChange.handler, new SNewBeeStatusChange.Handler(OnSNewBeeStatusChange));
		SHpStatusChange.handler = (SHpStatusChange.Handler)Delegate.Combine(SHpStatusChange.handler, new SHpStatusChange.Handler(OnHpStatusChange));
		SEpChange.handler = (SEpChange.Handler)Delegate.Combine(SEpChange.handler, new SEpChange.Handler(OnSEpChange));
		SSyncCameraLookPoint.handler = (SSyncCameraLookPoint.Handler)Delegate.Combine(SSyncCameraLookPoint.handler, new SSyncCameraLookPoint.Handler(OnSyncCameraLookPoint));
		BattleEvent.OnGunModeLoadFinishDelegate = (BattleEvent.OnGunModeLoadFinish)Delegate.Combine(BattleEvent.OnGunModeLoadFinishDelegate, new BattleEvent.OnGunModeLoadFinish(OnGunModeLoadFinish));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
		SSyncFallGround.handler = (SSyncFallGround.Handler)Delegate.Combine(SSyncFallGround.handler, new SSyncFallGround.Handler(OnSyncFallGround));
		SSyncHandWeapon.handler = (SSyncHandWeapon.Handler)Delegate.Combine(SSyncHandWeapon.handler, new SSyncHandWeapon.Handler(OnSyncHandWeapon));
		SOtherDropWear.handler = (SOtherDropWear.Handler)Delegate.Combine(SOtherDropWear.handler, new SOtherDropWear.Handler(OnSDropWear));
		SOtherPickWear.handler = (SOtherPickWear.Handler)Delegate.Combine(SOtherPickWear.handler, new SOtherPickWear.Handler(OnPickWear));
		SSyncOtherWear.handler = (SSyncOtherWear.Handler)Delegate.Combine(SSyncOtherWear.handler, new SSyncOtherWear.Handler(OnSyncOtherWear));
		SSyncGunPutPart.handler = (SSyncGunPutPart.Handler)Delegate.Combine(SSyncGunPutPart.handler, new SSyncGunPutPart.Handler(OnSSyncGunPutPart));
		SSyncGunRemovePart.handler = (SSyncGunRemovePart.Handler)Delegate.Combine(SSyncGunRemovePart.handler, new SSyncGunRemovePart.Handler(OnSSyncGunRemovePart));
		SShoot.handler = (SShoot.Handler)Delegate.Combine(SShoot.handler, new SShoot.Handler(OnSShoot));
		SSafeZone.handler = (SSafeZone.Handler)Delegate.Combine(SSafeZone.handler, new SSafeZone.Handler(OnSSafeZone));
		SToxicGasZone.handler = (SToxicGasZone.Handler)Delegate.Combine(SToxicGasZone.handler, new SToxicGasZone.Handler(OnSToxicGasZone));
		SGenSafeZone.handler = (SGenSafeZone.Handler)Delegate.Combine(SGenSafeZone.handler, new SGenSafeZone.Handler(OnSGenSafeZone));
		SStartShrinkToxicGas.handler = (SStartShrinkToxicGas.Handler)Delegate.Combine(SStartShrinkToxicGas.handler, new SStartShrinkToxicGas.Handler(OnSStartShrinkToxicGas));
		SEndShrinkToxicGas.handler = (SEndShrinkToxicGas.Handler)Delegate.Combine(SEndShrinkToxicGas.handler, new SEndShrinkToxicGas.Handler(OnSEndShrinkToxicGas));
		SRequestRobot.handler = (SRequestRobot.Handler)Delegate.Combine(SRequestRobot.handler, new SRequestRobot.Handler(OnSRequestRobot));
		SRoleLoadBullet.handler = (SRoleLoadBullet.Handler)Delegate.Combine(SRoleLoadBullet.handler, new SRoleLoadBullet.Handler(OnSRoleLoadBullt));
		SShowMine.handler = (SShowMine.Handler)Delegate.Combine(SShowMine.handler, new SShowMine.Handler(OnSShowMine));
		SHideMine.handler = (SHideMine.Handler)Delegate.Combine(SHideMine.handler, new SHideMine.Handler(OnSHideMine));
		SMineBlow.handler = (SMineBlow.Handler)Delegate.Combine(SMineBlow.handler, new SMineBlow.Handler(OnSMineBlow));
		SUpgradeBuilding.handler = (SUpgradeBuilding.Handler)Delegate.Combine(SUpgradeBuilding.handler, new SUpgradeBuilding.Handler(OnUpdateBuilding));
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		SBuildingHpChange.handler = (SBuildingHpChange.Handler)Delegate.Combine(SBuildingHpChange.handler, new SBuildingHpChange.Handler(OnSBuildingHpChange));
		SSendNearBuildingFinish.handler = (SSendNearBuildingFinish.Handler)Delegate.Combine(SSendNearBuildingFinish.handler, new SSendNearBuildingFinish.Handler(OnSendNearBuildingFinish));
		SMapObjectHpChange.handler = (SMapObjectHpChange.Handler)Delegate.Combine(SMapObjectHpChange.handler, new SMapObjectHpChange.Handler(OnSMapObjectHpChange));
		InitColliderPhysic();
		SVehicleInfo.handler = (SVehicleInfo.Handler)Delegate.Combine(SVehicleInfo.handler, new SVehicleInfo.Handler(OnSVehicleInfo));
		SVehicleDisappear.handler = (SVehicleDisappear.Handler)Delegate.Combine(SVehicleDisappear.handler, new SVehicleDisappear.Handler(OnSVehicleDisappear));
		SGetInVehicle.handler = (SGetInVehicle.Handler)Delegate.Combine(SGetInVehicle.handler, new SGetInVehicle.Handler(OnSGetInVehicle));
		SGetOutVehicle.handler = (SGetOutVehicle.Handler)Delegate.Combine(SGetOutVehicle.handler, new SGetOutVehicle.Handler(OnSGetOutVehicle));
		SSyncVehiclePos.handler = (SSyncVehiclePos.Handler)Delegate.Combine(SSyncVehiclePos.handler, new SSyncVehiclePos.Handler(OnSSyncVehiclePos));
		SSyncVehicleOrientation.handler = (SSyncVehicleOrientation.Handler)Delegate.Combine(SSyncVehicleOrientation.handler, new SSyncVehicleOrientation.Handler(OnSSyncVehicleOrientation));
		SSyncVehicleVelocity.handler = (SSyncVehicleVelocity.Handler)Delegate.Combine(SSyncVehicleVelocity.handler, new SSyncVehicleVelocity.Handler(OnSSyncVehicleVelocity));
		SSwitchSeat.handler = (SSwitchSeat.Handler)Delegate.Combine(SSwitchSeat.handler, new SSwitchSeat.Handler(OnSSwitchSeat));
		SSyncTanshen.handler = (SSyncTanshen.Handler)Delegate.Combine(SSyncTanshen.handler, new SSyncTanshen.Handler(OnSyncTanshen));
		SSyncVehicleFuel.handler = (SSyncVehicleFuel.Handler)Delegate.Combine(SSyncVehicleFuel.handler, new SSyncVehicleFuel.Handler(OnSSyncVehicleFuel));
		SSyncSound.handler = (SSyncSound.Handler)Delegate.Combine(SSyncSound.handler, new SSyncSound.Handler(OnSSyncSound));
		SSyncEngineSound.handler = (SSyncEngineSound.Handler)Delegate.Combine(SSyncEngineSound.handler, new SSyncEngineSound.Handler(OnSSyncEngineSound));
		SSyncCarWheelRotation.handler = (SSyncCarWheelRotation.Handler)Delegate.Combine(SSyncCarWheelRotation.handler, new SSyncCarWheelRotation.Handler(OnSSyncCarWheelRotation));
		SStopVehicleSound.handler = (SStopVehicleSound.Handler)Delegate.Combine(SStopVehicleSound.handler, new SStopVehicleSound.Handler(OnSStopVehicleSound));
		SVehicleHpChange.handler = (SVehicleHpChange.Handler)Delegate.Combine(SVehicleHpChange.handler, new SVehicleHpChange.Handler(OnSVehicleHpChange));
		SGetInVehicleError.handler = (SGetInVehicleError.Handler)Delegate.Combine(SGetInVehicleError.handler, new SGetInVehicleError.Handler(OnSGetInVehicleError));
		SVehicleSkinChange.handler = (SVehicleSkinChange.Handler)Delegate.Combine(SVehicleSkinChange.handler, new SVehicleSkinChange.Handler(OnSVehicleSkinChange));
		m_SendCullingMsg = true;
		SThrowGrenade.handler = (SThrowGrenade.Handler)Delegate.Combine(SThrowGrenade.handler, new SThrowGrenade.Handler(OnSThrowGrenade));
		SSyncMapObjectPos.handler = (SSyncMapObjectPos.Handler)Delegate.Combine(SSyncMapObjectPos.handler, new SSyncMapObjectPos.Handler(OnSSyncGrenade));
		SGrenadeExplode.handler = (SGrenadeExplode.Handler)Delegate.Combine(SGrenadeExplode.handler, new SGrenadeExplode.Handler(OnSGrenadeExplode));
		SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(OnSRebitrh));
		SettingEvent.SensitivityVerticalDelegate = (Utils.FloatDelegate)Delegate.Combine(SettingEvent.SensitivityVerticalDelegate, new Utils.FloatDelegate(OnSensitivityY));
		SettingEvent.SensitivityHorizontalDelegate = (Utils.FloatDelegate)Delegate.Combine(SettingEvent.SensitivityHorizontalDelegate, new Utils.FloatDelegate(OnSensitivityX));
		UICamrea = ViewMgr.Ins.UICamera;
		UICamrea.clearFlags = CameraClearFlags.Depth;
		GroundLayer = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Car")) | 0x8000 | 0x40000 | 0x80000 | 0x40000000;
		CheckUnderGroundLayer = (1 << LayerMask.NameToLayer("Car")) | 0x8000 | 0x40000 | 0x80000 | 0x40000000;
		ResMgr.Ins.LoadAB("sound/" + SoundCfg.Get(323).path + ".ab", null, false);
		ResMgr.Ins.LoadAB("sound/" + SoundCfg.Get(319).path + ".ab", null, false);
		ResMgr.Ins.LoadAB("sound/" + SoundCfg.Get(162).path + ".ab", null, false);
		SShowTree.handler = (SShowTree.Handler)Delegate.Combine(SShowTree.handler, new SShowTree.Handler(OnSShowTree));
		SHideTree.handler = (SHideTree.Handler)Delegate.Combine(SHideTree.handler, new SHideTree.Handler(OnSHideTree));
		STreeGrow.handler = (STreeGrow.Handler)Delegate.Combine(STreeGrow.handler, new STreeGrow.Handler(OnSTreeGrow));
		STreeDown.handler = (STreeDown.Handler)Delegate.Combine(STreeDown.handler, new STreeDown.Handler(OnSTreeDown));
		SPlantInfo.handler = (SPlantInfo.Handler)Delegate.Combine(SPlantInfo.handler, new SPlantInfo.Handler(OnSPlantInfo));
		SHidePlant.handler = (SHidePlant.Handler)Delegate.Combine(SHidePlant.handler, new SHidePlant.Handler(OnSHidePlant));
		SHarvest.handler = (SHarvest.Handler)Delegate.Combine(SHarvest.handler, new SHarvest.Handler(OnSHarvest));
		SRotateBuilding.handler = (SRotateBuilding.Handler)Delegate.Combine(SRotateBuilding.handler, new SRotateBuilding.Handler(OnSRotateBuilding));
		SBuildingToolBoxIdChanged.handler = (SBuildingToolBoxIdChanged.Handler)Delegate.Combine(SBuildingToolBoxIdChanged.handler, new SBuildingToolBoxIdChanged.Handler(OnSBuildingToolBoxIdChanged));
		InitSceneSound();
		BattleEvent.OnSceneChanged = (Utils.String2Delegate)Delegate.Combine(BattleEvent.OnSceneChanged, new Utils.String2Delegate(OnSceneChanged));
	}

	private void OnSRoleLoadBullt(SRoleLoadBullet msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (!(player == null))
		{
			BaseGun currentGun = player.GetCurrentGun();
			if (currentGun != null && currentGun.GunCfg.gunType == 8 && msg.bulletNumber > 0)
			{
				LoadOneRpgBulletFor3rd(currentGun);
			}
		}
	}

	public void LoadOneRpgBulletFor3rd(BaseGun gun)
	{
		if (gun != null && gun.GunCfg.gunType == 8 && gun.CurBulletNum3Rd > 0 && !(gun.GunInfo == null))
		{
			Transform transform = gun.GunInfo.Muzzle.Find("rpgzidan");
			if (!(transform != null))
			{
				GameObject getOneRpgBullet = Ins.GetOneRpgBullet;
				getOneRpgBullet.transform.SetParent(gun.GunInfo.Muzzle, false);
				getOneRpgBullet.GetComponent<BulletControl>().enabled = false;
				getOneRpgBullet.GetComponent<AutoRecycle>().enabled = false;
				getOneRpgBullet.GetComponent<Rigidbody>().Sleep();
				getOneRpgBullet.GetComponent<Collider>().enabled = false;
			}
		}
	}

	private void OnGunModeLoadFinish(BaseGun gun)
	{
		LoadOneRpgBulletFor3rd(gun);
	}

	private void OnSMapObjectHpChange(SMapObjectHpChange msg)
	{
		MapObject value;
		if (MapObjectDic.TryGetValue(msg.insId, out value))
		{
			value.Hp = msg.hp;
		}
	}

	public RaycastHit GetAimCollider(Vector2 screenPoint, float dis = 10000f)
	{
		Ray ray = MainCamera.ScreenPointToRay(screenPoint + new Vector2(MainCamera.Camera.pixelWidth, MainCamera.Camera.pixelHeight) * 0.5f);
		Vector3 point = ray.GetPoint(dis);
		RaycastHit hitInfo;
		if (Physics.SphereCast(ray.origin, 0.1f, ray.direction, out hitInfo, dis, 1847363585) && hitInfo.collider != null)
		{
			return hitInfo;
		}
		return hitInfo;
	}

	private void OnSceneChanged(string from, string to)
	{
		if (from != null)
		{
			from = from.ToLower();
			if (SceneName2SoundIds.ContainsKey(from))
			{
				foreach (int item in SceneName2SoundIds[from])
				{
					SceneSoundCfg sceneSoundCfg = SceneSoundCfg.Get(item);
					SoundCfg soundCfg = SoundCfg.Get(sceneSoundCfg.soundId);
					DelayInvoker.CancelInvoke(soundCfg.path);
					SingletonMono<TimerManager>.Ins.Destroy(soundCfg.path);
					SingletonMono<AudioManager>.Ins.Stop(soundCfg.path);
				}
			}
		}
		if (to == null)
		{
			return;
		}
		to = to.ToLower();
		if (!SceneName2SoundIds.ContainsKey(to))
		{
			return;
		}
		foreach (int item2 in SceneName2SoundIds[to])
		{
			_003COnSceneChanged_003Ec__AnonStorey7 _003COnSceneChanged_003Ec__AnonStorey = new _003COnSceneChanged_003Ec__AnonStorey7();
			_003COnSceneChanged_003Ec__AnonStorey._0024this = this;
			_003COnSceneChanged_003Ec__AnonStorey.sceneSoundCfg = SceneSoundCfg.Get(item2);
			_003COnSceneChanged_003Ec__AnonStorey.soundCfg = SoundCfg.Get(_003COnSceneChanged_003Ec__AnonStorey.sceneSoundCfg.soundId);
			DelayInvoker.DelayInvoke(_003COnSceneChanged_003Ec__AnonStorey.soundCfg.path, _003COnSceneChanged_003Ec__AnonStorey.sceneSoundCfg.deltaTime, _003COnSceneChanged_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private void SpaceSceneSound(SceneSoundCfg sceneSoundCfg, SoundCfg soundCfg)
	{
		_003CSpaceSceneSound_003Ec__AnonStorey8 _003CSpaceSceneSound_003Ec__AnonStorey = new _003CSpaceSceneSound_003Ec__AnonStorey8();
		_003CSpaceSceneSound_003Ec__AnonStorey.sceneSoundCfg = sceneSoundCfg;
		_003CSpaceSceneSound_003Ec__AnonStorey.soundCfg = soundCfg;
		if ((SingletonMono<DayNightSystem>.Ins.IsNight && _003CSpaceSceneSound_003Ec__AnonStorey.sceneSoundCfg.dayType == 1) || (!SingletonMono<DayNightSystem>.Ins.IsNight && _003CSpaceSceneSound_003Ec__AnonStorey.sceneSoundCfg.dayType == 0))
		{
			SingletonMono<AudioManager>.Ins.Play2D(_003CSpaceSceneSound_003Ec__AnonStorey.soundCfg.id);
		}
		SingletonMono<TimerManager>.Ins.AddTimerRepeat(_003CSpaceSceneSound_003Ec__AnonStorey.soundCfg.path, _003CSpaceSceneSound_003Ec__AnonStorey.sceneSoundCfg.spaceTime, _003CSpaceSceneSound_003Ec__AnonStorey._003C_003Em__0);
	}

	private void InitSceneSound()
	{
		List<SceneSoundCfg> allList = SceneSoundCfg.GetAllList();
		foreach (SceneSoundCfg item in allList)
		{
			List<int> value;
			if (SceneName2SoundIds.TryGetValue(item.sceneName, out value))
			{
				value.Add(item.id);
				continue;
			}
			value = new List<int>();
			value.Add(item.id);
			SceneName2SoundIds.Add(item.sceneName, value);
		}
	}

	private void OnSBuildingHpChange(SBuildingHpChange msg)
	{
		PartBehaviour value;
		if (SingletonMono<BuildManager>.Ins.PartDic.TryGetValue(msg.instanceId, out value))
		{
			value.SetHp(msg.hp);
		}
	}

	private void OnIsShowEquipBool(bool arg)
	{
		Ins.SelfPlayer.PutOffAllClothes();
		if (arg)
		{
			Ins.SelfPlayer.PutOnAllClothes(Singleton<BagMgr>.Ins.GetNowEquipsData());
		}
		else
		{
			Ins.SelfPlayer.PutOnAllClothes(Singleton<BagMgr>.Ins.GetNowSkinsData());
		}
	}

	private void OnSRotateBuilding(SRotateBuilding msg)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.buildingId);
		partByInsID.ChangeAreaState(State.Free);
		partByInsID.RemoveSelfSocketBusySpace();
		partByInsID.transform.SetEulerAnglesY(msg.eulerY);
		partByInsID.ChangeAreaState(State.Busy);
	}

	private void OnSBuildingToolBoxIdChanged(SBuildingToolBoxIdChanged msg)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.buildingId);
		if (partByInsID != null)
		{
			partByInsID.ToolBoxId = msg.toolBoxId;
		}
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		PartBehaviour value;
		if (SingletonMono<BuildManager>.Ins.PartDic.TryGetValue(msg.id, out value))
		{
			SingletonMono<BuildManager>.Ins.ChangeBuildEffectByStatus(msg.status, value);
		}
	}

	private void OnSendNearBuildingFinish(SSendNearBuildingFinish msg)
	{
		Invoke("HideLoadingPanel", 1f);
		Singleton<BattleScMgr>.Ins.UnbuiltNum = 0L;
	}

	private void HideLoadingPanel()
	{
		float y = Ins.GetGroundPos(new Vector3(SelfInfo.pos.x, SelfInfo.pos.y + 1f, SelfInfo.pos.z)).y;
		Ins.SelfPlayer.transform.SetY(y);
		ViewMgr.Ins.HideView<BattleLoadingPanel>();
	}

	private void OnUpdateBuilding(SUpgradeBuilding msg)
	{
		PartBehaviour value;
		if (SingletonMono<BuildManager>.Ins.PartDic.TryGetValue(msg.instanceId, out value))
		{
			BuildPart buildPart = BuildPart.Get(value.Id);
			BuildPart buildPart2 = BuildPart.Get(buildPart.nextId);
			Ins.PlayEffectAtWorldPos(buildPart.upLvEffectId, value.transform.position, value.transform.forward);
			SingletonMono<AudioManager>.Ins.Play(buildPart.upSoundId, value.transform.position);
			value.Init(buildPart2);
			if (EventHandlers.OnBuildPartUpdate != null)
			{
				EventHandlers.OnBuildPartUpdate(msg.instanceId, buildPart2);
			}
			SingletonMono<BuildManager>.Ins.ChangeBuildEffectByStatus(value.StatesMsg, value);
		}
	}

	private void OnSExitBuildState(SExitBuildState msg)
	{
		OtherPlayerController value;
		if (OtherPlayersDic.TryGetValue(msg.roleId, out value))
		{
			value.HideTuzhi();
		}
	}

	private void OnSEnterBuildState(SEnterBuildState msg)
	{
		OtherPlayerController value;
		if (OtherPlayersDic.TryGetValue(msg.roleId, out value))
		{
			value.ShowTuzhi();
		}
	}

	private void OnSPlantInfo(SPlantInfo msg)
	{
		PlantInfo plantInfo = PlantPoolDic[msg.plantId].Get();
		if (!(plantInfo == null))
		{
			plantInfo.gameObject.transform.position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
			plantInfo.gameObject.transform.eulerAngles = new Vector3(msg.orientation.x, msg.orientation.y, msg.orientation.z);
			plantInfo.gameObject.SetActiveBetter(true);
			plantInfo.MySPlantInfo = msg;
			plantInfo.GrowFinishTime = 0;
			plantInfo.InsId = msg.instanceId;
			plantInfo.CfgId = msg.plantId;
			PlantDic.Add(msg.instanceId, plantInfo);
		}
	}

	private void OnSHarvest(SHarvest msg)
	{
		PlantInfo value;
		if (PlantDic.TryGetValue(msg.instanceId, out value))
		{
			PlantPoolDic[value.MySPlantInfo.plantId].Recycle(value);
			PlantDic.Remove(msg.instanceId);
			SingletonMono<EffectMgr>.Ins.PlayEffect("effect/caiji_02.ab", value.transform);
		}
	}

	private void OnSHidePlant(SHidePlant msg)
	{
		PlantInfo value;
		if (PlantDic.TryGetValue(msg.instanceId, out value))
		{
			PlantPoolDic[value.MySPlantInfo.plantId].Recycle(value);
			PlantDic.Remove(msg.instanceId);
		}
	}

	private void OnSHideMine(SHideMine msg)
	{
		Mine value;
		if (MineDic.TryGetValue(msg.instanceId, out value))
		{
			MinePoolDic[value.ShowMineInfo.mineTypeId].Recycle(value);
			MineDic.Remove(msg.instanceId);
			MapObjectDic.Remove(msg.instanceId);
		}
	}

	private void OnSMineBlow(SMineBlow msg)
	{
		Mine value;
		if (MineDic.TryGetValue(msg.instanceId, out value))
		{
			MinePoolDic[value.ShowMineInfo.mineTypeId].Recycle(value);
			MineDic.Remove(msg.instanceId);
			MapObjectDic.Remove(msg.instanceId);
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/wakuang_02.ab", value.transform.position);
			Ins.SyncPlaySound(529);
		}
	}

	private void OnSShowMine(SShowMine msg)
	{
		Mine mine = MinePoolDic[msg.mineTypeId].Get();
		if (!(mine == null))
		{
			mine.transform.position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
			mine.transform.eulerAngles = new Vector3(msg.orientation.x, msg.orientation.y, msg.orientation.z);
			mine.gameObject.SetActiveBetter(true);
			mine.ShowMineInfo = msg;
			mine.InsId = msg.instanceId;
			mine.Hp = msg.hp;
			mine.Init(msg.mineTypeId);
			MineDic.Add(msg.instanceId, mine);
			MapObjectDic.Add(msg.instanceId, mine);
		}
	}

	private void OnSTreeDown(STreeDown msg)
	{
		_treeIdToTreeDetail.Remove(msg.treeId);
		TreeInfo treeInfo = Ins.TreesDic[msg.treeId];
		if (treeInfo != null)
		{
			Ins.MapObjectDic.Remove(treeInfo.InsId);
			treeInfo.gameObject.SetActiveBetter(false);
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/kanshu_02.ab", treeInfo.transform.position);
			Ins.SyncPlaySound(530);
		}
		else
		{
			Debug.LogError("tree not exit" + msg.treeId);
		}
	}

	private void OnSTreeGrow(STreeGrow msg)
	{
		TreeDetail value = default(TreeDetail);
		value.InsId = msg.treeInstanceId;
		value.TreeId = msg.treeId;
		TreeCfg treeCfg = TreeCfg.Get(msg.cfgId);
		value.Hp = treeCfg.hp;
		value.MyCfg = treeCfg;
		_treeIdToTreeDetail[msg.treeId] = value;
		TreeInfo treeInfo = Ins.TreesDic[msg.treeId];
		if (treeInfo != null)
		{
			treeInfo.InsId = msg.treeInstanceId;
			treeInfo.gameObject.SetActiveBetter(true);
			treeInfo.Init();
			treeInfo.Hp = treeInfo.MaxHp;
			Ins.MapObjectDic[msg.treeInstanceId] = treeInfo;
		}
		else
		{
			Debug.LogError("OnSTreeGrow tree not exit" + msg.treeId);
		}
	}

	private void OnSHideTree(SHideTree msg)
	{
		_treeIdToTreeDetail.Remove(msg.treeId);
		TreeInfo value;
		if (Ins.TreesDic.TryGetValue(msg.treeId, out value))
		{
			value.gameObject.SetActiveBetter(false);
			Ins.MapObjectDic.Remove(value.InsId);
		}
	}

	private void OnSShowTree(SShowTree msg)
	{
		TreeDetail value = default(TreeDetail);
		value.InsId = msg.treeInstanceId;
		value.TreeId = msg.treeId;
		TreeCfg myCfg = TreeCfg.Get(msg.cfgId);
		value.Hp = msg.hp;
		value.MyCfg = myCfg;
		_treeIdToTreeDetail[msg.treeId] = value;
		TreeInfo value2;
		if (Ins.TreesDic.TryGetValue(msg.treeId, out value2))
		{
			value2 = Ins.TreesDic[msg.treeId];
			value2.InsId = msg.treeInstanceId;
			value2.gameObject.SetActiveBetter(true);
			value2.Hp = msg.hp;
			value2.Init();
			Ins.MapObjectDic[msg.treeInstanceId] = value2;
		}
	}

	private void OnSRebitrh(SRebirth msg)
	{
		if (msg.playerInfo.roleId != Ins.SelfPlayer.RoleId)
		{
			OtherPlayerController playerById = GetPlayerById(msg.playerInfo.roleId);
			if (playerById != null)
			{
				playerById.Reset();
				UnityEngine.Object.Destroy(playerById.gameObject);
			}
			EnterPlayerInfo(msg.playerInfo);
		}
		BasePlayerController player = GetPlayer(msg.playerInfo.roleId);
		if (player != null)
		{
			player.SRebirth(msg.playerInfo);
		}
	}

	private void OnSRequestRobot(SRequestRobot msg)
	{
		ControlAiID = msg.roleId;
	}

	private IEnumerator ChangeCameraForWatch(OtherPlayerController p)
	{
		yield return 2f;
		LargeSceneManager.Ins.ToSceneCell(p.Pos.x, p.Pos.y, p.Pos.z);
		Ins.MainCamera.SetMainTarget(p.transform, "WatchModel");
		p.IsWatched = true;
	}

	private void OnSyncCameraLookPoint(SSyncCameraLookPoint msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetCamearforwardPoint(msg.point);
		}
	}

	private void OnSyncYaw(SSyncYaw msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetAnimatorYawValue(msg.yaw);
		}
	}

	private void OnSyncFootTexture(SSyncFootTexture msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.FootTexture = (BasePlayerController.FootTextureType)msg.textureType;
		}
	}

	private void OnDestroy()
	{
	}

	public static Coroutine StartConroutine(IEnumerator routine)
	{
		return Ins.StartCoroutine(routine);
	}

	public static void StopConroutine(Coroutine routine)
	{
		if (routine != null && Ins != null)
		{
			Ins.StopCoroutine(routine);
		}
	}

	private void OnSensitivityX(float x)
	{
		if ((bool)MainCamera)
		{
			MainCamera.SetMouseSensitivitX(x * 8f);
		}
	}

	private void OnSensitivityY(float y)
	{
		if ((bool)MainCamera)
		{
			MainCamera.SetMouseSensitivitY(y * 8f);
		}
	}

	private void OnSyncFallGround(SSyncFallGround msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
	}

	private void OnLeavelFlyBattlePlane(SLevelFlyBattlePlane msg)
	{
		SelfPlayer.SetPosition(new Vector3(msg.pos.x + (float)Utils.Random(-6, 6), msg.pos.y - 20f, msg.pos.z + (float)Utils.Random(-6, 6)));
		SelfPlayer.FSM.SwitchState(StateID.SkyDiving);
	}

	private void OnSyncHandWeapon(SSyncHandWeapon msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.ChangeHandWeapon(msg.handWeapon);
		}
	}

	private void OnSSyncGunRemovePart(SSyncGunRemovePart msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.RemoveGunPart(msg.index, msg.partId);
		}
	}

	private void OnSSyncGunPutPart(SSyncGunPutPart msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.AddGunPart(msg.index, msg.partId);
		}
	}

	private void OnSelfPickWear(int wearId)
	{
		SelfPlayer.PutOnCloth(wearId);
	}

	private void OnSelfEquipHpDelegate(int wearId, int hp)
	{
		SelfPlayer.OnClothHpChanged(wearId, hp);
	}

	private void OnSelfDropWear(int wearId)
	{
		SelfPlayer.PutOffCloth(wearId);
	}

	private void OnSyncOtherWear(SSyncOtherWear msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.otherId);
		if (playerById != null)
		{
			playerById.PutOffAllClothes();
			playerById.PutOnAllClothes(msg.wears);
		}
	}

	private void OnPickWear(SOtherPickWear msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.otherId);
		if (playerById != null)
		{
			playerById.PutOnCloth(msg.wId);
		}
	}

	private void OnSDropWear(SOtherDropWear msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.otherId);
		if (playerById != null)
		{
			playerById.PutOffCloth(msg.wId);
		}
	}

	private void OnSGrenadeExplode(SGrenadeExplode msg)
	{
		if (MapObjectDic.ContainsKey(msg.insId))
		{
			GameObject obj = MapObjectDic[msg.insId].gameObject;
			UnityEngine.Object.Destroy(obj);
			MapObjectDic.Remove(msg.insId);
		}
	}

	private void OnSThrowGrenade(SThrowGrenade msg)
	{
		_003COnSThrowGrenade_003Ec__AnonStorey9 _003COnSThrowGrenade_003Ec__AnonStorey = new _003COnSThrowGrenade_003Ec__AnonStorey9();
		_003COnSThrowGrenade_003Ec__AnonStorey.msg = msg;
		_003COnSThrowGrenade_003Ec__AnonStorey._0024this = this;
		if (_003COnSThrowGrenade_003Ec__AnonStorey.msg.playerInsId != Ins.SelfPlayer.InsId)
		{
			ThrowCfg throwCfg = ThrowCfg.Get(_003COnSThrowGrenade_003Ec__AnonStorey.msg.grenadeTypeId);
			ResMgr.Ins.CreateFromAB(throwCfg.path, null, _003COnSThrowGrenade_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	private void OnSSyncGrenade(SSyncMapObjectPos msg)
	{
		if (MapObjectDic.ContainsKey(msg.insId))
		{
			GameObject gameObject = MapObjectDic[msg.insId].gameObject;
			gameObject.transform.position = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
			gameObject.transform.eulerAngles = new Vector3(msg.rotation.x, msg.rotation.y, msg.rotation.z);
		}
	}

	private void OnSShoot(SShoot msg)
	{
		OtherPlayerController otherPlayerController = GetMapObjectByInsId(msg.insId) as OtherPlayerController;
		if (!(otherPlayerController != null))
		{
			return;
		}
		BaseGun currentGun = otherPlayerController.GetCurrentGun();
		if (currentGun != null)
		{
			currentGun.PlayShootEffectAndSound((msg.state & 1) != 0, (msg.state & 2) != 0);
			Utils.TriggerEvent(BattleEvent.OnShoot, msg.insId);
			PlayBulletEffect(currentGun.GunInfo.Muzzle, currentGun.GunCfg.factors[37], new Vector3(msg.pos.x, msg.pos.y, msg.pos.z));
			if (currentGun.GunCfg.gunType == 8)
			{
				Transform transform = currentGun.GunInfo.Muzzle.Find("rpgzidan");
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
		if (BattleEvent.roleGunSoundEvent != null)
		{
			BattleEvent.roleGunSoundEvent(otherPlayerController.gameObject, msg.insId, 1, (msg.state & 2) == 0);
		}
	}

	private void PlayBulletEffect(Transform t, float speed, Vector3 pos)
	{
		Bullet3rdControl bullet3rdControl = Bullet3rdPool.Get();
		bullet3rdControl.gameObject.SetActiveBetter(true);
		bullet3rdControl.SetPos(t.position);
		float num = Vector3.Distance(pos, t.position) / speed * 9.8f * 0.5f * 0.3f;
		pos.y -= num;
		Vector3 normalized = (pos - t.position).normalized;
		bullet3rdControl.transform.rotation = Quaternion.LookRotation(normalized);
		bullet3rdControl.dir = normalized;
		bullet3rdControl.Speed = speed;
	}

	private void OnSHitPlayer(SHitPlayer msg)
	{
		if (msg.roleId != ControlAiID)
		{
			BasePlayerController player = GetPlayer(msg.roleId);
			if (player != null && !player.IsDie)
			{
				player.PlayHitEffect(msg);
			}
		}
	}

	public void OnMakeMark(Vector3 pos)
	{
		cMakeMark.pos.x = pos.x;
		cMakeMark.pos.y = pos.y;
		cMakeMark.pos.z = pos.z;
		Client2Gs.Ins.Send(cMakeMark);
		mark = new Vec3();
		mark.x = pos.x;
		mark.y = pos.y;
		mark.z = pos.z;
	}

	public void OnRemoveMark()
	{
		mark = null;
		Client2Gs.Ins.Send(cRemoveMark);
	}

	public Vec3 GetMark()
	{
		return mark;
	}

	private void OnSAttachEffect(SAttachEffect msg)
	{
		if (msg.attcherRoleId > 0)
		{
			OtherPlayerController value = null;
			if (OtherPlayersDic.TryGetValue(msg.attcherRoleId, out value))
			{
				EffectCfg effectCfg = EffectCfg.Get(msg.effectId);
				Vector3 pos = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
				Vector3 forward = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
				SingletonMono<EffectMgr>.Ins.PlayEffectAtPos(effectCfg.name, value.transform, pos, forward);
			}
		}
	}

	private void OnSPlayEffectAtPos(SPlayEffectAtPos msg)
	{
		EffectCfg effectCfg = EffectCfg.Get(msg.effectId);
		Vector3 pos = new Vector3(msg.pos.x, msg.pos.y, msg.pos.z);
		Vector3 forward = new Vector3(msg.forward.x, msg.forward.y, msg.forward.z);
		SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(effectCfg.path, pos, forward);
	}

	public VehicleMonitor GetVehicle(int vehicleId)
	{
		VehicleMonitor value = null;
		battleObjectId2VehicleMonitor.TryGetValue(vehicleId, out value);
		return value;
	}

	private void OnSVehicleInfo(SVehicleInfo sVehicleInfo)
	{
		if (!battleObjectId2VehicleInfo.ContainsKey(sVehicleInfo.vehicleInfo.id))
		{
			battleObjectId2VehicleInfo[sVehicleInfo.vehicleInfo.id] = sVehicleInfo;
			VehicleLoader vehicleLoader = new VehicleLoader(sVehicleInfo.vehicleInfo.type, sVehicleInfo.vehicleInfo.id);
			vehicleLoader.Load();
		}
	}

	private static void CleanVehicleMonitor(VehicleMonitor monitor)
	{
		Transform[] seats = monitor.Seats;
		foreach (Transform transform in seats)
		{
			if (transform != null)
			{
				transform.DetachChildren();
			}
		}
		if (monitor.TanshenPosints == null)
		{
			return;
		}
		Transform[] tanshenPosints = monitor.TanshenPosints;
		foreach (Transform transform2 in tanshenPosints)
		{
			if (transform2 != null)
			{
				transform2.DetachChildren();
			}
		}
	}

	private void OnSVehicleDisappear(SVehicleDisappear sVehicleDisappear)
	{
		battleObjectId2VehicleInfo.Remove(sVehicleDisappear.id);
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sVehicleDisappear.id, out value))
		{
			CleanVehicleMonitor(value);
			UnityEngine.Object.Destroy(value.gameObject);
			battleObjectId2VehicleMonitor.Remove(sVehicleDisappear.id);
		}
		if (BattleEvent.OnVehicleDisappear != null)
		{
			BattleEvent.OnVehicleDisappear(sVehicleDisappear.id);
		}
	}

	private void OnSGetInVehicle(SGetInVehicle sGetInVehicle)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sGetInVehicle.id, out value))
		{
			OtherPlayerController value2;
			if (sGetInVehicle.roleId == Singleton<RoleMgr>.Ins.info.roleId && SelfPlayer != null)
			{
				value.GetInVehicle(SelfPlayer, sGetInVehicle.seat);
			}
			else if (OtherPlayersDic.TryGetValue(sGetInVehicle.roleId, out value2))
			{
				value.GetInVehicle(value2, sGetInVehicle.seat);
			}
		}
	}

	private void OnSGetOutVehicle(SGetOutVehicle sGetOutVehicle)
	{
		VehicleMonitor value;
		OtherPlayerController value3;
		if (battleObjectId2VehicleMonitor.TryGetValue(sGetOutVehicle.id, out value))
		{
			OtherPlayerController value2;
			if (OtherPlayersDic.TryGetValue(sGetOutVehicle.roleId, out value2))
			{
				value2.SetPosition(new Vector3(sGetOutVehicle.rolePos.x, sGetOutVehicle.rolePos.y, sGetOutVehicle.rolePos.z));
				value.GetOutVehicle(value2, false);
			}
		}
		else if (OtherPlayersDic.TryGetValue(sGetOutVehicle.roleId, out value3))
		{
			value3.SetPosition(new Vector3(sGetOutVehicle.rolePos.x, sGetOutVehicle.rolePos.y, sGetOutVehicle.rolePos.z));
			value3.OnGetOutCar();
		}
	}

	private void OnSSyncVehiclePos(SSyncVehiclePos sSyncVehiclePos)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sSyncVehiclePos.id, out value))
		{
			value.SetPos(sSyncVehiclePos.pos.x, sSyncVehiclePos.pos.y, sSyncVehiclePos.pos.z);
		}
	}

	private void OnSSyncVehicleOrientation(SSyncVehicleOrientation sSyncVehicleOrientation)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sSyncVehicleOrientation.id, out value))
		{
			value.SetOrientation(MathUtils.Short2Float(sSyncVehicleOrientation.orientation.x), MathUtils.Short2Float(sSyncVehicleOrientation.orientation.y), MathUtils.Short2Float(sSyncVehicleOrientation.orientation.z));
		}
	}

	private void OnSSyncVehicleVelocity(SSyncVehicleVelocity sSyncVehicleVelocity)
	{
	}

	private void OnSSwitchSeat(SSwitchSeat sSwitchSeat)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sSwitchSeat.id, out value))
		{
			GameObject playerByRoleId = GetPlayerByRoleId(sSwitchSeat.roleId);
			if (playerByRoleId != null)
			{
				value.SwitchTo(playerByRoleId, sSwitchSeat.seat, sSwitchSeat.roleId == Singleton<RoleMgr>.Ins.info.roleId);
			}
		}
	}

	private void OnSyncTanshen(SSyncTanshen msg)
	{
		VehicleMonitor value;
		if (!battleObjectId2VehicleMonitor.TryGetValue(msg.id, out value))
		{
			return;
		}
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null && playerById.InCar)
		{
			if (msg.isTanshen)
			{
				value.MoveToTanshenSeat(playerById.gameObject);
			}
			else
			{
				value.BackToSeat(playerById.gameObject);
			}
		}
	}

	public void SendGetInVehicle(int vehicleId, int seat)
	{
		cGetInVehicle.id = vehicleId;
		cGetInVehicle.roleId = Singleton<RoleMgr>.Ins.info.roleId;
		cGetInVehicle.seat = seat;
		Client2Gs.Ins.Send(cGetInVehicle);
	}

	public void SendGetOutVehicle(int vehicleId, Vector3 getOutPos)
	{
		cGetOutVehicle.id = vehicleId;
		cGetOutVehicle.roleId = Singleton<RoleMgr>.Ins.info.roleId;
		cGetOutVehicle.rolePos.x = getOutPos.x;
		cGetOutVehicle.rolePos.y = getOutPos.y;
		cGetOutVehicle.rolePos.z = getOutPos.z;
		Client2Gs.Ins.Send(cGetOutVehicle);
	}

	public void OnSwitchSeat(int vehicleId, int seat)
	{
		cSwitchSeat.id = vehicleId;
		cSwitchSeat.seat = seat;
		Client2Gs.Ins.Send(cSwitchSeat);
	}

	private void OnSSyncVehicleFuel(SSyncVehicleFuel syncVehicleFuel)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(syncVehicleFuel.id, out value))
		{
			value.SetFuel(syncVehicleFuel.fuel);
		}
	}

	private void OnSSyncSound(SSyncSound sSyncSound)
	{
		VehicleMonitor value;
		if (sSyncSound.roleId == 0 && battleObjectId2VehicleMonitor.TryGetValue(sSyncSound.id, out value))
		{
			SoundCfg soundCfg = SoundCfg.Get(sSyncSound.soundId);
			SingletonMono<AudioManager>.Ins.PlayOnTarget(soundCfg.path, value.gameObject, new Vector3(sSyncSound.pos.x, sSyncSound.pos.y, sSyncSound.pos.z), sSyncSound.playMoreThanOne, sSyncSound.volume, sSyncSound.pitch, sSyncSound.minDist, sSyncSound.maxDist, sSyncSound.loop);
		}
	}

	private void OnSSyncEngineSound(SSyncEngineSound sSyncEngineSound)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sSyncEngineSound.id, out value))
		{
			value.SyncEngineSound(sSyncEngineSound.volume);
			if (BattleEvent.vehicleSoundEvent != null)
			{
				BattleEvent.vehicleSoundEvent(value.gameObject, value.GetId(), value.GetEngineSoundId());
			}
		}
	}

	private void OnSSyncCarWheelRotation(SSyncCarWheelRotation sSyncCarWheelRotation)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sSyncCarWheelRotation.vehicleId, out value))
		{
			value.SyncWheelRotation(sSyncCarWheelRotation);
		}
	}

	private void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sStopVehicleSound.vehicleId, out value))
		{
			value.OnSStopVehicleSound(sStopVehicleSound);
		}
	}

	private void OnSVehicleHpChange(SVehicleHpChange sVehicleHpChange)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sVehicleHpChange.vehicleId, out value))
		{
			value.OnSVehicleHpChange(sVehicleHpChange);
		}
	}

	private void OnSVehicleSkinChange(SVehicleSkinChange sVehicleSkinChange)
	{
		VehicleMonitor value;
		if (battleObjectId2VehicleMonitor.TryGetValue(sVehicleSkinChange.vehicleInfo.id, out value))
		{
			VehicleLoader vehicleLoader = new VehicleLoader(sVehicleSkinChange.newVehicleTypeId, sVehicleSkinChange.vehicleInfo.id, true);
			vehicleLoader.Load();
		}
	}

	private void OnSGetInVehicleError(SGetInVehicleError sGetInVehicleError)
	{
		AlertBox.Show(221);
	}

	private void OnSServerPullBack(SServerPullback msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetPosition(new Vector3(msg.pos.x, msg.pos.y, msg.pos.z));
		}
	}

	private void OnSSyncLayerWeight(SSyncLayerWeight msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetLayerWeight(msg.layer, MathUtils.Short2Float(msg.value));
		}
	}

	private void OnSyncOrientation(SSyncOrientation msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetTargetRotation(msg.orientation);
		}
	}

	private void OnSyncAnimator(SSyncAnimator msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			CheckIsPa(playerById, msg.animationHash);
			playerById.PlayerAnimation(msg.layer, msg.animationHash);
		}
	}

	private void CheckIsPa(OtherPlayerController p, int aimHash)
	{
		if (aimHash == 1245347906)
		{
			p.IsPa = true;
		}
		else
		{
			p.IsPa = false;
		}
	}

	private void OnSyncVelocity(SSyncVelocity msg)
	{
	}

	private void OnSSyncPlayerPos(SSyncPlayerPos msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SyncCurrentPos(msg.pos);
		}
	}

	public void OnSHpChange(SHpChange msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (player != null)
		{
			player.SetHP(msg.hp);
			player.MaxHp = msg.hpMax;
			if ((bool)SelfPlayer && msg.roleId == SelfPlayer.KillMeRoleId)
			{
				MyBattlePanel.UpdateKillMeHp(GetPlayerById(msg.roleId));
			}
		}
	}

	private void OnSNewBeeStatusChange(SNewBeeStatusChange msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (player != null)
		{
			player.NewPlayer = msg.flag;
		}
	}

	private void OnHpStatusChange(SHpStatusChange msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (player != null)
		{
			player.SetHPStatus(msg.isSecond);
		}
	}

	public void OnSEpChange(SEpChange msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (player != null)
		{
			player.SetEP(msg.ep);
		}
	}

	private void OnSyncAnimatorSpeed(SSyncAnimatorSpeed msg)
	{
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			playerById.SetAnimatorSpeed(msg.speed);
		}
	}

	public void OnSPlayerDie(SPlayerDie msg)
	{
		BasePlayerController player = GetPlayer(msg.roleId);
		if (player != null)
		{
			if (player.VehicleId > 0)
			{
				VehicleMonitor value = null;
				if (battleObjectId2VehicleMonitor.TryGetValue(player.VehicleId, out value))
				{
					value.OnPlayerDie(player.gameObject, player.RoleId == Singleton<RoleMgr>.Ins.info.roleId);
				}
			}
			player.PlayerDie(msg);
			if (msg.roleId != Singleton<RoleMgr>.Ins.info.roleId)
			{
			}
		}
		if (TeamPlayerDic.ContainsKey(msg.roleId) && msg.roleId != SelfInfo.roleId)
		{
			SingletonMono<AudioManager>.Ins.Play2D(347);
		}
	}

	private void OnSPlayerDisappear(SPlayerDisappear msg)
	{
		if (!OtherPlayersDic.ContainsKey(msg.roleId))
		{
			return;
		}
		OtherPlayerController playerById = GetPlayerById(msg.roleId);
		if (playerById != null)
		{
			if (playerById.IsDie)
			{
				playerById.Reset();
				UnityEngine.Object.Destroy(playerById.gameObject);
			}
			else if (!TeamPlayerDic.ContainsKey(msg.roleId))
			{
				OtherRolePlayerPool.Recycle(playerById);
				playerById.RemoveRoleIdinDic();
			}
			else
			{
				playerById.Reset();
			}
		}
	}

	public OtherPlayerController GetPlayerById(long roleId)
	{
		OtherPlayerController value;
		OtherPlayersDic.TryGetValue(roleId, out value);
		return value;
	}

	public BasePlayerController GetPlayer(long roleId)
	{
		if (SelfPlayer != null && roleId == SelfPlayer.RoleId)
		{
			return SelfPlayer;
		}
		OtherPlayerController value;
		OtherPlayersDic.TryGetValue(roleId, out value);
		return value;
	}

	public void OnSplayerInfo(SPlayerInfo msg)
	{
		EnterPlayerInfo(msg.playerInfo);
	}

	public void EnterPlayerInfo(PlayerInfo playerInfo)
	{
		if (playerInfo.groupId > 0 && playerInfo.groupId == Singleton<TeamScMgr>.Ins.TeamId && BattleEvent.OnReciveTeamPlayerInfo != null)
		{
			BattleEvent.OnReciveTeamPlayerInfo(playerInfo);
		}
		if (playerInfo.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			return;
		}
		OtherPlayerController value;
		if (OtherPlayersDic.TryGetValue(playerInfo.roleId, out value))
		{
			if (playerInfo.groupId == Singleton<TeamScMgr>.Ins.TeamId)
			{
				if (playerInfo.groupId > 0 && playerInfo.groupId == Singleton<TeamScMgr>.Ins.TeamId)
				{
					if (!TeamPlayerDic.ContainsKey(playerInfo.roleId))
					{
						TeamPlayerDic.Add(playerInfo.roleId, value);
					}
					UnityEngine.Object.Destroy(Utils.GetChildObjByName("BigAimHelper", value.gameObject));
				}
				return;
			}
			value.Reset();
			UnityEngine.Object.Destroy(value.gameObject);
			Debug.LogError("bu zheng chang !!!!!!!!!!");
		}
		value = OtherRolePlayerPool.Get();
		value.gameObject.SetActive(true);
		OtherPlayersDic.Add(playerInfo.roleId, value);
		MapObjectDic.Add(playerInfo.insId, value);
		if (playerInfo.groupId > 0 && playerInfo.groupId == Singleton<TeamScMgr>.Ins.TeamId)
		{
			if (!TeamPlayerDic.ContainsKey(playerInfo.roleId))
			{
				TeamPlayerDic.Add(playerInfo.roleId, value);
			}
			UnityEngine.Object.Destroy(Utils.GetChildObjByName("BigAimHelper", value.gameObject));
		}
		value.Init(playerInfo);
	}

	public GameObject GetPlayerByRoleId(long roleId)
	{
		if (OtherPlayersDic.ContainsKey(roleId))
		{
			return OtherPlayersDic[roleId].gameObject;
		}
		if (roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			return SelfPlayer.gameObject;
		}
		return null;
	}

	public MapObject GetMapObjectByInsId(long insId)
	{
		if (MapObjectDic.ContainsKey(insId))
		{
			return MapObjectDic[insId];
		}
		return null;
	}

	private void InitSelfPlayer(GameObject go)
	{
		SelfPlayer = go.AddComponent<PlayerController>();
		go.AddComponent<CommonControllerFunc>();
		SelfPlayer.InitPrototyle();
		GameObject gameObject = new GameObject("light");
		gameObject.transform.parent = go.transform;
		gameObject.transform.SetLocalPositionY(1.4f);
		SelfPlayer.DayNightLight = gameObject.AddComponent<Light>();
		SelfPlayer.DayNightLight.type = LightType.Spot;
		SelfPlayer.DayNightLight.spotAngle = 57.8f;
		SelfPlayer.DayNightLight.intensity = 2f;
		SelfPlayer.DayNightLight.range = 28.3f;
		SelfPlayer.DayNightLight.color = new Color32(byte.MaxValue, 221, 169, byte.MaxValue);
		SelfPlayer.DayNightLight.renderMode = LightRenderMode.ForceVertex;
		SelfPlayer.DayNightLight.shadows = LightShadows.None;
		SelfPlayer.DayNightLight.enabled = false;
		SelfPlayer.DayNightLight.cullingMask = -262657;
		SelfPlayer.Init(SelfInfo);
		MapObjectDic.Add(SelfInfo.insId, SelfPlayer);
	}

	private void InitOtherPlayer()
	{
		m_RolePlayerModel.AddComponent<OtherPlayerController>().InitPrototyle();
		m_RolePlayerModel.AddComponent<CommonControllerFunc>();
		AimMagnet aimMagnet = Utils.GetChildObjByName("BigAimHelper", m_RolePlayerModel).AddComponent<AimMagnet>();
		aimMagnet.AimEffect = Utils.GetChildObjByName("xifu_effect_01", aimMagnet.gameObject);
		aimMagnet.AimEffect.SetActive(false);
		m_RolePlayerModel.transform.SetParent(m_OtherPlayerPoolParent, false);
		m_RolePlayerModel.transform.position = Vector3.zero;
		m_RolePlayerModel.SetActive(false);
		ObjectPool<OtherPlayerController>.CreateObject<OtherPlayerController> createFun = _003CInitOtherPlayer_003Em__1;
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CInitOtherPlayer_003Em__2;
		}
		OtherRolePlayerPool = new ObjectPool<OtherPlayerController>(20, createFun, _003C_003Ef__am_0024cache0, _003CInitOtherPlayer_003Em__3);
		SingletonMono<AudioManager>.Ins.Stop("Bgm_loading");
		LargeSceneManager.Ins.bInitFinish = true;
		StartConroutine(ZeroOneTick());
		StartCoroutine(Tick());
		StartCoroutine(FiveTick());
	}

	private void OnLoadSceneFinish()
	{
		StartCoroutine(LoadSceneFinish());
	}

	private IEnumerator LoadSceneFinish()
	{
		yield return StartCoroutine(SingletonMono<BuildManager>.Ins.PreparePartsCollections());
		if (!PlayTest)
		{
			MainCamera = Camera.main.GetComponent<vThirdPersonCamera>();
			InitObjectCulling(MainCamera.Camera);
			yield return DoSomeInit();
			yield return ResMgr.Ins.CreateFromAB("role/role.ab", "role", ((_003CLoadSceneFinish_003Ec__Iterator1)(object)this)._003C_003Em__0);
			GameObject selfGameObject2 = UnityEngine.Object.Instantiate(m_RolePlayerModel);
			selfGameObject2.transform.position = Vector3.zero;
			selfGameObject2.transform.rotation = Quaternion.identity;
			MainCamera.Init(selfGameObject2.transform, "Stand");
			SetCameraLayerCull();
			OnSensitivityX(SettingMgr.SensitivityHorizontal);
			OnSensitivityY(SettingMgr.SensitivityVertical);
			InitSelfPlayer(selfGameObject2);
			InitOtherPlayer();
			AimHelp = Camera.main.gameObject.AddComponent<AimHelper>();
			ViewMgr.Ins.ShowView<BattlePanel>();
		}
		else
		{
			MainCamera = Camera.main.GetComponent<vThirdPersonCamera>();
			InitObjectCulling(MainCamera.Camera);
			yield return DoSomeInit();
			m_RolePlayerModel = UnityEngine.Object.Instantiate(Resources.Load("role")) as GameObject;
			GameObject selfGameObject = UnityEngine.Object.Instantiate(m_RolePlayerModel);
			MainCamera.Init(selfGameObject.transform, "Stand");
			SetCameraLayerCull();
			OnSensitivityX(SettingMgr.SensitivityHorizontal);
			OnSensitivityY(SettingMgr.SensitivityVertical);
			InitSelfPlayer(selfGameObject);
			InitOtherPlayer();
			AimHelp = Camera.main.gameObject.AddComponent<AimHelper>();
			ViewMgr.Ins.ShowView<BattlePanel>();
		}
	}

	public void TestAi()
	{
		for (int i = 0; i < 10; i++)
		{
			CRequestRobot msg = new CRequestRobot();
			Client2Gs.Ins.Send(msg);
		}
	}

	private void SetCameraLayerCull()
	{
	}

	private void CopyZone(Zone from, Zone to)
	{
		to.centerX = from.centerX;
		to.centerY = from.centerY;
		to.radius = from.radius;
	}

	public SSafeZone GetSSafeZone()
	{
		return sSafeZone;
	}

	public SToxicGasZone GetToxicGasZone()
	{
		return sToxicGasZone;
	}

	private void OnSSafeZone(SSafeZone safeZone)
	{
		if (sSafeZone == null)
		{
			sSafeZone = new SSafeZone();
		}
		CopyZone(safeZone.zone, sSafeZone.zone);
		safeZoneCenter = new Vector3(safeZone.zone.centerX, 0f, safeZone.zone.centerY);
	}

	private void OnSToxicGasZone(SToxicGasZone toxicGasZone)
	{
		if (sToxicGasZone == null)
		{
			sToxicGasZone = new SToxicGasZone();
		}
		CopyZone(toxicGasZone.zone, sToxicGasZone.zone);
		sToxicGasZone.speed = toxicGasZone.speed;
		m_GasStartTime = Time.time;
		gasZoneCenter = new Vector3(sToxicGasZone.zone.centerX, 0f, sToxicGasZone.zone.centerY);
		m_GasCenterMoveSpeed = (safeZoneCenter - gasZoneCenter).normalized * Vector3.Distance(safeZoneCenter, gasZoneCenter) / (toxicGasZone.zone.radius - GetSSafeZone().zone.radius) * toxicGasZone.speed;
	}

	private void OnSGenSafeZone(SGenSafeZone sGenSafeZone)
	{
		int id = ((sGenSafeZone.round > 3) ? 356 : 355);
		SingletonMono<AudioManager>.Ins.Play2D(id);
	}

	private void OnSStartShrinkToxicGas(SStartShrinkToxicGas sStartShrinkToxicGas)
	{
		isToxicGasShrink = true;
		m_GasStartTime = Time.time;
		gasShrinkEndTime = Time.time + (float)sStartShrinkToxicGas.timeLeft;
		shrinkTotalTime = sStartShrinkToxicGas.totalTime;
		SingletonMono<AudioManager>.Ins.Play2D(440);
	}

	private void OnSEndShrinkToxicGas(SEndShrinkToxicGas sEndShrinkToxicGas)
	{
		isToxicGasShrink = false;
		gasShrinkStartTime = Time.time + (float)sEndShrinkToxicGas.nextTime;
		waitForShrinkTotalTime = sEndShrinkToxicGas.totalTime;
		shrinkBeginRadius = sEndShrinkToxicGas.shrinkBeginRadius;
		shrinkEndRadius = sEndShrinkToxicGas.shrinkEndRadius;
		shrinkBeginX = sEndShrinkToxicGas.shrinkBeginX;
		shrinkBeginY = sEndShrinkToxicGas.shrinkBeginY;
	}

	public bool IsToxicGasShrink()
	{
		return isToxicGasShrink;
	}

	public bool HasToxicGas()
	{
		return sToxicGasZone != null;
	}

	public float GetShrinkStartTime()
	{
		if (isToxicGasShrink)
		{
			return 0f;
		}
		return gasShrinkStartTime - Time.time;
	}

	public float GetShrinkEndTime()
	{
		if (isToxicGasShrink)
		{
			return gasShrinkEndTime - Time.time;
		}
		return 0f;
	}

	public float SaveTime()
	{
		if (isToxicGasShrink)
		{
			return 0f;
		}
		return gasShrinkStartTime - Time.time;
	}

	public float GetShrinkBeginRadius()
	{
		return shrinkBeginRadius;
	}

	public float GetShrinkEndRadius()
	{
		return shrinkEndRadius;
	}

	public float GetShrinkBeginX()
	{
		return shrinkBeginX;
	}

	public float GetShrinkBeginY()
	{
		return shrinkBeginY;
	}

	public float GetShrinkProgress()
	{
		if (shrinkTotalTime > 0f)
		{
			float num = gasShrinkEndTime - Time.time;
			if (num < 0f)
			{
				num = 0f;
			}
			return 1f - num / shrinkTotalTime;
		}
		return 0f;
	}

	private void InitColliderPhysic()
	{
		MinPhysicMaterial = new PhysicMaterial("min");
		MinPhysicMaterial.dynamicFriction = 0f;
		MinPhysicMaterial.staticFriction = 0f;
		MinPhysicMaterial.bounciness = 0f;
		MinPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
		MinPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
		MidPhysicMaterial = new PhysicMaterial("mid");
		MidPhysicMaterial.dynamicFriction = 0.1f;
		MidPhysicMaterial.staticFriction = 0.1f;
		MidPhysicMaterial.bounciness = 0f;
		MidPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
		MidPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
		MaxPhysicMaterial = new PhysicMaterial("max");
		MaxPhysicMaterial.dynamicFriction = 1f;
		MaxPhysicMaterial.staticFriction = 1f;
		MaxPhysicMaterial.bounciness = 0f;
		MaxPhysicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
		MaxPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
	}

	private void InitPoolParent()
	{
		GameObject gameObject = new GameObject("_BulletPool_");
		m_BulletParent = gameObject.transform;
		m_BulletParent.position = Vector3.zero;
		m_BulletParent.localScale = Vector3.one;
		gameObject = new GameObject("_RpgBulletPool_");
		m_RpgBulletParent = gameObject.transform;
		m_RpgBulletParent.position = Vector3.zero;
		m_RpgBulletParent.localScale = Vector3.one;
		gameObject = new GameObject("_DecalPool_");
		m_DecalParent = gameObject.transform;
		m_DecalParent.position = Vector3.zero;
		m_DecalParent.localScale = Vector3.one;
		gameObject = new GameObject("_OtherPlayerPool");
		m_OtherPlayerPoolParent = gameObject.transform;
		m_OtherPlayerPoolParent.position = Vector3.zero;
		m_OtherPlayerPoolParent.localScale = Vector3.one;
		gameObject = new GameObject("_VehiclePool_");
		m_VehiclesParent = gameObject.transform;
		m_VehiclesParent.position = Vector3.zero;
		m_VehiclesParent.localScale = Vector3.one;
		gameObject = new GameObject("_MinePool_");
		m_MineParent = gameObject.transform;
		m_MineParent.position = Vector3.zero;
		m_MineParent.localScale = Vector3.one;
		gameObject = new GameObject("_PlantPool_");
		m_plantParent = gameObject.transform;
		m_plantParent.position = Vector3.zero;
		m_plantParent.localScale = Vector3.one;
	}

	private IEnumerator DoSomeInit()
	{
		InitPoolParent();
		yield return ResMgr.Ins.CreateFromAB("model/zidan.ab", null, OnLoadZidanFinish);
		yield return ResMgr.Ins.CreateFromAB("model/zidan3rd.ab", null, OnLoadZidan3rdFinish);
		yield return ResMgr.Ins.CreateFromAB("model/rpgzidan.ab", null, OnLoadRpgZidanFinish);
		yield return ResMgr.Ins.CreateFromAB("model/decal.ab", null, OnLoadDecalFinish);
		yield return PrePareObjectPool();
		yield return ResMgr.Ins.CreateFromAB("model/wupin_tuzhi.ab", null, ((_003CDoSomeInit_003Ec__Iterator2)(object)this)._003C_003Em__0);
	}

	private IEnumerator PrePareObjectPool()
	{
		List<CutPlantCfg> PlantCfgs = CutPlantCfg.GetAllList();
		using (List<CutPlantCfg>.Enumerator enumerator = PlantCfgs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_003CPrePareObjectPool_003Ec__Iterator3._003CPrePareObjectPool_003Ec__AnonStoreyA _003CPrePareObjectPool_003Ec__AnonStoreyA = new _003CPrePareObjectPool_003Ec__Iterator3._003CPrePareObjectPool_003Ec__AnonStoreyA();
				_003CPrePareObjectPool_003Ec__AnonStoreyA._003C_003Ef__ref_00243 = this;
				_003CPrePareObjectPool_003Ec__AnonStoreyA.c = enumerator.Current;
				yield return ResMgr.Ins.CreateFromAB(_003CPrePareObjectPool_003Ec__AnonStoreyA.c.grownUpModel, null, _003CPrePareObjectPool_003Ec__AnonStoreyA._003C_003Em__0);
			}
		}
		List<MineCfg> mineCfgs = MineCfg.GetAllList();
		using (List<MineCfg>.Enumerator enumerator2 = mineCfgs.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				_003CPrePareObjectPool_003Ec__Iterator3._003CPrePareObjectPool_003Ec__AnonStoreyC _003CPrePareObjectPool_003Ec__AnonStoreyC = new _003CPrePareObjectPool_003Ec__Iterator3._003CPrePareObjectPool_003Ec__AnonStoreyC();
				_003CPrePareObjectPool_003Ec__AnonStoreyC._003C_003Ef__ref_00243 = this;
				_003CPrePareObjectPool_003Ec__AnonStoreyC.c = enumerator2.Current;
				yield return ResMgr.Ins.CreateFromAB(_003CPrePareObjectPool_003Ec__AnonStoreyC.c.model, null, _003CPrePareObjectPool_003Ec__AnonStoreyC._003C_003Em__0);
			}
		}
	}

	private Mine CreateMine(GameObject go)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(go.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.SetParent(m_MineParent);
		gameObject.SetActiveBetter(false);
		return gameObject.AddComponent<Mine>();
	}

	private PlantInfo CreatePlant(GameObject go)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(go.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.SetParent(m_plantParent);
		gameObject.SetActiveBetter(false);
		return gameObject.AddComponent<PlantInfo>();
	}

	private void OnLoadDecalFinish(GameObject go)
	{
		m_Decal = go;
		m_Decal.transform.SetParent(m_DecalParent);
		m_Decal.transform.localScale = Vector3.one;
		m_Decal.transform.localPosition = Vector3.zero;
		m_Decal.transform.rotation = Quaternion.identity;
		m_Decal.GetComponent<AutoRecycle>().enabled = false;
		m_Decal.SetActiveBetter(false);
		ObjectPool<Decal>.CreateObject<Decal> createFun = CreateDecal;
		if (_003C_003Ef__am_0024cache1 == null)
		{
			_003C_003Ef__am_0024cache1 = _003COnLoadDecalFinish_003Em__4;
		}
		ObjectPool<Decal>.DestroyObject<Decal> destroyFun = _003C_003Ef__am_0024cache1;
		if (_003C_003Ef__am_0024cache2 == null)
		{
			_003C_003Ef__am_0024cache2 = _003COnLoadDecalFinish_003Em__5;
		}
		DecalPool = new ObjectPool<Decal>(30, createFun, destroyFun, _003C_003Ef__am_0024cache2);
	}

	private void OnLoadZidanFinish(GameObject go)
	{
		m_Bullet = go;
		m_Bullet.transform.SetParent(m_BulletParent);
		m_Bullet.transform.localScale = Vector3.one;
		m_Bullet.transform.position = Vector3.zero;
		m_Bullet.transform.rotation = Quaternion.identity;
		m_Bullet.GetComponent<AutoRecycle>().enabled = false;
		m_Bullet.SetActiveBetter(false);
		ObjectPool<BulletControl>.CreateObject<BulletControl> createFun = CreateBullet;
		if (_003C_003Ef__am_0024cache3 == null)
		{
			_003C_003Ef__am_0024cache3 = _003COnLoadZidanFinish_003Em__6;
		}
		BulletPool = new ObjectPool<BulletControl>(30, createFun, _003C_003Ef__am_0024cache3, RecycleBullet);
	}

	private void OnLoadZidan3rdFinish(GameObject go)
	{
		m_Bullet3rd = go;
		m_Bullet3rd.transform.SetParent(m_BulletParent);
		m_Bullet3rd.transform.localScale = Vector3.one;
		m_Bullet3rd.transform.position = Vector3.zero;
		m_Bullet3rd.transform.rotation = Quaternion.identity;
		m_Bullet3rd.GetComponent<AutoRecycle>().enabled = false;
		m_Bullet3rd.SetActiveBetter(false);
		ObjectPool<Bullet3rdControl>.CreateObject<Bullet3rdControl> createFun = CreateBullet3rd;
		if (_003C_003Ef__am_0024cache4 == null)
		{
			_003C_003Ef__am_0024cache4 = _003COnLoadZidan3rdFinish_003Em__7;
		}
		Bullet3rdPool = new ObjectPool<Bullet3rdControl>(30, createFun, _003C_003Ef__am_0024cache4, RecycleBullet3rd);
	}

	private void OnLoadRpgZidanFinish(GameObject go)
	{
		m_RpgBullet = go;
		m_RpgBullet.transform.SetParent(m_BulletParent);
		m_RpgBullet.transform.localScale = Vector3.one;
		m_RpgBullet.transform.position = Vector3.zero;
		m_RpgBullet.transform.rotation = Quaternion.identity;
		m_RpgBullet.GetComponent<AutoRecycle>().enabled = false;
		m_RpgBullet.SetActiveBetter(false);
		ObjectPool<BulletControl>.CreateObject<BulletControl> createFun = CreateRpgBullet;
		if (_003C_003Ef__am_0024cache5 == null)
		{
			_003C_003Ef__am_0024cache5 = _003COnLoadRpgZidanFinish_003Em__8;
		}
		RpgBulletPool = new ObjectPool<BulletControl>(5, createFun, _003C_003Ef__am_0024cache5, RecycleBullet);
	}

	private BulletControl CreateBullet()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_Bullet.gameObject, Vector3.zero, Quaternion.identity);
		TrailRenderReset componentInChildren = m_Bullet.GetComponentInChildren<TrailRenderReset>();
		componentInChildren.trailRenderer.startWidth = componentInChildren._startWidth;
		componentInChildren.trailRenderer.endWidth = componentInChildren._endWidth;
		gameObject.transform.SetParent(m_BulletParent);
		gameObject.SetActiveBetter(true);
		gameObject.GetComponent<AutoRecycle>().enabled = true;
		return gameObject.GetComponent<BulletControl>();
	}

	private Bullet3rdControl CreateBullet3rd()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_Bullet3rd.gameObject, Vector3.zero, Quaternion.identity);
		TrailRenderReset componentInChildren = m_Bullet3rd.GetComponentInChildren<TrailRenderReset>();
		componentInChildren.trailRenderer.startWidth = componentInChildren._startWidth;
		componentInChildren.trailRenderer.endWidth = componentInChildren._endWidth;
		gameObject.transform.SetParent(m_BulletParent);
		gameObject.SetActiveBetter(true);
		gameObject.GetComponent<AutoRecycle>().enabled = true;
		return gameObject.GetComponent<Bullet3rdControl>();
	}

	private BulletControl CreateRpgBullet()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_RpgBullet.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(m_RpgBulletParent);
		gameObject.SetActiveBetter(true);
		gameObject.GetComponent<AutoRecycle>().enabled = true;
		return gameObject.GetComponent<BulletControl>();
	}

	private void RecycleBullet(BulletControl o)
	{
		o.gameObject.SetActiveBetter(false);
	}

	private void RecycleBullet3rd(Bullet3rdControl o)
	{
		o.gameObject.SetActive(false);
	}

	private Decal CreateDecal()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_Decal.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(m_DecalParent);
		gameObject.SetActiveBetter(true);
		gameObject.GetComponent<AutoRecycle>().enabled = true;
		return gameObject.GetComponent<Decal>();
	}

	private void OnSLoadBattleSceneFinish(SBattleLoginFinish msg)
	{
		SelfInfo = msg.playerInfo;
		StartPos = new Vector3(msg.playerInfo.pos.x, msg.playerInfo.pos.y, msg.playerInfo.pos.z);
		TeamPlayerDic.Clear();
	}

	public Vector3 GetBirthRandomPos()
	{
		Vector3 vector = new Vector3(SelfInfo.pos.x + (float)Utils.Random(-3, 3), SelfInfo.pos.y, SelfInfo.pos.z + (float)Utils.Random(-3, 3));
		if (Singleton<BattleScMgr>.Ins.NeedNewPlayerboot)
		{
			vector = new Vector3(SelfInfo.pos.x, SelfInfo.pos.y, SelfInfo.pos.z);
		}
		float y = Ins.GetGroundPos(new Vector3(vector.x, vector.y + 1f, vector.z)).y;
		return new Vector3(vector.x, y, vector.z);
	}

	public void AutoRecycle(string name, GameObject go)
	{
		if (!(go == null))
		{
			switch (name)
			{
			case "bullet":
				BulletPool.Recycle(go.GetComponent<BulletControl>());
				break;
			case "rpgbullet":
				RpgBulletPool.Recycle(go.GetComponent<BulletControl>());
				break;
			case "bullet3rd":
				Bullet3rdPool.Recycle(go.GetComponent<Bullet3rdControl>());
				break;
			case "decal":
				DecalPool.Recycle(go.GetComponent<Decal>());
				break;
			}
		}
	}

	public void PlayEffectAtWorldPos(int id, Vector3 pos, Vector3 forword)
	{
		EffectCfg effectCfg = EffectCfg.Get(id);
		if (effectCfg != null)
		{
			CPlayEffectAtPos cPlayEffectAtPos = new CPlayEffectAtPos();
			cPlayEffectAtPos.effectId = id;
			cPlayEffectAtPos.pos.x = pos.x;
			cPlayEffectAtPos.pos.y = pos.y;
			cPlayEffectAtPos.pos.z = pos.z;
			cPlayEffectAtPos.forward.x = forword.x;
			cPlayEffectAtPos.forward.y = forword.y;
			cPlayEffectAtPos.forward.z = forword.z;
			Client2Gs.Ins.Send(cPlayEffectAtPos);
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(effectCfg.path, pos, forword);
		}
	}

	public void AttachEffect(int id)
	{
		EffectCfg effectCfg = EffectCfg.Get(id);
		CAttachEffect cAttachEffect = new CAttachEffect();
		cAttachEffect.effectId = id;
		cAttachEffect.attcherRoleId = Singleton<RoleMgr>.Ins.info.roleId;
		Client2Gs.Ins.Send(cAttachEffect);
		SingletonMono<EffectMgr>.Ins.PlayEffect(effectCfg.path, SelfPlayer.transform);
	}

	public void AutoHuanDan()
	{
		if (SelfPlayer != null && !SelfPlayer.IsDie && SelfPlayer.CurGun != null && SelfPlayer.CurGun.CurLoadBulletNum <= 0 && SelfPlayer.AgainLoadBulletTime <= 0f)
		{
			HuanDan(Singleton<BagMgr>.Ins.ChangeBulletItemId(SelfPlayer.CurGun.InsId));
		}
	}

	public bool BagHaveBullet()
	{
		foreach (int bulletId in SelfPlayer.CurGun.GunCfg.bulletIds)
		{
			if (Singleton<BagMgr>.Ins.GetItemNum(bulletId) > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void HuanDan(int itemId)
	{
		if (SelfPlayer != null && !SelfPlayer.IsDie && SelfPlayer.CurGun != null && Singleton<BagMgr>.Ins.GetItemNum(itemId) > 0 && SelfPlayer.FSMUpBody.CurrentState.ID == StateID.Aim && SelfPlayer.CurGun.CanAddBullet)
		{
			if (SelfPlayer.CurGun.ChangeBulletMode == 0)
			{
				SelfPlayer.FSMUpBody.SwitchState(StateID.Zhuangtian, itemId);
			}
			else if (SelfPlayer.CurGun.ChangeBulletMode == 1)
			{
				SelfPlayer.FSMUpBody.SwitchState(StateID.Huanzidan, itemId);
			}
		}
	}

	public void Clear()
	{
		Debug.LogError("battle clear!!!!!");
		SmallSceneMgr.Ins.Clear();
		if (SelfPlayer != null)
		{
			SelfPlayer.Clear();
			UnityEngine.Object.Destroy(SelfPlayer.gameObject);
		}
		UnityEngine.Object.Destroy(m_Decal);
		UnityEngine.Object.Destroy(m_Bullet);
		ClearUiCameraChildPartical();
		LargeSceneManager.Ins.BattleEnd();
		OtherRolePlayerPool.Clear();
		BattleObjectId2GameObject.Clear();
		battleObjectId2VehicleInfo.Clear();
		battleObjectId2VehicleMonitor.Clear();
		OtherPlayersDic.Clear();
		NearOtherPlayersDic.Clear();
		TeamPlayerDic.Clear();
		MapObjectDic.Clear();
		StopAllCoroutines();
		Singleton<MapMgr>.Ins.Clear();
		SceneEvent.InitLoadSceneFinish = (Utils.VoidDelegate)Delegate.Remove(SceneEvent.InitLoadSceneFinish, new Utils.VoidDelegate(OnLoadSceneFinish));
		BattleEvent.OnDownThrowLei = null;
		BattleEvent.OnLoadBattleSceneFinish = null;
		BattlePackEvent.DropEquip = null;
		BattlePackEvent.PickEquip = null;
		BattlePackEvent.EquipHpDelegate = null;
		BattlePackEvent.UseItemDelegate = null;
		SettingEvent.SensitivityVerticalDelegate = null;
		SettingEvent.SensitivityHorizontalDelegate = null;
		BattleEvent.OnNeedBackSeat = null;
		BattleEvent.OnNeedTanshen = null;
		BattleEvent.OnAttack = null;
		BattleEvent.OnInitVoiceSdkFinish = null;
		SPlayerInfo.handler = null;
		SPlayerDisappear.handler = null;
		SPlayEffectAtPos.handler = null;
		SAttachEffect.handler = null;
		SHitPlayer.handler = null;
		SLevelFlyBattlePlane.handler = null;
		SSyncFootTexture.handler = null;
		SSyncPlayerPos.handler = null;
		SSyncOrientation.handler = null;
		SSyncVelocity.handler = null;
		SSyncAnimator.handler = null;
		SSyncLayerWeight.handler = null;
		SSyncAnimatorSpeed.handler = null;
		SSyncYaw.handler = null;
		SServerPullback.handler = null;
		SHpChange.handler = null;
		SHpStatusChange.handler = null;
		SEpChange.handler = null;
		SSyncCameraLookPoint.handler = null;
		SPlayerDie.handler = null;
		SSyncOpenParachute.handler = null;
		SSyncFallGround.handler = null;
		SSyncHandWeapon.handler = null;
		SOtherDropWear.handler = null;
		SOtherPickWear.handler = null;
		SSyncOtherWear.handler = null;
		SSyncGunPutPart.handler = null;
		SSyncGunRemovePart.handler = null;
		SWatchPlayer.handler = null;
		SShoot.handler = null;
		SSafeZone.handler = null;
		SToxicGasZone.handler = null;
		SGenSafeZone.handler = null;
		SStartShrinkToxicGas.handler = null;
		SEndShrinkToxicGas.handler = null;
		SToxicGasZone.handler = null;
		SSafeZone.handler = null;
		STeamateInfos.handler = null;
		SRequestRobot.handler = null;
		SSyncMapObjectPos.handler = null;
		SGrenadeExplode.handler = null;
		SVehicleInfo.handler = null;
		SVehicleDisappear.handler = null;
		SGetInVehicle.handler = null;
		SGetOutVehicle.handler = null;
		SSyncVehiclePos.handler = null;
		SSyncVehicleOrientation.handler = null;
		SSyncVehicleVelocity.handler = null;
		SSwitchSeat.handler = null;
		SSyncTanshen.handler = null;
		SSyncVehicleFuel.handler = null;
		SSyncSound.handler = null;
		SSyncEngineSound.handler = null;
		SSyncCarWheelRotation.handler = null;
		SStopVehicleSound.handler = null;
		SVehicleHpChange.handler = null;
		SFlyBattlePlane.handler = null;
		SFlyAllowParachuting.handler = null;
		SHpStatusChange.handler = null;
		SSyncPlayerSave.handler = null;
		SGetInVehicleError.handler = null;
		SVehicleSkinChange.handler = null;
		SUseNewProtocal.handler = null;
		SBloodAirportPlayerDie.handler = null;
		SRebirth.handler = null;
		SShowTree.handler = null;
		SHideTree.handler = null;
		STreeGrow.handler = null;
		STreeDown.handler = null;
		Ins = null;
		SceneManager.LoadScene("empty");
		ResMgr.Ins.CleanCahce();
		ViewMgr.Ins.Destroy<BattlePanel>();
		SingletonMono<AudioManager>.Ins.ClearAudio();
		ClearObjectCulling();
	}

	private void ClearUiCameraChildPartical()
	{
		EffectInfo[] componentsInChildren = ViewMgr.Ins.UICamera.GetComponentsInChildren<EffectInfo>();
		EffectInfo[] array = componentsInChildren;
		foreach (EffectInfo effectInfo in array)
		{
			UnityEngine.Object.Destroy(effectInfo.gameObject);
		}
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			UpdateNearOtherPlayerDic();
			UpdatePlayerVehicleLod();
			yield return Utils.WaitForSeconds(1f);
		}
	}

	private IEnumerator ZeroOneTick()
	{
		while (true)
		{
			yield return m_ZeroOne;
		}
	}

	private IEnumerator FiveTick()
	{
		while (true)
		{
			SendFps();
			yield return Utils.WaitForSeconds(60f);
		}
	}

	private void SendFps()
	{
		CFPSNum cFPSNum = new CFPSNum();
		cFPSNum.fpsNum = (byte)m_fps.m_CurrentFps;
	}

	private void UpdateNearOtherPlayerDic()
	{
		if (!SelfPlayer)
		{
			return;
		}
		foreach (KeyValuePair<long, OtherPlayerController> item in OtherPlayersDic)
		{
			if (Vector3.Distance(SelfPlayer.Pos, item.Value.Pos) <= 30f)
			{
				if (!NearOtherPlayersDic.ContainsKey(item.Key))
				{
					NearOtherPlayersDic.Add(item.Key, item.Value);
				}
			}
			else if (NearOtherPlayersDic.ContainsKey(item.Key))
			{
				NearOtherPlayersDic.Remove(item.Key);
			}
		}
	}

	private void UpdateOtherPlayerAnimatorSetting()
	{
		foreach (KeyValuePair<long, OtherPlayerController> item in OtherPlayersDic)
		{
			if (NearOtherPlayersDic.ContainsKey(item.Key))
			{
				item.Value.PlayerAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			else
			{
				item.Value.PlayerAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
		}
	}

	public float GetWaterDeep(Vector3 pos)
	{
		WaterSurfaceHeight = 10f;
		WaterDeep = 0f;
		RaycastHit hitInfo;
		if (Physics.Raycast(new Vector3(pos.x, -100f, pos.z), Vector3.up, out hitInfo, 1500f, 1 << BattleScMgr.WaterLayer))
		{
			WaterSurfaceHeight = hitInfo.point.y;
			if (WaterSurfaceHeight > pos.y)
			{
				WaterDeep = WaterSurfaceHeight - pos.y;
			}
		}
		return WaterDeep;
	}

	public void CameraInWater()
	{
		if (GetWaterDeep(MainCamera.transform.position) > 0f)
		{
			SingletonMono<DayNightSystem>.Ins.InWater = true;
			if (!m_lastCameraInWater)
			{
				m_lastCameraInWater = true;
			}
		}
		else
		{
			SingletonMono<DayNightSystem>.Ins.InWater = false;
			if (m_lastCameraInWater)
			{
				m_lastCameraInWater = false;
			}
		}
	}

	public float GetWaterLevel(Vector3 pos)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Vector3(pos.x, -100f, pos.z), Vector3.up, out hitInfo, 1500f, 1 << BattleScMgr.WaterLayer))
		{
			return hitInfo.point.y;
		}
		return -500f;
	}

	public int SyncPlaySound(int id)
	{
		SoundCfg soundCfg = SoundCfg.Get(id);
		if (soundCfg == null)
		{
			return -1;
		}
		m_SyncSoundMsg.soundId = id;
		m_SyncSoundMsg.loop = false;
		m_SyncSoundMsg.volume = soundCfg.volume;
		m_SyncSoundMsg.maxDist = soundCfg.radius;
		m_SyncSoundMsg.minDist = 0f;
		m_SyncSoundMsg.pitch = 1f;
		m_SyncSoundMsg.playMoreThanOne = false;
		m_SyncSoundMsg.pos.x = SelfPlayer.transform.position.x;
		m_SyncSoundMsg.pos.y = SelfPlayer.transform.position.y;
		m_SyncSoundMsg.pos.z = SelfPlayer.transform.position.z;
		return SingletonMono<AudioManager>.Ins.Play(soundCfg.path, SelfPlayer.transform.position, false, soundCfg.volume, true, 0f, soundCfg.radius);
	}

	public int SyncPlaySound(int id, Vector3 pos)
	{
		SoundCfg soundCfg = SoundCfg.Get(id);
		if (soundCfg == null)
		{
			return -1;
		}
		m_SyncSoundMsg.soundId = id;
		m_SyncSoundMsg.loop = false;
		m_SyncSoundMsg.volume = soundCfg.volume;
		m_SyncSoundMsg.maxDist = soundCfg.radius;
		m_SyncSoundMsg.minDist = 0f;
		m_SyncSoundMsg.pitch = 1f;
		m_SyncSoundMsg.playMoreThanOne = false;
		m_SyncSoundMsg.pos.x = pos.x;
		m_SyncSoundMsg.pos.y = pos.y;
		m_SyncSoundMsg.pos.z = pos.z;
		return SingletonMono<AudioManager>.Ins.Play(soundCfg.path, pos, false, soundCfg.volume, true, 0f, soundCfg.radius);
	}

	public float GetAwayGroundDistance(Vector3 pos)
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(pos + Vector3.up * 1f, 0.05f, Vector3.down, out hitInfo, 2000f, GroundLayer, QueryTriggerInteraction.Ignore))
		{
			return pos.y - hitInfo.point.y;
		}
		return 10000f;
	}

	public Vector3 GetGroundPos(Vector3 pos)
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(pos + Vector3.up * 0.1f, 0.05f, Vector3.down, out hitInfo, 1000f, GroundLayer, QueryTriggerInteraction.Ignore))
		{
			pos.y = hitInfo.point.y + 0.1f;
		}
		return pos;
	}

	public Vector3 GetGroundPosForCar(Vector3 pos)
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(pos, 0.05f, Vector3.down, out hitInfo, 10000f, 1 << BattleScMgr.DefaultLayer, QueryTriggerInteraction.Ignore) && !hitInfo.collider.gameObject.name.Equals("haidi"))
		{
			pos.y = hitInfo.point.y + 0.1f;
			return pos;
		}
		pos.y = 2000f;
		return pos;
	}

	public bool IsOnHaiDi(Vector3 pos)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(pos, Vector3.down, out hitInfo, 1.5f) && hitInfo.collider.gameObject.name.Equals("haidi"))
		{
			return true;
		}
		return false;
	}

	private void InitGasoline()
	{
		foreach (ItemCfg all in ItemCfg.GetAllList())
		{
			if (all.type == 36)
			{
				_mGasolineId = all.id;
				_mUseGasolineTime = all.useTime;
				_mGasolineName = all.name;
				break;
			}
		}
	}

	public void LookNextPeople()
	{
	}

	public void LookPreviousPeople()
	{
	}

	public void RegisOtherPlayerForCulling(OtherPlayerController p)
	{
		if (p.CullingIndex >= 0)
		{
			Debug.LogError(string.Format("[battle]CullingIndex={0} not -1 when RegisOtherPlayerForCulling for {1}", p.CullingIndex, p.Name));
			return;
		}
		if (m_OtherPlayerForCullingCount >= s_OtherPlayerBoundingSphereArray.Length)
		{
			Debug.LogError("[battle]m_OtherPlayerForCullingCount exceed max " + m_OtherPlayerForCullingCount);
			return;
		}
		BoundingSphere boundingSphere = s_OtherPlayerBoundingSphereArray[m_OtherPlayerForCullingCount];
		boundingSphere.position = p.Pos;
		boundingSphere.radius = 15f;
		s_OtherPlayerBoundingSphereArray[m_OtherPlayerForCullingCount] = boundingSphere;
		s_OtherPlayersForCulling[m_OtherPlayerForCullingCount] = p;
		p.CullingIndex = m_OtherPlayerForCullingCount++;
		s_OtherPlayerCullingGroup.SetBoundingSpheres(s_OtherPlayerBoundingSphereArray);
		s_OtherPlayerCullingGroup.SetBoundingSphereCount(m_OtherPlayerForCullingCount);
		p.Culled = true;
	}

	public void UnRegisOtherPlayerForCulling(OtherPlayerController p)
	{
		if (p.CullingIndex < 0)
		{
			Debug.LogError(string.Format("[battle]CullingIndex={0} when UnRegisOtherPlayerForCulling for {1}", p.CullingIndex, p.Name));
			return;
		}
		if (m_OtherPlayerForCullingCount <= p.CullingIndex)
		{
			Debug.LogError(string.Format("[battle]CullingIndex {0} exceed m_OtherPlayerForCullingCount {1}", p.CullingIndex, m_OtherPlayerForCullingCount));
			return;
		}
		BoundingSphere boundingSphere = s_OtherPlayerBoundingSphereArray[m_OtherPlayerForCullingCount - 1];
		OtherPlayerController otherPlayerController = s_OtherPlayersForCulling[m_OtherPlayerForCullingCount - 1];
		s_OtherPlayerBoundingSphereArray[p.CullingIndex] = boundingSphere;
		s_OtherPlayersForCulling[p.CullingIndex] = otherPlayerController;
		otherPlayerController.CullingIndex = p.CullingIndex;
		s_OtherPlayersForCulling[m_OtherPlayerForCullingCount - 1] = null;
		m_OtherPlayerForCullingCount--;
		p.CullingIndex = -1;
		s_OtherPlayerCullingGroup.SetBoundingSpheres(s_OtherPlayerBoundingSphereArray);
		s_OtherPlayerCullingGroup.SetBoundingSphereCount(m_OtherPlayerForCullingCount);
		p.Culled = true;
		cRemoveRolesFromCanSee.roleIds.Add(p.RoleId);
		cAddRolesToCanSee.roleIds.Remove(p.RoleId);
	}

	public void SetOtherPlayerCullingPos(int index, Vector3 pos)
	{
		if (index >= 0)
		{
			s_OtherPlayerBoundingSphereArray[index].position = pos;
		}
	}

	public void RegisVehicleForCulling(VehicleMonitor v)
	{
		if (v.CullingIndex < 0)
		{
			if (m_VehicleCountForCulling >= s_VehicleBoundingSphereArray.Length)
			{
				Debug.LogError("[battle]m_VehicleCountForCulling exceed max " + m_VehicleCountForCulling);
				return;
			}
			BoundingSphere boundingSphere = s_VehicleBoundingSphereArray[m_VehicleCountForCulling];
			boundingSphere.position = v.VehicleTransform.position;
			boundingSphere.radius = 30f;
			s_VehicleBoundingSphereArray[m_VehicleCountForCulling] = boundingSphere;
			s_VehiclesForCulling[m_VehicleCountForCulling] = v;
			v.CullingIndex = m_VehicleCountForCulling++;
			s_VehicleCullingGroup.SetBoundingSpheres(s_VehicleBoundingSphereArray);
			s_VehicleCullingGroup.SetBoundingSphereCount(m_VehicleCountForCulling);
			v.Culled = true;
		}
	}

	public void UnRegisVehicleForCulling(VehicleMonitor v)
	{
		if (v.CullingIndex >= 0)
		{
			if (m_VehicleCountForCulling <= v.CullingIndex)
			{
				Debug.LogError(string.Format("[battle]CullingIndex {0} exceed m_VehicleCountForCulling {1}", v.CullingIndex, m_VehicleCountForCulling));
				return;
			}
			BoundingSphere boundingSphere = s_VehicleBoundingSphereArray[m_VehicleCountForCulling - 1];
			VehicleMonitor vehicleMonitor = s_VehiclesForCulling[m_VehicleCountForCulling - 1];
			s_VehicleBoundingSphereArray[v.CullingIndex] = boundingSphere;
			s_VehiclesForCulling[v.CullingIndex] = vehicleMonitor;
			vehicleMonitor.CullingIndex = v.CullingIndex;
			s_VehiclesForCulling[m_VehicleCountForCulling - 1] = null;
			m_VehicleCountForCulling--;
			v.CullingIndex = -1;
			s_VehicleCullingGroup.SetBoundingSpheres(s_VehicleBoundingSphereArray);
			s_VehicleCullingGroup.SetBoundingSphereCount(m_VehicleCountForCulling);
			v.Culled = true;
			cRemoveVehiclesFromCanSee.vehicleIds.Add(v.GetId());
			cAddVehiclesToCanSee.vehicleIds.Remove(v.GetId());
		}
	}

	public void SetVehicleCullingPos(int index, Vector3 pos)
	{
		if (index >= 0)
		{
			s_VehicleBoundingSphereArray[index].position = pos;
		}
	}

	public void InitObjectCulling(Camera cam)
	{
		s_OtherPlayerCullingGroup.targetCamera = cam;
		s_OtherPlayerCullingGroup.SetDistanceReferencePoint(cam.transform);
		s_VehicleCullingGroup.targetCamera = cam;
		s_VehicleCullingGroup.SetDistanceReferencePoint(cam.transform);
		s_OtherPlayerCullingGroup.SetBoundingDistances(OtherPlayerDistancesArray);
		s_VehicleCullingGroup.SetBoundingDistances(VehicleDistancesArray);
		s_OtherPlayerCullingGroup.onStateChanged = OnOtherPlayerCullingStateChanged;
		s_VehicleCullingGroup.onStateChanged = OnVehicleCullingStateChanged;
		s_OtherPlayerCullingGroup.SetBoundingSphereCount(0);
		s_VehicleCullingGroup.SetBoundingSphereCount(0);
		m_OtherPlayerForCullingCount = 0;
		m_VehicleCountForCulling = 0;
	}

	public void ClearObjectCulling()
	{
		s_OtherPlayerCullingGroup.targetCamera = null;
		s_VehicleCullingGroup.targetCamera = null;
		s_OtherPlayerCullingGroup.SetDistanceReferencePoint(null);
		s_VehicleCullingGroup.SetDistanceReferencePoint(null);
		m_OtherPlayerForCullingCount = 0;
		m_VehicleCountForCulling = 0;
		s_OtherPlayerCullingGroup.SetBoundingSphereCount(0);
		s_VehicleCullingGroup.SetBoundingSphereCount(0);
		for (int i = 0; i < s_OtherPlayersForCulling.Length; i++)
		{
			s_OtherPlayersForCulling[i] = null;
		}
		for (int j = 0; j < s_VehiclesForCulling.Length; j++)
		{
			s_VehiclesForCulling[j] = null;
		}
	}

	public void OnOtherPlayerCullingStateChanged(CullingGroupEvent e)
	{
		OtherPlayerController otherPlayerController = s_OtherPlayersForCulling[e.index];
		if (e.currentDistance == 0 && otherPlayerController.Culled)
		{
			otherPlayerController.Culled = false;
			cAddRolesToCanSee.roleIds.Add(otherPlayerController.RoleId);
			cRemoveRolesFromCanSee.roleIds.Remove(otherPlayerController.RoleId);
		}
		else if (e.currentDistance > 0)
		{
			if (e.isVisible && otherPlayerController.Culled)
			{
				otherPlayerController.Culled = false;
				cAddRolesToCanSee.roleIds.Add(otherPlayerController.RoleId);
				cRemoveRolesFromCanSee.roleIds.Remove(otherPlayerController.RoleId);
			}
			else if (!e.isVisible && !otherPlayerController.Culled)
			{
				otherPlayerController.Culled = true;
				cRemoveRolesFromCanSee.roleIds.Add(otherPlayerController.RoleId);
				cAddRolesToCanSee.roleIds.Remove(otherPlayerController.RoleId);
			}
		}
	}

	public void OnVehicleCullingStateChanged(CullingGroupEvent e)
	{
		VehicleMonitor vehicleMonitor = s_VehiclesForCulling[e.index];
		if (e.currentDistance == 0 && vehicleMonitor.Culled)
		{
			vehicleMonitor.Culled = false;
			cAddVehiclesToCanSee.vehicleIds.Add(vehicleMonitor.GetId());
			cRemoveVehiclesFromCanSee.vehicleIds.Remove(vehicleMonitor.GetId());
		}
		else if (e.currentDistance > 0)
		{
			if (e.isVisible && vehicleMonitor.Culled)
			{
				vehicleMonitor.Culled = false;
				cAddVehiclesToCanSee.vehicleIds.Add(vehicleMonitor.GetId());
				cRemoveVehiclesFromCanSee.vehicleIds.Remove(vehicleMonitor.GetId());
			}
			else if (!e.isVisible && !vehicleMonitor.Culled)
			{
				vehicleMonitor.Culled = true;
				cRemoveVehiclesFromCanSee.vehicleIds.Add(vehicleMonitor.GetId());
				cAddVehiclesToCanSee.vehicleIds.Remove(vehicleMonitor.GetId());
			}
		}
	}

	public void LateUpdate()
	{
		if (m_SendCullingMsg)
		{
			if (cAddRolesToCanSee.roleIds.Count > 0)
			{
				Client2Gs.Ins.Send(cAddRolesToCanSee);
				cAddRolesToCanSee.roleIds.Clear();
			}
			if (cRemoveRolesFromCanSee.roleIds.Count > 0)
			{
				Client2Gs.Ins.Send(cRemoveRolesFromCanSee);
				cRemoveRolesFromCanSee.roleIds.Clear();
			}
			if (cAddVehiclesToCanSee.vehicleIds.Count > 0)
			{
				Client2Gs.Ins.Send(cAddVehiclesToCanSee);
				cAddVehiclesToCanSee.vehicleIds.Clear();
			}
			if (cRemoveVehiclesFromCanSee.vehicleIds.Count > 0)
			{
				Client2Gs.Ins.Send(cRemoveVehiclesFromCanSee);
				cRemoveVehiclesFromCanSee.vehicleIds.Clear();
			}
		}
	}

	public Vector3 GetForwardInSlop(Transform t)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(t.position + Vector3.up * 1f, Vector3.down, out hitInfo, 5f, GroundLayer))
		{
			Vector3 normal = hitInfo.normal;
			Vector3 lhs = Vector3.Cross(normal, t.forward);
			Vector3 target = Vector3.Cross(lhs, normal);
			t.forward = Vector3.MoveTowards(t.forward, target, 0f);
		}
		return t.forward;
	}

	private void Update()
	{
	}

	private void UpdatePlayerVehicleLod()
	{
		foreach (KeyValuePair<long, OtherPlayerController> item in OtherPlayersDic)
		{
			item.Value.UpdateLod();
		}
		foreach (KeyValuePair<int, VehicleMonitor> item2 in battleObjectId2VehicleMonitor)
		{
			item2.Value.UpdateLod();
		}
	}

	[CompilerGenerated]
	private void _003CAwake_003Em__0(long id)
	{
		if (TeamPlayerDic.ContainsKey(id))
		{
			TeamPlayerDic[id].NameLabel.gameObject.SetActiveBetter(false);
			TeamPlayerDic.Remove(id);
		}
	}

	[CompilerGenerated]
	private OtherPlayerController _003CInitOtherPlayer_003Em__1()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(m_RolePlayerModel.gameObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(m_OtherPlayerPoolParent);
		return gameObject.GetComponent<OtherPlayerController>();
	}

	[CompilerGenerated]
	private static void _003CInitOtherPlayer_003Em__2(OtherPlayerController o)
	{
		UnityEngine.Object.Destroy(o.gameObject);
	}

	[CompilerGenerated]
	private void _003CInitOtherPlayer_003Em__3(OtherPlayerController o)
	{
		o.Reset();
		o.transform.SetParent(m_OtherPlayerPoolParent);
		o.gameObject.SetActive(false);
	}

	[CompilerGenerated]
	private static void _003COnLoadDecalFinish_003Em__4(Decal o)
	{
		UnityEngine.Object.Destroy(o.gameObject);
	}

	[CompilerGenerated]
	private static void _003COnLoadDecalFinish_003Em__5(Decal o)
	{
		o.gameObject.SetActiveBetter(false);
	}

	[CompilerGenerated]
	private static void _003COnLoadZidanFinish_003Em__6(BulletControl o)
	{
		UnityEngine.Object.Destroy(o.gameObject);
	}

	[CompilerGenerated]
	private static void _003COnLoadZidan3rdFinish_003Em__7(Bullet3rdControl o)
	{
		UnityEngine.Object.Destroy(o.gameObject);
	}

	[CompilerGenerated]
	private static void _003COnLoadRpgZidanFinish_003Em__8(BulletControl o)
	{
		UnityEngine.Object.Destroy(o.gameObject);
	}
}

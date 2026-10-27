using System;
using UnityEngine;
using gs.battle.scmsg;

public class BattleEvent
{
	public delegate void LoadGunFinish(Gun gun);

	public delegate void NearCar(VehicleMonitor car);

	public delegate void OnHpChangeDelegate(int hp, int changeNum);

	public delegate void OnGunModeLoadFinish(BaseGun baseGun);

	public delegate void RoleFootSoundEvent(GameObject go, long roleId, int soundId);

	public delegate void RoleGunSoundEvent(GameObject go, long roleId, int soundId, bool isSilencer);

	public delegate void VehicleSoundEvent(GameObject go, int vehicleId, int soundId);

	public delegate void AirdropSoundEvent(GameObject go, int vehicleId, int soundId);

	public delegate void OnswitchState(StateID from, StateID to, long senderInsId);

	public delegate void OnSelfSwitchStateEvent(StateID from, StateID to);

	public delegate void OnloadBattleSceneFinish(SBattleLoginFinish sEnterRoom);

	public delegate void OnReciveteamPlayerInfo(PlayerInfo playerinfo);

	public delegate void OnGetOutOfVehicle(int vehicleId, Vector3 pos);

	public static OnGunModeLoadFinish OnGunModeLoadFinishDelegate;

	public static Utils.IntDelegate OnCurLoadBulletNumChange;

	public static OnHpChangeDelegate OnSelfHpChange;

	public static Utils.IntDelegate OnEpChange;

	public static Utils.Vector3Delegate onSelfDie;

	public static Utils.Vector2Delegate onDie;

	public static Utils.VoidDelegate onSelfBirth;

	public static Utils.VoidDelegate OnFarCar;

	public static Utils.VoidDelegate OnAttack;

	public static Utils.VoidDelegate OnChangeHandGun;

	public static Utils.VoidDelegate OnCarControllTypeChange;

	public static Utils.VoidDelegate OnOpenKaiJing;

	public static Utils.VoidDelegate OnCloseKaiJing;

	public static Utils.VoidDelegate OnOpenJiMiao;

	public static Utils.VoidDelegate OnCloseJiMiao;

	public static Utils.VoidDelegate OnKaijingShoot;

	public static Utils.VoidDelegate OnUpLei;

	public static Utils.VoidDelegate OnCanThrowLei;

	public static Utils.VoidDelegate OnCanFlyLei;

	public static Utils.VoidDelegate OnRefreshTeamateInfo;

	public static Utils.IntDelegate OnChangeLei;

	public static Utils.IntDelegate OnDownThrowLei;

	public static Utils.VoidDelegate OnAllowAwayPlane;

	public static Utils.VoidDelegate OnAllowKaiSan;

	public static Utils.VoidDelegate OnZhuolu;

	public static Utils.VoidDelegate OnGetOutVehicle;

	public static Utils.VoidDelegate OnChangePart;

	public static Utils.VoidDelegate OnNeedBackSeat;

	public static Utils.VoidDelegate OnNeedTanshen;

	public static Utils.VoidDelegate OnEnterCrouch;

	public static Utils.VoidDelegate OnEnterPa;

	public static Utils.VoidDelegate OnEnterStand;

	public static Utils.Vector3Delegate OnHited;

	public static Utils.VoidDelegate OnHitOtherPlayer;

	public static Utils.VoidDelegate OnInitVoiceSdkFinish;

	public static Utils.VoidDelegate OnDestoryStartPlane;

	public static Utils.VoidDelegate OnDownTouchPad;

	public static Utils.IntDelegate OnUseItemFinish;

	public static Utils.LongDelegate OnSelfKilled;

	public static Utils.VoidDelegate OnSelfKilledSelf;

	public static Utils.LongDelegate OnAiDie;

	public static LoadGunFinish OnLoadGunFinish;

	public static NearCar OnNearCar;

	public static RoleFootSoundEvent roleFootSoundEvent;

	public static RoleGunSoundEvent roleGunSoundEvent;

	public static VehicleSoundEvent vehicleSoundEvent;

	public static AirdropSoundEvent airdropSoundEvent;

	public static OnswitchState OnSwitchState;

	public static OnSelfSwitchStateEvent OnSelfSwitchState;

	public static OnloadBattleSceneFinish OnLoadBattleSceneFinish;

	public static OnReciveteamPlayerInfo OnReciveTeamPlayerInfo;

	public static OnGetOutOfVehicle onGetOutOfVehicle;

	public static Utils.VoidDelegate OnGetInVehicle;

	public static Utils.IntDelegate OnOtherGetInVehicle;

	public static Utils.IntDelegate OnVehicleDisappear;

	public static Utils.VoidDelegate OnJoinVoiceRoomSuccess;

	public static Utils.IntLongDelegate OnClickUseBuild;

	public static Utils.BoolDelegate OnWantEnterBuildState;

	public static Utils.BoolDelegate OnWantExitBuildState;

	public static Utils.VoidDelegate OnEnteredBuildState;

	public static Utils.VoidDelegate OnExitedBuildState;

	public static Utils.VoidDelegate OnEnteredBuildStateFromQuickUse;

	public static Utils.VoidDelegate OnExitedBuildStateFromQuickUse;

	public static Utils.VoidDelegate OnClickBuildBtn;

	public static Utils.String2Delegate OnSceneChanged;

	public static Utils.LongDelegate OnBuildNameChanged;

	public static Utils.BoolDelegate OnSelfDown;

	public static Utils.LongDelegate OnShoot;

	public static Utils.VoidDelegate OnChangeNight;

	public static Utils.VoidDelegate OnChangeDay;

	public static Utils.VoidDelegate OnChangeQingchen;

	public static Utils.VoidDelegate OnChangeHuanghun;

	public static event Action<GameObject> OnTrusteeshipDownFireAction;

	public static event Action OnPlayerOperateStopTrusteeship;

	public static void TrusteeshipFire()
	{
		if (SingletonMono<TrusteeshipMgr>.Ins.IsInTrusteeship && BattleEvent.OnTrusteeshipDownFireAction != null)
		{
			BattleEvent.OnTrusteeshipDownFireAction(null);
		}
	}

	public static void StopTrusteeship()
	{
		if (SingletonMono<TrusteeshipMgr>.Ins.IsInTrusteeship && BattleEvent.OnPlayerOperateStopTrusteeship != null)
		{
			BattleEvent.OnPlayerOperateStopTrusteeship();
		}
	}
}

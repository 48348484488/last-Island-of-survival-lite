using System;
using UnityEngine;
using gs.battle.scmsg;

public class InCarState : FSMState
{
	private vThirdPersonCameraState inCarState;

	private CarInfo m_CarInfo;

	private ShipInfo m_ShipInfo;

	private PlaneInfo m_PlaneInfo;

	private TwoWheelMotoInfo m_TwoWheelMotoInfo;

	private int m_VehicleType;

	private int m_WeaponInsId = -1;

	private int m_SeatIndex;

	private bool m_NeedBack;

	private bool m_NeedHandIk;

	private CSyncTanshen m_SyncTanshen = new CSyncTanshen();

	public InCarState()
	{
		stateID = StateID.InCar;
		BattleEvent.OnNeedBackSeat = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnNeedBackSeat, new Utils.VoidDelegate(BackSeat));
		BattleEvent.OnNeedTanshen = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnNeedTanshen, new Utils.VoidDelegate(DoTanshen));
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (Battle.Ins.SelfPlayer.CurGun != null)
		{
			Battle.Ins.SelfPlayer.CurGun.StopShoot();
		}
		Player.CloseAutoRun();
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		Player.Input = Vector3.zero;
		m_WeaponInsId = Player.CurrentWeaponInsId;
		m_VehicleType = Player.NearCar.GetVehicleType();
		if (m_VehicleType == 0)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DriveCar");
			inCarState = Battle.Ins.MainCamera.CameraStateList.FindState("DriveCar");
			m_CarInfo = Player.NearCar.GetComponent<CarInfo>();
		}
		else if (m_VehicleType == 2)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DrivePlane");
			inCarState = Battle.Ins.MainCamera.CameraStateList.FindState("DrivePlane");
			m_PlaneInfo = Player.NearCar.GetComponent<PlaneInfo>();
		}
		else if (m_VehicleType == 1)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DriveShip");
			inCarState = Battle.Ins.MainCamera.CameraStateList.FindState("DriveShip");
			m_ShipInfo = Player.NearCar.GetComponent<ShipInfo>();
		}
		else if (m_VehicleType == 3)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DriveMoto");
			inCarState = Battle.Ins.MainCamera.CameraStateList.FindState("DriveMoto");
			m_TwoWheelMotoInfo = Player.NearCar.GetComponent<TwoWheelMotoInfo>();
		}
		if (Player.Tanshen)
		{
			DoTanshen();
		}
		Player.PlayerAnimator.applyRootMotion = false;
		Player.SetZaijvRigidbodyValue();
	}

	public override void DoBeforeLeaving()
	{
		Player.PlayerAnimator.applyRootMotion = true;
		Player.ResetRigidbodyValue();
		Battle.Ins.MainCamera.SetMainTarget(Player.transform, "Stand");
		Player.transform.eulerAngles = new Vector3(0f, Battle.Ins.MainCamera.transform.eulerAngles.y, 0f);
		Player.transform.parent = null;
		if (!Player.HaveGunInHand && Player.GetWeapon(m_WeaponInsId) != null)
		{
			Player.PlayChangeWeapen(m_WeaponInsId);
		}
		Player.Tanshen = false;
		m_SyncTanshen.isTanshen = false;
		Client2Gs.Ins.Send(m_SyncTanshen);
	}

	public override void Act()
	{
		Player.SetAnimatorHorizontalValue(0f);
		Player.SetAnimatorForwardValue(0f);
		UpdateCameraState();
		Player.transform.localPosition = Vector3.zero;
		Player.CloseAllIK();
		if (!Player.HaveGun && Player.Tanshen)
		{
			BackSeat();
		}
		UpdateTargetAnim();
	}

	public override void LateUpdate()
	{
		if (Player.IsDriver)
		{
			Player.ChangeHandWeapon(-1);
			Player.Tanshen = false;
			inCarState.followDirectionY = SettingMgr.OpenCameraAI && Player.NearCar.GetSpeed() >= 5f;
			inCarState.followSmooth = Mathf.MoveTowards(0f, 3f, (Player.NearCar.GetSpeed() - 5f) * 0.2f);
		}
		else
		{
			inCarState.followDirectionY = false;
		}
		if (m_VehicleType == 0)
		{
			Player.transform.localEulerAngles = Vector3.zero;
			if (Player.Tanshen)
			{
				float num;
				for (num = Battle.Ins.MainCamera.transform.eulerAngles.y - Player.transform.eulerAngles.y; num < 0f; num += 360f)
				{
				}
				while (num >= 360f)
				{
					num -= 360f;
				}
				Player.SetAnimatorYawValue(num);
			}
		}
		else if (m_VehicleType == 1 || m_VehicleType == 2)
		{
			if (Player.Tanshen)
			{
				Player.SetRotation(Battle.Ins.MainCamera.transform);
			}
			else
			{
				Player.transform.localEulerAngles = Vector3.zero;
			}
		}
		else if (m_VehicleType == 3)
		{
			Player.PlayChangeWeapen(-1);
			Player.SetAnimatorYawValue(((TwoWheelMotoController)Player.NearCar.GetIVehicle()).FrontWheelColliderSteerAngle);
			Player.transform.localEulerAngles = Vector3.zero;
		}
		if (m_NeedHandIk)
		{
			Player.m_HoldIK.SetIKWeight(1f);
			Player.m_HoldIK.SetIKPosition(m_TwoWheelMotoInfo.leftHandAttachPoint.position);
			Player.m_RightHandHoldIK.SetIKWeight(1f);
			Player.m_RightHandHoldIK.SetIKPosition(m_TwoWheelMotoInfo.rightHandAttachPoint.position);
		}
	}

	public override void Reason()
	{
	}

	public void UpdateTargetAnim()
	{
		if (Player.IsDriver)
		{
			if (m_VehicleType == 3)
			{
				m_NeedHandIk = true;
				if (Player.NearCar.GetSpeed() > 14f)
				{
					m_TargetAimatorName = "InCar.zaiju_motuo_jiashi01";
				}
				else
				{
					Player.SetAnimatorYValue(Player.NearCar.GetSpeed());
					m_TargetAimatorName = "InCar.zaiju_motuo_jiashi03";
				}
			}
			else
			{
				m_NeedHandIk = false;
				m_TargetAimatorName = "InCar.zaiju_jiashi01";
			}
			if (Player.FSMUpBody.CurrentState.ID == StateID.UseItem)
			{
				m_NeedHandIk = false;
			}
		}
		else
		{
			m_NeedHandIk = false;
			if (!Player.Tanshen)
			{
				if (m_VehicleType == 0)
				{
					m_TargetAimatorName = "InCar.zaiju_chengzuo01";
				}
				else if (m_VehicleType == 2)
				{
					m_TargetAimatorName = "InCar.zaiju_chengzuo02";
				}
				else if (m_VehicleType == 1)
				{
					m_TargetAimatorName = "InCar.zaiju_chengzuo01";
				}
				else if (m_VehicleType == 3)
				{
					m_TargetAimatorName = "InCar.zaiju_motuo_chengzuo01";
				}
			}
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}

	private void BackSeat()
	{
		Player.transform.localPosition = Vector3.zero;
		Player.Tanshen = false;
		m_SyncTanshen.isTanshen = false;
		Client2Gs.Ins.Send(m_SyncTanshen);
		Player.NearCar.BackToSeat(Player.gameObject);
		if (Player.CurGun != null)
		{
			Player.CurGun.CloseJiMiao();
		}
	}

	private void UpdateCameraState()
	{
		if (!Player.Tanshen)
		{
			if (m_VehicleType == 2)
			{
				Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DrivePlane");
			}
			else if (m_VehicleType == 3)
			{
				Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DriveMoto");
			}
			else
			{
				Battle.Ins.MainCamera.SetMainTarget(Player.NearCar.transform, "DriveCar");
			}
		}
	}

	private void DoTanshen()
	{
		if (!Player.HaveGun)
		{
			return;
		}
		if (!Player.HaveGun || !Player.HaveGunInHand)
		{
		}
		Player.Tanshen = true;
		m_SeatIndex = Player.NearCar.GetSeatIndex(Player.gameObject);
		if (m_VehicleType == 0)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.transform, "TanshenCar");
			if (m_SeatIndex == 1 || m_SeatIndex == 3)
			{
				m_TargetAimatorName = "InCar.tanshenright";
			}
			else
			{
				m_TargetAimatorName = "InCar.tanshen";
			}
			m_SyncTanshen.isTanshen = true;
			Client2Gs.Ins.Send(m_SyncTanshen);
			Player.NearCar.MoveToTanshenSeat(Player.gameObject);
		}
		else if (m_VehicleType == 2)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.transform, "TanshenPlane");
			m_TargetAimatorName = "InCar.tanshenship";
		}
		else if (m_VehicleType == 1)
		{
			Battle.Ins.MainCamera.SetMainTarget(Player.transform, "TanshenCar");
			m_TargetAimatorName = "InCar.tanshenship";
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}
}

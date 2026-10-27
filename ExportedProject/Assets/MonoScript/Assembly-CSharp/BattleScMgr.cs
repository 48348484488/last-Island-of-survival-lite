using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using SC.LargeScene;
using SC.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using gs.battle.scmsg;

public class BattleScMgr : Singleton<BattleScMgr>
{
	public bool NeedNewPlayerboot;

	public static int PlayerLayer;

	public static int SelfPlayerLayer;

	public static int SelfPlayerColliderLayer;

	public static int OtherPlayerColliderLayer;

	public static int CarForBulletLayer;

	public static int StopBulletLayer;

	public static int CarLayer;

	public static int CarTriggerLayer;

	public static int DoorLayer;

	public static int WindowLayer;

	public static int DefaultLayer;

	public static int OutlineLayer;

	public static int WaterLayer;

	public static int BulletLayer;

	public static int GunLayer;

	public static long RoomId;

	public static long LastExitBattleRoomId;

	public static int IgnoreRaycastLayer;

	public static int MonsterLayer;

	public static int SceneLayerMask = LayerMask.GetMask("Default", "Door", "Car", "CarForBullet");

	public long UnbuiltNum;

	private CBuildChildPart m_cBuildChildPartMsg = new CBuildChildPart();

	private CBuildRootPart m_cBuildRootPartMsg = new CBuildRootPart();

	private CDestoryBuilding m_cDestoryBuildingMsg = new CDestoryBuilding();

	private CBuildJiaju m_buildJiaJvMsg = new CBuildJiaju();

	private SBattleLoginFinish m_SEnterRoom;

	private bool m_IsFirstEnterRoom;

	public int CanFreePlayerNum = 20;

	public int MaxPlayerNumInLand = 30;

	public void Init()
	{
		PlayerLayer = LayerMask.NameToLayer("Player");
		SelfPlayerLayer = LayerMask.NameToLayer("SelfPlayer");
		OtherPlayerColliderLayer = LayerMask.NameToLayer("OtherPlayerCollider");
		CarForBulletLayer = LayerMask.NameToLayer("CarForBullet");
		StopBulletLayer = LayerMask.NameToLayer("StopBullet");
		CarLayer = LayerMask.NameToLayer("Car");
		CarTriggerLayer = LayerMask.NameToLayer("CarTrigger");
		DoorLayer = LayerMask.NameToLayer("Door");
		WindowLayer = LayerMask.NameToLayer("Window");
		DefaultLayer = LayerMask.NameToLayer("Default");
		OutlineLayer = LayerMask.NameToLayer("Outline");
		WaterLayer = LayerMask.NameToLayer("Water");
		BulletLayer = LayerMask.NameToLayer("Bullet");
		SelfPlayerColliderLayer = LayerMask.NameToLayer("Enemy");
		GunLayer = LayerMask.NameToLayer("Gun");
		IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
		MonsterLayer = LayerMask.NameToLayer("Monster");
		SceneManager.sceneLoaded += SceneLoadCompleteCallBack;
		SQuitBattle.handler = SQuitBattleHandel;
		SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Combine(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(OnSBattleLoginFinish));
		SWitchBagGun.handler = (SWitchBagGun.Handler)Delegate.Combine(SWitchBagGun.handler, new SWitchBagGun.Handler(OnSwichBagGun));
		SWitchHandWeapon.handler = (SWitchHandWeapon.Handler)Delegate.Combine(SWitchHandWeapon.handler, new SWitchHandWeapon.Handler(OnSwichHandWeapon));
		SBuildPart.handler = (SBuildPart.Handler)Delegate.Combine(SBuildPart.handler, new SBuildPart.Handler(OnSBuildRootPart));
		SEnterBuilding.handler = (SEnterBuilding.Handler)Delegate.Combine(SEnterBuilding.handler, new SEnterBuilding.Handler(OnSEnterBuildings));
		SLeaveBuilding.handler = (SLeaveBuilding.Handler)Delegate.Combine(SLeaveBuilding.handler, new SLeaveBuilding.Handler(OnSLeaveBuilding));
		SDestoryBuilding.handler = (SDestoryBuilding.Handler)Delegate.Combine(SDestoryBuilding.handler, new SDestoryBuilding.Handler(OnDestoryBuilding));
		SReBuildJiaju.handler = (SReBuildJiaju.Handler)Delegate.Combine(SReBuildJiaju.handler, new SReBuildJiaju.Handler(OnSReBuildJiajv));
		EventHandlers.OnWantPlacedPart += OnWantPlacedPart;
		EventHandlers.OnWantDestoryBuilding += OnWantDestoryBuilding;
		SetIgnorLayer();
	}

	private void OnSwichHandWeapon(SWitchHandWeapon msg)
	{
		if (msg.roleId != Singleton<RoleMgr>.Ins.info.roleId)
		{
			BasePlayerController player = Battle.Ins.GetPlayer(msg.roleId);
			if (player != null)
			{
				player.DestoryAllWeapon();
				if (msg.weaponId > 0)
				{
					player.AddWeapon(msg.weaponId, new OtherNearWeapon(player, msg.weaponId, -1));
					player.ChangeHandWeapon(msg.weaponId);
				}
			}
		}
		else if (Battle.Ins.SelfPlayer.GetCurrentWeapon() != null)
		{
			if (Battle.Ins.SelfPlayer.CurrentWeaponInsId != msg.instanceId)
			{
				Battle.Ins.SelfPlayer.PlayChangeWeapenFromServer(msg.instanceId, false);
			}
		}
		else
		{
			Battle.Ins.SelfPlayer.PlayChangeWeapenFromServer(msg.instanceId, false);
		}
	}

	private void OnSwichBagGun(SWitchBagGun msg)
	{
		if (msg.roleId != Singleton<RoleMgr>.Ins.info.roleId)
		{
			BasePlayerController player = Battle.Ins.GetPlayer(msg.roleId);
			if (player != null)
			{
				player.DestoryAllWeapon();
				player.AddWeapon(msg.gun.gunId, new OtherGun(player, msg.gun.gunId, -1, msg.gun.accessory, msg.gun.bulletNumber));
				player.ChangeHandWeapon(msg.gun.gunId);
				BaseGun currentGun = player.GetCurrentGun();
				if (currentGun != null)
				{
					currentGun.CurBulletNum3Rd = msg.gun.bulletNumber;
					Battle.Ins.LoadOneRpgBulletFor3rd(currentGun);
				}
			}
		}
		else if (Battle.Ins.SelfPlayer.CurGun != null)
		{
			if (Battle.Ins.SelfPlayer.CurGun.InsId != msg.instanceId)
			{
				Battle.Ins.SelfPlayer.PlayChangeWeapenFromServer(msg.instanceId, false);
			}
		}
		else
		{
			Battle.Ins.SelfPlayer.PlayChangeWeapenFromServer(msg.instanceId, false);
		}
	}

	private void OnDestoryBuilding(SDestoryBuilding msg)
	{
		SingletonMono<BuilderBehaviour>.Ins.RemovePrefabFromMsg(msg.instanceId);
	}

	private void OnSLeaveBuilding(SLeaveBuilding msg)
	{
		PartBehaviour value = null;
		if (SingletonMono<BuildManager>.Ins.PartDic.TryGetValue(msg.instanceId, out value))
		{
			UnityEngine.Object.Destroy(value.gameObject);
			SingletonMono<BuildManager>.Ins.PartDic.Remove(msg.instanceId);
		}
		if (EventHandlers.OnBuildPartDestory != null)
		{
			EventHandlers.OnBuildPartDestory(msg.instanceId);
		}
	}

	private void OnSEnterBuildings(SEnterBuilding msg)
	{
		BuildPartInfo buildPartInfo = msg.buildPartInfo;
		UnbuiltNum++;
		OnSBuildPartInfo(buildPartInfo);
	}

	private void OnSBuildRootPart(SBuildPart msg)
	{
		OnSBuildPartInfo(msg.buildPartInfo);
	}

	private void OnSReBuildJiajv(SReBuildJiaju msg)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.insId);
		if (partByInsID != null)
		{
			partByInsID.transform.position = new Vector3(msg.posX, msg.posY, msg.posZ);
			partByInsID.transform.SetEulerAnglesY(msg.eulerY);
			partByInsID.gameObject.SetActiveBetter(true);
		}
		if (SingletonMono<BuilderBehaviour>.Ins.CurrentPreview != null)
		{
			SingletonMono<BuilderBehaviour>.Ins.SelectedPrefab = null;
			UnityEngine.Object.Destroy(SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.gameObject);
		}
	}

	private void OnSBuildPartInfo(BuildPartInfo buildPartInfo)
	{
		SingletonMono<BuilderBehaviour>.Ins.PlaceRootPrefabFromMsg(buildPartInfo);
	}

	private void OnWantPlacedPart(PartBehaviour part, SocketBehaviour socket)
	{
		if (part.FreeMove)
		{
			SendBuildJiajvMsg(part);
		}
		else if (socket != null)
		{
			SendBuildChildPartMsg(part.Id, socket.AttachedPart.InsId, socket.Index);
		}
		else
		{
			SendBuildRootPartMsg(part.Id, part.transform.position, part.transform.eulerAngles);
		}
	}

	private void OnWantDestoryBuilding(long insId)
	{
		SendDestoryBuildingMsg(insId);
	}

	private void SetIgnorLayer()
	{
		Physics.IgnoreLayerCollision(BulletLayer, BulletLayer, true);
		Physics.IgnoreLayerCollision(BulletLayer, GunLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, DefaultLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, CarLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, BulletLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, SelfPlayerColliderLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, SelfPlayerLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, GunLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, WindowLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, PlayerLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, DoorLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerLayer, OtherPlayerColliderLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, SelfPlayerColliderLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, PlayerLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, GunLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, BulletLayer, true);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, CarLayer, false);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, OtherPlayerColliderLayer, false);
		Physics.IgnoreLayerCollision(SelfPlayerColliderLayer, WindowLayer, false);
		Physics.IgnoreLayerCollision(MonsterLayer, GunLayer, true);
		Physics.IgnoreLayerCollision(PlayerLayer, PlayerLayer, true);
		Physics.IgnoreLayerCollision(PlayerLayer, DoorLayer, true);
		Physics.IgnoreLayerCollision(PlayerLayer, DefaultLayer, true);
		Physics.IgnoreLayerCollision(OtherPlayerColliderLayer, DoorLayer, true);
		Physics.IgnoreLayerCollision(OtherPlayerColliderLayer, CarLayer, true);
		Physics.IgnoreLayerCollision(OtherPlayerColliderLayer, DefaultLayer, false);
		Physics.IgnoreLayerCollision(GunLayer, LayerMask.NameToLayer("Grass"), true);
		Physics.IgnoreLayerCollision(CarLayer, WindowLayer, true);
		Physics.IgnoreLayerCollision(CarLayer, DoorLayer, true);
		Physics.IgnoreLayerCollision(CarLayer, CarForBulletLayer, true);
		Physics.IgnoreLayerCollision(IgnoreRaycastLayer, CarLayer, false);
		Physics.IgnoreLayerCollision(IgnoreRaycastLayer, SelfPlayerColliderLayer, false);
		Physics.IgnoreLayerCollision(IgnoreRaycastLayer, GunLayer, false);
	}

	private void SQuitBattleHandel(SQuitBattle msg)
	{
		TimeManager.UnregisterCountDown("QuitBattle");
		ExitGame();
	}

	private void OnSBattleLoginFinish(SBattleLoginFinish msg)
	{
		if (Utils.IsLowEndProduct())
		{
			Time.fixedDeltaTime = 0.0333f;
			CanFreePlayerNum = 5;
			MaxPlayerNumInLand = 10;
		}
		else
		{
			Time.fixedDeltaTime = 0.0166666f;
			CanFreePlayerNum = 10;
			MaxPlayerNumInLand = 20;
		}
		ViewMgr.Ins.Reset();
		ViewMgr.Ins.ShowTopView<BattleLoadingPanel>();
		m_SEnterRoom = msg;
		LargeSceneManager.battlesceneAb = SmallSceneInfo.LoadSceneSync("battle", LoadSceneMode.Single);
	}

	private void SceneLoadCompleteCallBack(Scene s, LoadSceneMode arg1)
	{
		if (s.name == "battle")
		{
			GameObject.Find("Base").AddComponent<Battle>();
			if (BattleEvent.OnLoadBattleSceneFinish != null)
			{
				BattleEvent.OnLoadBattleSceneFinish(m_SEnterRoom);
			}
		}
	}

	public void SendLoadFinish()
	{
		CLoadingFinish cLoadingFinish = new CLoadingFinish();
		cLoadingFinish.roleId = Singleton<RoleMgr>.Ins.info.roleId;
		cLoadingFinish.pos.x = Battle.Ins.SelfInfo.pos.x;
		cLoadingFinish.pos.y = Battle.Ins.SelfInfo.pos.y;
		cLoadingFinish.pos.z = Battle.Ins.SelfInfo.pos.z;
		if (m_IsFirstEnterRoom)
		{
			Vector3 birthRandomPos = Battle.Ins.GetBirthRandomPos();
			Battle.Ins.SelfPlayer.SetPosition(birthRandomPos);
			cLoadingFinish.pos.x = birthRandomPos.x;
			cLoadingFinish.pos.y = birthRandomPos.y;
			cLoadingFinish.pos.z = birthRandomPos.z;
		}
		Client2Gs.Ins.Send(cLoadingFinish);
		Battle.Ins.LoadFinish = true;
	}

	public void ReConnect()
	{
	}

	private void ExitGame()
	{
		LastExitBattleRoomId = RoomId;
		GameObject.Find("UICamera").GetComponent<AudioListener>().enabled = true;
	}

	public float GetAudioPercent(int id, float dis)
	{
		float result = 1f;
		switch (id)
		{
		case 1:
			result = ((!(dis > 100f)) ? (0.008f * dis * dis - 1.6f * dis + 100f) : (1111.1f / dis + 8.9f));
			break;
		case 2:
			result = 1000f / dis;
			break;
		}
		return result;
	}

	private void SendBuildChildPartMsg(int typeId, long InsId, byte index)
	{
		m_cBuildChildPartMsg.typeId = typeId;
		m_cBuildChildPartMsg.rotateY = SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.SelfRotateAngle;
		Dictionary<long, byte> inDic = SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.GetInDic();
		m_cBuildChildPartMsg.inmap = inDic;
		m_cBuildChildPartMsg.inmap[InsId] = index;
		m_cBuildChildPartMsg.parentInsId = InsId;
		MapList<long, byte> outDic = SingletonMono<BuilderBehaviour>.Ins.CurrentPreview.GetOutDic();
		m_cBuildChildPartMsg.outmap = outDic.AsDictionary();
		Client2Gs.Ins.Send(m_cBuildChildPartMsg);
	}

	private void SendBuildRootPartMsg(int typeId, Vector3 pos, Vector3 orientation)
	{
		m_cBuildRootPartMsg.typeId = typeId;
		m_cBuildRootPartMsg.pos.x = pos.x;
		m_cBuildRootPartMsg.pos.y = pos.y;
		m_cBuildRootPartMsg.pos.z = pos.z;
		m_cBuildRootPartMsg.eulerY = orientation.y;
		Client2Gs.Ins.Send(m_cBuildRootPartMsg);
	}

	private void SendBuildJiajvMsg(PartBehaviour part)
	{
		if (part.ProtoInsid == -1)
		{
			if (part.ParentInfo == null)
			{
				m_buildJiaJvMsg.parentInsId = -1L;
			}
			else
			{
				m_buildJiaJvMsg.parentInsId = part.ParentInfo.InsId;
			}
			m_buildJiaJvMsg.typeId = part.Id;
			m_buildJiaJvMsg.posX = part.transform.position.x;
			m_buildJiaJvMsg.posY = part.transform.position.y;
			m_buildJiaJvMsg.posZ = part.transform.position.z;
			m_buildJiaJvMsg.eulerY = part.transform.eulerAngles.y;
			Client2Gs.Ins.Send(m_buildJiaJvMsg);
			return;
		}
		CReBuildJiaju cReBuildJiaju = new CReBuildJiaju();
		if (part.ParentInfo == null)
		{
			cReBuildJiaju.parentInsId = -1L;
		}
		else
		{
			cReBuildJiaju.parentInsId = part.ParentInfo.InsId;
		}
		cReBuildJiaju.insId = part.ProtoInsid;
		cReBuildJiaju.posX = part.transform.position.x;
		cReBuildJiaju.posY = part.transform.position.y;
		cReBuildJiaju.posZ = part.transform.position.z;
		cReBuildJiaju.eulerY = part.transform.eulerAngles.y;
		Client2Gs.Ins.Send(cReBuildJiaju);
	}

	private void SendDestoryBuildingMsg(long insId)
	{
		m_cDestoryBuildingMsg.instanceId = insId;
		Client2Gs.Ins.Send(m_cDestoryBuildingMsg);
	}

	public void AddForceDie(Vector3 forward, byte addbodyPart, GameObject hip)
	{
		RollInfo component = hip.GetComponent<RollInfo>();
		Vector3 normalized = forward.normalized;
		switch (addbodyPart)
		{
		case 9:
			component.head.AddForce(normalized * 2f, ForceMode.Impulse);
			break;
		case 0:
			component.arm.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 1:
			component.arm.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 2:
			component.arm.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 3:
			component.arm.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 4:
			component.leg.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 5:
			component.leg.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 6:
			component.leg.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 7:
			component.leg.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		case 8:
			component.body.AddForce(normalized * 5f, ForceMode.Impulse);
			break;
		default:
			component.body.AddForce(Vector3.up, ForceMode.Impulse);
			break;
		}
	}
}

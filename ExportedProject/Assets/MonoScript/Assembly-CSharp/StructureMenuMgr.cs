using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using cfg;
using gs.battle.scmsg;

public class StructureMenuMgr : Singleton<StructureMenuMgr>
{
	public const int UseStructure = 1;

	public const int OpenDoor = 100;

	public const int CloseDoor = 101;

	public const int ChangeDoorPassword = 102;

	public const int InputDoorPassword = 103;

	public const int AddLockToDoor = 104;

	public const int RemoveLock = 105;

	public const int OpenSteelTrap = 109;

	public const int Rename = 111;

	public const int OpenSwitch = 112;

	public const int CloseSwitch = 113;

	private readonly HashSet<int> _canUseTypes = new HashSet<int>();

	private readonly Dictionary<int, Func<long, bool>> _dicItemType2PermitFunc = new Dictionary<int, Func<long, bool>>();

	private long _aimedInstanceId;

	private int _aimedItemId;

	private readonly CChangeBuildingName _cChangeBuildingName = new CChangeBuildingName();

	public void Init()
	{
		_canUseTypes.Add(97);
		_canUseTypes.Add(98);
		_canUseTypes.Add(106);
		_canUseTypes.Add(105);
		_canUseTypes.Add(104);
		_canUseTypes.Add(101);
		_canUseTypes.Add(100);
		_canUseTypes.Add(99);
		_canUseTypes.Add(110);
		_canUseTypes.Add(109);
		_canUseTypes.Add(111);
		_canUseTypes.Add(120);
		EventHandlers.OnClickExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnClickExtraBtn, new Utils.IntDelegate(ClickBtnCallBack));
		AddOnBattlePanelShowEvent();
	}

	public void AddPermitFunc(int itemChildType, Func<long, bool> func)
	{
		_dicItemType2PermitFunc[itemChildType] = func;
	}

	private void AddOnBattlePanelShowEvent()
	{
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(DelayedAttachEvent);
	}

	private void DelayedAttachEvent()
	{
		EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimBuilding));
		BattleEvent.OnSelfDown = (Utils.BoolDelegate)Delegate.Combine(BattleEvent.OnSelfDown, new Utils.BoolDelegate(OnSelfDown));
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", DelayedAttachEvent);
	}

	private void OnSelfDown(bool isDown)
	{
		if (isDown)
		{
			EventHandlers.OnRemoveExtraBtn(1);
			EventHandlers.OnRemoveExtraBtn(111);
			EventHandlers.OnRemoveExtraBtn(112);
			EventHandlers.OnRemoveExtraBtn(113);
			ViewMgr.Ins.HideView<WorkbenchPanel>();
			ViewMgr.Ins.HideView<DrawPanel>();
			ViewMgr.Ins.HideView<FirePanel>();
			ViewMgr.Ins.HideView<BoxBagPanel>();
			ViewMgr.Ins.HideView<LockPanel>();
			ViewMgr.Ins.HideView<ToolboxPanel>();
			ViewMgr.Ins.HideView<WoodPanel>();
			ViewMgr.Ins.HideView<TurretPanel>();
			ViewMgr.Ins.HideView<TurretSplitPanel>();
			ViewMgr.Ins.HideView<TurretPermitPanel>();
			ViewMgr.Ins.HideView<NumberPanel>();
			ViewMgr.Ins.HideView<FurnacePanel>();
			ViewMgr.Ins.HideView<FacilityRenamePanel>();
			ViewMgr.Ins.HideView<DianluPanel>();
			ViewMgr.Ins.HideView<BagSplitPanel>();
		}
		else
		{
			OnAimBuilding(_aimedInstanceId);
		}
	}

	private void OnAimBuilding(long arg)
	{
		_aimedItemId = -1;
		_aimedInstanceId = arg;
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(arg);
		if ((bool)partByInsID && !Battle.Ins.SelfPlayer.IsDownWaitSave)
		{
			ItemCfg itemCfg = ItemCfg.Get(partByInsID.LV1Id);
			if (itemCfg != null)
			{
				if (EventHandlers.OnAddExtraBtn != null)
				{
					Func<long, bool> value;
					if (_canUseTypes.Contains(itemCfg.childType) && (!_dicItemType2PermitFunc.TryGetValue(itemCfg.childType, out value) || value(arg)))
					{
						EventHandlers.OnAddExtraBtn(1);
						_aimedItemId = itemCfg.id;
					}
					else if (EventHandlers.OnRemoveExtraBtn != null)
					{
						EventHandlers.OnRemoveExtraBtn(1);
					}
					BuildPart buildPart = BuildPart.Get(partByInsID.LV1Id);
					if (buildPart != null && buildPart.canRename && Singleton<FriendPermitMgr>.Ins.HavePermitByInstanceId(arg))
					{
						EventHandlers.OnAddExtraBtn(111);
					}
					else if (EventHandlers.OnRemoveExtraBtn != null)
					{
						EventHandlers.OnRemoveExtraBtn(111);
					}
					if (buildPart != null && buildPart.functionType == 14)
					{
						EventHandlers.OnAddExtraBtn((!Singleton<ElectricityMgr>.Ins.IsSwitchOn(arg)) ? 112 : 113);
					}
				}
			}
			else if (EventHandlers.OnRemoveExtraBtn != null)
			{
				EventHandlers.OnRemoveExtraBtn(1);
				EventHandlers.OnRemoveExtraBtn(111);
			}
		}
		else if (EventHandlers.OnRemoveExtraBtn != null)
		{
			EventHandlers.OnRemoveExtraBtn(1);
			EventHandlers.OnRemoveExtraBtn(111);
			EventHandlers.OnRemoveExtraBtn(112);
			EventHandlers.OnRemoveExtraBtn(113);
		}
	}

	private void ClickBtnCallBack(int cfgId)
	{
		switch (cfgId)
		{
		case 1:
			if (_aimedItemId > 0)
			{
				BattleEvent.OnClickUseBuild(_aimedItemId, _aimedInstanceId);
			}
			break;
		case 100:
			Singleton<DoorMgr>.Ins.OpenDoor();
			break;
		case 101:
			Singleton<DoorMgr>.Ins.CloseDoor();
			break;
		case 104:
			Singleton<LockMgr>.Ins.ShowAddLock();
			break;
		case 102:
			Singleton<LockMgr>.Ins.ShowChangePassword();
			break;
		case 103:
			Singleton<LockMgr>.Ins.ShowInputPassword();
			break;
		case 105:
			Singleton<LockMgr>.Ins.SendRemoveLock();
			break;
		case 109:
			Singleton<SteelTrapMgr>.Ins.SendRoleOpenSteelTrap(_aimedInstanceId);
			break;
		case 111:
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(_aimedInstanceId);
			string oldName = ((!partByInsID) ? partByInsID.MapObjectName : string.Empty);
			FacilityRenamePanel.Show(SendChangeBuildingNameMsg, oldName, _aimedInstanceId);
			break;
		}
		case 112:
			Singleton<ElectricityMgr>.Ins.SendSwitchOnOffMsg(_aimedInstanceId, true);
			break;
		case 113:
			Singleton<ElectricityMgr>.Ins.SendSwitchOnOffMsg(_aimedInstanceId, false);
			break;
		}
	}

	public void SendChangeBuildingNameMsg(long insId, string name)
	{
		if (insId > 0)
		{
			_cChangeBuildingName.insId = insId;
			_cChangeBuildingName.name = name;
			Client2Gs.Ins.Send(_cChangeBuildingName);
		}
	}
}

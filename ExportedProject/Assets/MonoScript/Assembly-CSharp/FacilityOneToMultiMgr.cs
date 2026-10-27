using System;
using System.Collections.Generic;
using SC.UI;
using cfg;
using gs.bag.scmsg;
using gs.facilitybuilding.scmsg;
using gs.smelter.scmsg;

public class FacilityOneToMultiMgr : Singleton<FacilityOneToMultiMgr>
{
	public BagItem JiaShaA;

	public Action AiJiaShaJiaSha;

	private readonly CFacilityBuildingInfo _cFacilityBuildingInfo = new CFacilityBuildingInfo();

	private readonly gs.facilitybuilding.scmsg.CAddRawMaterial _cAddRawMaterial = new gs.facilitybuilding.scmsg.CAddRawMaterial();

	private readonly gs.facilitybuilding.scmsg.CAddFuel _cAddFuel = new gs.facilitybuilding.scmsg.CAddFuel();

	private readonly gs.facilitybuilding.scmsg.CStart _cStart = new gs.facilitybuilding.scmsg.CStart();

	private readonly gs.facilitybuilding.scmsg.CStop _cStop = new gs.facilitybuilding.scmsg.CStop();

	private readonly gs.facilitybuilding.scmsg.CGetRawMaterial _cGetRawMaterial = new gs.facilitybuilding.scmsg.CGetRawMaterial();

	private readonly gs.facilitybuilding.scmsg.CGetFuel _cGetFuel = new gs.facilitybuilding.scmsg.CGetFuel();

	private readonly gs.facilitybuilding.scmsg.CGetFinished _cGetFinished = new gs.facilitybuilding.scmsg.CGetFinished();

	public void Init()
	{
		SFacilityBuildingInfo.handler = (SFacilityBuildingInfo.Handler)Delegate.Combine(SFacilityBuildingInfo.handler, new SFacilityBuildingInfo.Handler(OnSFacilityBuildingInfo));
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnClickUseBtn));
	}

	private void OnClickUseBtn(int buildPartCfgId, long insId)
	{
		ItemCfg itemCfg = ItemCfg.Get(buildPartCfgId);
		if (itemCfg != null && itemCfg.childType == 125)
		{
			SendFacilityBuildingInfoMsg(insId);
		}
	}

	private void OnSFacilityBuildingInfo(SFacilityBuildingInfo msg)
	{
		if (!ViewMgr.Ins.IsShow<FenjiejiPanel>())
		{
			GNSSCapacityCfg gNSSCapacityCfg = GNSSCapacityCfg.Get(msg.cfgId);
			if (gNSSCapacityCfg != null && gNSSCapacityCfg.fuelCapacity <= 0)
			{
				ViewMgr.Ins.ShowView<FenjiejiPanel>(msg, false);
			}
		}
	}

	public void SendFacilityBuildingInfoMsg(long buildingId)
	{
		_cFacilityBuildingInfo.facilityBuildingId = buildingId;
		Client2Gs.Ins.Send(_cFacilityBuildingInfo);
	}

	public void SendAddRawMaterialMsg(long buildingId, Dictionary<int, UseItem> materials)
	{
		_cAddRawMaterial.facilityBuildingId = buildingId;
		_cAddRawMaterial.rawMaterials = materials;
		Client2Gs.Ins.Send(_cAddRawMaterial);
	}

	public void SendAddFuelMsg(long buildingId, Dictionary<int, UseItem> fuels)
	{
		_cAddFuel.facilityBuildingId = buildingId;
		_cAddFuel.fuels = fuels;
		Client2Gs.Ins.Send(_cAddFuel);
	}

	public void SendStartMsg(long buildingId)
	{
		_cStart.facilityBuildingId = buildingId;
		Client2Gs.Ins.Send(_cStart);
	}

	public void SendStopMsg(long buildingId)
	{
		_cStop.facilityBuildingId = buildingId;
		Client2Gs.Ins.Send(_cStop);
	}

	public void SendGetRawMaterialMsg(long buildingId, int index, bool isAllGet = false)
	{
		_cGetRawMaterial.facilityBuildingId = buildingId;
		_cGetRawMaterial.getIndex = index;
		_cGetRawMaterial.isAll = isAllGet;
		Client2Gs.Ins.Send(_cGetRawMaterial);
	}

	public void SendGetFuelMsg(long buildingId, int index, bool isAllGet = false)
	{
		_cGetFuel.facilityBuildingId = buildingId;
		_cGetFuel.getIndex = index;
		_cGetFuel.isAll = isAllGet;
		Client2Gs.Ins.Send(_cGetFuel);
	}

	public void SendGetFinishedMsg(long buildingId, int index, bool isAllGet = false)
	{
		_cGetFinished.facilityBuildingId = buildingId;
		_cGetFinished.getIndex = index;
		_cGetFinished.isAll = isAllGet;
		Client2Gs.Ins.Send(_cGetFinished);
	}
}

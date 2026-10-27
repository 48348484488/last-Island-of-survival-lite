using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using Share;
using cfg;
using gs.battle.scmsg;

public class FacilityStatusMgr : Singleton<FacilityStatusMgr>
{
	private readonly Dictionary<long, FacilityStatusEffectData> _dicInsId2Effects = new Dictionary<long, FacilityStatusEffectData>();

	public void Init()
	{
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuild));
		EventHandlers.OnDestroyedPart += OnDestroy;
	}

	private void OnDestroy(PartBehaviour part)
	{
		_dicInsId2Effects.Remove(part.InsId);
	}

	private void OnBuild(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
		if ((bool)partByInsID)
		{
			FacilityStatusEffectData component = partByInsID.GetComponent<FacilityStatusEffectData>();
			if ((bool)component)
			{
				_dicInsId2Effects.Add(insId, component);
				component.StatusChange(status, partByInsID.Lv - 1);
			}
		}
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		FacilityStatusEffectData value;
		if (_dicInsId2Effects.TryGetValue(msg.id, out value))
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.id);
			if ((bool)partByInsID)
			{
				value.StatusChange(msg.status, partByInsID.Lv - 1);
			}
		}
	}
}

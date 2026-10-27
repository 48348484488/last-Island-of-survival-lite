using System;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;

public class TrapMgr : Singleton<TrapMgr>
{
	public class MonitorDelegates
	{
		public Action<long, GameObject> AddMonitorAct;

		public Action<long> RemoveMonitorAct;

		public Func<long, bool> CanAddMonitorFunc;
	}

	private const float DistanceToCheck = 200f;

	private readonly WaitForSeconds _wait10 = new WaitForSeconds(10f);

	private readonly Dictionary<long, KeyValuePair<GameObject, int>> _dicInstanceId2Go = new Dictionary<long, KeyValuePair<GameObject, int>>();

	private readonly Dictionary<int, MonitorDelegates> _dicFunctionType2MonitorActs = new Dictionary<int, MonitorDelegates>();

	private readonly HashSet<long> _setMonitoredIds = new HashSet<long>();

	private readonly HashSet<int> _setTrapFunctionTypes = new HashSet<int>();

	private Coroutine _checkForStartMonitor;

	public void Init()
	{
		_setTrapFunctionTypes.Add(7);
		_setTrapFunctionTypes.Add(9);
		_setTrapFunctionTypes.Add(15);
		_setTrapFunctionTypes.Add(17);
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		EventHandlers.OnDestroyedPart += OnBuildDestroy;
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(StartBattle);
	}

	public void AddMonitorAct(int functionType, MonitorDelegates acts)
	{
		_dicFunctionType2MonitorActs[functionType] = acts;
	}

	private void StartBattle()
	{
		_checkForStartMonitor = Battle.StartConroutine(CheckForStartMonitor());
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", StartBattle);
	}

	private IEnumerator CheckForStartMonitor()
	{
		while (Battle.Ins.SelfPlayer != null)
		{
			Vector3 pos = Battle.Ins.SelfPlayer.Pos;
			foreach (KeyValuePair<long, KeyValuePair<GameObject, int>> item in _dicInstanceId2Go)
			{
				MonitorDelegates value;
				if (_dicFunctionType2MonitorActs.TryGetValue(item.Value.Value, out value) && !_setMonitoredIds.Contains(item.Key) && (value.CanAddMonitorFunc == null || value.CanAddMonitorFunc(item.Key)))
				{
					Transform transform = item.Value.Key.transform;
					if (CalIfMonitor(pos, transform.position))
					{
						value.AddMonitorAct(item.Key, item.Value.Key);
						_setMonitoredIds.Add(item.Key);
					}
					else if (_setMonitoredIds.Contains(item.Key))
					{
						value.RemoveMonitorAct(item.Key);
						_setMonitoredIds.Remove(item.Key);
					}
				}
			}
			yield return _wait10;
		}
	}

	private bool CalIfMonitor(Vector3 selfPos, Vector3 targetPos)
	{
		float num = selfPos.x - targetPos.x;
		float num2 = selfPos.z - targetPos.z;
		return num < 200f && num > -200f && num2 < 200f && num2 > -200f;
	}

	private void OnBuildDestroy(PartBehaviour part)
	{
		long insId = part.InsId;
		_dicInstanceId2Go.Remove(insId);
		MonitorDelegates value;
		if (part.MyCfg != null && IsTrap(part.MyCfg.functionType) && _setMonitoredIds.Contains(insId) && _dicFunctionType2MonitorActs.TryGetValue(part.MyCfg.functionType, out value))
		{
			value.RemoveMonitorAct(insId);
			_setMonitoredIds.Remove(insId);
		}
	}

	private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		if (!IsTrap(buildPartCfg.functionType))
		{
			return;
		}
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
		if (partByInsID != null)
		{
			GameObject gameObject = partByInsID.gameObject;
			_dicInstanceId2Go[insId] = new KeyValuePair<GameObject, int>(gameObject, buildPartCfg.functionType);
			MonitorDelegates value;
			if (!_setMonitoredIds.Contains(insId) && CalIfMonitor(Battle.Ins.SelfPlayer.Pos, partByInsID.transform.position) && _dicFunctionType2MonitorActs.TryGetValue(buildPartCfg.functionType, out value) && (value.CanAddMonitorFunc == null || value.CanAddMonitorFunc(insId)))
			{
				value.AddMonitorAct(insId, gameObject);
				_setMonitoredIds.Add(insId);
			}
		}
	}

	private bool IsTrap(int functionTrap)
	{
		return _setTrapFunctionTypes.Contains(functionTrap);
	}
}

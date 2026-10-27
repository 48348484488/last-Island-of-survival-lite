using System.Collections;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using gs.battle.map.sunkens.scmsg;

public class SharpWoodMgr : Singleton<SharpWoodMgr>
{
	private readonly Vector3 _yOffset = Vector3.up * -0.5f;

	private readonly Collider[] _playerCols = new Collider[16];

	private readonly Vector3 _halfExtends = Vector3.one;

	private readonly Dictionary<long, KeyValuePair<HashSet<long>, Transform>> _dicInsId2TransIds = new Dictionary<long, KeyValuePair<HashSet<long>, Transform>>();

	private readonly HashSet<long> _setInRoleIds = new HashSet<long>();

	private Coroutine _checkForInOut;

	private readonly WaitForSeconds _wait01 = new WaitForSeconds(0.1f);

	private readonly CRolesInSunkens _cRolesInSunkens = new CRolesInSunkens();

	private readonly CRoleOutSunkens _cRoleOutSunkens = new CRoleOutSunkens();

	private long _selfRoleId;

	public void Init()
	{
		Singleton<TrapMgr>.Ins.AddMonitorAct(9, new TrapMgr.MonitorDelegates
		{
			AddMonitorAct = AddMonitor,
			RemoveMonitorAct = RemoveMonitor
		});
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(StartBattle);
	}

	private void AddMonitor(long instanceId, GameObject go)
	{
		_dicInsId2TransIds.Add(instanceId, new KeyValuePair<HashSet<long>, Transform>(new HashSet<long>(), go.transform));
	}

	private void RemoveMonitor(long instanceId)
	{
		_dicInsId2TransIds.Remove(instanceId);
	}

	private void StartBattle()
	{
		_selfRoleId = Singleton<RoleMgr>.Ins.info.roleId;
		_checkForInOut = Battle.StartConroutine(CheckForInOut());
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", StartBattle);
	}

	private IEnumerator CheckForInOut()
	{
		while (true)
		{
			yield return _wait01;
			foreach (KeyValuePair<long, KeyValuePair<HashSet<long>, Transform>> dicInsId2TransId in _dicInsId2TransIds)
			{
				_setInRoleIds.Clear();
				KeyValuePair<HashSet<long>, Transform> value;
				if (!_dicInsId2TransIds.TryGetValue(dicInsId2TransId.Key, out value) || !value.Value)
				{
					continue;
				}
				if (CubeCast(value.Value.position, value.Value.rotation) > 0)
				{
					HashSet<long> key = dicInsId2TransId.Value.Key;
					if (key.Contains(_selfRoleId) && !_setInRoleIds.Contains(_selfRoleId))
					{
						key.Remove(_selfRoleId);
						_cRoleOutSunkens.sunkensId = dicInsId2TransId.Key;
						Client2Gs.Ins.Send(_cRoleOutSunkens);
					}
					_cRolesInSunkens.inRoleIds.Clear();
					_cRolesInSunkens.sunkensId = dicInsId2TransId.Key;
					foreach (long setInRoleId in _setInRoleIds)
					{
						if (!key.Contains(setInRoleId))
						{
							key.Add(setInRoleId);
							_cRolesInSunkens.inRoleIds.Add(setInRoleId);
						}
					}
					if (_cRolesInSunkens.inRoleIds.Count > 0)
					{
						Client2Gs.Ins.Send(_cRolesInSunkens);
					}
				}
				else if (dicInsId2TransId.Value.Key.Contains(_selfRoleId))
				{
					dicInsId2TransId.Value.Key.Remove(_selfRoleId);
					_cRoleOutSunkens.sunkensId = dicInsId2TransId.Key;
					Client2Gs.Ins.Send(_cRoleOutSunkens);
				}
			}
		}
	}

	private int CubeCast(Vector3 center, Quaternion rotation)
	{
		int num = Physics.OverlapBoxNonAlloc(center + _yOffset, _halfExtends, _playerCols, rotation, Singleton<LandMineMgr>.Ins.PlayerLayerMask);
		for (int i = 0; i < num; i++)
		{
			BasePlayerController componentInParent = _playerCols[i].GetComponentInParent<BasePlayerController>();
			if ((bool)componentInParent && componentInParent.Pos.y >= center.y - 0.2f)
			{
				_setInRoleIds.Add(componentInParent.RoleId);
			}
		}
		return _setInRoleIds.Count;
	}
}

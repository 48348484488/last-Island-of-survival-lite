using System.Collections.Generic;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class BombMgr
{
	private static Collider[] _bombThings = new Collider[128];

	private static CBombDamage _cBombDamage = new CBombDamage();

	private static CTriggerBomb _cTriggerBomb = new CTriggerBomb();

	public static void TriggerExplosion(long instanceId, int cfgId)
	{
		ThrowCfg throwCfg = ThrowCfg.Get(cfgId);
		if (throwCfg == null)
		{
			Debug.LogError("[BombMgr.cs]Can't find cfgId : " + cfgId);
			return;
		}
		_cTriggerBomb.insId = instanceId;
		Client2Gs.Ins.Send(_cTriggerBomb);
	}

	public static void ExplosionDamage(float radius, Vector3 position, List<long> idList)
	{
		idList.Clear();
		int num = Physics.OverlapSphereNonAlloc(position, radius, _bombThings, -1, QueryTriggerInteraction.Ignore);
		if (num <= 0)
		{
			return;
		}
		HashSet<long> hashSet = new HashSet<long>();
		for (int i = 0; i < num; i++)
		{
			MapObject componentInParent = _bombThings[i].GetComponentInParent<MapObject>();
			if ((bool)componentInParent && CheckNoBlock(_bombThings[i].gameObject, position))
			{
				hashSet.Add(componentInParent.InsId);
			}
		}
		foreach (long item in hashSet)
		{
			idList.Add(item);
		}
	}

	public static void ExplosionDamage(long instanceId, int cfgId, Vector3 position)
	{
		ThrowCfg throwCfg = ThrowCfg.Get(cfgId);
		if (throwCfg == null)
		{
			Debug.LogError("[BombMgr.cs]Can't find cfgId : " + cfgId);
			return;
		}
		_cBombDamage.insId = instanceId;
		List<long> damagedList = _cBombDamage.damagedList;
		damagedList.Clear();
		int num = Physics.OverlapSphereNonAlloc(position, throwCfg.radius, _bombThings, -1, QueryTriggerInteraction.Ignore);
		if (num > 0)
		{
			HashSet<long> hashSet = new HashSet<long>();
			for (int i = 0; i < num; i++)
			{
				MapObject componentInParent = _bombThings[i].GetComponentInParent<MapObject>();
				if ((bool)componentInParent && componentInParent.InsId != instanceId && CheckNoBlock(_bombThings[i].gameObject, position))
				{
					hashSet.Add(componentInParent.InsId);
					Rigidbody component = componentInParent.GetComponent<Rigidbody>();
					if ((bool)component)
					{
						component.AddExplosionForce(400f, position, throwCfg.radius, 2f, ForceMode.VelocityChange);
					}
				}
			}
			foreach (long item in hashSet)
			{
				damagedList.Add(item);
			}
		}
		Client2Gs.Ins.Send(_cBombDamage);
	}

	public static bool CheckNoBlock(GameObject targetGo, Vector3 checkCenter)
	{
		Vector3 vector = checkCenter + Vector3.up;
		Vector3 position = targetGo.transform.position;
		Vector3 direction = position - vector;
		Ray ray = new Ray(vector, direction);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, Vector3.Distance(vector, position), (1 << targetGo.layer) | (1 << BattleScMgr.DefaultLayer) | (1 << BattleScMgr.OutlineLayer), QueryTriggerInteraction.Ignore))
		{
			if (hitInfo.collider.gameObject == targetGo)
			{
				return true;
			}
			return false;
		}
		return true;
	}
}

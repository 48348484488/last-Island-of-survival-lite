using System;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetAction : TrusteeshipAction
{
	private const float SphereRadius = 50f;

	private const float SquareMaxDistance = 1000000f;

	private readonly Collider[] _treeCos = new Collider[16];

	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.FindTarget;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return Vector3.zero;
		}
	}

	protected override bool NeedTurnCamera
	{
		get
		{
			return false;
		}
	}

	protected override float WaitTurnFinishTime
	{
		get
		{
			return 0.05f;
		}
	}

	protected override void AttachEvent()
	{
	}

	protected override void DetachEvent()
	{
	}

	protected override void StartAction(Vector3 selfPos)
	{
		TargetTrans = null;
		SetTargetObject(selfPos);
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.GoOver);
	}

	private void SetTargetObject(Vector3 selfPos)
	{
		if (SingletonMono<TrusteeshipMgr>.Ins.BagHaveCapacity)
		{
			if (SingletonMono<TrusteeshipMgr>.Ins.GetSettingCollect(SettingMgr.TrusteeshipCollectType.Tree, -1))
			{
				int num = Physics.OverlapSphereNonAlloc(selfPos, 50f, _treeCos, SingletonMono<TrusteeshipMgr>.Ins.TreeLayerMask, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < num; i++)
				{
					Collider collider = _treeCos[i];
					if (collider.CompareTag("tree"))
					{
						Transform transform = collider.transform;
						if (!TargetTrans)
						{
							TargetTrans = transform;
						}
						else if (CalDistance(TargetTrans.position, selfPos) > CalDistance(transform.position, selfPos))
						{
							TargetTrans = transform;
						}
					}
				}
			}
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			Dictionary<long, Mine>.ValueCollection values = Battle.Ins.MineDic.Values;
			FindNearestObj(selfPos, values, SettingMgr.TrusteeshipCollectType.Mine, dictionary);
			dictionary.Clear();
			Dictionary<long, PlantInfo>.ValueCollection values2 = Battle.Ins.PlantDic.Values;
			FindNearestObj(selfPos, values2, SettingMgr.TrusteeshipCollectType.Plant, dictionary);
			dictionary.Clear();
			HashSet<long> trashcanIds = Singleton<BattleDropMgr>.Ins.TrashcanIds;
			HashSet<long> hashSet = new HashSet<long>();
			foreach (long item in trashcanIds)
			{
				GameObject obj;
				if (!Singleton<BattleDropMgr>.Ins.GetGameObject(item, out obj) || !obj)
				{
					hashSet.Add(item);
				}
				else
				{
					if (!(CalDistance(SingletonMono<TrusteeshipMgr>.Ins.StartPos, selfPos) < 1000000f))
					{
						continue;
					}
					int num2 = Math.Abs(Convert.ToInt32(obj.name));
					bool value;
					if (!dictionary.TryGetValue(num2, out value))
					{
						value = (dictionary[num2] = SingletonMono<TrusteeshipMgr>.Ins.GetSettingCollect(SettingMgr.TrusteeshipCollectType.Trashcan, num2));
					}
					if (value)
					{
						Transform transform2 = obj.transform;
						if (!TargetTrans)
						{
							TargetTrans = transform2;
						}
						else if (CalDistance(TargetTrans.position, selfPos) > CalDistance(transform2.position, selfPos))
						{
							TargetTrans = transform2;
						}
					}
				}
			}
			foreach (long item2 in hashSet)
			{
				trashcanIds.Remove(item2);
			}
			if (!TargetTrans)
			{
				MessageBoxPanel.ShowConfirm(368);
				SingletonMono<TrusteeshipMgr>.Ins.StopTrusteeship();
			}
		}
		else
		{
			MessageBoxPanel.ShowConfirm(384);
			SingletonMono<TrusteeshipMgr>.Ins.StopTrusteeship();
		}
	}

	private void FindNearestObj<TKey, TValue>(Vector3 selfPos, Dictionary<TKey, TValue>.ValueCollection objs, SettingMgr.TrusteeshipCollectType type, Dictionary<int, bool> selfCfgId2Collect) where TValue : MapObject
	{
		foreach (TValue obj in objs)
		{
			TValue current = obj;
			if (type == SettingMgr.TrusteeshipCollectType.Plant)
			{
				PlantInfo plantInfo = current as PlantInfo;
				if ((bool)plantInfo && plantInfo.GrowFinishTime > 0)
				{
					continue;
				}
			}
			Transform transform = current.transform;
			if (!transform.gameObject.activeSelf || !(CalDistance(SingletonMono<TrusteeshipMgr>.Ins.StartPos, selfPos) < 1000000f))
			{
				continue;
			}
			int cfgId = current.CfgId;
			bool value;
			if (!selfCfgId2Collect.TryGetValue(cfgId, out value))
			{
				value = (selfCfgId2Collect[cfgId] = SingletonMono<TrusteeshipMgr>.Ins.GetSettingCollect(type, cfgId));
			}
			if (value)
			{
				if (!TargetTrans)
				{
					TargetTrans = transform;
				}
				else if (CalDistance(TargetTrans.position, selfPos) > CalDistance(transform.position, selfPos))
				{
					TargetTrans = transform;
				}
			}
		}
	}

	private float CalDistance(Vector3 pos1, Vector3 pos2)
	{
		float num = pos1.x - pos2.x;
		float num2 = pos1.z - pos2.z;
		return num * num + num2 * num2;
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		return TargetTrans;
	}
}

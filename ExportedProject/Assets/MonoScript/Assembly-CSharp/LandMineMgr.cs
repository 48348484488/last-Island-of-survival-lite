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
using gs.battle.scmsg;

public class LandMineMgr : Singleton<LandMineMgr>
{
	public int PlayerLayerMask;

	private Coroutine _check;

	private Vector3 _yOffset = Vector3.zero;

	private readonly Dictionary<long, KeyValuePair<int, Vector3>> _dicInstanceId2ItemIdPos = new Dictionary<long, KeyValuePair<int, Vector3>>();

	private readonly Collider[] _playerCols = new Collider[8];

	public void Init()
	{
		PlayerLayerMask = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("OtherPlayerCollider"));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		SBombExlpode.handler = (SBombExlpode.Handler)Delegate.Combine(SBombExlpode.handler, new SBombExlpode.Handler(OnRealExplosion));
		EventHandlers.OnDestroyedPart += OnDestroy;
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(StartBattle);
	}

	private void OnDestroy(PartBehaviour part)
	{
		if (part.MyCfg != null && part.MyCfg.functionType == 6)
		{
			_dicInstanceId2ItemIdPos.Remove(part.InsId);
		}
	}

	private void OnRealExplosion(SBombExlpode msg)
	{
		KeyValuePair<int, Vector3> value;
		if (_dicInstanceId2ItemIdPos.TryGetValue(msg.insId, out value))
		{
			ThrowCfg throwCfg = ThrowCfg.Get(value.Key);
			SingletonMono<AudioManager>.Ins.Play(throwCfg.boomSound, value.Value);
			Battle.Ins.PlayEffectAtWorldPos(throwCfg.boomEffect, value.Value, Vector3.zero);
			_dicInstanceId2ItemIdPos.Remove(msg.insId);
			Battle.StartConroutine(WaitUploadDamage(msg.insId, value.Key, value.Value));
		}
	}

	private IEnumerator WaitUploadDamage(long instanceId, int cfgId, Vector3 position)
	{
		yield return Utils.WaitForSeconds(0.1f);
		BombMgr.ExplosionDamage(instanceId, cfgId, position);
	}

	private void StartBattle()
	{
		_check = Battle.StartConroutine(CheckForLandMine());
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", StartBattle);
	}

	private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		if (buildPartCfg.functionType != 6)
		{
			return;
		}
		GameObject partGoByInsID = SingletonMono<BuildManager>.Ins.GetPartGoByInsID(insId);
		if (!partGoByInsID)
		{
			return;
		}
		Animator componentInChildren = partGoByInsID.GetComponentInChildren<Animator>();
		if ((bool)componentInChildren)
		{
			componentInChildren.enabled = true;
		}
		_dicInstanceId2ItemIdPos[insId] = new KeyValuePair<int, Vector3>(buildPartCfg.id, partGoByInsID.transform.position);
		ItemCfg itemCfg = ItemCfg.Get(buildPartCfg.id);
		if (itemCfg != null)
		{
			if (itemCfg.childType == 121)
			{
				Battle.StartConroutine(DelayBomb(insId, buildPartCfg.id));
			}
			else if (_yOffset == Vector3.zero && itemCfg.extras.Count > 1)
			{
				_yOffset.Set(0f, 0f, itemCfg.extras[1]);
			}
		}
	}

	private WaitForSeconds GetDelayTime(int cfgId)
	{
		ThrowCfg throwCfg = ThrowCfg.Get(cfgId);
		if (throwCfg == null || throwCfg.delayTime <= 0f)
		{
			return Utils.WaitForSeconds(1f);
		}
		return Utils.WaitForSeconds(throwCfg.delayTime);
	}

	private IEnumerator DelayBomb(long insId, int cfgId)
	{
		yield return GetDelayTime(cfgId);
		BombMgr.TriggerExplosion(insId, cfgId);
	}

	private IEnumerator CheckForLandMine()
	{
		while (true)
		{
			yield return Utils.WaitForSeconds(0.1f);
			foreach (KeyValuePair<long, KeyValuePair<int, Vector3>> dicInstanceId2ItemIdPo in _dicInstanceId2ItemIdPos)
			{
				ItemCfg itemCfg = ItemCfg.Get(dicInstanceId2ItemIdPo.Value.Key);
				if (itemCfg != null && itemCfg.extras.Count > 1)
				{
					if (OverlapSphereNonAlloc(dicInstanceId2ItemIdPo.Value.Value + _yOffset, itemCfg.extras[0]))
					{
						BombMgr.TriggerExplosion(dicInstanceId2ItemIdPo.Key, dicInstanceId2ItemIdPo.Value.Key);
					}
				}
				else
				{
					Debug.LogError("Can't find ItemCfg with cfgId : " + dicInstanceId2ItemIdPo.Value.Key + ".Or the extras.Count smaller than 1.");
				}
			}
		}
	}

	private bool OverlapSphereNonAlloc(Vector3 position, float radius)
	{
		int num = Physics.OverlapSphereNonAlloc(position, radius, _playerCols, PlayerLayerMask);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = _playerCols[i].gameObject;
				if (BombMgr.CheckNoBlock(gameObject, position))
				{
					return true;
				}
			}
		}
		return false;
	}
}

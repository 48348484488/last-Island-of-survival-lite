using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.battle.monster.scmsg;

public class MonsterMgr : SingletonMono<MonsterMgr>
{
	[CompilerGenerated]
	private sealed class _003COnShowMonster_003Ec__AnonStorey1
	{
		internal SShowMonster msg;

		internal MonsterMgr _0024this;

		internal void _003C_003Em__0(GameObject go)
		{
			if (!_0024this.MonsterDic.ContainsKey(msg.monsterInfo.instanceId))
			{
				UnityEngine.Object.Destroy(go);
				return;
			}
			MonsterController monsterController = go.AddComponent<MonsterController>();
			_0024this.MonsterDic[msg.monsterInfo.instanceId] = monsterController;
			monsterController.Init(msg.monsterInfo);
			monsterController.HasOwner = msg.monsterInfo.hasOwner;
		}
	}

	public Dictionary<long, MonsterController> MonsterDic = new Dictionary<long, MonsterController>();

	private static readonly CControllMonster cControllMonster = new CControllMonster();

	private static readonly CUnControllMonster cUnControllMonster = new CUnControllMonster();

	private static readonly CSyncMonsterVelocity cSyncMonsterVeloctity = new CSyncMonsterVelocity();

	public override void Init()
	{
		SShowMonster.handler = (SShowMonster.Handler)Delegate.Combine(SShowMonster.handler, new SShowMonster.Handler(OnShowMonster));
		SMonsterDisappear.handler = (SMonsterDisappear.Handler)Delegate.Combine(SMonsterDisappear.handler, new SMonsterDisappear.Handler(OnDisapperaMonster));
		SControllMonster.handler = (SControllMonster.Handler)Delegate.Combine(SControllMonster.handler, new SControllMonster.Handler(OnSControllMonster));
		SUnControllMonster.handler = (SUnControllMonster.Handler)Delegate.Combine(SUnControllMonster.handler, new SUnControllMonster.Handler(OnSUnControllMonster));
		SSyncMonsterPos.handler = (SSyncMonsterPos.Handler)Delegate.Combine(SSyncMonsterPos.handler, new SSyncMonsterPos.Handler(OnSyncMonsterPos));
		SSyncMonsterOrientation.handler = (SSyncMonsterOrientation.Handler)Delegate.Combine(SSyncMonsterOrientation.handler, new SSyncMonsterOrientation.Handler(OnSyncMonsterOrientation));
		SSyncMonsterAnimator.handler = (SSyncMonsterAnimator.Handler)Delegate.Combine(SSyncMonsterAnimator.handler, new SSyncMonsterAnimator.Handler(OnSyncMonsterAnimator));
		SMonsterDie.handler = (SMonsterDie.Handler)Delegate.Combine(SMonsterDie.handler, new SMonsterDie.Handler(OnMonsterDie));
		SMonsterHpChange.handler = (SMonsterHpChange.Handler)Delegate.Combine(SMonsterHpChange.handler, new SMonsterHpChange.Handler(OnMonsterHpChange));
		Utils.StartConroutine(Tick());
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			foreach (MonsterController value in MonsterDic.Values)
			{
				if (value != null)
				{
					value.UpdateLod();
				}
			}
			yield return Utils.WaitForSeconds(1f);
		}
	}

	private void OnMonsterHpChange(SMonsterHpChange msg)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(msg.instanceId);
		if (monsterByInsId != null)
		{
			monsterByInsId.Hp = msg.hp;
			monsterByInsId.BeHit(msg.fromInsId);
		}
	}

	private void OnMonsterDie(SMonsterDie msg)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(msg.instanceId);
		if (monsterByInsId != null)
		{
			monsterByInsId.Hp = 0f;
			monsterByInsId.Die(msg);
		}
	}

	private void OnSyncMonsterAnimator(SSyncMonsterAnimator msg)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(msg.instanceId);
		if (monsterByInsId != null)
		{
			monsterByInsId.PlayerAnimation(msg.layer, msg.animationHash);
		}
	}

	private void OnSyncMonsterOrientation(SSyncMonsterOrientation msg)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(msg.instanceId);
		if (monsterByInsId != null)
		{
			monsterByInsId.SetTargetRotationFromMsg(msg.orientation);
		}
	}

	public MonsterController GetMonsterByInsId(long insId)
	{
		MonsterController value;
		if (MonsterDic.TryGetValue(insId, out value))
		{
			return value;
		}
		return null;
	}

	private void OnSyncMonsterPos(SSyncMonsterPos msg)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(msg.instanceId);
		if (monsterByInsId != null)
		{
			monsterByInsId.SetTargetPosFromMsg(msg.pos);
		}
	}

	private void OnSControllMonster(SControllMonster msg)
	{
		MonsterController value;
		if (MonsterDic.TryGetValue(msg.instanceId, out value) && (bool)value)
		{
			if (msg.roleId == Battle.Ins.SelfPlayer.RoleId)
			{
				value.StartController();
			}
			value.HasOwner = true;
		}
	}

	private void OnSUnControllMonster(SUnControllMonster msg)
	{
		MonsterController value;
		if (MonsterDic.TryGetValue(msg.instanceId, out value) && (bool)value)
		{
			if (value.IsMyControl)
			{
				value.StopController();
			}
			value.HasOwner = false;
			value.ChangeNoOwner();
		}
	}

	private void OnDisapperaMonster(SMonsterDisappear msg)
	{
		MonsterController value;
		if (!MonsterDic.TryGetValue(msg.instanceId, out value))
		{
			return;
		}
		if (value != null)
		{
			if (value.IsMyControl)
			{
				value.StopController();
				SendUnControlMonsterMsg(value.InsId);
			}
			UnityEngine.Object.Destroy(value.gameObject);
		}
		MonsterDic.Remove(msg.instanceId);
	}

	private void OnShowMonster(SShowMonster msg)
	{
		_003COnShowMonster_003Ec__AnonStorey1 _003COnShowMonster_003Ec__AnonStorey = new _003COnShowMonster_003Ec__AnonStorey1();
		_003COnShowMonster_003Ec__AnonStorey.msg = msg;
		_003COnShowMonster_003Ec__AnonStorey._0024this = this;
		if (MonsterDic.ContainsKey(_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.instanceId))
		{
			return;
		}
		MonsterDic.Add(_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.instanceId, null);
		MonsterCfg monsterCfg = MonsterCfg.Get(_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.id);
		if (monsterCfg.weaponId > 0 && Battle.Ins.PlayTest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("monsterrole")) as GameObject;
			if (!MonsterDic.ContainsKey(_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.instanceId))
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			MonsterController monsterController = gameObject.AddComponent<MonsterController>();
			MonsterDic[_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.instanceId] = monsterController;
			monsterController.Init(_003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo);
			monsterController.HasOwner = _003COnShowMonster_003Ec__AnonStorey.msg.monsterInfo.hasOwner;
		}
		else
		{
			string resName = null;
			string abPath = monsterCfg.modelPath + ".ab";
			if (monsterCfg.modelPath == "monsterrole")
			{
				resName = "monsterrole";
				abPath = "role/role.ab";
			}
			ResMgr.Ins.CreateFromAB(abPath, resName, _003COnShowMonster_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	public void SendControlMonsterMsg(long insId)
	{
		MonsterController monsterByInsId = GetMonsterByInsId(insId);
		if ((bool)monsterByInsId && monsterByInsId.IsOnGround())
		{
			cControllMonster.instanceId = insId;
			Client2Gs.Ins.Send(cControllMonster);
		}
	}

	public void SendUnControlMonsterMsg(long insId)
	{
		cUnControllMonster.instanceId = insId;
		Client2Gs.Ins.Send(cUnControllMonster);
	}
}

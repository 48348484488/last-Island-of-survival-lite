using System;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC;
using UnityEngine;
using cfg;
using gs.battle.monster.scmsg;
using gs.battle.scmsg;

public class HandAttack : MonoBehaviour
{
	private static readonly CCloseWeaponHitPlayer cSelfHitPlayerMsg = new CCloseWeaponHitPlayer();

	private static readonly CCloseWeaponHitNpc cSelfHitNpcMsg = new CCloseWeaponHitNpc();

	private static readonly COtherCloseWeaponHitPlayer cOtherHitPlayerMsg = new COtherCloseWeaponHitPlayer();

	private static readonly CCloseWeaponHitNoLiveObj cSelfHitNoLiveMsg = new CCloseWeaponHitNoLiveObj();

	private static readonly COtherCloseWeaponHitNoLiveObj cOtherHitNoLiveMsg = new COtherCloseWeaponHitNoLiveObj();

	public bool First;

	[NonSerialized]
	public int ItemId = -1;

	private ItemCfg m_ItemCfg;

	private int m_RandomAudio;

	private Rigidbody m_Rigidbody;

	public long InsId;

	private bool IsSelfWeapon
	{
		get
		{
			return InsId == Battle.Ins.SelfPlayer.InsId;
		}
	}

	public void OnTriggerEnter1()
	{
		if (ItemId != -1)
		{
			m_ItemCfg = ItemCfg.Get(ItemId);
		}
		RaycastHit aimCollider = Battle.Ins.GetAimCollider(Vector2.zero, 4.5f);
		if (aimCollider.collider == null)
		{
			PlayHitAudio(null);
			return;
		}
		First = true;
		Collider collider = aimCollider.collider;
		m_RandomAudio = Utils.Random(0, 2);
		if (collider.gameObject.layer == BattleScMgr.PlayerLayer)
		{
			PlayHitAudio(collider);
			OtherPlayerController componentInParent = collider.gameObject.GetComponentInParent<OtherPlayerController>();
			PartType component = collider.transform.GetComponent<PartType>();
			if (component != null)
			{
				First = false;
				if (IsSelfWeapon)
				{
					SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/dk_jianxue_xiao.ab", collider.transform.position, base.transform.forward);
					componentInParent.PlayAttackIk(base.transform.forward);
					cSelfHitPlayerMsg.roleId = componentInParent.Info.roleId;
					cSelfHitPlayerMsg.bodyPart = (byte)component.partType;
					cSelfHitPlayerMsg.forward.x = MathUtils.Float2Short(base.transform.forward.x);
					cSelfHitPlayerMsg.forward.y = MathUtils.Float2Short(base.transform.forward.y);
					cSelfHitPlayerMsg.forward.z = MathUtils.Float2Short(base.transform.forward.z);
					cSelfHitPlayerMsg.hitPos.x = 0;
					cSelfHitPlayerMsg.hitPos.y = 0;
					cSelfHitPlayerMsg.hitPos.z = 0;
					Client2Gs.Ins.Send(cSelfHitPlayerMsg);
					Singleton<BagMgr>.Ins.ReduceDurability();
				}
				else
				{
					cOtherHitPlayerMsg.roleId = componentInParent.Info.roleId;
					cOtherHitPlayerMsg.bodyPart = (byte)component.partType;
					cOtherHitPlayerMsg.forward.x = MathUtils.Float2Short(base.transform.forward.x);
					cOtherHitPlayerMsg.forward.y = MathUtils.Float2Short(base.transform.forward.y);
					cOtherHitPlayerMsg.forward.z = MathUtils.Float2Short(base.transform.forward.z);
					cOtherHitPlayerMsg.hitPos.x = 0;
					cOtherHitPlayerMsg.hitPos.y = 0;
					cOtherHitPlayerMsg.hitPos.z = 0;
					cOtherHitPlayerMsg.otherInsId = InsId;
					Client2Gs.Ins.Send(cOtherHitPlayerMsg);
				}
			}
			return;
		}
		if (collider.gameObject.layer == BattleScMgr.WindowLayer)
		{
			First = false;
			return;
		}
		if (collider.gameObject.layer == BattleScMgr.CarForBulletLayer)
		{
			First = false;
			if (ItemId != -1)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(19).path, aimCollider.point);
			}
			PlayHitAudio(collider);
			return;
		}
		MapObject componentInParent2 = collider.gameObject.GetComponentInParent<MapObject>();
		if (componentInParent2 != null)
		{
			if (componentInParent2 is Mine)
			{
				if (ItemId == -1 || (ItemId != -1 && Battle.Ins.CanHitMineWapId.Contains(ItemId)))
				{
					if (IsSelfWeapon)
					{
						CMine cMine = new CMine();
						cMine.instanceId = componentInParent2.InsId;
						Client2Gs.Ins.Send(cMine);
						Singleton<BagMgr>.Ins.ReduceDurability();
						Singleton<RoleMgr>.Ins.HungerChange(-ConstsBs.MINE_CONSUME_HUNGER);
					}
				}
				else
				{
					SendHitNoLiveMsg(componentInParent2);
				}
				First = false;
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/wakuang_01.ab", aimCollider.point);
				ShowDecal(aimCollider, Decal.DecalType.ShiTouhen);
				if (ItemId != -1)
				{
					Battle.Ins.SyncPlaySound(481);
				}
			}
			else if (componentInParent2 is TreeInfo)
			{
				if (ItemId == -1 || (ItemId != -1 && Battle.Ins.CanHitTreeWapId.Contains(ItemId)))
				{
					if (IsSelfWeapon)
					{
						CCutTree cCutTree = new CCutTree();
						cCutTree.treeId = (componentInParent2 as TreeInfo).Id;
						Client2Gs.Ins.Send(cCutTree);
						Singleton<RoleMgr>.Ins.HungerChange(-ConstsBs.CUT_TREE_CONSUME_HUNGER);
						Singleton<BagMgr>.Ins.ReduceDurability();
					}
				}
				else
				{
					SendHitNoLiveMsg(componentInParent2);
				}
				First = false;
				ShowDecal(aimCollider, Decal.DecalType.Shuhen);
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/kanshu_01.ab", aimCollider.point);
				if (ItemId != -1)
				{
					Battle.Ins.SyncPlaySound(480);
				}
			}
			else if (componentInParent2 is PartBehaviour)
			{
				First = false;
				SendHitNoLiveMsg(componentInParent2);
				PlayDefaultEffect(aimCollider);
			}
			else if (componentInParent2 is MonsterController)
			{
				PartType component2 = collider.gameObject.GetComponent<PartType>();
				if (component2 != null)
				{
					First = false;
					if (IsSelfWeapon)
					{
						SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/dk_jianxue_xiao.ab", base.transform.position, base.transform.forward);
						PlayHitAudio(collider);
						Vector3 vector = collider.transform.InverseTransformPoint(base.transform.position);
						cSelfHitNpcMsg.instanceId = componentInParent2.InsId;
						cSelfHitNpcMsg.bodyPart = (byte)component2.partType;
						cSelfHitNpcMsg.forward.x = MathUtils.Float2Short(base.transform.forward.x);
						cSelfHitNpcMsg.forward.y = MathUtils.Float2Short(base.transform.forward.y);
						cSelfHitNpcMsg.forward.z = MathUtils.Float2Short(base.transform.forward.z);
						cSelfHitNpcMsg.hitPos.x = 0;
						cSelfHitNpcMsg.hitPos.y = 0;
						cSelfHitNpcMsg.hitPos.z = 0;
						Client2Gs.Ins.Send(cSelfHitNpcMsg);
					}
					else
					{
						cOtherHitPlayerMsg.roleId = componentInParent2.InsId;
						cOtherHitPlayerMsg.bodyPart = (byte)component2.partType;
						cOtherHitPlayerMsg.forward.x = MathUtils.Float2Short(base.transform.forward.x);
						cOtherHitPlayerMsg.forward.y = MathUtils.Float2Short(base.transform.forward.y);
						cOtherHitPlayerMsg.forward.z = MathUtils.Float2Short(base.transform.forward.z);
						cOtherHitPlayerMsg.hitPos.x = 0;
						cOtherHitPlayerMsg.hitPos.y = 0;
						cOtherHitPlayerMsg.hitPos.z = 0;
						cOtherHitPlayerMsg.otherInsId = InsId;
						Client2Gs.Ins.Send(cOtherHitPlayerMsg);
					}
				}
			}
			else
			{
				PlayDefaultEffect(aimCollider);
				SendHitNoLiveMsg(componentInParent2);
			}
		}
		else
		{
			PlayDefaultEffect(aimCollider);
		}
	}

	private void PlayDefaultEffect(RaycastHit info, bool ignoreTrigger = true)
	{
		if (!info.collider.isTrigger || !ignoreTrigger)
		{
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(19).path, info.point);
			PlayHitAudio(info.collider);
		}
	}

	private void SendHitNoLiveMsg(MapObject mapObj)
	{
		if (IsSelfWeapon)
		{
			cSelfHitNoLiveMsg.insId = mapObj.InsId;
			Client2Gs.Ins.Send(cSelfHitNoLiveMsg);
			Singleton<BagMgr>.Ins.ReduceDurability();
		}
		else
		{
			cOtherHitNoLiveMsg.instanceId = mapObj.InsId;
			cOtherHitNoLiveMsg.otherInsId = InsId;
			Client2Gs.Ins.Send(cOtherHitNoLiveMsg);
		}
	}

	public void PlayHitAudio(Collider other)
	{
		if (other == null)
		{
			if (m_ItemCfg != null)
			{
				Battle.Ins.SyncPlaySound((m_RandomAudio != 0) ? ((int)m_ItemCfg.extras[6]) : ((int)m_ItemCfg.extras[5]));
			}
		}
		else if (other.gameObject.layer == BattleScMgr.PlayerLayer || other.gameObject.layer == BattleScMgr.MonsterLayer)
		{
			if (ItemId == -1)
			{
				Battle.Ins.SyncPlaySound((m_RandomAudio != 0) ? 164 : 163);
			}
			else if (m_ItemCfg != null)
			{
				Battle.Ins.SyncPlaySound((m_RandomAudio != 0) ? ((int)m_ItemCfg.extras[4]) : ((int)m_ItemCfg.extras[3]));
			}
		}
		else if (ItemId == -1)
		{
			Battle.Ins.SyncPlaySound((m_RandomAudio != 0) ? 172 : 171);
		}
		else if (m_ItemCfg != null)
		{
			Battle.Ins.SyncPlaySound((m_RandomAudio != 0) ? ((int)m_ItemCfg.extras[2]) : ((int)m_ItemCfg.extras[1]));
		}
	}

	private void ShowDecal(RaycastHit hit, Decal.DecalType decalType)
	{
		Decal decal = Battle.Ins.DecalPool.Get();
		Quaternion rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
		if (decal != null)
		{
			decal.SetDecal(decalType);
			decal.transform.SetParent(hit.collider.transform);
			decal.transform.position = hit.point;
			decal.transform.rotation = rotation;
			if (decal.transform.localScale != Vector3.one)
			{
				decal.transform.localScale = new Vector3(1f / decal.transform.parent.transform.localScale.x, 1f / decal.transform.parent.transform.localScale.y, 1f / decal.transform.parent.transform.localScale.z);
			}
			else
			{
				decal.transform.localScale = Vector3.one;
			}
			decal.gameObject.SetActive(true);
		}
	}
}

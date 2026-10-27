using System.Collections;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class MonsterAttackState : FSMState
{
	private static readonly COtherBulletHitPlayer cHitPlayerMsg = new COtherBulletHitPlayer();

	private static readonly COtherShoot cShoot = new COtherShoot();

	private static readonly COtherCloseWeaponHitPlayer cOtherCloseWeaponHitPlayer = new COtherCloseWeaponHitPlayer();

	public MonsterAttackState()
	{
		stateID = StateID.MonsterAttack;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Monster.ResetRevengeTime();
		Monster.SetAnimatorForwardValue(0f);
		if (Monster.MyGunCfg != null)
		{
			HumanGunMonsterAttack();
			return;
		}
		Debug.LogError("&&&&&&&&&&&&&");
		NormalMonsterAttack();
	}

	private void HumanGunMonsterAttack()
	{
		if (Monster.MyGunInfo != null)
		{
			SingletonMono<EffectMgr>.Ins.PlayEffect(EffectCfg.Get(Monster.MyGunCfg.effects[0]).path, Monster.MyGunInfo.Muzzle);
			m_TargetAimatorName = "Shoot." + Monster.MyGunCfg.shootZhan;
			Monster.ChangeAnimatorStates(m_TargetAimatorName, 1);
			Monster.PlaySound(Monster.MyGunCfg.audios[(!(Monster.DisToMySelf < 100f)) ? 1 : 0], Singleton<BattleScMgr>.Ins.GetAudioPercent(1, Monster.DisToMySelf));
			FireBullet();
		}
	}

	private void NormalMonsterAttack()
	{
		Battle.Ins.SyncPlaySound(Monster.MyCfg.attackSound);
		m_TargetAimatorName = "attack01";
		Monster.ChangeAnimatorStates(m_TargetAimatorName);
	}

	private void SendHitMsg()
	{
		if (Monster.Target != null && Monster.MyGunCfg == null)
		{
			BasePlayerController componentInParent = Monster.Target.GetComponentInParent<BasePlayerController>();
			if (componentInParent != null)
			{
				cOtherCloseWeaponHitPlayer.bodyPart = 8;
				cOtherCloseWeaponHitPlayer.roleId = componentInParent.RoleId;
				cOtherCloseWeaponHitPlayer.otherInsId = Monster.InsId;
				cOtherCloseWeaponHitPlayer.forward.x = MathUtils.Float2Short(Monster.transform.forward.x);
				cOtherCloseWeaponHitPlayer.forward.y = MathUtils.Float2Short(Monster.transform.forward.y);
				cOtherCloseWeaponHitPlayer.forward.z = MathUtils.Float2Short(Monster.transform.forward.z);
				cOtherCloseWeaponHitPlayer.hitPos.x = 0;
				cOtherCloseWeaponHitPlayer.hitPos.y = 0;
				cOtherCloseWeaponHitPlayer.hitPos.z = 0;
				Client2Gs.Ins.Send(cOtherCloseWeaponHitPlayer);
			}
		}
	}

	private void FireBullet()
	{
		BulletControl bulletControl = Battle.Ins.BulletPool.Get();
		Vector3 position = Monster.MyGunInfo.Muzzle.position;
		if (Monster.Target != null)
		{
			BasePlayerController componentInParent = Monster.Target.GetComponentInParent<BasePlayerController>();
			if ((bool)componentInParent)
			{
				Vector3 zero = Vector3.zero;
				zero = ((Random.Range(1, 10) >= 3) ? componentInParent.GetBodyPart(BasePlayerController.BodyPart.Body).position : componentInParent.HeadBub.position);
				Vector3 normalized = (zero - position).normalized;
				bulletControl.Fire(Monster.MyGunCfg.id, Monster.InsId, position, normalized, normalized * Monster.MyGunCfg.factors[37], 0x6E1C8801 | (1 << BattleScMgr.SelfPlayerLayer));
				bulletControl.BulletRigidbody.useGravity = false;
			}
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds((float)Monster.MyCfg.waitTime * 0.001f);
		SendHitMsg();
		yield return new WaitForSeconds(2.5f);
		if (Monster.MyGunCfg != null)
		{
			Monster.ChangeAnimatorStates("Aim." + Monster.MyGunCfg.aimZhan, 1);
		}
		Monster.FSM.SwitchState(StateID.MonsterDefence);
	}

	public override void Act()
	{
		Monster.SetTargetRotation(Quaternion.LookRotation(Monster.Target.transform.position - Monster.transform.position, Vector3.up));
		if (Monster.DisToTarget <= Monster.AttackRange)
		{
			Monster.Stop();
		}
	}

	public override void Reason()
	{
		if (IsTargetAimatorPlayFinish())
		{
			if (Monster.MyGunCfg != null)
			{
				Monster.ChangeAnimatorStates("Aim." + Monster.MyGunCfg.aimZhan, 1);
			}
			Monster.FSM.SwitchState(StateID.MonsterDefence);
		}
	}

	public override void DoBeforeLeaving()
	{
		Monster.NextAttackTime = Monster.MyCfg.attackSpaceTime * 0.001f;
	}
}

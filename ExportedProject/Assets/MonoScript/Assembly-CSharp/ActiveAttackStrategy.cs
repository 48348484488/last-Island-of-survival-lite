public class ActiveAttackStrategy : IEnterAttackStrategy
{
	public bool CanEnterAttackState(MonsterController Monster)
	{
		if (Monster.DisToTarget <= Monster.AttackRange && Monster.MyCfg.zhianDis <= 0 && Monster.NextAttackTime <= 0f && Monster.RevengeTime > 0f && Monster.Target != null && Monster.CanSeeTarget())
		{
			BasePlayerController componentInParent = Monster.Target.GetComponentInParent<BasePlayerController>();
			if ((bool)componentInParent && !componentInParent.IsDie)
			{
				return true;
			}
		}
		return false;
	}
}

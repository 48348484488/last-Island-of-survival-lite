public class AttackShooterStrategy : IEnterAttackStrategy
{
	public bool CanEnterAttackState(MonsterController Monster)
	{
		if (Monster.NeedAttackShooter && Monster.DisToTarget <= (float)Monster.MyCfg.zhianDis && Monster.NextAttackTime <= 0f && Monster.RevengeTime > 0f && Monster.Target != null && Monster.CanSeeTarget())
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

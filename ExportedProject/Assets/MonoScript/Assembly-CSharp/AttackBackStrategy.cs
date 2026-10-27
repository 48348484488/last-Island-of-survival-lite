public class AttackBackStrategy : IEnterAttackStrategy
{
	public bool CanEnterAttackState(MonsterController Monster)
	{
		if (Monster.NeedAttackBack && Monster.DisToTarget <= (float)Monster.MyCfg.backAttackDis && Monster.NextAttackTime <= 0f && Monster.RevengeTime > 0f && Monster.Target != null && Monster.CanSeeTarget())
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

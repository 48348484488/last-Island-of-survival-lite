using System.Collections.Generic;
using Share;

namespace gs.ladder.scmsg
{
	public class LadderInfo : Marshal
	{
		public const int NormalTask = 0;

		public const int SpecialTask = 1;

		public const int SuperTask = 2;

		public int level;

		public int exp;

		public int taskType;

		public HashSet<int> rewardedNormalTaskIds = new HashSet<int>();

		public HashSet<int> rewardedBuyedTaskIds = new HashSet<int>();

		public int timeToRefreshWeekTask;

		public bool isGetBox;

		public int weekFinishTaskCount;

		public Octets marshal(Octets oc)
		{
			oc.push(level);
			oc.push(exp);
			oc.push(taskType);
			oc.push(rewardedNormalTaskIds.Count);
			foreach (int rewardedNormalTaskId in rewardedNormalTaskIds)
			{
				oc.push(rewardedNormalTaskId);
			}
			oc.push(rewardedBuyedTaskIds.Count);
			foreach (int rewardedBuyedTaskId in rewardedBuyedTaskIds)
			{
				oc.push(rewardedBuyedTaskId);
			}
			oc.push(timeToRefreshWeekTask);
			oc.push(isGetBox);
			oc.push(weekFinishTaskCount);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			level = oc.pop_int();
			exp = oc.pop_int();
			taskType = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				rewardedNormalTaskIds.Add(oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				rewardedBuyedTaskIds.Add(oc.pop_int());
			}
			timeToRefreshWeekTask = oc.pop_int();
			isGetBox = oc.pop_bool();
			weekFinishTaskCount = oc.pop_int();
			return oc;
		}
	}
}

using System.Collections.Generic;
using Net;
using Share;

namespace gs.activity.scmsg
{
	public class SWeekSignInfo : Message
	{
		public delegate void Handler(SWeekSignInfo msg);

		public const int TYPE = 29363128;

		public static Handler handler;

		public HashSet<int> rewardedIndexes = new HashSet<int>();

		public int signinDays;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 29363128;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(rewardedIndexes.Count);
			foreach (int rewardedIndex in rewardedIndexes)
			{
				oc.push(rewardedIndex);
			}
			oc.push(signinDays);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				rewardedIndexes.Add(oc.pop_int());
			}
			signinDays = oc.pop_int();
			return oc;
		}
	}
}

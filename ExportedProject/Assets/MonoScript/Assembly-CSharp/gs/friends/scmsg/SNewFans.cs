using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SNewFans : Message
	{
		public delegate void Handler(SNewFans msg);

		public const int TYPE = 13634491;

		public static Handler handler;

		public HashSet<long> newRoleIds = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634491;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(newRoleIds.Count);
			foreach (long newRoleId in newRoleIds)
			{
				oc.push(newRoleId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				newRoleIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}

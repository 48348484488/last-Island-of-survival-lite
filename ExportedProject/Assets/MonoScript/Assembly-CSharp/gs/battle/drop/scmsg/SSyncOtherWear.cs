using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SSyncOtherWear : Message
	{
		public delegate void Handler(SSyncOtherWear msg);

		public const int TYPE = 12585926;

		public static Handler handler;

		public long otherId;

		public HashSet<int> wears = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585926;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(wears.Count);
			foreach (int wear in wears)
			{
				oc.push(wear);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				wears.Add(oc.pop_int());
			}
			return oc;
		}
	}
}

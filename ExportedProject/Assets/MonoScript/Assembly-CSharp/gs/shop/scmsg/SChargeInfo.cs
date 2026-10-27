using System.Collections.Generic;
using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SChargeInfo : Message
	{
		public delegate void Handler(SChargeInfo msg);

		public const int TYPE = 6294461;

		public static Handler handler;

		public HashSet<int> usedFirstIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294461;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(usedFirstIds.Count);
			foreach (int usedFirstId in usedFirstIds)
			{
				oc.push(usedFirstId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				usedFirstIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}

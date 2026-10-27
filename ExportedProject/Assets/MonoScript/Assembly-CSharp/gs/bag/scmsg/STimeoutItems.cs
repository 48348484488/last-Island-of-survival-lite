using System.Collections.Generic;
using Net;
using Share;

namespace gs.bag.scmsg
{
	public class STimeoutItems : Message
	{
		public delegate void Handler(STimeoutItems msg);

		public const int TYPE = 8391638;

		public static Handler handler;

		public HashSet<int> itemIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391638;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemIds.Count);
			foreach (int itemId in itemIds)
			{
				oc.push(itemId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				itemIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}

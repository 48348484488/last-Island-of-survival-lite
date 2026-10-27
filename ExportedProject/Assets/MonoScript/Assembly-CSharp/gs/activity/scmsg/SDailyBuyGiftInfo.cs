using System.Collections.Generic;
using Net;
using Share;

namespace gs.activity.scmsg
{
	public class SDailyBuyGiftInfo : Message
	{
		public delegate void Handler(SDailyBuyGiftInfo msg);

		public const int TYPE = 29363134;

		public static Handler handler;

		public HashSet<int> buyedGiftIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 29363134;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buyedGiftIds.Count);
			foreach (int buyedGiftId in buyedGiftIds)
			{
				oc.push(buyedGiftId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				buyedGiftIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}

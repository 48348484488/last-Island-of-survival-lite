using System.Collections.Generic;
using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SShopInfo : Message
	{
		public delegate void Handler(SShopInfo msg);

		public const int TYPE = 6294460;

		public static Handler handler;

		public HashSet<int> shangJiaShopIds = new HashSet<int>();

		public Dictionary<int, int> daylyBuyInfo = new Dictionary<int, int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294460;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shangJiaShopIds.Count);
			foreach (int shangJiaShopId in shangJiaShopIds)
			{
				oc.push(shangJiaShopId);
			}
			oc.push(daylyBuyInfo.Count);
			foreach (KeyValuePair<int, int> item in daylyBuyInfo)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				shangJiaShopIds.Add(oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				daylyBuyInfo.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}

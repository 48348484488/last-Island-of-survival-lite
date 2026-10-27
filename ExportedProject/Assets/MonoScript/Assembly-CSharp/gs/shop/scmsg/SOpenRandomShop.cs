using System.Collections.Generic;
using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SOpenRandomShop : Message
	{
		public delegate void Handler(SOpenRandomShop msg);

		public const int TYPE = 6294463;

		public static Handler handler;

		public List<RandomShopItemInfo> randomShopItemInfos = new List<RandomShopItemInfo>();

		public int refreshNumber;

		public int timeToRefresh;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294463;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(randomShopItemInfos.Count);
			foreach (RandomShopItemInfo randomShopItemInfo in randomShopItemInfos)
			{
				oc.push(randomShopItemInfo);
			}
			oc.push(refreshNumber);
			oc.push(timeToRefresh);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				RandomShopItemInfo randomShopItemInfo = new RandomShopItemInfo();
				oc.pop(randomShopItemInfo);
				randomShopItemInfos.Add(randomShopItemInfo);
			}
			refreshNumber = oc.pop_int();
			timeToRefresh = oc.pop_int();
			return oc;
		}
	}
}

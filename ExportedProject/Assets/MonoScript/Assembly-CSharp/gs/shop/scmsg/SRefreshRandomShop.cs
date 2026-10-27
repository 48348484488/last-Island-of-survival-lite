using System.Collections.Generic;
using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SRefreshRandomShop : Message
	{
		public delegate void Handler(SRefreshRandomShop msg);

		public const int TYPE = 6294467;

		public static Handler handler;

		public List<RandomShopItemInfo> randomShopItemInfos = new List<RandomShopItemInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294467;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(randomShopItemInfos.Count);
			foreach (RandomShopItemInfo randomShopItemInfo in randomShopItemInfos)
			{
				oc.push(randomShopItemInfo);
			}
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
			return oc;
		}
	}
}

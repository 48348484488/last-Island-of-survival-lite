using System.Collections.Generic;
using Share;

namespace cfg
{
	public class RechargeCfg
	{
		public const string Path = "cfg.RechargeCfg.oc";

		private static Dictionary<int, RechargeCfg> all;

		private static List<RechargeCfg> allList;

		public int id;

		public string icon;

		public int platformType;

		public int rechargeType;

		public List<ShopPriceInfo> priceInfos = new List<ShopPriceInfo>();

		public RechargeCfg(Octets oc)
		{
			id = oc.pop_int();
			icon = oc.pop_string();
			platformType = oc.pop_int();
			rechargeType = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ShopPriceInfo item = new ShopPriceInfo(oc);
				priceInfos.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, RechargeCfg>();
			allList = new List<RechargeCfg>();
			while (!octets.is_empty())
			{
				RechargeCfg rechargeCfg = new RechargeCfg(octets);
				all.Add(rechargeCfg.id, rechargeCfg);
				allList.Add(rechargeCfg);
			}
		}

		public static RechargeCfg Get(int key)
		{
			RechargeCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, RechargeCfg> GetAll()
		{
			return all;
		}

		public static List<RechargeCfg> GetAllList()
		{
			return allList;
		}
	}
}

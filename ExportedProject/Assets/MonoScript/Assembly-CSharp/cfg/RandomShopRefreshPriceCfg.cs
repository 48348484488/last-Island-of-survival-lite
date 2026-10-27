using System.Collections.Generic;
using Share;

namespace cfg
{
	public class RandomShopRefreshPriceCfg
	{
		public const string Path = "cfg.RandomShopRefreshPriceCfg.oc";

		private static Dictionary<int, RandomShopRefreshPriceCfg> all;

		private static List<RandomShopRefreshPriceCfg> allList;

		public int id;

		public List<RandomShopRefreshPriceInfo> refreshPriceInfos = new List<RandomShopRefreshPriceInfo>();

		public RandomShopRefreshPriceCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				RandomShopRefreshPriceInfo item = new RandomShopRefreshPriceInfo(oc);
				refreshPriceInfos.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, RandomShopRefreshPriceCfg>();
			allList = new List<RandomShopRefreshPriceCfg>();
			while (!octets.is_empty())
			{
				RandomShopRefreshPriceCfg randomShopRefreshPriceCfg = new RandomShopRefreshPriceCfg(octets);
				all.Add(randomShopRefreshPriceCfg.id, randomShopRefreshPriceCfg);
				allList.Add(randomShopRefreshPriceCfg);
			}
		}

		public static RandomShopRefreshPriceCfg Get(int key)
		{
			RandomShopRefreshPriceCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, RandomShopRefreshPriceCfg> GetAll()
		{
			return all;
		}

		public static List<RandomShopRefreshPriceCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class RandomShopRefreshCfg
	{
		public const string Path = "cfg.RandomShopRefreshCfg.oc";

		private static Dictionary<int, RandomShopRefreshCfg> all;

		private static List<RandomShopRefreshCfg> allList;

		public int id;

		public List<RandomShopRefreshInfo> refreshInfos = new List<RandomShopRefreshInfo>();

		public RandomShopRefreshCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				RandomShopRefreshInfo item = new RandomShopRefreshInfo(oc);
				refreshInfos.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, RandomShopRefreshCfg>();
			allList = new List<RandomShopRefreshCfg>();
			while (!octets.is_empty())
			{
				RandomShopRefreshCfg randomShopRefreshCfg = new RandomShopRefreshCfg(octets);
				all.Add(randomShopRefreshCfg.id, randomShopRefreshCfg);
				allList.Add(randomShopRefreshCfg);
			}
		}

		public static RandomShopRefreshCfg Get(int key)
		{
			RandomShopRefreshCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, RandomShopRefreshCfg> GetAll()
		{
			return all;
		}

		public static List<RandomShopRefreshCfg> GetAllList()
		{
			return allList;
		}
	}
}

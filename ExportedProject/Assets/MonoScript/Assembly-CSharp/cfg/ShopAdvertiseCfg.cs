using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ShopAdvertiseCfg
	{
		public const string Path = "cfg.ShopAdvertiseCfg.oc";

		private static Dictionary<int, ShopAdvertiseCfg> all;

		private static List<ShopAdvertiseCfg> allList;

		public int id;

		public string icon;

		public ShopAdvertiseCfg(Octets oc)
		{
			id = oc.pop_int();
			icon = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ShopAdvertiseCfg>();
			allList = new List<ShopAdvertiseCfg>();
			while (!octets.is_empty())
			{
				ShopAdvertiseCfg shopAdvertiseCfg = new ShopAdvertiseCfg(octets);
				all.Add(shopAdvertiseCfg.id, shopAdvertiseCfg);
				allList.Add(shopAdvertiseCfg);
			}
		}

		public static ShopAdvertiseCfg Get(int key)
		{
			ShopAdvertiseCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ShopAdvertiseCfg> GetAll()
		{
			return all;
		}

		public static List<ShopAdvertiseCfg> GetAllList()
		{
			return allList;
		}
	}
}

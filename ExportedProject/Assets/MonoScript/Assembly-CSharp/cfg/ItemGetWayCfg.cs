using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ItemGetWayCfg
	{
		public const string Path = "cfg.ItemGetWayCfg.oc";

		private static Dictionary<int, ItemGetWayCfg> all;

		private static List<ItemGetWayCfg> allList;

		public int id;

		public List<OneItemGetWay> getWayList = new List<OneItemGetWay>();

		public ItemGetWayCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				OneItemGetWay item = new OneItemGetWay(oc);
				getWayList.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ItemGetWayCfg>();
			allList = new List<ItemGetWayCfg>();
			while (!octets.is_empty())
			{
				ItemGetWayCfg itemGetWayCfg = new ItemGetWayCfg(octets);
				all.Add(itemGetWayCfg.id, itemGetWayCfg);
				allList.Add(itemGetWayCfg);
			}
		}

		public static ItemGetWayCfg Get(int key)
		{
			ItemGetWayCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ItemGetWayCfg> GetAll()
		{
			return all;
		}

		public static List<ItemGetWayCfg> GetAllList()
		{
			return allList;
		}
	}
}

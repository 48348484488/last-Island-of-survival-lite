using System.Collections.Generic;
using Share;

namespace cfg
{
	public class DaylyGiftCfg
	{
		public const string Path = "cfg.DaylyGiftCfg.oc";

		private static Dictionary<int, DaylyGiftCfg> all;

		private static List<DaylyGiftCfg> allList;

		public int id;

		public int price;

		public int dropId;

		public DaylyGiftCfg(Octets oc)
		{
			id = oc.pop_int();
			price = oc.pop_int();
			dropId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, DaylyGiftCfg>();
			allList = new List<DaylyGiftCfg>();
			while (!octets.is_empty())
			{
				DaylyGiftCfg daylyGiftCfg = new DaylyGiftCfg(octets);
				all.Add(daylyGiftCfg.id, daylyGiftCfg);
				allList.Add(daylyGiftCfg);
			}
		}

		public static DaylyGiftCfg Get(int key)
		{
			DaylyGiftCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, DaylyGiftCfg> GetAll()
		{
			return all;
		}

		public static List<DaylyGiftCfg> GetAllList()
		{
			return allList;
		}
	}
}

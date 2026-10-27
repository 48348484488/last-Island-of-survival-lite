using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CookbookCfg
	{
		public const string Path = "cfg.CookbookCfg.oc";

		private static Dictionary<int, CookbookCfg> all;

		private static List<CookbookCfg> allList;

		public int id;

		public int time;

		public Dictionary<int, int> material = new Dictionary<int, int>();

		public int outNum;

		public CookbookCfg(Octets oc)
		{
			id = oc.pop_int();
			time = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				int value = oc.pop_int();
				material.Add(key, value);
			}
			outNum = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CookbookCfg>();
			allList = new List<CookbookCfg>();
			while (!octets.is_empty())
			{
				CookbookCfg cookbookCfg = new CookbookCfg(octets);
				all.Add(cookbookCfg.id, cookbookCfg);
				allList.Add(cookbookCfg);
			}
		}

		public static CookbookCfg Get(int key)
		{
			CookbookCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CookbookCfg> GetAll()
		{
			return all;
		}

		public static List<CookbookCfg> GetAllList()
		{
			return allList;
		}
	}
}

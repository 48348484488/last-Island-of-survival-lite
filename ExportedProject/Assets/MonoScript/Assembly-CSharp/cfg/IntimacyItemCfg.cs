using System.Collections.Generic;
using Share;

namespace cfg
{
	public class IntimacyItemCfg
	{
		public const string Path = "cfg.IntimacyItemCfg.oc";

		private static Dictionary<int, IntimacyItemCfg> all;

		private static List<IntimacyItemCfg> allList;

		public int id;

		public int addValue;

		public IntimacyItemCfg(Octets oc)
		{
			id = oc.pop_int();
			addValue = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, IntimacyItemCfg>();
			allList = new List<IntimacyItemCfg>();
			while (!octets.is_empty())
			{
				IntimacyItemCfg intimacyItemCfg = new IntimacyItemCfg(octets);
				all.Add(intimacyItemCfg.id, intimacyItemCfg);
				allList.Add(intimacyItemCfg);
			}
		}

		public static IntimacyItemCfg Get(int key)
		{
			IntimacyItemCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, IntimacyItemCfg> GetAll()
		{
			return all;
		}

		public static List<IntimacyItemCfg> GetAllList()
		{
			return allList;
		}
	}
}

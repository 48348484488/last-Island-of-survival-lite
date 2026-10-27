using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MonthSignInCfg
	{
		public const string Path = "cfg.MonthSignInCfg.oc";

		private static Dictionary<int, MonthSignInCfg> all;

		private static List<MonthSignInCfg> allList;

		public int id;

		public int dropId;

		public MonthSignInCfg(Octets oc)
		{
			id = oc.pop_int();
			dropId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MonthSignInCfg>();
			allList = new List<MonthSignInCfg>();
			while (!octets.is_empty())
			{
				MonthSignInCfg monthSignInCfg = new MonthSignInCfg(octets);
				all.Add(monthSignInCfg.id, monthSignInCfg);
				allList.Add(monthSignInCfg);
			}
		}

		public static MonthSignInCfg Get(int key)
		{
			MonthSignInCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MonthSignInCfg> GetAll()
		{
			return all;
		}

		public static List<MonthSignInCfg> GetAllList()
		{
			return allList;
		}
	}
}

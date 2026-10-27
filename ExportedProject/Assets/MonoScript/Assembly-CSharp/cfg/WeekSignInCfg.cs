using System.Collections.Generic;
using Share;

namespace cfg
{
	public class WeekSignInCfg
	{
		public const string Path = "cfg.WeekSignInCfg.oc";

		private static Dictionary<int, WeekSignInCfg> all;

		private static List<WeekSignInCfg> allList;

		public int id;

		public int dropId;

		public string name;

		public WeekSignInCfg(Octets oc)
		{
			id = oc.pop_int();
			dropId = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, WeekSignInCfg>();
			allList = new List<WeekSignInCfg>();
			while (!octets.is_empty())
			{
				WeekSignInCfg weekSignInCfg = new WeekSignInCfg(octets);
				all.Add(weekSignInCfg.id, weekSignInCfg);
				allList.Add(weekSignInCfg);
			}
		}

		public static WeekSignInCfg Get(int key)
		{
			WeekSignInCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, WeekSignInCfg> GetAll()
		{
			return all;
		}

		public static List<WeekSignInCfg> GetAllList()
		{
			return allList;
		}
	}
}

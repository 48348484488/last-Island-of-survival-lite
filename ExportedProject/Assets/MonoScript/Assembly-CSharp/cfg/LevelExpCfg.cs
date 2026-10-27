using System.Collections.Generic;
using Share;

namespace cfg
{
	public class LevelExpCfg
	{
		public const string Path = "cfg.LevelExpCfg.oc";

		private static Dictionary<int, LevelExpCfg> all;

		private static List<LevelExpCfg> allList;

		public int id;

		public int uplevelExp;

		public string uplevelExpIcon;

		public LevelExpCfg(Octets oc)
		{
			id = oc.pop_int();
			uplevelExp = oc.pop_int();
			uplevelExpIcon = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, LevelExpCfg>();
			allList = new List<LevelExpCfg>();
			while (!octets.is_empty())
			{
				LevelExpCfg levelExpCfg = new LevelExpCfg(octets);
				all.Add(levelExpCfg.id, levelExpCfg);
				allList.Add(levelExpCfg);
			}
		}

		public static LevelExpCfg Get(int key)
		{
			LevelExpCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, LevelExpCfg> GetAll()
		{
			return all;
		}

		public static List<LevelExpCfg> GetAllList()
		{
			return allList;
		}
	}
}

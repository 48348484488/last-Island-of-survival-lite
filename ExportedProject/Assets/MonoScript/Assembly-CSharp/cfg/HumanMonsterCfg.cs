using System.Collections.Generic;
using Share;

namespace cfg
{
	public class HumanMonsterCfg
	{
		public const string Path = "cfg.HumanMonsterCfg.oc";

		private static Dictionary<int, HumanMonsterCfg> all;

		private static List<HumanMonsterCfg> allList;

		public int id;

		public string name;

		public List<int> skinIds = new List<int>();

		public bool isMan;

		public HumanMonsterCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				skinIds.Add(item);
			}
			isMan = oc.pop_boolean();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, HumanMonsterCfg>();
			allList = new List<HumanMonsterCfg>();
			while (!octets.is_empty())
			{
				HumanMonsterCfg humanMonsterCfg = new HumanMonsterCfg(octets);
				all.Add(humanMonsterCfg.id, humanMonsterCfg);
				allList.Add(humanMonsterCfg);
			}
		}

		public static HumanMonsterCfg Get(int key)
		{
			HumanMonsterCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, HumanMonsterCfg> GetAll()
		{
			return all;
		}

		public static List<HumanMonsterCfg> GetAllList()
		{
			return allList;
		}
	}
}

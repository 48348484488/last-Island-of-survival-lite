using System.Collections.Generic;
using Share;

namespace cfg
{
	public class LadderCfg
	{
		public const string Path = "cfg.LadderCfg.oc";

		private static Dictionary<int, LadderCfg> all;

		private static List<LadderCfg> allList;

		public int id;

		public int freeDropId;

		public int payDropId;

		public int exp;

		public int buyLevelPrice;

		public string unlockSeniorContition;

		public string unlockJuniorContition;

		public LadderCfg(Octets oc)
		{
			id = oc.pop_int();
			freeDropId = oc.pop_int();
			payDropId = oc.pop_int();
			exp = oc.pop_int();
			buyLevelPrice = oc.pop_int();
			unlockSeniorContition = oc.pop_string();
			unlockJuniorContition = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, LadderCfg>();
			allList = new List<LadderCfg>();
			while (!octets.is_empty())
			{
				LadderCfg ladderCfg = new LadderCfg(octets);
				all.Add(ladderCfg.id, ladderCfg);
				allList.Add(ladderCfg);
			}
		}

		public static LadderCfg Get(int key)
		{
			LadderCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, LadderCfg> GetAll()
		{
			return all;
		}

		public static List<LadderCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CutPlantCfg
	{
		public const string Path = "cfg.CutPlantCfg.oc";

		private static Dictionary<int, CutPlantCfg> all;

		private static List<CutPlantCfg> allList;

		public int id;

		public string name;

		public string grownUpModel;

		public int seedId;

		public int grownUpTime;

		public string seedModel;

		public string littleBattleMapIcon;

		public CutPlantCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			grownUpModel = oc.pop_string();
			seedId = oc.pop_int();
			grownUpTime = oc.pop_int();
			seedModel = oc.pop_string();
			littleBattleMapIcon = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CutPlantCfg>();
			allList = new List<CutPlantCfg>();
			while (!octets.is_empty())
			{
				CutPlantCfg cutPlantCfg = new CutPlantCfg(octets);
				all.Add(cutPlantCfg.id, cutPlantCfg);
				allList.Add(cutPlantCfg);
			}
		}

		public static CutPlantCfg Get(int key)
		{
			CutPlantCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CutPlantCfg> GetAll()
		{
			return all;
		}

		public static List<CutPlantCfg> GetAllList()
		{
			return allList;
		}
	}
}

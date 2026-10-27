using System.Collections.Generic;
using Share;

namespace cfg
{
	public class PlantCfg
	{
		public const string Path = "cfg.PlantCfg.oc";

		private static Dictionary<int, PlantCfg> all;

		private static List<PlantCfg> allList;

		public int id;

		public string name;

		public int seedId;

		public string seedModel;

		public string grownUpModel;

		public PlantCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			seedId = oc.pop_int();
			seedModel = oc.pop_string();
			grownUpModel = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, PlantCfg>();
			allList = new List<PlantCfg>();
			while (!octets.is_empty())
			{
				PlantCfg plantCfg = new PlantCfg(octets);
				all.Add(plantCfg.id, plantCfg);
				allList.Add(plantCfg);
			}
		}

		public static PlantCfg Get(int key)
		{
			PlantCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, PlantCfg> GetAll()
		{
			return all;
		}

		public static List<PlantCfg> GetAllList()
		{
			return allList;
		}
	}
}

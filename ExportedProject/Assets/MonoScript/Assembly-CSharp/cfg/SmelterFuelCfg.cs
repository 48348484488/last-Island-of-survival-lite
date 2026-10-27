using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SmelterFuelCfg
	{
		public const string Path = "cfg.SmelterFuelCfg.oc";

		private static Dictionary<int, SmelterFuelCfg> all;

		private static List<SmelterFuelCfg> allList;

		public int id;

		public int burnTime;

		public SmelterFuelCfg(Octets oc)
		{
			id = oc.pop_int();
			burnTime = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SmelterFuelCfg>();
			allList = new List<SmelterFuelCfg>();
			while (!octets.is_empty())
			{
				SmelterFuelCfg smelterFuelCfg = new SmelterFuelCfg(octets);
				all.Add(smelterFuelCfg.id, smelterFuelCfg);
				allList.Add(smelterFuelCfg);
			}
		}

		public static SmelterFuelCfg Get(int key)
		{
			SmelterFuelCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SmelterFuelCfg> GetAll()
		{
			return all;
		}

		public static List<SmelterFuelCfg> GetAllList()
		{
			return allList;
		}
	}
}

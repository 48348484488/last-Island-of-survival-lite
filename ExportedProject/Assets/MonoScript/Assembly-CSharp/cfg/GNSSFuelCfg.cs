using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GNSSFuelCfg
	{
		public const string Path = "cfg.GNSSFuelCfg.oc";

		private static Dictionary<int, GNSSFuelCfg> all;

		private static List<GNSSFuelCfg> allList;

		public int id;

		public Dictionary<int, GNSSFuelInfo> fuelInfos = new Dictionary<int, GNSSFuelInfo>();

		public GNSSFuelCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				GNSSFuelInfo value = new GNSSFuelInfo(oc);
				fuelInfos.Add(key, value);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GNSSFuelCfg>();
			allList = new List<GNSSFuelCfg>();
			while (!octets.is_empty())
			{
				GNSSFuelCfg gNSSFuelCfg = new GNSSFuelCfg(octets);
				all.Add(gNSSFuelCfg.id, gNSSFuelCfg);
				allList.Add(gNSSFuelCfg);
			}
		}

		public static GNSSFuelCfg Get(int key)
		{
			GNSSFuelCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GNSSFuelCfg> GetAll()
		{
			return all;
		}

		public static List<GNSSFuelCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GNSSCapacityCfg
	{
		public const string Path = "cfg.GNSSCapacityCfg.oc";

		private static Dictionary<int, GNSSCapacityCfg> all;

		private static List<GNSSCapacityCfg> allList;

		public int id;

		public int materialCapacity;

		public int fuelCapacity;

		public int finishedCapacity;

		public bool isVolume;

		public GNSSCapacityCfg(Octets oc)
		{
			id = oc.pop_int();
			materialCapacity = oc.pop_int();
			fuelCapacity = oc.pop_int();
			finishedCapacity = oc.pop_int();
			isVolume = oc.pop_boolean();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GNSSCapacityCfg>();
			allList = new List<GNSSCapacityCfg>();
			while (!octets.is_empty())
			{
				GNSSCapacityCfg gNSSCapacityCfg = new GNSSCapacityCfg(octets);
				all.Add(gNSSCapacityCfg.id, gNSSCapacityCfg);
				allList.Add(gNSSCapacityCfg);
			}
		}

		public static GNSSCapacityCfg Get(int key)
		{
			GNSSCapacityCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GNSSCapacityCfg> GetAll()
		{
			return all;
		}

		public static List<GNSSCapacityCfg> GetAllList()
		{
			return allList;
		}
	}
}

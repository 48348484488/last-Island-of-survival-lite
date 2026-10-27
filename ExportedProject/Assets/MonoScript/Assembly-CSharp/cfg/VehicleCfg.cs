using System.Collections.Generic;
using Share;

namespace cfg
{
	public class VehicleCfg
	{
		public const string Path = "cfg.VehicleCfg.oc";

		private static Dictionary<int, VehicleCfg> all;

		private static List<VehicleCfg> allList;

		public int id;

		public string name;

		public string abPath;

		public int vehicleType;

		public string icon;

		public VehicleCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			abPath = oc.pop_string();
			vehicleType = oc.pop_int();
			icon = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, VehicleCfg>();
			allList = new List<VehicleCfg>();
			while (!octets.is_empty())
			{
				VehicleCfg vehicleCfg = new VehicleCfg(octets);
				all.Add(vehicleCfg.id, vehicleCfg);
				allList.Add(vehicleCfg);
			}
		}

		public static VehicleCfg Get(int key)
		{
			VehicleCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, VehicleCfg> GetAll()
		{
			return all;
		}

		public static List<VehicleCfg> GetAllList()
		{
			return allList;
		}
	}
}

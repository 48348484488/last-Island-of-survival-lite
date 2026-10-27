using System.Collections.Generic;
using Share;

namespace cfg
{
	public class PowerCfg
	{
		public const string Path = "cfg.PowerCfg.oc";

		private static Dictionary<int, PowerCfg> all;

		private static List<PowerCfg> allList;

		public int id;

		public Dictionary<int, int> fuels = new Dictionary<int, int>();

		public PowerCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				int value = oc.pop_int();
				fuels.Add(key, value);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, PowerCfg>();
			allList = new List<PowerCfg>();
			while (!octets.is_empty())
			{
				PowerCfg powerCfg = new PowerCfg(octets);
				all.Add(powerCfg.id, powerCfg);
				allList.Add(powerCfg);
			}
		}

		public static PowerCfg Get(int key)
		{
			PowerCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, PowerCfg> GetAll()
		{
			return all;
		}

		public static List<PowerCfg> GetAllList()
		{
			return allList;
		}
	}
}

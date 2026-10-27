using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CircuitryCfg
	{
		public const string Path = "cfg.CircuitryCfg.oc";

		private static Dictionary<int, CircuitryCfg> all;

		private static List<CircuitryCfg> allList;

		public int id;

		public int power;

		public int parentCount;

		public int childCount;

		public bool canOpenClose;

		public string desc;

		public bool isDelaySwitch;

		public CircuitryCfg(Octets oc)
		{
			id = oc.pop_int();
			power = oc.pop_int();
			parentCount = oc.pop_int();
			childCount = oc.pop_int();
			canOpenClose = oc.pop_boolean();
			desc = oc.pop_string();
			isDelaySwitch = oc.pop_boolean();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CircuitryCfg>();
			allList = new List<CircuitryCfg>();
			while (!octets.is_empty())
			{
				CircuitryCfg circuitryCfg = new CircuitryCfg(octets);
				all.Add(circuitryCfg.id, circuitryCfg);
				allList.Add(circuitryCfg);
			}
		}

		public static CircuitryCfg Get(int key)
		{
			CircuitryCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CircuitryCfg> GetAll()
		{
			return all;
		}

		public static List<CircuitryCfg> GetAllList()
		{
			return allList;
		}
	}
}

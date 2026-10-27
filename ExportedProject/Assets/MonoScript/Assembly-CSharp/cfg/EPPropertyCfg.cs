using System.Collections.Generic;
using Share;

namespace cfg
{
	public class EPPropertyCfg
	{
		public const string Path = "cfg.EPPropertyCfg.oc";

		private static Dictionary<int, EPPropertyCfg> all;

		private static List<EPPropertyCfg> allList;

		public int id;

		public int maxValue;

		public float consume;

		public float addHP;

		public float addSpeed;

		public EPPropertyCfg(Octets oc)
		{
			id = oc.pop_int();
			maxValue = oc.pop_int();
			consume = oc.pop_float();
			addHP = oc.pop_float();
			addSpeed = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, EPPropertyCfg>();
			allList = new List<EPPropertyCfg>();
			while (!octets.is_empty())
			{
				EPPropertyCfg ePPropertyCfg = new EPPropertyCfg(octets);
				all.Add(ePPropertyCfg.id, ePPropertyCfg);
				allList.Add(ePPropertyCfg);
			}
		}

		public static EPPropertyCfg Get(int key)
		{
			EPPropertyCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, EPPropertyCfg> GetAll()
		{
			return all;
		}

		public static List<EPPropertyCfg> GetAllList()
		{
			return allList;
		}
	}
}

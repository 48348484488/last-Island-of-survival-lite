using System.Collections.Generic;
using Share;

namespace cfg
{
	public class DoButtonCfg
	{
		public const string Path = "cfg.DoButtonCfg.oc";

		private static Dictionary<int, DoButtonCfg> all;

		private static List<DoButtonCfg> allList;

		public int id;

		public string name;

		public DoButtonCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, DoButtonCfg>();
			allList = new List<DoButtonCfg>();
			while (!octets.is_empty())
			{
				DoButtonCfg doButtonCfg = new DoButtonCfg(octets);
				all.Add(doButtonCfg.id, doButtonCfg);
				allList.Add(doButtonCfg);
			}
		}

		public static DoButtonCfg Get(int key)
		{
			DoButtonCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, DoButtonCfg> GetAll()
		{
			return all;
		}

		public static List<DoButtonCfg> GetAllList()
		{
			return allList;
		}
	}
}

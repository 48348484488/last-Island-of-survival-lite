using System.Collections.Generic;
using Share;

namespace cfg
{
	public class PartCfg
	{
		public const string Path = "cfg.PartCfg.oc";

		private static Dictionary<int, PartCfg> all;

		private static List<PartCfg> allList;

		public int id;

		public string name;

		public string desc;

		public List<PartProp> props = new List<PartProp>();

		public PartCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			desc = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				PartProp item = new PartProp(oc);
				props.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, PartCfg>();
			allList = new List<PartCfg>();
			while (!octets.is_empty())
			{
				PartCfg partCfg = new PartCfg(octets);
				all.Add(partCfg.id, partCfg);
				allList.Add(partCfg);
			}
		}

		public static PartCfg Get(int key)
		{
			PartCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, PartCfg> GetAll()
		{
			return all;
		}

		public static List<PartCfg> GetAllList()
		{
			return allList;
		}
	}
}

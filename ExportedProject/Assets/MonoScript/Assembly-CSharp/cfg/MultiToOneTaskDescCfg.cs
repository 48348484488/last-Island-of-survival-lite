using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MultiToOneTaskDescCfg
	{
		public const string Path = "cfg.MultiToOneTaskDescCfg.oc";

		private static Dictionary<int, MultiToOneTaskDescCfg> all;

		private static List<MultiToOneTaskDescCfg> allList;

		public int id;

		public string desc;

		public MultiToOneTaskDescCfg(Octets oc)
		{
			id = oc.pop_int();
			desc = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MultiToOneTaskDescCfg>();
			allList = new List<MultiToOneTaskDescCfg>();
			while (!octets.is_empty())
			{
				MultiToOneTaskDescCfg multiToOneTaskDescCfg = new MultiToOneTaskDescCfg(octets);
				all.Add(multiToOneTaskDescCfg.id, multiToOneTaskDescCfg);
				allList.Add(multiToOneTaskDescCfg);
			}
		}

		public static MultiToOneTaskDescCfg Get(int key)
		{
			MultiToOneTaskDescCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MultiToOneTaskDescCfg> GetAll()
		{
			return all;
		}

		public static List<MultiToOneTaskDescCfg> GetAllList()
		{
			return allList;
		}
	}
}

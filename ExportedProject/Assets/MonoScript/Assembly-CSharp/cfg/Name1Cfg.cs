using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Name1Cfg
	{
		public const string Path = "cfg.Name1Cfg.oc";

		private static Dictionary<int, Name1Cfg> all;

		private static List<Name1Cfg> allList;

		public int id;

		public string name;

		public Name1Cfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Name1Cfg>();
			allList = new List<Name1Cfg>();
			while (!octets.is_empty())
			{
				Name1Cfg name1Cfg = new Name1Cfg(octets);
				all.Add(name1Cfg.id, name1Cfg);
				allList.Add(name1Cfg);
			}
		}

		public static Name1Cfg Get(int key)
		{
			Name1Cfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Name1Cfg> GetAll()
		{
			return all;
		}

		public static List<Name1Cfg> GetAllList()
		{
			return allList;
		}
	}
}

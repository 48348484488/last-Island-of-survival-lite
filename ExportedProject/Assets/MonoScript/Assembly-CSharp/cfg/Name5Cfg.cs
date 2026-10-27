using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Name5Cfg
	{
		public const string Path = "cfg.Name5Cfg.oc";

		private static Dictionary<int, Name5Cfg> all;

		private static List<Name5Cfg> allList;

		public int id;

		public string name;

		public Name5Cfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Name5Cfg>();
			allList = new List<Name5Cfg>();
			while (!octets.is_empty())
			{
				Name5Cfg name5Cfg = new Name5Cfg(octets);
				all.Add(name5Cfg.id, name5Cfg);
				allList.Add(name5Cfg);
			}
		}

		public static Name5Cfg Get(int key)
		{
			Name5Cfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Name5Cfg> GetAll()
		{
			return all;
		}

		public static List<Name5Cfg> GetAllList()
		{
			return allList;
		}
	}
}

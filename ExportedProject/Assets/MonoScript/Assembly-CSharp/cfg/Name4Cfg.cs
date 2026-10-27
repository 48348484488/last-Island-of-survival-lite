using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Name4Cfg
	{
		public const string Path = "cfg.Name4Cfg.oc";

		private static Dictionary<int, Name4Cfg> all;

		private static List<Name4Cfg> allList;

		public int id;

		public string name;

		public Name4Cfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Name4Cfg>();
			allList = new List<Name4Cfg>();
			while (!octets.is_empty())
			{
				Name4Cfg name4Cfg = new Name4Cfg(octets);
				all.Add(name4Cfg.id, name4Cfg);
				allList.Add(name4Cfg);
			}
		}

		public static Name4Cfg Get(int key)
		{
			Name4Cfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Name4Cfg> GetAll()
		{
			return all;
		}

		public static List<Name4Cfg> GetAllList()
		{
			return allList;
		}
	}
}

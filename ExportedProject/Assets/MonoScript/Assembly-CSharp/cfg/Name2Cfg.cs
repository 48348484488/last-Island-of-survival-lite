using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Name2Cfg
	{
		public const string Path = "cfg.Name2Cfg.oc";

		private static Dictionary<int, Name2Cfg> all;

		private static List<Name2Cfg> allList;

		public int id;

		public string name;

		public Name2Cfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Name2Cfg>();
			allList = new List<Name2Cfg>();
			while (!octets.is_empty())
			{
				Name2Cfg name2Cfg = new Name2Cfg(octets);
				all.Add(name2Cfg.id, name2Cfg);
				allList.Add(name2Cfg);
			}
		}

		public static Name2Cfg Get(int key)
		{
			Name2Cfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Name2Cfg> GetAll()
		{
			return all;
		}

		public static List<Name2Cfg> GetAllList()
		{
			return allList;
		}
	}
}

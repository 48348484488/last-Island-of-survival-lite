using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Name3Cfg
	{
		public const string Path = "cfg.Name3Cfg.oc";

		private static Dictionary<int, Name3Cfg> all;

		private static List<Name3Cfg> allList;

		public int id;

		public string name;

		public Name3Cfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Name3Cfg>();
			allList = new List<Name3Cfg>();
			while (!octets.is_empty())
			{
				Name3Cfg name3Cfg = new Name3Cfg(octets);
				all.Add(name3Cfg.id, name3Cfg);
				allList.Add(name3Cfg);
			}
		}

		public static Name3Cfg Get(int key)
		{
			Name3Cfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Name3Cfg> GetAll()
		{
			return all;
		}

		public static List<Name3Cfg> GetAllList()
		{
			return allList;
		}
	}
}

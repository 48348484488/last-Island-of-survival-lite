using System.Collections.Generic;
using Share;

namespace cfg
{
	public class Strings
	{
		public const string Path = "cfg.Strings.oc";

		private static Dictionary<int, Strings> all;

		private static List<Strings> allList;

		public int id;

		public string content;

		public Strings(Octets oc)
		{
			id = oc.pop_int();
			content = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, Strings>();
			allList = new List<Strings>();
			while (!octets.is_empty())
			{
				Strings strings = new Strings(octets);
				all.Add(strings.id, strings);
				allList.Add(strings);
			}
		}

		public static Strings Get(int key)
		{
			Strings value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, Strings> GetAll()
		{
			return all;
		}

		public static List<Strings> GetAllList()
		{
			return allList;
		}
	}
}

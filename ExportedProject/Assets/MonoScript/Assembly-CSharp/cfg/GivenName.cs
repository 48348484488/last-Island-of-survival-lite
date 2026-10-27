using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GivenName
	{
		public const string Path = "cfg.GivenName.oc";

		private static Dictionary<int, GivenName> all;

		private static List<GivenName> allList;

		public int id;

		public string givenName;

		public GivenName(Octets oc)
		{
			id = oc.pop_int();
			givenName = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GivenName>();
			allList = new List<GivenName>();
			while (!octets.is_empty())
			{
				GivenName givenName = new GivenName(octets);
				all.Add(givenName.id, givenName);
				allList.Add(givenName);
			}
		}

		public static GivenName Get(int key)
		{
			GivenName value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GivenName> GetAll()
		{
			return all;
		}

		public static List<GivenName> GetAllList()
		{
			return allList;
		}
	}
}

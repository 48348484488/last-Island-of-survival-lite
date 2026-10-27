using System.Collections.Generic;
using Share;

namespace cfg
{
	public class FamilyName
	{
		public const string Path = "cfg.FamilyName.oc";

		private static Dictionary<int, FamilyName> all;

		private static List<FamilyName> allList;

		public int id;

		public string familyName;

		public FamilyName(Octets oc)
		{
			id = oc.pop_int();
			familyName = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, FamilyName>();
			allList = new List<FamilyName>();
			while (!octets.is_empty())
			{
				FamilyName familyName = new FamilyName(octets);
				all.Add(familyName.id, familyName);
				allList.Add(familyName);
			}
		}

		public static FamilyName Get(int key)
		{
			FamilyName value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, FamilyName> GetAll()
		{
			return all;
		}

		public static List<FamilyName> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class RoleHeadCfg
	{
		public const string Path = "cfg.RoleHeadCfg.oc";

		private static Dictionary<int, RoleHeadCfg> all;

		private static List<RoleHeadCfg> allList;

		public int id;

		public string name;

		public string icon;

		public bool sex;

		public bool isFree;

		public int itemId;

		public RoleHeadCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			icon = oc.pop_string();
			sex = oc.pop_bool();
			isFree = oc.pop_boolean();
			itemId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, RoleHeadCfg>();
			allList = new List<RoleHeadCfg>();
			while (!octets.is_empty())
			{
				RoleHeadCfg roleHeadCfg = new RoleHeadCfg(octets);
				all.Add(roleHeadCfg.id, roleHeadCfg);
				allList.Add(roleHeadCfg);
			}
		}

		public static RoleHeadCfg Get(int key)
		{
			RoleHeadCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, RoleHeadCfg> GetAll()
		{
			return all;
		}

		public static List<RoleHeadCfg> GetAllList()
		{
			return allList;
		}
	}
}

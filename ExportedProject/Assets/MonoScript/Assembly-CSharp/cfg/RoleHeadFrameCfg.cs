using System.Collections.Generic;
using Share;

namespace cfg
{
	public class RoleHeadFrameCfg
	{
		public const string Path = "cfg.RoleHeadFrameCfg.oc";

		private static Dictionary<int, RoleHeadFrameCfg> all;

		private static List<RoleHeadFrameCfg> allList;

		public int id;

		public string name;

		public string frame;

		public string des;

		public RoleHeadFrameCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			frame = oc.pop_string();
			des = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, RoleHeadFrameCfg>();
			allList = new List<RoleHeadFrameCfg>();
			while (!octets.is_empty())
			{
				RoleHeadFrameCfg roleHeadFrameCfg = new RoleHeadFrameCfg(octets);
				all.Add(roleHeadFrameCfg.id, roleHeadFrameCfg);
				allList.Add(roleHeadFrameCfg);
			}
		}

		public static RoleHeadFrameCfg Get(int key)
		{
			RoleHeadFrameCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, RoleHeadFrameCfg> GetAll()
		{
			return all;
		}

		public static List<RoleHeadFrameCfg> GetAllList()
		{
			return allList;
		}
	}
}

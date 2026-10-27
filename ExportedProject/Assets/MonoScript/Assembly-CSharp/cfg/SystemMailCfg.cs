using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SystemMailCfg
	{
		public const string Path = "cfg.SystemMailCfg.oc";

		private static Dictionary<int, SystemMailCfg> all;

		private static List<SystemMailCfg> allList;

		public int id;

		public int serverId;

		public bool newRoleCanGet;

		public int levelMin;

		public int levelMax;

		public List<FixDrop> fixDrops = new List<FixDrop>();

		public SystemMailCfg(Octets oc)
		{
			id = oc.pop_int();
			serverId = oc.pop_int();
			newRoleCanGet = oc.pop_boolean();
			levelMin = oc.pop_int();
			levelMax = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				FixDrop item = new FixDrop(oc);
				fixDrops.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SystemMailCfg>();
			allList = new List<SystemMailCfg>();
			while (!octets.is_empty())
			{
				SystemMailCfg systemMailCfg = new SystemMailCfg(octets);
				all.Add(systemMailCfg.id, systemMailCfg);
				allList.Add(systemMailCfg);
			}
		}

		public static SystemMailCfg Get(int key)
		{
			SystemMailCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SystemMailCfg> GetAll()
		{
			return all;
		}

		public static List<SystemMailCfg> GetAllList()
		{
			return allList;
		}
	}
}

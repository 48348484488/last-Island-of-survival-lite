using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MineCfg
	{
		public const string Path = "cfg.MineCfg.oc";

		private static Dictionary<int, MineCfg> all;

		private static List<MineCfg> allList;

		public int id;

		public string name;

		public string model;

		public int hp;

		public int itemId;

		public List<MineToolInfo> mineToolInfos = new List<MineToolInfo>();

		public string headIcon;

		public string headIconBg;

		public MineCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			model = oc.pop_string();
			hp = oc.pop_int();
			itemId = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				MineToolInfo item = new MineToolInfo(oc);
				mineToolInfos.Add(item);
			}
			headIcon = oc.pop_string();
			headIconBg = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MineCfg>();
			allList = new List<MineCfg>();
			while (!octets.is_empty())
			{
				MineCfg mineCfg = new MineCfg(octets);
				all.Add(mineCfg.id, mineCfg);
				allList.Add(mineCfg);
			}
		}

		public static MineCfg Get(int key)
		{
			MineCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MineCfg> GetAll()
		{
			return all;
		}

		public static List<MineCfg> GetAllList()
		{
			return allList;
		}
	}
}

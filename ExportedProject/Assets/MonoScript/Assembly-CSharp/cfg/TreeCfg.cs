using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TreeCfg
	{
		public const string Path = "cfg.TreeCfg.oc";

		private static Dictionary<int, TreeCfg> all;

		private static List<TreeCfg> allList;

		public int id;

		public string name;

		public int hp;

		public int itemId;

		public List<CutTreeToolInfo> cutTreeToolInfos = new List<CutTreeToolInfo>();

		public string headIcon;

		public string headIconBg;

		public TreeCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			hp = oc.pop_int();
			itemId = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				CutTreeToolInfo item = new CutTreeToolInfo(oc);
				cutTreeToolInfos.Add(item);
			}
			headIcon = oc.pop_string();
			headIconBg = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TreeCfg>();
			allList = new List<TreeCfg>();
			while (!octets.is_empty())
			{
				TreeCfg treeCfg = new TreeCfg(octets);
				all.Add(treeCfg.id, treeCfg);
				allList.Add(treeCfg);
			}
		}

		public static TreeCfg Get(int key)
		{
			TreeCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TreeCfg> GetAll()
		{
			return all;
		}

		public static List<TreeCfg> GetAllList()
		{
			return allList;
		}
	}
}

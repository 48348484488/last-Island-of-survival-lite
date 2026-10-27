using System.Collections.Generic;
using Share;

namespace cfg
{
	public class BoxCfg
	{
		public const string Path = "cfg.BoxCfg.oc";

		private static Dictionary<int, BoxCfg> all;

		private static List<BoxCfg> allList;

		public int id;

		public string name;

		public string prefabName;

		public string icon;

		public string abPath;

		public int hp;

		public string headIcon;

		public string headIconBg;

		public string breakEffect;

		public BoxCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			prefabName = oc.pop_string();
			icon = oc.pop_string();
			abPath = oc.pop_string();
			hp = oc.pop_int();
			headIcon = oc.pop_string();
			headIconBg = oc.pop_string();
			breakEffect = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, BoxCfg>();
			allList = new List<BoxCfg>();
			while (!octets.is_empty())
			{
				BoxCfg boxCfg = new BoxCfg(octets);
				all.Add(boxCfg.id, boxCfg);
				allList.Add(boxCfg);
			}
		}

		public static BoxCfg Get(int key)
		{
			BoxCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, BoxCfg> GetAll()
		{
			return all;
		}

		public static List<BoxCfg> GetAllList()
		{
			return allList;
		}
	}
}

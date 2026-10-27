using System.Collections.Generic;
using Share;

namespace cfg
{
	public class LoadPanelCfg
	{
		public const string Path = "cfg.LoadPanelCfg.oc";

		private static Dictionary<int, LoadPanelCfg> all;

		private static List<LoadPanelCfg> allList;

		public int id;

		public string texture;

		public LoadPanelCfg(Octets oc)
		{
			id = oc.pop_int();
			texture = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, LoadPanelCfg>();
			allList = new List<LoadPanelCfg>();
			while (!octets.is_empty())
			{
				LoadPanelCfg loadPanelCfg = new LoadPanelCfg(octets);
				all.Add(loadPanelCfg.id, loadPanelCfg);
				allList.Add(loadPanelCfg);
			}
		}

		public static LoadPanelCfg Get(int key)
		{
			LoadPanelCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, LoadPanelCfg> GetAll()
		{
			return all;
		}

		public static List<LoadPanelCfg> GetAllList()
		{
			return allList;
		}
	}
}

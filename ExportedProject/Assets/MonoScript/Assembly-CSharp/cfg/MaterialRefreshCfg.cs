using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MaterialRefreshCfg
	{
		public const string Path = "cfg.MaterialRefreshCfg.oc";

		private static Dictionary<int, MaterialRefreshCfg> all;

		private static List<MaterialRefreshCfg> allList;

		public int id;

		public string prefabName;

		public MaterialRefreshCfg(Octets oc)
		{
			id = oc.pop_int();
			prefabName = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MaterialRefreshCfg>();
			allList = new List<MaterialRefreshCfg>();
			while (!octets.is_empty())
			{
				MaterialRefreshCfg materialRefreshCfg = new MaterialRefreshCfg(octets);
				all.Add(materialRefreshCfg.id, materialRefreshCfg);
				allList.Add(materialRefreshCfg);
			}
		}

		public static MaterialRefreshCfg Get(int key)
		{
			MaterialRefreshCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MaterialRefreshCfg> GetAll()
		{
			return all;
		}

		public static List<MaterialRefreshCfg> GetAllList()
		{
			return allList;
		}
	}
}

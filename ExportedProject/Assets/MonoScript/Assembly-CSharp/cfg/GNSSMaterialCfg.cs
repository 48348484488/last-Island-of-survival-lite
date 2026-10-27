using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GNSSMaterialCfg
	{
		public const string Path = "cfg.GNSSMaterialCfg.oc";

		private static Dictionary<int, GNSSMaterialCfg> all;

		private static List<GNSSMaterialCfg> allList;

		public int id;

		public Dictionary<int, GNSSMaterialInfo> materialInfos = new Dictionary<int, GNSSMaterialInfo>();

		public GNSSMaterialCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				GNSSMaterialInfo value = new GNSSMaterialInfo(oc);
				materialInfos.Add(key, value);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GNSSMaterialCfg>();
			allList = new List<GNSSMaterialCfg>();
			while (!octets.is_empty())
			{
				GNSSMaterialCfg gNSSMaterialCfg = new GNSSMaterialCfg(octets);
				all.Add(gNSSMaterialCfg.id, gNSSMaterialCfg);
				allList.Add(gNSSMaterialCfg);
			}
		}

		public static GNSSMaterialCfg Get(int key)
		{
			GNSSMaterialCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GNSSMaterialCfg> GetAll()
		{
			return all;
		}

		public static List<GNSSMaterialCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SkinCfg
	{
		public const string Path = "cfg.SkinCfg.oc";

		private static Dictionary<int, SkinCfg> all;

		private static List<SkinCfg> allList;

		public int id;

		public string name;

		public int skinType;

		public int cellIndex;

		public SkinCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			skinType = oc.pop_int();
			cellIndex = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SkinCfg>();
			allList = new List<SkinCfg>();
			while (!octets.is_empty())
			{
				SkinCfg skinCfg = new SkinCfg(octets);
				all.Add(skinCfg.id, skinCfg);
				allList.Add(skinCfg);
			}
		}

		public static SkinCfg Get(int key)
		{
			SkinCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SkinCfg> GetAll()
		{
			return all;
		}

		public static List<SkinCfg> GetAllList()
		{
			return allList;
		}
	}
}

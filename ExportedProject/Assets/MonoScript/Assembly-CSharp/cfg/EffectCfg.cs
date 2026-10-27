using System.Collections.Generic;
using Share;

namespace cfg
{
	public class EffectCfg
	{
		public const string Path = "cfg.EffectCfg.oc";

		private static Dictionary<int, EffectCfg> all;

		private static List<EffectCfg> allList;

		public int id;

		public string path;

		public string name;

		public EffectCfg(Octets oc)
		{
			id = oc.pop_int();
			path = oc.pop_string();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, EffectCfg>();
			allList = new List<EffectCfg>();
			while (!octets.is_empty())
			{
				EffectCfg effectCfg = new EffectCfg(octets);
				all.Add(effectCfg.id, effectCfg);
				allList.Add(effectCfg);
			}
		}

		public static EffectCfg Get(int key)
		{
			EffectCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, EffectCfg> GetAll()
		{
			return all;
		}

		public static List<EffectCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MultiLanguageCfg
	{
		public const string Path = "cfg.MultiLanguageCfg.oc";

		private static Dictionary<string, MultiLanguageCfg> all;

		private static List<MultiLanguageCfg> allList;

		public string id;

		public List<string> languages = new List<string>();

		public MultiLanguageCfg(Octets oc)
		{
			id = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				string item = oc.pop_string();
				languages.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<string, MultiLanguageCfg>();
			allList = new List<MultiLanguageCfg>();
			while (!octets.is_empty())
			{
				MultiLanguageCfg multiLanguageCfg = new MultiLanguageCfg(octets);
				all.Add(multiLanguageCfg.id, multiLanguageCfg);
				allList.Add(multiLanguageCfg);
			}
		}

		public static MultiLanguageCfg Get(string key)
		{
			MultiLanguageCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<string, MultiLanguageCfg> GetAll()
		{
			return all;
		}

		public static List<MultiLanguageCfg> GetAllList()
		{
			return allList;
		}
	}
}

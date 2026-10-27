using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MultiLanguageNameCfg
	{
		public const string Path = "cfg.MultiLanguageNameCfg.oc";

		private static Dictionary<int, MultiLanguageNameCfg> all;

		private static List<MultiLanguageNameCfg> allList;

		public int id;

		public List<string> languageNames = new List<string>();

		public MultiLanguageNameCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				string item = oc.pop_string();
				languageNames.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MultiLanguageNameCfg>();
			allList = new List<MultiLanguageNameCfg>();
			while (!octets.is_empty())
			{
				MultiLanguageNameCfg multiLanguageNameCfg = new MultiLanguageNameCfg(octets);
				all.Add(multiLanguageNameCfg.id, multiLanguageNameCfg);
				allList.Add(multiLanguageNameCfg);
			}
		}

		public static MultiLanguageNameCfg Get(int key)
		{
			MultiLanguageNameCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MultiLanguageNameCfg> GetAll()
		{
			return all;
		}

		public static List<MultiLanguageNameCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MultiLanguageIndexCfg
	{
		public const string Path = "cfg.MultiLanguageIndexCfg.oc";

		private static Dictionary<int, MultiLanguageIndexCfg> all;

		private static List<MultiLanguageIndexCfg> allList;

		public int id;

		public int index;

		public string language;

		public int nameIndex;

		public int nameLength;

		public MultiLanguageIndexCfg(Octets oc)
		{
			id = oc.pop_int();
			index = oc.pop_int();
			language = oc.pop_string();
			nameIndex = oc.pop_int();
			nameLength = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MultiLanguageIndexCfg>();
			allList = new List<MultiLanguageIndexCfg>();
			while (!octets.is_empty())
			{
				MultiLanguageIndexCfg multiLanguageIndexCfg = new MultiLanguageIndexCfg(octets);
				all.Add(multiLanguageIndexCfg.id, multiLanguageIndexCfg);
				allList.Add(multiLanguageIndexCfg);
			}
		}

		public static MultiLanguageIndexCfg Get(int key)
		{
			MultiLanguageIndexCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MultiLanguageIndexCfg> GetAll()
		{
			return all;
		}

		public static List<MultiLanguageIndexCfg> GetAllList()
		{
			return allList;
		}
	}
}

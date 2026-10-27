using System.Collections.Generic;
using Share;

namespace cfg
{
	public class OpenableBoxCfg
	{
		public const string Path = "cfg.OpenableBoxCfg.oc";

		private static Dictionary<int, OpenableBoxCfg> all;

		private static List<OpenableBoxCfg> allList;

		public int id;

		public int moneyType;

		public int money;

		public OpenableBoxCfg(Octets oc)
		{
			id = oc.pop_int();
			moneyType = oc.pop_int();
			money = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, OpenableBoxCfg>();
			allList = new List<OpenableBoxCfg>();
			while (!octets.is_empty())
			{
				OpenableBoxCfg openableBoxCfg = new OpenableBoxCfg(octets);
				all.Add(openableBoxCfg.id, openableBoxCfg);
				allList.Add(openableBoxCfg);
			}
		}

		public static OpenableBoxCfg Get(int key)
		{
			OpenableBoxCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, OpenableBoxCfg> GetAll()
		{
			return all;
		}

		public static List<OpenableBoxCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SmelterCfg
	{
		public const string Path = "cfg.SmelterCfg.oc";

		private static Dictionary<int, SmelterCfg> all;

		private static List<SmelterCfg> allList;

		public int id;

		public int type;

		public int needTime;

		public int targetItemId;

		public int targetNum;

		public SmelterCfg(Octets oc)
		{
			id = oc.pop_int();
			type = oc.pop_int();
			needTime = oc.pop_int();
			targetItemId = oc.pop_int();
			targetNum = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SmelterCfg>();
			allList = new List<SmelterCfg>();
			while (!octets.is_empty())
			{
				SmelterCfg smelterCfg = new SmelterCfg(octets);
				all.Add(smelterCfg.id, smelterCfg);
				allList.Add(smelterCfg);
			}
		}

		public static SmelterCfg Get(int key)
		{
			SmelterCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SmelterCfg> GetAll()
		{
			return all;
		}

		public static List<SmelterCfg> GetAllList()
		{
			return allList;
		}
	}
}

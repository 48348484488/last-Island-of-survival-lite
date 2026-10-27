using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ZiyuanRoadGenCfg
	{
		public const string Path = "cfg.ZiyuanRoadGenCfg.oc";

		private static Dictionary<string, ZiyuanRoadGenCfg> all;

		private static List<ZiyuanRoadGenCfg> allList;

		public string id;

		public int count;

		public ziyuanRoadShuaxin lajidui;

		public List<ziyuanRoadShuaxin> xiangzis = new List<ziyuanRoadShuaxin>();

		public ZiyuanRoadGenCfg(Octets oc)
		{
			id = oc.pop_string();
			count = oc.pop_int();
			lajidui = new ziyuanRoadShuaxin(oc);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ziyuanRoadShuaxin item = new ziyuanRoadShuaxin(oc);
				xiangzis.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<string, ZiyuanRoadGenCfg>();
			allList = new List<ZiyuanRoadGenCfg>();
			while (!octets.is_empty())
			{
				ZiyuanRoadGenCfg ziyuanRoadGenCfg = new ZiyuanRoadGenCfg(octets);
				all.Add(ziyuanRoadGenCfg.id, ziyuanRoadGenCfg);
				allList.Add(ziyuanRoadGenCfg);
			}
		}

		public static ZiyuanRoadGenCfg Get(string key)
		{
			ZiyuanRoadGenCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<string, ZiyuanRoadGenCfg> GetAll()
		{
			return all;
		}

		public static List<ZiyuanRoadGenCfg> GetAllList()
		{
			return allList;
		}
	}
}

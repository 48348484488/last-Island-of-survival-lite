using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ZiyuanGenCfg
	{
		public const string Path = "cfg.ZiyuanGenCfg.oc";

		private static Dictionary<string, ZiyuanGenCfg> all;

		private static List<ZiyuanGenCfg> allList;

		public string id;

		public ziyuanShuaxin kuang;

		public ziyuanShuaxin kuaiwu;

		public ZiyuanGenCfg(Octets oc)
		{
			id = oc.pop_string();
			kuang = new ziyuanShuaxin(oc);
			kuaiwu = new ziyuanShuaxin(oc);
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<string, ZiyuanGenCfg>();
			allList = new List<ZiyuanGenCfg>();
			while (!octets.is_empty())
			{
				ZiyuanGenCfg ziyuanGenCfg = new ZiyuanGenCfg(octets);
				all.Add(ziyuanGenCfg.id, ziyuanGenCfg);
				allList.Add(ziyuanGenCfg);
			}
		}

		public static ZiyuanGenCfg Get(string key)
		{
			ZiyuanGenCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<string, ZiyuanGenCfg> GetAll()
		{
			return all;
		}

		public static List<ZiyuanGenCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TrusteeshipCollectCfg
	{
		public const string Path = "cfg.TrusteeshipCollectCfg.oc";

		private static Dictionary<int, TrusteeshipCollectCfg> all;

		private static List<TrusteeshipCollectCfg> allList;

		public int id;

		public List<TrusteeshipCollectItem> items = new List<TrusteeshipCollectItem>();

		public TrusteeshipCollectCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TrusteeshipCollectItem item = new TrusteeshipCollectItem(oc);
				items.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TrusteeshipCollectCfg>();
			allList = new List<TrusteeshipCollectCfg>();
			while (!octets.is_empty())
			{
				TrusteeshipCollectCfg trusteeshipCollectCfg = new TrusteeshipCollectCfg(octets);
				all.Add(trusteeshipCollectCfg.id, trusteeshipCollectCfg);
				allList.Add(trusteeshipCollectCfg);
			}
		}

		public static TrusteeshipCollectCfg Get(int key)
		{
			TrusteeshipCollectCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TrusteeshipCollectCfg> GetAll()
		{
			return all;
		}

		public static List<TrusteeshipCollectCfg> GetAllList()
		{
			return allList;
		}
	}
}

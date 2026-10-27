using System.Collections.Generic;
using Share;

namespace cfg
{
	public class QrCodeCfg
	{
		public const string Path = "cfg.QrCodeCfg.oc";

		private static Dictionary<int, QrCodeCfg> all;

		private static List<QrCodeCfg> allList;

		public int id;

		public string packageName;

		public string icon;

		public string url;

		public QrCodeCfg(Octets oc)
		{
			id = oc.pop_int();
			packageName = oc.pop_string();
			icon = oc.pop_string();
			url = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, QrCodeCfg>();
			allList = new List<QrCodeCfg>();
			while (!octets.is_empty())
			{
				QrCodeCfg qrCodeCfg = new QrCodeCfg(octets);
				all.Add(qrCodeCfg.id, qrCodeCfg);
				allList.Add(qrCodeCfg);
			}
		}

		public static QrCodeCfg Get(int key)
		{
			QrCodeCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, QrCodeCfg> GetAll()
		{
			return all;
		}

		public static List<QrCodeCfg> GetAllList()
		{
			return allList;
		}
	}
}

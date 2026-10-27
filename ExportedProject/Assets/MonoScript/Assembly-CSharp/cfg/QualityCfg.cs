using System.Collections.Generic;
using Share;

namespace cfg
{
	public class QualityCfg
	{
		public const string Path = "cfg.QualityCfg.oc";

		private static Dictionary<int, QualityCfg> all;

		private static List<QualityCfg> allList;

		public int id;

		public string icon;

		public string color;

		public string iconForZhunPan;

		public string colorRGB;

		public QualityCfg(Octets oc)
		{
			id = oc.pop_int();
			icon = oc.pop_string();
			color = oc.pop_string();
			iconForZhunPan = oc.pop_string();
			colorRGB = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, QualityCfg>();
			allList = new List<QualityCfg>();
			while (!octets.is_empty())
			{
				QualityCfg qualityCfg = new QualityCfg(octets);
				all.Add(qualityCfg.id, qualityCfg);
				allList.Add(qualityCfg);
			}
		}

		public static QualityCfg Get(int key)
		{
			QualityCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, QualityCfg> GetAll()
		{
			return all;
		}

		public static List<QualityCfg> GetAllList()
		{
			return allList;
		}
	}
}

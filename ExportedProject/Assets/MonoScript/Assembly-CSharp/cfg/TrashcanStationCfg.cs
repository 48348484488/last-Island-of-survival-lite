using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TrashcanStationCfg
	{
		public const string Path = "cfg.TrashcanStationCfg.oc";

		private static Dictionary<int, TrashcanStationCfg> all;

		private static List<TrashcanStationCfg> allList;

		public int id;

		public string name;

		public TrashcanStationCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TrashcanStationCfg>();
			allList = new List<TrashcanStationCfg>();
			while (!octets.is_empty())
			{
				TrashcanStationCfg trashcanStationCfg = new TrashcanStationCfg(octets);
				all.Add(trashcanStationCfg.id, trashcanStationCfg);
				allList.Add(trashcanStationCfg);
			}
		}

		public static TrashcanStationCfg Get(int key)
		{
			TrashcanStationCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TrashcanStationCfg> GetAll()
		{
			return all;
		}

		public static List<TrashcanStationCfg> GetAllList()
		{
			return allList;
		}
	}
}

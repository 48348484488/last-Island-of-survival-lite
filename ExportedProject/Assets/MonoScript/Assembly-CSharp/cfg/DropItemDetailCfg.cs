using System.Collections.Generic;
using Share;

namespace cfg
{
	public class DropItemDetailCfg
	{
		public const string Path = "cfg.DropItemDetailCfg.oc";

		private static Dictionary<int, DropItemDetailCfg> all;

		private static List<DropItemDetailCfg> allList;

		public int id;

		public List<DropInfo> subItems = new List<DropInfo>();

		public DropItemDetailCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				DropInfo item = new DropInfo(oc);
				subItems.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, DropItemDetailCfg>();
			allList = new List<DropItemDetailCfg>();
			while (!octets.is_empty())
			{
				DropItemDetailCfg dropItemDetailCfg = new DropItemDetailCfg(octets);
				all.Add(dropItemDetailCfg.id, dropItemDetailCfg);
				allList.Add(dropItemDetailCfg);
			}
		}

		public static DropItemDetailCfg Get(int key)
		{
			DropItemDetailCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, DropItemDetailCfg> GetAll()
		{
			return all;
		}

		public static List<DropItemDetailCfg> GetAllList()
		{
			return allList;
		}
	}
}

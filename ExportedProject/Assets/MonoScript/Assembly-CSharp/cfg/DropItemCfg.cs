using System.Collections.Generic;
using Share;

namespace cfg
{
	public class DropItemCfg
	{
		public const string Path = "cfg.DropItemCfg.oc";

		private static Dictionary<int, DropItemCfg> all;

		private static List<DropItemCfg> allList;

		public int id;

		public int dropNum;

		public List<FixDrop> fixDrops = new List<FixDrop>();

		public bool relation;

		public List<DropDetailInfo> detailIds = new List<DropDetailInfo>();

		public DropItemCfg(Octets oc)
		{
			id = oc.pop_int();
			dropNum = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				FixDrop item = new FixDrop(oc);
				fixDrops.Add(item);
			}
			relation = oc.pop_boolean();
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				DropDetailInfo item2 = new DropDetailInfo(oc);
				detailIds.Add(item2);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, DropItemCfg>();
			allList = new List<DropItemCfg>();
			while (!octets.is_empty())
			{
				DropItemCfg dropItemCfg = new DropItemCfg(octets);
				all.Add(dropItemCfg.id, dropItemCfg);
				allList.Add(dropItemCfg);
			}
		}

		public static DropItemCfg Get(int key)
		{
			DropItemCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, DropItemCfg> GetAll()
		{
			return all;
		}

		public static List<DropItemCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class BulletCfg
	{
		public const string Path = "cfg.BulletCfg.oc";

		private static Dictionary<int, BulletCfg> all;

		private static List<BulletCfg> allList;

		public int id;

		public string name;

		public int chuanjia;

		public float roleDamageFactor;

		public BulletCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			chuanjia = oc.pop_int();
			roleDamageFactor = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, BulletCfg>();
			allList = new List<BulletCfg>();
			while (!octets.is_empty())
			{
				BulletCfg bulletCfg = new BulletCfg(octets);
				all.Add(bulletCfg.id, bulletCfg);
				allList.Add(bulletCfg);
			}
		}

		public static BulletCfg Get(int key)
		{
			BulletCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, BulletCfg> GetAll()
		{
			return all;
		}

		public static List<BulletCfg> GetAllList()
		{
			return allList;
		}
	}
}

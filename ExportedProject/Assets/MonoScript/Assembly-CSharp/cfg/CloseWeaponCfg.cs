using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CloseWeaponCfg
	{
		public const string Path = "cfg.CloseWeaponCfg.oc";

		private static Dictionary<int, CloseWeaponCfg> all;

		private static List<CloseWeaponCfg> allList;

		public int id;

		public string name;

		public int baseDamage;

		public int chuanjia;

		public List<int> sounds = new List<int>();

		public CloseWeaponCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			baseDamage = oc.pop_int();
			chuanjia = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				sounds.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CloseWeaponCfg>();
			allList = new List<CloseWeaponCfg>();
			while (!octets.is_empty())
			{
				CloseWeaponCfg closeWeaponCfg = new CloseWeaponCfg(octets);
				all.Add(closeWeaponCfg.id, closeWeaponCfg);
				allList.Add(closeWeaponCfg);
			}
		}

		public static CloseWeaponCfg Get(int key)
		{
			CloseWeaponCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CloseWeaponCfg> GetAll()
		{
			return all;
		}

		public static List<CloseWeaponCfg> GetAllList()
		{
			return allList;
		}
	}
}

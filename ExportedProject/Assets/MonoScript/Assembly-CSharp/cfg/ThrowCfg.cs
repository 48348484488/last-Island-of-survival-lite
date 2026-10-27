using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ThrowCfg
	{
		public const string Path = "cfg.ThrowCfg.oc";

		private static Dictionary<int, ThrowCfg> all;

		private static List<ThrowCfg> allList;

		public int id;

		public string name;

		public int type;

		public string path;

		public int laSound;

		public int boomSound;

		public int boomEffect;

		public int damage;

		public int speed;

		public float radius;

		public float maxRadius;

		public int chuanjia;

		public float delayTime;

		public ThrowCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			type = oc.pop_int();
			path = oc.pop_string();
			laSound = oc.pop_int();
			boomSound = oc.pop_int();
			boomEffect = oc.pop_int();
			damage = oc.pop_int();
			speed = oc.pop_int();
			radius = oc.pop_float();
			maxRadius = oc.pop_float();
			chuanjia = oc.pop_int();
			delayTime = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ThrowCfg>();
			allList = new List<ThrowCfg>();
			while (!octets.is_empty())
			{
				ThrowCfg throwCfg = new ThrowCfg(octets);
				all.Add(throwCfg.id, throwCfg);
				allList.Add(throwCfg);
			}
		}

		public static ThrowCfg Get(int key)
		{
			ThrowCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ThrowCfg> GetAll()
		{
			return all;
		}

		public static List<ThrowCfg> GetAllList()
		{
			return allList;
		}
	}
}

using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TurretCfg
	{
		public const string Path = "cfg.TurretCfg.oc";

		private static Dictionary<int, TurretCfg> all;

		private static List<TurretCfg> allList;

		public int id;

		public string name;

		public int type;

		public string icon;

		public string desc;

		public string modelPath;

		public string dropModel;

		public int clipNum;

		public List<int> bulletMaxNum = new List<int>();

		public float checkRange;

		public float levelRange;

		public float fireProbability;

		public int gunId;

		public float defaultRefreshTargePartTime;

		public float fireRange;

		public string fireAnimation;

		public string fireEffect;

		public string killIcon;

		public TurretCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			type = oc.pop_int();
			icon = oc.pop_string();
			desc = oc.pop_string();
			modelPath = oc.pop_string();
			dropModel = oc.pop_string();
			clipNum = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				bulletMaxNum.Add(item);
			}
			checkRange = oc.pop_float();
			levelRange = oc.pop_float();
			fireProbability = oc.pop_float();
			gunId = oc.pop_int();
			defaultRefreshTargePartTime = oc.pop_float();
			fireRange = oc.pop_float();
			fireAnimation = oc.pop_string();
			fireEffect = oc.pop_string();
			killIcon = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TurretCfg>();
			allList = new List<TurretCfg>();
			while (!octets.is_empty())
			{
				TurretCfg turretCfg = new TurretCfg(octets);
				all.Add(turretCfg.id, turretCfg);
				allList.Add(turretCfg);
			}
		}

		public static TurretCfg Get(int key)
		{
			TurretCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TurretCfg> GetAll()
		{
			return all;
		}

		public static List<TurretCfg> GetAllList()
		{
			return allList;
		}
	}
}

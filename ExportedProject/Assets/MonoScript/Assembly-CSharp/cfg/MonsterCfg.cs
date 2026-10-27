using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MonsterCfg
	{
		public const string Path = "cfg.MonsterCfg.oc";

		private static Dictionary<int, MonsterCfg> all;

		private static List<MonsterCfg> allList;

		public int id;

		public string name;

		public string modelPath;

		public float walkSpeed;

		public float runSpeed;

		public int type;

		public int weaponId;

		public int bulletId;

		public float fistBaseDamage;

		public float freeMoveRange;

		public float defenceRange;

		public float attackRange;

		public float followRange;

		public int revengeTime;

		public int exp;

		public int drop;

		public int hp;

		public int headDefence;

		public int bodyDefence;

		public float attackSpaceTime;

		public int attackSound;

		public int dieSound;

		public string headIcon;

		public string headIconBg;

		public int chuanJia;

		public float extraDamageFactor;

		public int freeSound;

		public int waitTime;

		public bool canMove;

		public int backAttackDis;

		public int zhianDis;

		public MonsterCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			modelPath = oc.pop_string();
			walkSpeed = oc.pop_float();
			runSpeed = oc.pop_float();
			type = oc.pop_int();
			weaponId = oc.pop_int();
			bulletId = oc.pop_int();
			fistBaseDamage = oc.pop_float();
			freeMoveRange = oc.pop_float();
			defenceRange = oc.pop_float();
			attackRange = oc.pop_float();
			followRange = oc.pop_float();
			revengeTime = oc.pop_int();
			exp = oc.pop_int();
			drop = oc.pop_int();
			hp = oc.pop_int();
			headDefence = oc.pop_int();
			bodyDefence = oc.pop_int();
			attackSpaceTime = oc.pop_float();
			attackSound = oc.pop_int();
			dieSound = oc.pop_int();
			headIcon = oc.pop_string();
			headIconBg = oc.pop_string();
			chuanJia = oc.pop_int();
			extraDamageFactor = oc.pop_float();
			freeSound = oc.pop_int();
			waitTime = oc.pop_int();
			canMove = oc.pop_boolean();
			backAttackDis = oc.pop_int();
			zhianDis = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MonsterCfg>();
			allList = new List<MonsterCfg>();
			while (!octets.is_empty())
			{
				MonsterCfg monsterCfg = new MonsterCfg(octets);
				all.Add(monsterCfg.id, monsterCfg);
				allList.Add(monsterCfg);
			}
		}

		public static MonsterCfg Get(int key)
		{
			MonsterCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MonsterCfg> GetAll()
		{
			return all;
		}

		public static List<MonsterCfg> GetAllList()
		{
			return allList;
		}
	}
}

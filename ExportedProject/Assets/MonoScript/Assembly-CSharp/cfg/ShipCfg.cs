using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ShipCfg
	{
		public const string Path = "cfg.ShipCfg.oc";

		private static Dictionary<int, ShipCfg> all;

		private static List<ShipCfg> allList;

		public int id;

		public float mass;

		public int maxHp;

		public float maxSpeed;

		public float engineTorque;

		public float turnSpeed;

		public int maxFuel;

		public float fuelConsume;

		public List<int> soundIds = new List<int>();

		public float hitByShipPlayerDropHpRate1;

		public float hitByShipPlayerDropHpRate2;

		public float hitShipPlayerDropHpRate1;

		public float hitShipPlayerDropHpRate2;

		public float hitShipDropHpRate1;

		public float hitShipDropHpRate2;

		public ShipCfg(Octets oc)
		{
			id = oc.pop_int();
			mass = oc.pop_float();
			maxHp = oc.pop_int();
			maxSpeed = oc.pop_float();
			engineTorque = oc.pop_float();
			turnSpeed = oc.pop_float();
			maxFuel = oc.pop_int();
			fuelConsume = oc.pop_float();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				soundIds.Add(item);
			}
			hitByShipPlayerDropHpRate1 = oc.pop_float();
			hitByShipPlayerDropHpRate2 = oc.pop_float();
			hitShipPlayerDropHpRate1 = oc.pop_float();
			hitShipPlayerDropHpRate2 = oc.pop_float();
			hitShipDropHpRate1 = oc.pop_float();
			hitShipDropHpRate2 = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ShipCfg>();
			allList = new List<ShipCfg>();
			while (!octets.is_empty())
			{
				ShipCfg shipCfg = new ShipCfg(octets);
				all.Add(shipCfg.id, shipCfg);
				allList.Add(shipCfg);
			}
		}

		public static ShipCfg Get(int key)
		{
			ShipCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ShipCfg> GetAll()
		{
			return all;
		}

		public static List<ShipCfg> GetAllList()
		{
			return allList;
		}
	}
}

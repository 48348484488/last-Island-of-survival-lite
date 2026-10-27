using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CarCfg
	{
		public const string Path = "cfg.CarCfg.oc";

		private static Dictionary<int, CarCfg> all;

		private static List<CarCfg> allList;

		public int id;

		public float mass;

		public int maxHp;

		public float steerAngle;

		public float highspeedsteerAngle;

		public float highspeedsteerAngleAtspeed;

		public float antiRollFrontHorizontal;

		public float antiRollRearHorizontal;

		public int driveType;

		public int gearNum;

		public float switchGearTime;

		public float engineTorque;

		public float brakeTorque;

		public float maxSpeed;

		public float maxSpeedWithNitrogen;

		public float comX;

		public float comY;

		public float comZ;

		public int maxFuel;

		public float fuelConsume;

		public float extraFuelConsume;

		public float extraEngineTorqueRate;

		public List<int> soundIds = new List<int>();

		public float hitByCarPlayerDropHpRate1;

		public float hitByCarPlayerDropHpRate2;

		public float jumpCarPlayerDropHpRate1;

		public float jumpCarPlayerDropHpRate2;

		public float hitCarPlayerDropHpRate1;

		public float hitCarPlayerDropHpRate2;

		public float hitCarDropHpRate1;

		public float hitCarDropHpRate2;

		public CarCfg(Octets oc)
		{
			id = oc.pop_int();
			mass = oc.pop_float();
			maxHp = oc.pop_int();
			steerAngle = oc.pop_float();
			highspeedsteerAngle = oc.pop_float();
			highspeedsteerAngleAtspeed = oc.pop_float();
			antiRollFrontHorizontal = oc.pop_float();
			antiRollRearHorizontal = oc.pop_float();
			driveType = oc.pop_int();
			gearNum = oc.pop_int();
			switchGearTime = oc.pop_float();
			engineTorque = oc.pop_float();
			brakeTorque = oc.pop_float();
			maxSpeed = oc.pop_float();
			maxSpeedWithNitrogen = oc.pop_float();
			comX = oc.pop_float();
			comY = oc.pop_float();
			comZ = oc.pop_float();
			maxFuel = oc.pop_int();
			fuelConsume = oc.pop_float();
			extraFuelConsume = oc.pop_float();
			extraEngineTorqueRate = oc.pop_float();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				soundIds.Add(item);
			}
			hitByCarPlayerDropHpRate1 = oc.pop_float();
			hitByCarPlayerDropHpRate2 = oc.pop_float();
			jumpCarPlayerDropHpRate1 = oc.pop_float();
			jumpCarPlayerDropHpRate2 = oc.pop_float();
			hitCarPlayerDropHpRate1 = oc.pop_float();
			hitCarPlayerDropHpRate2 = oc.pop_float();
			hitCarDropHpRate1 = oc.pop_float();
			hitCarDropHpRate2 = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CarCfg>();
			allList = new List<CarCfg>();
			while (!octets.is_empty())
			{
				CarCfg carCfg = new CarCfg(octets);
				all.Add(carCfg.id, carCfg);
				allList.Add(carCfg);
			}
		}

		public static CarCfg Get(int key)
		{
			CarCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CarCfg> GetAll()
		{
			return all;
		}

		public static List<CarCfg> GetAllList()
		{
			return allList;
		}
	}
}

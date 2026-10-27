using System.Collections.Generic;
using Share;

namespace cfg
{
	public class PlaneCfg
	{
		public const string Path = "cfg.PlaneCfg.oc";

		private static Dictionary<int, PlaneCfg> all;

		private static List<PlaneCfg> allList;

		public int id;

		public float mass;

		public int maxHp;

		public float upChangeSpeedRate;

		public float maxUpChangeSpeedRate;

		public float tiltForce;

		public float tiltSpeed;

		public float forwardForce;

		public float forwardSpeed;

		public float turnSpeed;

		public float maxFlyHeight;

		public float turnForce;

		public int maxFuel;

		public float fuelConsume;

		public List<int> soundIds = new List<int>();

		public float hitByPlanePlayerDropHpRate1;

		public float hitByPlanePlayerDropHpRate2;

		public float hitPlanePlayerDropHpRate1;

		public float hitPlanePlayerDropHpRate2;

		public float hitPlaneDropHpRate1;

		public float hitPlaneDropHpRate2;

		public float undercarriagHitPlayerDropHpRate1;

		public float undercarriagHitPlayerDropHpRate2;

		public float undercarriagHitDropHpRate1;

		public float undercarriagHitDropHpRate2;

		public PlaneCfg(Octets oc)
		{
			id = oc.pop_int();
			mass = oc.pop_float();
			maxHp = oc.pop_int();
			upChangeSpeedRate = oc.pop_float();
			maxUpChangeSpeedRate = oc.pop_float();
			tiltForce = oc.pop_float();
			tiltSpeed = oc.pop_float();
			forwardForce = oc.pop_float();
			forwardSpeed = oc.pop_float();
			turnSpeed = oc.pop_float();
			maxFlyHeight = oc.pop_float();
			turnForce = oc.pop_float();
			maxFuel = oc.pop_int();
			fuelConsume = oc.pop_float();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				soundIds.Add(item);
			}
			hitByPlanePlayerDropHpRate1 = oc.pop_float();
			hitByPlanePlayerDropHpRate2 = oc.pop_float();
			hitPlanePlayerDropHpRate1 = oc.pop_float();
			hitPlanePlayerDropHpRate2 = oc.pop_float();
			hitPlaneDropHpRate1 = oc.pop_float();
			hitPlaneDropHpRate2 = oc.pop_float();
			undercarriagHitPlayerDropHpRate1 = oc.pop_float();
			undercarriagHitPlayerDropHpRate2 = oc.pop_float();
			undercarriagHitDropHpRate1 = oc.pop_float();
			undercarriagHitDropHpRate2 = oc.pop_float();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, PlaneCfg>();
			allList = new List<PlaneCfg>();
			while (!octets.is_empty())
			{
				PlaneCfg planeCfg = new PlaneCfg(octets);
				all.Add(planeCfg.id, planeCfg);
				allList.Add(planeCfg);
			}
		}

		public static PlaneCfg Get(int key)
		{
			PlaneCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, PlaneCfg> GetAll()
		{
			return all;
		}

		public static List<PlaneCfg> GetAllList()
		{
			return allList;
		}
	}
}

using Share;

namespace gs.smelter.scmsg
{
	public class UseItem : Marshal
	{
		public int id;

		public int instanceId;

		public int num;

		public int fuelTime;

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(instanceId);
			oc.push(num);
			oc.push(fuelTime);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			instanceId = oc.pop_int();
			num = oc.pop_int();
			fuelTime = oc.pop_int();
			return oc;
		}
	}
}

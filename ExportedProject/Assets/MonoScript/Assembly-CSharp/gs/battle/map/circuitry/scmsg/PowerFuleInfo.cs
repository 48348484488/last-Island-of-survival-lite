using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class PowerFuleInfo : Marshal
	{
		public int fuelId;

		public int fuelNum;

		public Octets marshal(Octets oc)
		{
			oc.push(fuelId);
			oc.push(fuelNum);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			fuelId = oc.pop_int();
			fuelNum = oc.pop_int();
			return oc;
		}
	}
}

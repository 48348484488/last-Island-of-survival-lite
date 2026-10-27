using Share;

namespace gs.battle.drop.scmsg
{
	public class ItemInfo : Marshal
	{
		public long objId;

		public int itemId;

		public int number;

		public int duration;

		public Octets extraInfo = new Octets();

		public Octets marshal(Octets oc)
		{
			oc.push(objId);
			oc.push(itemId);
			oc.push(number);
			oc.push(duration);
			oc.push(extraInfo);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			objId = oc.pop_long();
			itemId = oc.pop_int();
			number = oc.pop_int();
			duration = oc.pop_int();
			extraInfo = oc.pop_octets();
			return oc;
		}
	}
}

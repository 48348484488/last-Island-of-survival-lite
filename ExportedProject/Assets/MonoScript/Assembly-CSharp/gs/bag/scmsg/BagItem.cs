using Share;

namespace gs.bag.scmsg
{
	public class BagItem : Marshal
	{
		public int instanceId;

		public int itemId;

		public int number;

		public int duration;

		public int timeout;

		public bool isBind;

		public Octets extraInfo = new Octets();

		public Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(itemId);
			oc.push(number);
			oc.push(duration);
			oc.push(timeout);
			oc.push(isBind);
			oc.push(extraInfo);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			itemId = oc.pop_int();
			number = oc.pop_int();
			duration = oc.pop_int();
			timeout = oc.pop_int();
			isBind = oc.pop_bool();
			extraInfo = oc.pop_octets();
			return oc;
		}
	}
}

using Share;

namespace gs.battle.scmsg
{
	public class ManorChestItem : Marshal
	{
		public int itemId;

		public int number;

		public Octets marshal(Octets oc)
		{
			oc.push(itemId);
			oc.push(number);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

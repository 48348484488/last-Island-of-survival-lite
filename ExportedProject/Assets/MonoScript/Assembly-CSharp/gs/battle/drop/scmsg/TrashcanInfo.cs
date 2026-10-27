using Share;

namespace gs.battle.drop.scmsg
{
	public class TrashcanInfo : Marshal
	{
		public byte index;

		public long instanceId;

		public int typeId;

		public int hp;

		public bool isBox;

		public Octets marshal(Octets oc)
		{
			oc.push(index);
			oc.push(instanceId);
			oc.push(typeId);
			oc.push(hp);
			oc.push(isBox);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			index = oc.pop_byte();
			instanceId = oc.pop_long();
			typeId = oc.pop_int();
			hp = oc.pop_int();
			isBox = oc.pop_bool();
			return oc;
		}
	}
}

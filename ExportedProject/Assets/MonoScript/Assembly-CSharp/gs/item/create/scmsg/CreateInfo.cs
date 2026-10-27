using Share;

namespace gs.item.create.scmsg
{
	public class CreateInfo : Marshal
	{
		public int createId;

		public int createNum;

		public Octets marshal(Octets oc)
		{
			oc.push(createId);
			oc.push(createNum);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			createId = oc.pop_int();
			createNum = oc.pop_int();
			return oc;
		}
	}
}

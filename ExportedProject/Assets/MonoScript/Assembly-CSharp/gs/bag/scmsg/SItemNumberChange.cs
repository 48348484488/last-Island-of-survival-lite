using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SItemNumberChange : Message
	{
		public delegate void Handler(SItemNumberChange msg);

		public const int TYPE = 8391652;

		public static Handler handler;

		public int itemId;

		public int change;

		public bool isBind;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391652;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			oc.push(change);
			oc.push(isBind);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			change = oc.pop_int();
			isBind = oc.pop_bool();
			return oc;
		}
	}
}

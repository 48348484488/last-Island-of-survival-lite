using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CSetShowEquip : Message
	{
		public delegate void Handler(CSetShowEquip msg);

		public const int TYPE = 8391653;

		public static Handler handler;

		public bool showEquip;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391653;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(showEquip);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			showEquip = oc.pop_bool();
			return oc;
		}
	}
}

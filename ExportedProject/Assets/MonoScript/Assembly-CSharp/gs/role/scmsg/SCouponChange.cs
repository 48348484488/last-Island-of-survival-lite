using Net;
using Share;

namespace gs.role.scmsg
{
	public class SCouponChange : Message
	{
		public delegate void Handler(SCouponChange msg);

		public const int TYPE = 4197310;

		public static Handler handler;

		public int coupon;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197310;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(coupon);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			coupon = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.activity.scmsg
{
	public class SMonthSignInfo : Message
	{
		public delegate void Handler(SMonthSignInfo msg);

		public const int TYPE = 29363131;

		public static Handler handler;

		public int loginDays;

		public int signedDays;

		public bool canSignIn;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 29363131;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(loginDays);
			oc.push(signedDays);
			oc.push(canSignIn);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			loginDays = oc.pop_int();
			signedDays = oc.pop_int();
			canSignIn = oc.pop_bool();
			return oc;
		}
	}
}

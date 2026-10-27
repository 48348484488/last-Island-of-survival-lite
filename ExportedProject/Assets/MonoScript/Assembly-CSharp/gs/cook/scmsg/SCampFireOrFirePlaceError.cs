using Net;
using Share;

namespace gs.cook.scmsg
{
	public class SCampFireOrFirePlaceError : Message
	{
		public delegate void Handler(SCampFireOrFirePlaceError msg);

		public const int TYPE = 20974527;

		public static Handler handler;

		public const int CAMPFIREORFIREPLACE_NOT_EXIST = 1;

		public const int POWER_NOT_ENOUGH = 2;

		public const int ITEM_NOT_ENOUGH = 3;

		public const int OUT_OF_BAGSIZE = 4;

		public const int PARAM_IS_WRONG = 5;

		public const int IS_ALREADY_START = 6;

		public const int IS_NOT_START = 7;

		public const int ITEM_NOT_GET = 8;

		public int code;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 20974527;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(code);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.smelter.scmsg
{
	public class SSmelterError : Message
	{
		public delegate void Handler(SSmelterError msg);

		public const int TYPE = 17828801;

		public static Handler handler;

		public const int SMELTER_NOT_EXIST = 1;

		public const int OUT_OF_SIZE = 2;

		public const int POWER_NOT_ENOUGH = 3;

		public const int OUT_OF_SMELTER = 4;

		public const int ITEM_NOT_ENOUGH = 5;

		public const int OUT_OF_BAGSIZE = 6;

		public const int PARAM_IS_WRONG = 7;

		public const int IS_ALREADY_START = 8;

		public const int IS_NOT_START = 9;

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
			return 17828801;
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

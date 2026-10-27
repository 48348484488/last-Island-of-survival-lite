using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class SWorkbenchError : Message
	{
		public delegate void Handler(SWorkbenchError msg);

		public const int TYPE = 16780227;

		public static Handler handler;

		public const int WORKBENCH_NOT_EXIST = 1;

		public const int IS_NOT_PERMISSIONS = 2;

		public const int LEVEL_NOT_ENOUGH = 3;

		public const int CFG_NOT_EXIST = 4;

		public const int IS_MAX_LEVEL = 5;

		public const int ITEM_NOT_ENOUGH = 6;

		public const int OUT_OF_BAGSIZE = 7;

		public const int IS_ALREADY_GET = 8;

		public const int PARAM_IS_WRONG = 9;

		public const int IS_NOT_FINISH = 10;

		public const int ITEM_NOT_EXIST = 11;

		public const int CAN_NOT_OPERATION = 12;

		public const int MONEY_NOT_ENOUGH = 13;

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
			return 16780227;
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

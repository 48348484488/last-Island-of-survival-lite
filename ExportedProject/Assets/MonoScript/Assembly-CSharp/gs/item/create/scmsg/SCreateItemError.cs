using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class SCreateItemError : Message
	{
		public delegate void Handler(SCreateItemError msg);

		public const int TYPE = 14683081;

		public static Handler handler;

		public const int DRAWING_NOT_EXIST = 1;

		public const int OUT_OF_SIZE = 2;

		public const int ROLE_LEVEL_ERROR = 3;

		public const int WORKBENCH_LEVEL_ERROR = 4;

		public const int OUT_OF_WORKBENCH = 5;

		public const int ITEM_NOT_ENOUGH = 6;

		public const int OUT_OF_BAGSIZE = 7;

		public const int OUT_OF_MAXNUM = 8;

		public const int PARAM_IS_WRONG = 9;

		public const int DRAWING_IS_DEFAULT = 10;

		public const int IS_ALREADY_HAVE = 11;

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
			return 14683081;
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

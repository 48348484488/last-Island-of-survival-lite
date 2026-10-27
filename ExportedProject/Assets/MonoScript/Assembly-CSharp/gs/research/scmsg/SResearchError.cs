using Net;
using Share;

namespace gs.research.scmsg
{
	public class SResearchError : Message
	{
		public delegate void Handler(SResearchError msg);

		public const int TYPE = 18877374;

		public static Handler handler;

		public const int RESEARCH_NOT_EXIST = 1;

		public const int IS_NOT_PERMISSIONS = 2;

		public const int CFG_NOT_EXIST = 3;

		public const int ITEM_NOT_ENOUGH = 4;

		public const int OUT_OF_BAGSIZE = 5;

		public const int IS_ALREADY_GET = 6;

		public const int PARAM_IS_WRONG = 7;

		public const int IS_NOT_FINISH = 8;

		public const int OUT_OF_MAX = 9;

		public const int IS_ALREADY_START = 10;

		public const int IS_NOT_START = 11;

		public const int ITEM_IS_FINSHED = 12;

		public const int DURATION__NOT_ENOUGH = 13;

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
			return 18877374;
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

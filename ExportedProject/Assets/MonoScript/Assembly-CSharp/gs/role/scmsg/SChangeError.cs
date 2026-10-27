using Net;
using Share;

namespace gs.role.scmsg
{
	public class SChangeError : Message
	{
		public delegate void Handler(SChangeError msg);

		public const int TYPE = 4197308;

		public static Handler handler;

		public const int NAME_SUCCESS = 1;

		public const int NAME_SPECIAL_CHARACTER = 2;

		public const int NAME_TOO_SHORT = 3;

		public const int NAME_TOO_LONG = 4;

		public const int NAME_IS_VISITOR = 5;

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
			return 4197308;
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

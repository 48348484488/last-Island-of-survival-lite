using Net;
using Share;

namespace gs.online.scmsg
{
	public class SLoginError : Message
	{
		public delegate void Handler(SLoginError msg);

		public const int TYPE = 2100155;

		public static Handler handler;

		public const int NO_ERROR = 0;

		public const int NAME_SENSITIVE_WORLD = 1;

		public const int NAME_TOO_SHORT = 2;

		public const int NAME_TOO_LONG = 3;

		public const int ACCOUNT_SPECIAL_CHARACTER = 4;

		public const int ACCOUNT_TOO_SHORT = 5;

		public const int ACCOUNT_TOO_LONG = 6;

		public const int VERSION_error = 7;

		public const int NAME_BE_USED = 8;

		public const int ReLogin_SessionKey_Error = 9;

		public const int SERVER_OVERLOAD = 10;

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
			return 2100155;
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

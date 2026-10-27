using Net;
using Share;

namespace gs.role.scmsg
{
	public class SChangeName : Message
	{
		public delegate void Handler(SChangeName msg);

		public const int TYPE = 4197315;

		public static Handler handler;

		public const int NAME_SENSITIVE_WORLD = 1;

		public const int NAME_TOO_SHORT = 2;

		public const int NAME_TOO_LONG = 3;

		public const int NAME_BE_USED = 8;

		public const int Success = 0;

		public int result;

		public string name = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197315;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(result);
			oc.push(name);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			result = oc.pop_int();
			name = oc.pop_string();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.online.scmsg
{
	public class CAutoLoginRole : Message
	{
		public delegate void Handler(CAutoLoginRole msg);

		public const int TYPE = 2100158;

		public static Handler handler;

		public const int IOS = 0;

		public const int ANDROID = 1;

		public const int OTHER = -1;

		public bool reLogin;

		public string account = string.Empty;

		public string sessionKey = string.Empty;

		public string version = string.Empty;

		public string code_version = string.Empty;

		public string phoneID = string.Empty;

		public int platformType;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100158;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(reLogin);
			oc.push(account);
			oc.push(sessionKey);
			oc.push(version);
			oc.push(code_version);
			oc.push(phoneID);
			oc.push(platformType);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			reLogin = oc.pop_bool();
			account = oc.pop_string();
			sessionKey = oc.pop_string();
			version = oc.pop_string();
			code_version = oc.pop_string();
			phoneID = oc.pop_string();
			platformType = oc.pop_int();
			return oc;
		}
	}
}

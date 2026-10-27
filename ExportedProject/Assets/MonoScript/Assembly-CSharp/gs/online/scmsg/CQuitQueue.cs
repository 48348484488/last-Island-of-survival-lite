using Net;
using Share;

namespace gs.online.scmsg
{
	public class CQuitQueue : Message
	{
		public delegate void Handler(CQuitQueue msg);

		public const int TYPE = 2100167;

		public static Handler handler;

		public string account = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100167;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(account);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			account = oc.pop_string();
			return oc;
		}
	}
}

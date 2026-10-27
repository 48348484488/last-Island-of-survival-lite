using Net;
using Share;

namespace gs.role.scmsg
{
	public class SChangeSignature : Message
	{
		public delegate void Handler(SChangeSignature msg);

		public const int TYPE = 4197317;

		public static Handler handler;

		public string str = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197317;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(str);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			str = oc.pop_string();
			return oc;
		}
	}
}

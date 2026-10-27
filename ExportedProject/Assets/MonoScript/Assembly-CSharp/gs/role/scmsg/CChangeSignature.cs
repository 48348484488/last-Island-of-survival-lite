using Net;
using Share;

namespace gs.role.scmsg
{
	public class CChangeSignature : Message
	{
		public delegate void Handler(CChangeSignature msg);

		public const int TYPE = 4197316;

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
			return 4197316;
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

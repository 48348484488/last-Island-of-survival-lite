using Net;
using Share;

namespace gs.chat.scmsg
{
	public class SPublicMsg : Message
	{
		public delegate void Handler(SPublicMsg msg);

		public const int TYPE = 9440185;

		public static Handler handler;

		public MsgBean info = new MsgBean();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440185;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(info);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.chat.scmsg
{
	public class SBigHorn : Message
	{
		public delegate void Handler(SBigHorn msg);

		public const int TYPE = 9440186;

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
			return 9440186;
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

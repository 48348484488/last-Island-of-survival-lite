using Net;
using Share;

namespace gs.chat.scmsg
{
	public class SPrivateMsg : Message
	{
		public delegate void Handler(SPrivateMsg msg);

		public const int TYPE = 9440189;

		public static Handler handler;

		public PrivateBean bean = new PrivateBean();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440189;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(bean);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(bean);
			return oc;
		}
	}
}

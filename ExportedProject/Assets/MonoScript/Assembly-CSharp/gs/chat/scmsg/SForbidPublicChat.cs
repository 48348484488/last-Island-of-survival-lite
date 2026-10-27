using Net;
using Share;

namespace gs.chat.scmsg
{
	public class SForbidPublicChat : Message
	{
		public delegate void Handler(SForbidPublicChat msg);

		public const int TYPE = 9440187;

		public static Handler handler;

		public int dueTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 9440187;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(dueTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			dueTime = oc.pop_int();
			return oc;
		}
	}
}

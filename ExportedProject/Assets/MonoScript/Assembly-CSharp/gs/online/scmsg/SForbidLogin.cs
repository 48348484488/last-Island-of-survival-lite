using Net;
using Share;

namespace gs.online.scmsg
{
	public class SForbidLogin : Message
	{
		public delegate void Handler(SForbidLogin msg);

		public const int TYPE = 2100156;

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
			return 2100156;
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

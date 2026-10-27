using Net;
using Share;

namespace gs.online.scmsg
{
	public class Ping_Gs_Client : Message
	{
		public delegate void Handler(Ping_Gs_Client msg);

		public const int TYPE = 2100161;

		public static Handler handler;

		public int time;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100161;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(time);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			time = oc.pop_int();
			return oc;
		}
	}
}

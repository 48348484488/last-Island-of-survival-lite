using Net;
using Share;

namespace gs.online.scmsg
{
	public class SServerInfo : Message
	{
		public delegate void Handler(SServerInfo msg);

		public const int TYPE = 2100165;

		public static Handler handler;

		public int serverId;

		public int time;

		public int serverOpenTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100165;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(serverId);
			oc.push(time);
			oc.push(serverOpenTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			serverId = oc.pop_int();
			time = oc.pop_int();
			serverOpenTime = oc.pop_int();
			return oc;
		}
	}
}

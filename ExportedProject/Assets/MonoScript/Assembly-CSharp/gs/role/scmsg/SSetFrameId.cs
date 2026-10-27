using Net;
using Share;

namespace gs.role.scmsg
{
	public class SSetFrameId : Message
	{
		public delegate void Handler(SSetFrameId msg);

		public const int TYPE = 4197319;

		public static Handler handler;

		public int frameId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197319;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(frameId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			frameId = oc.pop_int();
			return oc;
		}
	}
}

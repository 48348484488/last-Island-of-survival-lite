using Net;
using Share;

namespace gs.role.scmsg
{
	public class CSetFrameId : Message
	{
		public delegate void Handler(CSetFrameId msg);

		public const int TYPE = 4197318;

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
			return 4197318;
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

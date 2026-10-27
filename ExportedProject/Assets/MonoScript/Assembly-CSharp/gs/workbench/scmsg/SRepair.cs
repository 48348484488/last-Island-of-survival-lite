using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class SRepair : Message
	{
		public delegate void Handler(SRepair msg);

		public const int TYPE = 16780221;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780221;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}

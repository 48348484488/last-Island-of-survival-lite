using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class CCancelDevelopment : Message
	{
		public delegate void Handler(CCancelDevelopment msg);

		public const int TYPE = 16780223;

		public static Handler handler;

		public long workbenchId;

		public int developmentId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780223;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			oc.push(developmentId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			developmentId = oc.pop_int();
			return oc;
		}
	}
}

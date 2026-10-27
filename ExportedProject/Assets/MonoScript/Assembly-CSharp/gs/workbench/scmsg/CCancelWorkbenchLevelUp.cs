using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class CCancelWorkbenchLevelUp : Message
	{
		public delegate void Handler(CCancelWorkbenchLevelUp msg);

		public const int TYPE = 16780219;

		public static Handler handler;

		public long workbenchId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780219;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class CGetItem : Message
	{
		public delegate void Handler(CGetItem msg);

		public const int TYPE = 16780225;

		public static Handler handler;

		public long workbenchId;

		public int workbenchLevel;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780225;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			oc.push(workbenchLevel);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			workbenchLevel = oc.pop_int();
			return oc;
		}
	}
}

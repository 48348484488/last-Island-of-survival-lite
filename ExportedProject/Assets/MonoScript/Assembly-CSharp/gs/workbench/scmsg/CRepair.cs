using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class CRepair : Message
	{
		public delegate void Handler(CRepair msg);

		public const int TYPE = 16780220;

		public static Handler handler;

		public long workbenchId;

		public int repairId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780220;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			oc.push(repairId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			repairId = oc.pop_int();
			return oc;
		}
	}
}

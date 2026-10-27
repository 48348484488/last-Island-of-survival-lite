using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class CDevelopment : Message
	{
		public delegate void Handler(CDevelopment msg);

		public const int TYPE = 16780222;

		public static Handler handler;

		public long workbenchId;

		public int developmentId;

		public bool isUsedAssist;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780222;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(workbenchId);
			oc.push(developmentId);
			oc.push(isUsedAssist);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			workbenchId = oc.pop_long();
			developmentId = oc.pop_int();
			isUsedAssist = oc.pop_bool();
			return oc;
		}
	}
}

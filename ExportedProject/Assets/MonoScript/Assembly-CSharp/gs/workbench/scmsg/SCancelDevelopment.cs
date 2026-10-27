using Net;
using Share;

namespace gs.workbench.scmsg
{
	public class SCancelDevelopment : Message
	{
		public delegate void Handler(SCancelDevelopment msg);

		public const int TYPE = 16780224;

		public static Handler handler;

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
			return 16780224;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(developmentId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			developmentId = oc.pop_int();
			return oc;
		}
	}
}

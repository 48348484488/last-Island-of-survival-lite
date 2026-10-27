using Net;
using Share;
using gs.drop.scmsg;

namespace gs.workbench.scmsg
{
	public class SGetItem : Message
	{
		public delegate void Handler(SGetItem msg);

		public const int TYPE = 16780226;

		public static Handler handler;

		public int developmentId;

		public DropDetail dropDetail = new DropDetail();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 16780226;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(developmentId);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			developmentId = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}

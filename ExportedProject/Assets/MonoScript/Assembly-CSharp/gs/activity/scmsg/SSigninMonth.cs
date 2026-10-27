using Net;
using Share;
using gs.drop.scmsg;

namespace gs.activity.scmsg
{
	public class SSigninMonth : Message
	{
		public delegate void Handler(SSigninMonth msg);

		public const int TYPE = 29363133;

		public static Handler handler;

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
			return 29363133;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(dropDetail);
			return oc;
		}
	}
}

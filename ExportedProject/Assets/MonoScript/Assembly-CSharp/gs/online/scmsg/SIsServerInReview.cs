using Net;
using Share;

namespace gs.online.scmsg
{
	public class SIsServerInReview : Message
	{
		public delegate void Handler(SIsServerInReview msg);

		public const int TYPE = 2100159;

		public static Handler handler;

		public bool isServerInReview;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 2100159;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(isServerInReview);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			isServerInReview = oc.pop_bool();
			return oc;
		}
	}
}

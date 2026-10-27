using Net;
using Share;

namespace gs.bag.scmsg
{
	public class COpenBox : Message
	{
		public delegate void Handler(COpenBox msg);

		public const int TYPE = 8391640;

		public static Handler handler;

		public long boxId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391640;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boxId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boxId = oc.pop_long();
			return oc;
		}
	}
}

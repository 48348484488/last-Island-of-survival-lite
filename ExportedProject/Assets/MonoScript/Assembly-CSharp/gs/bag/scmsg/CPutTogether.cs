using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CPutTogether : Message
	{
		public delegate void Handler(CPutTogether msg);

		public const int TYPE = 8391647;

		public static Handler handler;

		public int toInstanceId;

		public int fromInstanceId;

		public int number;

		public bool fromBag;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391647;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(toInstanceId);
			oc.push(fromInstanceId);
			oc.push(number);
			oc.push(fromBag);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			toInstanceId = oc.pop_int();
			fromInstanceId = oc.pop_int();
			number = oc.pop_int();
			fromBag = oc.pop_bool();
			return oc;
		}
	}
}

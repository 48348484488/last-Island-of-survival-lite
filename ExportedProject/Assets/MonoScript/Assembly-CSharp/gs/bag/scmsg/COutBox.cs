using Net;
using Share;

namespace gs.bag.scmsg
{
	public class COutBox : Message
	{
		public delegate void Handler(COutBox msg);

		public const int TYPE = 8391645;

		public static Handler handler;

		public long boxId;

		public int boxItemInstanceId;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391645;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boxId);
			oc.push(boxItemInstanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boxId = oc.pop_long();
			boxItemInstanceId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

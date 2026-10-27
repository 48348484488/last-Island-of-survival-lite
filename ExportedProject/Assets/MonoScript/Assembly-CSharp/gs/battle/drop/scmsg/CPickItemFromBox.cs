using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class CPickItemFromBox : Message
	{
		public delegate void Handler(CPickItemFromBox msg);

		public const int TYPE = 12585924;

		public static Handler handler;

		public long boxId;

		public long itemInstanceId;

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
			return 12585924;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boxId);
			oc.push(itemInstanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boxId = oc.pop_long();
			itemInstanceId = oc.pop_long();
			number = oc.pop_int();
			return oc;
		}
	}
}

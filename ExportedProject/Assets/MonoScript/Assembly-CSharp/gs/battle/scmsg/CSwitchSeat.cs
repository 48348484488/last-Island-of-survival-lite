using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSwitchSeat : Message
	{
		public delegate void Handler(CSwitchSeat msg);

		public const int TYPE = 11537378;

		public static Handler handler;

		public int id;

		public int seat;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537378;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(seat);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			seat = oc.pop_int();
			return oc;
		}
	}
}

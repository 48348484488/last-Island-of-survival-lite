using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CHandleTroopInvite : Message
	{
		public delegate void Handler(CHandleTroopInvite msg);

		public const int TYPE = 15731648;

		public static Handler handler;

		public int troopId;

		public long otherId;

		public bool accept;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731648;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(troopId);
			oc.push(otherId);
			oc.push(accept);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			troopId = oc.pop_int();
			otherId = oc.pop_long();
			accept = oc.pop_bool();
			return oc;
		}
	}
}

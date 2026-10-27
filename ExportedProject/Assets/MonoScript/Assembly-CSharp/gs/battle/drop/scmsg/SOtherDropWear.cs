using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SOtherDropWear : Message
	{
		public delegate void Handler(SOtherDropWear msg);

		public const int TYPE = 12585921;

		public static Handler handler;

		public long otherId;

		public int wId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585921;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(wId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			wId = oc.pop_int();
			return oc;
		}
	}
}

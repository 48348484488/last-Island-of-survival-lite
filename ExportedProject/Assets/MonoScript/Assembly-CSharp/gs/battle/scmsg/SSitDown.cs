using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSitDown : Message
	{
		public delegate void Handler(SSitDown msg);

		public const int TYPE = 11537547;

		public static Handler handler;

		public long insId;

		public long roleId;

		public int seatIndex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537547;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(roleId);
			oc.push(seatIndex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			roleId = oc.pop_long();
			seatIndex = oc.pop_int();
			return oc;
		}
	}
}

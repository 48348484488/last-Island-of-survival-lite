using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGetup : Message
	{
		public delegate void Handler(SGetup msg);

		public const int TYPE = 11537549;

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
			return 11537549;
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

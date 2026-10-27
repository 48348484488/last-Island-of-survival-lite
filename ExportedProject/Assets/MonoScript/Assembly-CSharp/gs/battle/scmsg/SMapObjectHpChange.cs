using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SMapObjectHpChange : Message
	{
		public delegate void Handler(SMapObjectHpChange msg);

		public const int TYPE = 11537545;

		public static Handler handler;

		public long insId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537545;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			hp = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class SPickWear : Message
	{
		public delegate void Handler(SPickWear msg);

		public const int TYPE = 12585919;

		public static Handler handler;

		public int wId;

		public int hp;

		public int skinId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585919;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(wId);
			oc.push(hp);
			oc.push(skinId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			wId = oc.pop_int();
			hp = oc.pop_int();
			skinId = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SHpChange : Message
	{
		public delegate void Handler(SHpChange msg);

		public const int TYPE = 11537359;

		public static Handler handler;

		public long roleId;

		public int hp;

		public int hpMax;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537359;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(hp);
			oc.push(hpMax);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			hp = oc.pop_int();
			hpMax = oc.pop_int();
			return oc;
		}
	}
}

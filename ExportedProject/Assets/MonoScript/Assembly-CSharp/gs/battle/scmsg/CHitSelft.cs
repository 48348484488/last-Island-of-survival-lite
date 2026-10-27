using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CHitSelft : Message
	{
		public delegate void Handler(CHitSelft msg);

		public const int TYPE = 11537560;

		public static Handler handler;

		public byte hitType;

		public int damage;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537560;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(hitType);
			oc.push(damage);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			hitType = oc.pop_byte();
			damage = oc.pop_int();
			return oc;
		}
	}
}

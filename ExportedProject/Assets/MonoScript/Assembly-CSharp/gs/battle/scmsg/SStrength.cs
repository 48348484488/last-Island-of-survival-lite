using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SStrength : Message
	{
		public delegate void Handler(SStrength msg);

		public const int TYPE = 11537514;

		public static Handler handler;

		public int strength;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537514;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(strength);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			strength = oc.pop_int();
			return oc;
		}
	}
}

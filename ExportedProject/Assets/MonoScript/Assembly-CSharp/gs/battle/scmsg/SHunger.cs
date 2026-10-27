using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SHunger : Message
	{
		public delegate void Handler(SHunger msg);

		public const int TYPE = 11537513;

		public static Handler handler;

		public int hunger;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537513;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(hunger);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			hunger = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SLockCooling : Message
	{
		public delegate void Handler(SLockCooling msg);

		public const int TYPE = 11537521;

		public static Handler handler;

		public int remainSeconds;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537521;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(remainSeconds);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			remainSeconds = oc.pop_int();
			return oc;
		}
	}
}

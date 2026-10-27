using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncVelocity : Message
	{
		public delegate void Handler(CSyncVelocity msg);

		public const int TYPE = 11537353;

		public static Handler handler;

		public ShortVec3 velocity = new ShortVec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537353;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(velocity);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(velocity);
			return oc;
		}
	}
}

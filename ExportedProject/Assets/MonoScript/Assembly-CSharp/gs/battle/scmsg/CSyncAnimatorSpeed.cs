using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncAnimatorSpeed : Message
	{
		public delegate void Handler(CSyncAnimatorSpeed msg);

		public const int TYPE = 11537412;

		public static Handler handler;

		public float speed;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537412;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(speed);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			speed = oc.pop_float();
			return oc;
		}
	}
}

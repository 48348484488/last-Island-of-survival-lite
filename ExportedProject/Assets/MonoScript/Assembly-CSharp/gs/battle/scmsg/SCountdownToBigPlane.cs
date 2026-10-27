using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SCountdownToBigPlane : Message
	{
		public delegate void Handler(SCountdownToBigPlane msg);

		public const int TYPE = 11537423;

		public static Handler handler;

		public int second;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537423;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(second);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			second = oc.pop_int();
			return oc;
		}
	}
}

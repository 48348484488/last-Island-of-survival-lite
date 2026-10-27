using Net;
using Share;

namespace gs.battle.drop.scmsg
{
	public class CUseBullet : Message
	{
		public delegate void Handler(CUseBullet msg);

		public const int TYPE = 12585922;

		public static Handler handler;

		public int gunIndex;

		public int num;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 12585922;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunIndex);
			oc.push(num);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunIndex = oc.pop_int();
			num = oc.pop_int();
			return oc;
		}
	}
}

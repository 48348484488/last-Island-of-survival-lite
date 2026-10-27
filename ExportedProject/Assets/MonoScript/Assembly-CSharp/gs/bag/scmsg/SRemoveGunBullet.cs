using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SRemoveGunBullet : Message
	{
		public delegate void Handler(SRemoveGunBullet msg);

		public const int TYPE = 8391623;

		public static Handler handler;

		public int gunInstanceId;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391623;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

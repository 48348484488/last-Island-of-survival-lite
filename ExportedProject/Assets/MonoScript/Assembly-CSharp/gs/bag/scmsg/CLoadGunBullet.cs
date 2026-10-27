using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CLoadGunBullet : Message
	{
		public delegate void Handler(CLoadGunBullet msg);

		public const int TYPE = 8391620;

		public static Handler handler;

		public int gunInstanceId;

		public int bulletInstanceId;

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
			return 8391620;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			oc.push(bulletInstanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			bulletInstanceId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

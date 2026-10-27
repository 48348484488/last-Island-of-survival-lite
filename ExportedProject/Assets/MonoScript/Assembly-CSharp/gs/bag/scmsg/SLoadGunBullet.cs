using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SLoadGunBullet : Message
	{
		public delegate void Handler(SLoadGunBullet msg);

		public const int TYPE = 8391621;

		public static Handler handler;

		public int gunInstanceId;

		public int bulletItemId;

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
			return 8391621;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			oc.push(bulletItemId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			bulletItemId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

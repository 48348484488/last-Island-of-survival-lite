using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SUseGunBullet : Message
	{
		public delegate void Handler(SUseGunBullet msg);

		public const int TYPE = 8391651;

		public static Handler handler;

		public int gunInstanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391651;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			return oc;
		}
	}
}

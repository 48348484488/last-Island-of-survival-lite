using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CRemoveGunPart : Message
	{
		public delegate void Handler(CRemoveGunPart msg);

		public const int TYPE = 8391618;

		public static Handler handler;

		public int gunInstanceId;

		public int itemId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391618;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(gunInstanceId);
			oc.push(itemId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			gunInstanceId = oc.pop_int();
			itemId = oc.pop_int();
			return oc;
		}
	}
}

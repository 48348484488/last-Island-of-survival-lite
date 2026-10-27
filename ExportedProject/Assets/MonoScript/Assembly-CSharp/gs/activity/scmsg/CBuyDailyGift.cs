using Net;
using Share;

namespace gs.activity.scmsg
{
	public class CBuyDailyGift : Message
	{
		public delegate void Handler(CBuyDailyGift msg);

		public const int TYPE = 29363135;

		public static Handler handler;

		public int index;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 29363135;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(index);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			index = oc.pop_int();
			return oc;
		}
	}
}

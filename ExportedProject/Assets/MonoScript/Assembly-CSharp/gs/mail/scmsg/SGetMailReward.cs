using Net;
using Share;

namespace gs.mail.scmsg
{
	public class SGetMailReward : Message
	{
		public delegate void Handler(SGetMailReward msg);

		public const int TYPE = 5245882;

		public static Handler handler;

		public int id;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 5245882;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CThrowGrenade : Message
	{
		public delegate void Handler(CThrowGrenade msg);

		public const int TYPE = 11537400;

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
			return 11537400;
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

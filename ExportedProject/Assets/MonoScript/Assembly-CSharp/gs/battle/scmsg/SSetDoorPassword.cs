using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSetDoorPassword : Message
	{
		public delegate void Handler(SSetDoorPassword msg);

		public const int TYPE = 11537518;

		public static Handler handler;

		public long id;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537518;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			return oc;
		}
	}
}

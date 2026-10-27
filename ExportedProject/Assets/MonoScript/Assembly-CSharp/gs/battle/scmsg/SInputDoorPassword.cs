using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SInputDoorPassword : Message
	{
		public delegate void Handler(SInputDoorPassword msg);

		public const int TYPE = 11537520;

		public static Handler handler;

		public long id;

		public bool success;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537520;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(success);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			success = oc.pop_bool();
			return oc;
		}
	}
}

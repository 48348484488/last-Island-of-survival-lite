using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBuildingStatusChange : Message
	{
		public delegate void Handler(SBuildingStatusChange msg);

		public const int TYPE = 11537471;

		public static Handler handler;

		public long id;

		public byte status;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537471;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(status);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			status = oc.pop_byte();
			return oc;
		}
	}
}

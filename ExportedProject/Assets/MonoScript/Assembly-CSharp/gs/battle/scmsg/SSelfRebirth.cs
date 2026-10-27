using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSelfRebirth : Message
	{
		public delegate void Handler(SSelfRebirth msg);

		public const int TYPE = 11537526;

		public static Handler handler;

		public long buildingId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537526;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildingId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			buildingId = oc.pop_long();
			return oc;
		}
	}
}

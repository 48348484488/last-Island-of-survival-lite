using Net;
using Share;

namespace gs.smelter.scmsg
{
	public class CStop : Message
	{
		public delegate void Handler(CStop msg);

		public const int TYPE = 17828797;

		public static Handler handler;

		public long smelterId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 17828797;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(smelterId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			smelterId = oc.pop_long();
			return oc;
		}
	}
}

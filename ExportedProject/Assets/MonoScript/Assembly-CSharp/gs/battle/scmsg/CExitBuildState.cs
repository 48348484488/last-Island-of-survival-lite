using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CExitBuildState : Message
	{
		public delegate void Handler(CExitBuildState msg);

		public const int TYPE = 11537507;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537507;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CQuitBattle : Message
	{
		public delegate void Handler(CQuitBattle msg);

		public const int TYPE = 11537398;

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
			return 11537398;
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

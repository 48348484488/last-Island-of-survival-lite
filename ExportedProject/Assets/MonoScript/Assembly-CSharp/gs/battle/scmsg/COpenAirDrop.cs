using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COpenAirDrop : Message
	{
		public delegate void Handler(COpenAirDrop msg);

		public const int TYPE = 11537459;

		public static Handler handler;

		public int cfgId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537459;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cfgId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cfgId = oc.pop_int();
			return oc;
		}
	}
}

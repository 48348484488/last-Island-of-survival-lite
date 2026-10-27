using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SOpenAirDrop : Message
	{
		public delegate void Handler(SOpenAirDrop msg);

		public const int TYPE = 11537460;

		public static Handler handler;

		public int cfgId;

		public bool isRedSide;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537460;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cfgId);
			oc.push(isRedSide);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cfgId = oc.pop_int();
			isRedSide = oc.pop_bool();
			return oc;
		}
	}
}

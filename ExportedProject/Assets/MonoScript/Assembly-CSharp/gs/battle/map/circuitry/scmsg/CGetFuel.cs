using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CGetFuel : Message
	{
		public delegate void Handler(CGetFuel msg);

		public const int TYPE = 27265982;

		public static Handler handler;

		public long powerId;

		public byte gridIndex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265982;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(powerId);
			oc.push(gridIndex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			powerId = oc.pop_long();
			gridIndex = oc.pop_byte();
			return oc;
		}
	}
}

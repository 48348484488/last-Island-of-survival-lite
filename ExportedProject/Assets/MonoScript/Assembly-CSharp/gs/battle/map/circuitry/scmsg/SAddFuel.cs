using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SAddFuel : Message
	{
		public delegate void Handler(SAddFuel msg);

		public const int TYPE = 27265981;

		public static Handler handler;

		public long powerId;

		public byte gridIndex;

		public int fuelId;

		public int fuelNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265981;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(powerId);
			oc.push(gridIndex);
			oc.push(fuelId);
			oc.push(fuelNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			powerId = oc.pop_long();
			gridIndex = oc.pop_byte();
			fuelId = oc.pop_int();
			fuelNum = oc.pop_int();
			return oc;
		}
	}
}

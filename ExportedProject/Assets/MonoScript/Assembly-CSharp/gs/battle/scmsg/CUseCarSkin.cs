using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CUseCarSkin : Message
	{
		public delegate void Handler(CUseCarSkin msg);

		public const int TYPE = 11537467;

		public static Handler handler;

		public int carId;

		public int skinId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537467;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(carId);
			oc.push(skinId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			carId = oc.pop_int();
			skinId = oc.pop_int();
			return oc;
		}
	}
}

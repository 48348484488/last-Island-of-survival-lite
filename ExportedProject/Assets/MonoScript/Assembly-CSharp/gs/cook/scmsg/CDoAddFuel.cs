using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.cook.scmsg
{
	public class CDoAddFuel : Message
	{
		public delegate void Handler(CDoAddFuel msg);

		public const int TYPE = 20974522;

		public static Handler handler;

		public long instanceId;

		public UseItem fuels = new UseItem();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 20974522;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(fuels);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			oc.pop(fuels);
			return oc;
		}
	}
}

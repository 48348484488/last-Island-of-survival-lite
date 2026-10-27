using Net;
using Share;

namespace gs.cook.scmsg
{
	public class CDoGetFuel : Message
	{
		public delegate void Handler(CDoGetFuel msg);

		public const int TYPE = 20974525;

		public static Handler handler;

		public long instanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 20974525;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SPutToHand : Message
	{
		public delegate void Handler(SPutToHand msg);

		public const int TYPE = 8391650;

		public static Handler handler;

		public int instanceId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391650;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			return oc;
		}
	}
}

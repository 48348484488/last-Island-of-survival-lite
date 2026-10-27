using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SItemDurationChanged : Message
	{
		public delegate void Handler(SItemDurationChanged msg);

		public const int TYPE = 8391655;

		public static Handler handler;

		public int instanceId;

		public int duration;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391655;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(duration);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			duration = oc.pop_int();
			return oc;
		}
	}
}

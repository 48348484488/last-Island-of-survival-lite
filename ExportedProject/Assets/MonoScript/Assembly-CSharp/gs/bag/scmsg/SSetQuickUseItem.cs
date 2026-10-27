using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SSetQuickUseItem : Message
	{
		public delegate void Handler(SSetQuickUseItem msg);

		public const int TYPE = 8391613;

		public static Handler handler;

		public int instanceId;

		public int index;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391613;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(index);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			index = oc.pop_int();
			return oc;
		}
	}
}

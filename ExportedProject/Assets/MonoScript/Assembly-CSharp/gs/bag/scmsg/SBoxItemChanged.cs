using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SBoxItemChanged : Message
	{
		public delegate void Handler(SBoxItemChanged msg);

		public const int TYPE = 8391642;

		public static Handler handler;

		public long boxId;

		public int instanceId;

		public int itemId;

		public int num;

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
			return 8391642;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(boxId);
			oc.push(instanceId);
			oc.push(itemId);
			oc.push(num);
			oc.push(duration);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			boxId = oc.pop_long();
			instanceId = oc.pop_int();
			itemId = oc.pop_int();
			num = oc.pop_int();
			duration = oc.pop_int();
			return oc;
		}
	}
}

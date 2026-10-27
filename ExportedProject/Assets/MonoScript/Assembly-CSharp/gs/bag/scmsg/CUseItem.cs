using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CUseItem : Message
	{
		public delegate void Handler(CUseItem msg);

		public const int TYPE = 8391632;

		public static Handler handler;

		public int instanceId;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391632;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SUseItem2 : Message
	{
		public delegate void Handler(SUseItem2 msg);

		public const int TYPE = 8391637;

		public static Handler handler;

		public const int Success = 1;

		public const int DontHaveItem = 2;

		public const int AlreadyInUse = 3;

		public int code;

		public BagItem bagItem = new BagItem();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391637;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(code);
			oc.push(bagItem);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			oc.pop(bagItem);
			return oc;
		}
	}
}

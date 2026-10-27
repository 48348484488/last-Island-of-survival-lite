using Net;
using Share;
using gs.drop.scmsg;

namespace gs.bag.scmsg
{
	public class SUseItem : Message
	{
		public delegate void Handler(SUseItem msg);

		public const int TYPE = 8391633;

		public static Handler handler;

		public int instanceId;

		public int number;

		public DropDetail dropDetail = new DropDetail();

		public bool special_useItem_success;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391633;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(number);
			oc.push(dropDetail);
			oc.push(special_useItem_success);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			number = oc.pop_int();
			oc.pop(dropDetail);
			special_useItem_success = oc.pop_bool();
			return oc;
		}
	}
}

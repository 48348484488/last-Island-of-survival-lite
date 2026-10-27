using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SBreakItem : Message
	{
		public delegate void Handler(SBreakItem msg);

		public const int TYPE = 8391611;

		public static Handler handler;

		public int instanceId;

		public int newNumber;

		public int newItemInstanceId;

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
			return 8391611;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(newNumber);
			oc.push(newItemInstanceId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			newNumber = oc.pop_int();
			newItemInstanceId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.task.scmsg
{
	public class CUploadItem : Message
	{
		public delegate void Handler(CUploadItem msg);

		public const int TYPE = 10488768;

		public static Handler handler;

		public int instanceId;

		public int itemId;

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
			return 10488768;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(itemId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			itemId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}

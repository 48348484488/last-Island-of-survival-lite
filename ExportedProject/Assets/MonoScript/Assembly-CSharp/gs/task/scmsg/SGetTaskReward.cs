using Net;
using Share;
using gs.drop.scmsg;

namespace gs.task.scmsg
{
	public class SGetTaskReward : Message
	{
		public delegate void Handler(SGetTaskReward msg);

		public const int TYPE = 10488764;

		public static Handler handler;

		public long taskId;

		public DropDetail dropDetail = new DropDetail();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488764;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskId);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskId = oc.pop_long();
			oc.pop(dropDetail);
			return oc;
		}
	}
}

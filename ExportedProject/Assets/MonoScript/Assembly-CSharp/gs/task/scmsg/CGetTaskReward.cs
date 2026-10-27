using Net;
using Share;

namespace gs.task.scmsg
{
	public class CGetTaskReward : Message
	{
		public delegate void Handler(CGetTaskReward msg);

		public const int TYPE = 10488763;

		public static Handler handler;

		public long taskId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488763;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskId = oc.pop_long();
			return oc;
		}
	}
}

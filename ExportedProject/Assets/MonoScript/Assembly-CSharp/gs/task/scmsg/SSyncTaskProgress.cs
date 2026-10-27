using Net;
using Share;

namespace gs.task.scmsg
{
	public class SSyncTaskProgress : Message
	{
		public delegate void Handler(SSyncTaskProgress msg);

		public const int TYPE = 10488766;

		public static Handler handler;

		public long taskId;

		public int progress;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488766;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(taskId);
			oc.push(progress);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			taskId = oc.pop_long();
			progress = oc.pop_int();
			return oc;
		}
	}
}

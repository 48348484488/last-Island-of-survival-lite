using Net;
using Share;

namespace gs.task.scmsg
{
	public class SAddTask : Message
	{
		public delegate void Handler(SAddTask msg);

		public const int TYPE = 10488761;

		public static Handler handler;

		public TaskInfo task = new TaskInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488761;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(task);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(task);
			return oc;
		}
	}
}

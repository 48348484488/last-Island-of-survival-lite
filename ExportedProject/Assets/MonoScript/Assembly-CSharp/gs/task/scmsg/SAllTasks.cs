using System.Collections.Generic;
using Net;
using Share;

namespace gs.task.scmsg
{
	public class SAllTasks : Message
	{
		public delegate void Handler(SAllTasks msg);

		public const int TYPE = 10488760;

		public static Handler handler;

		public List<TaskInfo> tasks = new List<TaskInfo>();

		public bool isReadTask;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 10488760;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(tasks.Count);
			foreach (TaskInfo task in tasks)
			{
				oc.push(task);
			}
			oc.push(isReadTask);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TaskInfo taskInfo = new TaskInfo();
				oc.pop(taskInfo);
				tasks.Add(taskInfo);
			}
			isReadTask = oc.pop_bool();
			return oc;
		}
	}
}

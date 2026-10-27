using Share;

namespace gs.task.scmsg
{
	public class TaskInfo : Marshal
	{
		public long taskId;

		public int taskTypeId;

		public bool rewarded;

		public bool finishedBefore;

		public int progress;

		public int progressMax;

		public Octets extra = new Octets();

		public Octets marshal(Octets oc)
		{
			oc.push(taskId);
			oc.push(taskTypeId);
			oc.push(rewarded);
			oc.push(finishedBefore);
			oc.push(progress);
			oc.push(progressMax);
			oc.push(extra);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			taskId = oc.pop_long();
			taskTypeId = oc.pop_int();
			rewarded = oc.pop_bool();
			finishedBefore = oc.pop_bool();
			progress = oc.pop_int();
			progressMax = oc.pop_int();
			extra = oc.pop_octets();
			return oc;
		}
	}
}

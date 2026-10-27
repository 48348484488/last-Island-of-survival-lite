using System.Collections.Generic;
using SC.UI;
using gs.task.scmsg;

public class RewardShowMgr : Singleton<RewardShowMgr>
{
	private Queue<SGetTaskReward> queue = new Queue<SGetTaskReward>();

	public void Add(SGetTaskReward msg)
	{
		if (!Singleton<DropMgr>.Ins.isEmpty(msg.dropDetail))
		{
			queue.Enqueue(msg);
			ClosePanel(null);
		}
	}

	public void ClosePanel(SGetTaskReward msg)
	{
		if (queue.Count > 0 && queue.Peek() == msg)
		{
			queue.Dequeue();
		}
		if (queue.Count > 0)
		{
			ViewMgr.Ins.ShowView<RewardPanel>(queue.Peek(), false);
		}
		else
		{
			ViewMgr.Ins.Destroy<RewardPanel>();
		}
	}

	public void ClearPanel()
	{
		queue.Clear();
	}
}

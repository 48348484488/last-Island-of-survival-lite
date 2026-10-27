using System.Collections.Generic;
using SC.UI;
using gs.drop.scmsg;

public class GainMgr : Singleton<GainMgr>
{
	private Queue<DropDetail> queue = new Queue<DropDetail>();

	public void Add(DropDetail dropDetail)
	{
		if (!Singleton<DropMgr>.Ins.isEmpty(dropDetail))
		{
			queue.Enqueue(dropDetail);
			ClosePanel(null);
		}
	}

	public void ClosePanel(DropDetail dropDeate)
	{
		if (queue.Count > 0 && queue.Peek() == dropDeate)
		{
			queue.Dequeue();
		}
		if (queue.Count > 0)
		{
			ViewMgr.Ins.ShowView<GainPanel>(queue.Peek(), false);
		}
	}

	public void ClearPanel()
	{
		queue.Clear();
	}
}

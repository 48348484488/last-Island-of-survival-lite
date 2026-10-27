using System;
using System.Collections.Generic;
using SC.UI;
using gs.bag.scmsg;

public class BoxMgr : Singleton<BoxMgr>
{
	public Dictionary<long, List<BagItem>> Boxes = new Dictionary<long, List<BagItem>>();

	public long CurrentBoxId;

	public List<BagItem> CurrentBoxItems;

	private COpenBox _cOpenBox = new COpenBox();

	private CPutBox _cPutBox = new CPutBox();

	private COutBox _cOutBox = new COutBox();

	public void Init()
	{
		SOpenBox.handler = (SOpenBox.Handler)Delegate.Combine(SOpenBox.handler, new SOpenBox.Handler(SOpenBoxHandle));
		SBoxItemChanged.handler = (SBoxItemChanged.Handler)Delegate.Combine(SBoxItemChanged.handler, new SBoxItemChanged.Handler(SBoxItemChangedHandle));
	}

	private void SBoxItemChangedHandle(SBoxItemChanged msg)
	{
		if (!Boxes.ContainsKey(msg.boxId))
		{
			return;
		}
		List<BagItem> list = Boxes[msg.boxId];
		BagItem bagItem = null;
		foreach (BagItem item in list)
		{
			if (item.instanceId == msg.instanceId)
			{
				bagItem = item;
				break;
			}
		}
		if (bagItem == null)
		{
			bagItem = new BagItem();
			bagItem.instanceId = msg.instanceId;
			bagItem.itemId = msg.itemId;
			bagItem.number = msg.num;
			bagItem.duration = msg.duration;
			list.Add(bagItem);
		}
		else if (msg.num == 0)
		{
			list.Remove(bagItem);
		}
		else
		{
			bagItem.number = msg.num;
		}
		Utils.TriggerEvent(BoxEvent.RefreshBox, msg.boxId);
	}

	private void SOpenBoxHandle(SOpenBox msg)
	{
		Boxes[msg.boxId] = msg.bagItems;
		CurrentBoxId = msg.boxId;
		CurrentBoxItems = Boxes[msg.boxId];
		ViewMgr.Ins.ShowView<BoxBagPanel>(CurrentBoxId);
	}

	public void OpenBox(long id)
	{
		_cOpenBox.boxId = id;
		Client2Gs.Ins.Send(_cOpenBox);
	}

	public void PutBox(long currentBoxId, int bagInstanceId, int num)
	{
		_cPutBox.boxId = currentBoxId;
		_cPutBox.bagIteminstanceId = bagInstanceId;
		_cPutBox.number = num;
		Client2Gs.Ins.Send(_cPutBox);
	}

	public void OutBox(long boxId, int boxItemInstanceId, int num)
	{
		_cOutBox.boxId = boxId;
		_cOutBox.boxItemInstanceId = boxItemInstanceId;
		_cOutBox.number = num;
		Client2Gs.Ins.Send(_cOutBox);
	}

	public void AddGetBagItemByInstanceId(List<BagItem> bagItems, int instanceId, int addNum)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (bagItem.instanceId == instanceId)
			{
				bagItem.number += addNum;
			}
		}
	}

	public BagItem GetBagItemFromList(List<BagItem> bagItems, int instanceId)
	{
		foreach (BagItem bagItem in bagItems)
		{
			if (bagItem.instanceId == instanceId)
			{
				return bagItem;
			}
		}
		return null;
	}

	public void RemoveBoxBagItemFromList(List<BagItem> bagItems, int instanceId)
	{
		BagItem bagItem = null;
		foreach (BagItem bagItem2 in bagItems)
		{
			if (bagItem2.instanceId == instanceId)
			{
				bagItem = bagItem2;
				break;
			}
		}
		if (bagItem != null)
		{
			bagItems.Remove(bagItem);
		}
	}

	public List<BagItem> GetBoxBagItems(long boxId)
	{
		if (Boxes.ContainsKey(boxId))
		{
			return Boxes[boxId];
		}
		return null;
	}
}

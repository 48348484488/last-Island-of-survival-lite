using Share;

public class TreeObject : SceneObject
{
	public int nTreeType;

	public int nTreeId;

	public override void Recycle()
	{
		Holder = null;
		SmallSceneMgr.Ins.TreeObjectPool.Recycle(this);
	}

	public override void AddToScene(SmallScene scene)
	{
		if (Holder != null)
		{
			return;
		}
		Holder = SmallSceneMgr.Ins.BorrowObject(PrefabName);
		if (Holder != null)
		{
			TreeInfo component = Holder.Go.GetComponent<TreeInfo>();
			if (component != null)
			{
				component.treeType = nTreeType;
				component.Id = nTreeId;
				component.AddTreeToDic();
			}
			Holder.Go.SetActive(true);
			Holder.Trans.position = Pos;
			Holder.Trans.eulerAngles = Angles;
			Holder.Trans.localScale = Scale;
		}
	}

	public override void RemoveFromScene(SmallScene scene)
	{
		if (Holder != null)
		{
			Holder.Go.SetActive(false);
			TreeInfo component = Holder.Go.GetComponent<TreeInfo>();
			if (component != null)
			{
				component.RemoveFromDic();
			}
			Holder.Trans.position = SceneObject.HidePos;
			SmallSceneMgr.Ins.ReturnObject(PrefabName, Holder);
			Holder = null;
		}
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		nTreeType = oc.pop_byte();
		nTreeId = oc.pop_int();
	}
}

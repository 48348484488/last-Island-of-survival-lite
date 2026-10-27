using cfg;

public class TreeInfo : MapObject
{
	public TreeCfg MyCfg;

	public int Id;

	public int treeType;

	public void AddTreeToDic()
	{
		Battle.Ins.TreesDic[Id] = this;
		if (Battle.Ins._treeIdToTreeDetail.ContainsKey(Id))
		{
			InsId = Battle.Ins._treeIdToTreeDetail[Id].InsId;
			MyCfg = Battle.Ins._treeIdToTreeDetail[Id].MyCfg;
			Hp = Battle.Ins._treeIdToTreeDetail[Id].Hp;
			MaxHp = MyCfg.hp;
			MapObjectName = MyCfg.name;
			Battle.Ins.MapObjectDic[InsId] = this;
		}
	}

	public void RemoveFromDic()
	{
		Battle.Ins.TreesDic.Remove(Id);
	}

	public void Init()
	{
		MyCfg = TreeCfg.Get(treeType);
		if (MyCfg != null)
		{
			MapObjectName = MyCfg.name;
			MaxHp = MyCfg.hp;
		}
	}
}

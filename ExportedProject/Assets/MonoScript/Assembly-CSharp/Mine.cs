using cfg;
using gs.battle.scmsg;

public class Mine : MapObject
{
	public SShowMine ShowMineInfo;

	public MineCfg MyCfg;

	public void Init(int itemId)
	{
		MyCfg = MineCfg.Get(itemId);
		CfgId = itemId;
		MapObjectName = MyCfg.name;
		MaxHp = MyCfg.hp;
	}
}

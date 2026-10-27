using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class BattlePackEvent
{
	public delegate void GunInfoDelegate(BagGun gunInfo, int index);

	public delegate void WearDamageDelegate(long roleId, int itemId, int hp);

	public delegate void SEnterRoomDelegate(SBattleLoginFinish arg);

	public static Utils.IntDelegate AddDelegate;

	public static Utils.IntDelegate RefreshGunDelegate;

	public static Utils.VoidDelegate RefreshBattlePack;

	public static Utils.VoidDelegate RefreshHead;

	public static Utils.VoidDelegate RefreshBulletproof;

	public static Utils.VoidDelegate RefreshEquipBag;

	public static Utils.VoidDelegate RefreshBestClothe;

	public static Utils.VoidDelegate RefreshBagCapacity;

	public static Utils.VoidDelegate RefreshBullet;

	public static Utils.IntDelegate RefreshEquip;

	public static Utils.Int2Delegate PickEquip;

	public static Utils.Int2Delegate DropEquip;

	public static Utils.Int2Delegate EquipHpDelegate;

	public static Utils.Int2Delegate LoadBulletFinishDelegate;

	public static Utils.VoidDelegate ChangeGunDelegate;

	public static Utils.IntDelegate DropGunDelegate;

	public static Utils.IntDelegate DropGunByIdDelegate;

	public static GunInfoDelegate PickGunDelegate;

	public static Utils.Int2Delegate RemovePartDelegate;

	public static Utils.Int2Delegate DropPartToGroundDelegate;

	public static Utils.Int2Delegate AddPartDelegate;

	public static Utils.VoidDelegate NeedLoadBullet;

	public static Utils.IntDelegate UseItemDelegate;

	public static Utils.IntDelegate UseItemDelega2te;

	public static Utils.Int2Delegate UseItemByInstanceItemId;

	public static Utils.Int2Delegate DropItemDelegate;

	public static Utils.VoidDelegate ReplaceItemDelegate;

	public static Utils.IntDelegate PickNearGunDelegate;

	public static Utils.VoidDelegate DropNearGunDelegate;

	public static Utils.IntDelegate DropNearGunByIdDelegate;

	public static Utils.VoidDelegate RefreshNearGunDelegate;

	public static Utils.VoidDelegate PickItemDelegate;

	public static Utils.IntDelegate PickItemIdDelegate;

	public static Utils.IntDelegate OpenDeadBoxDelegate;

	public static SEnterRoomDelegate OnSBattleLoadingFinishDelegate;

	public static Utils.VoidDelegate OnClearEquipsDelegate;

	public static Utils.VoidDelegate OnBreakUseItem;

	public static bool TriggerEvent(GunInfoDelegate e, BagGun arg, int index)
	{
		if (e == null)
		{
			return false;
		}
		e(arg, index);
		return true;
	}

	public static bool TriggerEvent(SEnterRoomDelegate e, SBattleLoginFinish arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}

	public static bool TriggerEvent(WearDamageDelegate e, long roleId, int itemId, int hp)
	{
		if (e == null)
		{
			return false;
		}
		e(roleId, itemId, hp);
		return true;
	}
}

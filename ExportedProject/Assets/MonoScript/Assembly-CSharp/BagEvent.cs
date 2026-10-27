using System;
using gs.bag.scmsg;

public class BagEvent
{
	public delegate void BagItemDelegate(BagItem arg);

	public delegate void GunDataChangeDelegate(BagMgr.GunData gunData);

	public static Utils.VoidDelegate RefreshBag;

	public static Utils.VoidDelegate RefreshQuickUse;

	public static Utils.IntDelegate RefreshQuickUseByIndex;

	public static Utils.VoidDelegate UseItemSucess;

	public static Utils.VoidDelegate SplitItemSucess;

	public static Utils.VoidDelegate RefreshGunParts;

	public static Utils.VoidDelegate RefreshSkins;

	public static Utils.IntDelegate PutSkinByItemId;

	public static Utils.VoidDelegate RefreshEquips;

	public static Utils.IntDelegate PutEquipByItemId;

	public static Utils.IntDelegate RemoveSkinByItemId;

	public static Utils.IntDelegate RemoveEquipByItemId;

	public static Utils.VoidDelegate RefreshBullet;

	public static Utils.VoidDelegate SetQuickDelegate;

	public static Utils.VoidDelegate OutQuickDelegate;

	public static Utils.VoidDelegate CloseBagPanel;

	public static Utils.Int2Delegate AddGunPart;

	public static Utils.Int2Delegate RemoveGunPart;

	public static Utils.Int2Delegate AddOrRemoveBullet;

	public static Utils.Int2Delegate ShootBullet;

	public static Utils.IntDelegate DropGun;

	public static Utils.IntDelegate UseGun;

	public static Utils.IntDelegate ChangeGunFactors;

	public static GunDataChangeDelegate OnGunDataChanged;

	public static Utils.Int2Delegate ItemChange;

	public static Utils.IntDelegate PutEquipByItemId2Battle;

	public static Utils.IntDelegate RemoveEquipByItemId2Battle;

	public static Utils.IntDelegate PutSkinByItemId2Battle;

	public static Utils.IntDelegate RemoveSkinByItemId2Battle;

	public static Utils.IntLongDelegate DurabilityChange;

	public static Utils.LongDelegate ChangeHandInstanceIdDelegate;

	public static Utils.IntDelegate RefreshGunAttrDelegate;

	public static Action<BagItem, int> OnClickLingDiGuiBtn;

	public static Utils.BoolDelegate IsShowEquipBoolDelegate;

	public static Utils.VoidDelegate DiscardDelegate;

	public static bool TriggerEvent(BagItemDelegate e, BagItem arg)
	{
		if (e == null)
		{
			return false;
		}
		e(arg);
		return true;
	}
}

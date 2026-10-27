using System;
using System.Collections.Generic;
using UnityEngine;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class BattleDropEvent
{
	public delegate void IntVec3Delegate(int arg, Vec3 arg2);

	public static Utils.VoidDelegate DropNumChangeDelegate;

	public static Utils.IntDelegate ClickBoxDelegate;

	public static Utils.IntDelegate FlagDisappearDelegate;

	public static IntVec3Delegate FlagAppearDelegate;

	public static Action<byte> ShowPickPanel;

	public static Action HidePickPanel;

	public static Action<List<TrashcanInfo>, Action<TrashcanInfo, GameObject>> LaJiZhanBaiXiangZiAction;
}

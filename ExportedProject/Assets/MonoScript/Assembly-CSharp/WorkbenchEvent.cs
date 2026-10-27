using System;
using SC.UI;
using cfg;
using gs.bag.scmsg;
using gs.workbench.scmsg;

public class WorkbenchEvent
{
	public static Action<WorkbenchPanel.WorkbenchPageType, int> ChangePageDelegate;

	public static Action<SWorkbenchInfo> RefreshPageDelegate;

	public static Action<BagItem> SetDragFromDelegate;

	public static Action<WorkbenchPanel.WorkbenchPageType> SetDragTargetDelegate;

	public static Action<int, int> SetInUseItemInstanceIdDelegate;

	public static Action<int> RemoveInUseItemIdDelegate;

	public static Action RemoveAllInUseItemDelegate;

	public static Action<int> ChangeStudyLevelDelegate;

	public static Action<WorkbenchRepairCfg> AutoAddRepairMaterialDelegate;
}

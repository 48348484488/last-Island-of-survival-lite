using System;
using System.Collections.Generic;

public class CookEvent
{
	public static Action<LinkedListNode<CookMgr.CookMaterial>, int> OnAddMaterialAction;

	public static Action OnRemoveMaterialAction;
}

using System.Runtime.CompilerServices;
using UnityEngine;

namespace SC.UI
{
	public class TeamMessagePanel : View
	{
		private GameObject btn_team;

		private GameObject btn_toolbox;

		protected override void onInit()
		{
			base.onInit();
			btn_toolbox.SetActive(false);
			ClickListener.Get(btn_team, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_team = component.GameObjects[0].gameObject;
			btn_toolbox = component.GameObjects[1].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<TeamInforPanel>();
			Hide();
		}
	}
}

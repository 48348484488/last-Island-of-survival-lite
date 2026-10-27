using System.Runtime.CompilerServices;
using UnityEngine;

namespace SC.UI
{
	public class SettingPopupPanel : View
	{
		private GameObject m_rec_bg;

		private GameObject btn_confirm;

		private GameObject btn_back;

		protected override void onInit()
		{
			ClickListener.Get(btn_back, string.Empty).onClick = (ClickListener.Get(btn_confirm, string.Empty).onClick = _003ConInit_003Em__0);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_rec_bg = component.GameObjects[0].gameObject;
			btn_confirm = component.GameObjects[1].gameObject;
			btn_back = component.GameObjects[2].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}

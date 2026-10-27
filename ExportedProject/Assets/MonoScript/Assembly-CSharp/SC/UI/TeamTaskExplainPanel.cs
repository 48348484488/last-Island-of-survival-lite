using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class TeamTaskExplainPanel : View
	{
		private GameObject m_bg;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject btn_confirm;

		protected override void onInit()
		{
			base.onInit();
			ClickListener.Get(btn_confirm, string.Empty).onClick = (ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			txt_name = component.GameObjects[1].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			btn_confirm = component.GameObjects[2].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}

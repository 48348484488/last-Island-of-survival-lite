using UnityEngine;

namespace SC.UI
{
	public class TeamTaskPopupPanel : View
	{
		private static TeamTaskPopupPanel ins;

		private static Utils.VoidDelegate okFunc;

		private static Utils.VoidDelegate cancelFunc;

		private static int _tip;

		private GameObject m_bg;

		private GameObject btn_close;

		private GameObject m_friend;

		private GameObject m_team;

		private GameObject btn_confirm;

		private GameObject btn_cancel;

		protected override void onInit()
		{
			ins = this;
			UIEventListener.Get(btn_close, string.Empty).onClick = OnClickBack;
			UIEventListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			UIEventListener.Get(btn_cancel, string.Empty).onClick = OnClickBack;
		}

		protected override void onDestroy()
		{
		}

		protected override void onShow(object param = null, string childView = null)
		{
			m_friend.SetActiveBetter(_tip == 1);
			m_team.SetActiveBetter(_tip == 2);
		}

		protected override void onHide(string childView = null)
		{
			Reset();
			StopAllCoroutines();
		}

		public static void Show(int tip, Utils.VoidDelegate onOk = null, Utils.VoidDelegate onCancel = null)
		{
			_tip = tip;
			okFunc = onOk;
			cancelFunc = onCancel;
			if (ins == null || !ins.IsShow)
			{
				ViewMgr.Ins.ShowTopView<TeamTaskPopupPanel>();
			}
			else
			{
				ins.onShow();
			}
		}

		private void OnClickBack(GameObject go)
		{
			if (cancelFunc != null)
			{
				cancelFunc();
			}
			Hide();
		}

		private void OnClickConfirm(GameObject go)
		{
			if (okFunc != null)
			{
				okFunc();
			}
			Hide();
		}

		private static void Reset()
		{
			_tip = 0;
			okFunc = null;
			cancelFunc = null;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			btn_close = component.GameObjects[1].gameObject;
			m_friend = component.GameObjects[2].gameObject;
			m_team = component.GameObjects[3].gameObject;
			btn_confirm = component.GameObjects[4].gameObject;
			btn_cancel = component.GameObjects[5].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}

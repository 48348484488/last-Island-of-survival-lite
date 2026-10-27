using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class AlertBox : View
	{
		private static AlertBox _ins;

		private string recentTip = string.Empty;

		private Vector3 _mScale;

		private int timerId;

		private GameObject m_alert;

		private GameObject txt_alertBox;

		private Text txt_alertBoxText;

		protected override void onInit()
		{
			_ins = this;
			_mScale = txt_alertBox.transform.localScale;
			View.SetLabelText(txt_alertBox, string.Empty);
			UIEventListener.Get(m_alert, string.Empty).onClick = OnClick;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			Singleton<AlertMgr>.Ins._showing = true;
			recentTip = param.ToString();
			m_alert.transform.localScale = new Vector3(0f, 0f);
			View.SetLabelText(txt_alertBox, recentTip);
			PlayAnimation();
			Invoke("myHide", Singleton<AlertMgr>.Ins.SpaceTime);
		}

		protected override void onHide(string childView = null)
		{
			Singleton<AlertMgr>.Ins.CloseAlertPanel();
			recentTip = string.Empty;
		}

		public static void Show(string tip = "此功能正在玩命开发，即将开启")
		{
			Singleton<AlertMgr>.Ins.Add(tip);
		}

		public static void Show(int strId, params object[] args)
		{
			string @string = Utils.GetString(strId, args);
			if (@string != null)
			{
				Show(@string);
			}
		}

		private void OnClick(GameObject go)
		{
			Hide();
		}

		private void PlayAnimation()
		{
			m_alert.transform.localScale = new Vector3(0.3f, 0.3f);
			m_alert.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f);
		}

		private void myHide()
		{
			Hide();
		}

		protected override void onDestroy()
		{
		}

		public GameObject GetTxtAlertBox()
		{
			return txt_alertBox;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_alert = component.GameObjects[0].gameObject;
			txt_alertBox = component.GameObjects[1].gameObject;
			txt_alertBoxText = txt_alertBox.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}
	}
}

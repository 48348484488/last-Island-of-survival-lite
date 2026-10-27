using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPanel : View
{
	private static MessageBoxPanel ins;

	private static string recentTip = string.Empty;

	private static Utils.VoidDelegate okFunc;

	private static Utils.VoidDelegate cancelFunc;

	private static bool confirm;

	public static object Context;

	private bool mAniming;

	private GameObject m_main_window;

	private GameObject txt_msg;

	private Text txt_msgText;

	private GameObject btn_confirm;

	private GameObject btn_ok;

	private GameObject btn_back;

	protected override void onInit()
	{
		ins = this;
		UIEventListener.Get(btn_back, string.Empty).onClick = OnClickBack;
		UIEventListener.Get(btn_ok, string.Empty).onClick = OnClickOk;
		UIEventListener.Get(btn_confirm, string.Empty).onClick = OnClickOk;
		View.SetLabelText(txt_msg, string.Empty);
	}

	protected override void onDestroy()
	{
	}

	protected override void onShow(object param = null, string childView = null)
	{
		ViewMgr.Ins.HideView("AlertBox");
		View.SetLabelText(txt_msg, recentTip);
		btn_back.SetActive(!confirm);
		btn_ok.SetActive(!confirm);
		btn_confirm.SetActive(confirm);
		m_main_window.transform.localScale = Vector3.one;
		mAniming = false;
	}

	public override void _SetRenderSort(int RendingSort)
	{
		base._SetRenderSort(100);
	}

	protected override void onHide(string childView = null)
	{
		Reset();
		StopAllCoroutines();
	}

	public static void ShowConfirm(string tip, Utils.VoidDelegate onOk = null)
	{
		recentTip = tip;
		okFunc = onOk;
		cancelFunc = null;
		confirm = true;
		if (ins == null || !ins.IsShow)
		{
			ViewMgr.Ins.ShowTopView<MessageBoxPanel>();
		}
		else
		{
			ins.onShow();
		}
	}

	public static void ShowConfirm(int tip, Utils.VoidDelegate onOk = null)
	{
		recentTip = Utils.GetString(tip);
		okFunc = onOk;
		cancelFunc = null;
		confirm = true;
		if (ins == null || !ins.IsShow)
		{
			ViewMgr.Ins.ShowTopView<MessageBoxPanel>();
		}
		else
		{
			ins.onShow();
		}
	}

	public static void Show(string tip, Utils.VoidDelegate onOk = null, Utils.VoidDelegate onCancel = null, object context = null)
	{
		recentTip = tip;
		okFunc = onOk;
		cancelFunc = onCancel;
		confirm = false;
		Context = context;
		if (ins == null || !ins.IsShow)
		{
			ViewMgr.Ins.ShowTopView<MessageBoxPanel>();
		}
		else
		{
			ins.onShow();
		}
	}

	public static void Show(int tip, Utils.VoidDelegate onOk = null, Utils.VoidDelegate onCancel = null, object context = null)
	{
		recentTip = Utils.GetString(tip);
		okFunc = onOk;
		cancelFunc = onCancel;
		confirm = false;
		Context = context;
		if (ins == null || !ins.IsShow)
		{
			ViewMgr.Ins.ShowTopView<MessageBoxPanel>();
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

	private IEnumerator hideAnim()
	{
		mAniming = true;
		m_main_window.transform.localScale = Vector3.one;
		yield return StartCoroutine(Utils.HidePopupWindow(m_main_window));
		mAniming = false;
		Hide();
	}

	private IEnumerator showAnim()
	{
		mAniming = true;
		yield return StartCoroutine(Utils.ShowPopupWindow(m_main_window));
		mAniming = false;
	}

	private void OnClickOk(GameObject go)
	{
		if (okFunc != null)
		{
			okFunc();
		}
		Hide();
	}

	private static void Reset()
	{
		recentTip = string.Empty;
		okFunc = null;
		cancelFunc = null;
		confirm = false;
	}

	protected override void Awake()
	{
		base.Awake();
		GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
		m_main_window = component.GameObjects[0].gameObject;
		txt_msg = component.GameObjects[1].gameObject;
		txt_msgText = txt_msg.GetComponent<Text>();
		btn_confirm = component.GameObjects[2].gameObject;
		btn_ok = component.GameObjects[3].gameObject;
		btn_back = component.GameObjects[4].gameObject;
		ViewMgr.Ins.addView(this);
	}
}

using System.Runtime.CompilerServices;
using UnityEngine;

namespace SC.UI
{
	public class LoginAccoutPanel : View
	{
		private GameObject btn_ok;

		private GameObject btn_clear;

		private GameObject inp_usename;

		private GameObject inp_password;

		[CompilerGenerated]
		private static Utils.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			View.SetInputText(inp_usename, PlayerPrefsData.Account);
			View.SetInputText(inp_password, PlayerPrefsData.SessionKey);
			ClickListener.Get(btn_clear, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_ok, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onHide(string childView = null)
		{
			ViewMgr.Ins.Destroy<LoginAccoutPanel>();
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_ok = component.GameObjects[0].gameObject;
			btn_clear = component.GameObjects[1].gameObject;
			inp_usename = component.GameObjects[2].gameObject;
			inp_password = component.GameObjects[3].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			View.SetInputText(inp_usename, string.Empty);
			View.SetInputText(inp_password, string.Empty);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			string inputText = View.GetInputText(inp_usename);
			if (!string.IsNullOrEmpty(inputText))
			{
				PlayerPrefsData.Account = View.GetInputText(inp_usename).Trim();
				PlayerPrefsData.SessionKey = View.GetInputText(inp_password).Trim();
				if (_003C_003Ef__am_0024cache0 == null)
				{
					_003C_003Ef__am_0024cache0 = _003ConInit_003Em__2;
				}
				TweenTime.Begin(go, 5f, _003C_003Ef__am_0024cache0);
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__2()
		{
			Singleton<PlatformMgr>.Ins.Login();
		}
	}
}

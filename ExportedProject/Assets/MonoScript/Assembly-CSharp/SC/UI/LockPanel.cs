using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LockPanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey1
		{
			internal int i1;

			internal LockPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this._password += i1;
				View.SetLabelText(_0024this.txt_show_numText, _0024this._password);
				if (_0024this._password.Length == cfg.Consts.LOCK_PASSWORD_LENGTH)
				{
					if (_0024this._sendMsg == null)
					{
						Debug.LogError("Show LockPanel in a wrong posture.");
						return;
					}
					_0024this._sendMsg(_0024this._password);
					_0024this.Hide();
				}
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this.Clear();
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this.Hide();
			}
		}

		private string _password;

		private Action<string> _sendMsg;

		private GameObject m_rec_bg;

		private GameObject txt_show_num;

		private Text txt_show_numText;

		private GameObject[] m_num_btns;

		private GameObject m_num_btnsObj;

		private GameObject btn_delect;

		protected override void onInit()
		{
			int i = 0;
			for (int num = m_num_btns.Length; i < num; i++)
			{
				_003ConInit_003Ec__AnonStorey1 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey1();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.i1 = i;
				ClickListener.Get(m_num_btns[i], string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
				ClickListener.Get(btn_delect, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__1;
				ClickListener.Get(m_rec_bg, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__2;
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			Clear();
			if (param == null)
			{
				StartCoroutine(WaitHide());
				return;
			}
			switch ((int)param)
			{
			case 103:
				_sendMsg = Singleton<LockMgr>.Ins.SendInputPasswordMsg;
				break;
			case 104:
				AlertBox.Show(341);
				_sendMsg = Singleton<LockMgr>.Ins.SendPutLockOnDoor;
				break;
			case 102:
				_sendMsg = Singleton<LockMgr>.Ins.SendChangePasswordMsg;
				break;
			default:
				StartCoroutine(WaitHide());
				break;
			}
		}

		private IEnumerator WaitHide()
		{
			yield return Utils.WaitForSeconds(0.1f);
			Hide();
		}

		protected override void onHide(string childView = null)
		{
		}

		private void Clear()
		{
			_password = string.Empty;
			View.SetLabelText(txt_show_numText, string.Empty);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_rec_bg = component.GameObjects[0].gameObject;
			txt_show_num = component.GameObjects[1].gameObject;
			txt_show_numText = txt_show_num.GetComponent<Text>();
			m_num_btns = component.GameObjects[2].gameObject.GetComponent<UIGameObjectList>().objects;
			m_num_btnsObj = component.GameObjects[2].gameObject;
			btn_delect = component.GameObjects[3].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}

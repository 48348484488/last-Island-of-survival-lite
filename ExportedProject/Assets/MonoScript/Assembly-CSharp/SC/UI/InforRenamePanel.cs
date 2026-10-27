using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.role.scmsg;

namespace SC.UI
{
	public class InforRenamePanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal ItemCfg itemCfg;

			internal InforRenamePanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.Hide();
			}

			internal void _003C_003Em__1(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemCfg);
			}

			internal void _003C_003Em__2(GameObject go)
			{
				if (string.IsNullOrEmpty(_0024this._inp.text))
				{
					AlertBox.Show(132);
				}
				else if (_0024this.LengthString(_0024this._inp.text) > cfg.Consts.MAX_ROLE_NAME_LENGTH)
				{
					AlertBox.Show(256);
				}
				else if (_0024this.ContainSpecialCharacter(_0024this._inp.text))
				{
					AlertBox.Show(131);
				}
				else if (Singleton<BagMgr>.Ins.GetItemNum(cfg.Consts.GAIMINGKA_ITEM_ID) <= 0)
				{
					AlertBox.Show(299);
				}
				else if (!Singleton<RoleMgr>.Ins.info.name.Equals(_0024this._inp.text.Trim()))
				{
					_0024this._cChangeName.name = _0024this._inp.text.Trim();
					Client2Gs.Ins.Send(_0024this._cChangeName);
				}
				else
				{
					AlertBox.Show(273);
				}
			}
		}

		private InputField _inp;

		private CChangeName _cChangeName = new CChangeName();

		private readonly string REGEX_STRING = " `~!@#$%^&*()+=|{}':;',[].<>/?~！@#￥%&*（）——+|{}【】‘；：”“’。，、？\"‥…▪•";

		private GameObject btn_close;

		private GameObject btn_confirm;

		private GameObject inp_new_name;

		private GameObject m_rename_card_icon;

		protected override void onInit()
		{
			_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
			_003ConInit_003Ec__AnonStorey._0024this = this;
			_inp = inp_new_name.GetComponent<InputField>();
			_003ConInit_003Ec__AnonStorey.itemCfg = ItemCfg.Get(7);
			View.SetItemSprite(m_rename_card_icon, _003ConInit_003Ec__AnonStorey.itemCfg.icon);
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(m_rename_card_icon, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__1;
			ClickListener.Get(btn_confirm, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_inp.text = string.Empty;
			SChangeName.handler = (SChangeName.Handler)Delegate.Combine(SChangeName.handler, new SChangeName.Handler(OnSChangeName));
		}

		private void OnItemNumerChange(int itemId, int num)
		{
		}

		private void OnSChangeName(SChangeName msg)
		{
			if (msg.result == 0)
			{
				Hide();
			}
		}

		protected override void onHide(string childView = null)
		{
			SChangeName.handler = (SChangeName.Handler)Delegate.Remove(SChangeName.handler, new SChangeName.Handler(OnSChangeName));
		}

		private int LengthString(string name)
		{
			int num = 0;
			for (int num2 = name.Length - 1; num2 >= 0; num2--)
			{
				num = ((name[num2] < '\0' || name[num2] >= '\u007f') ? (num + 2) : (num + 1));
			}
			return num;
		}

		private bool ContainSpecialCharacter(string str)
		{
			for (int num = str.Length - 1; num >= 0; num--)
			{
				for (int num2 = REGEX_STRING.Length - 1; num2 >= 0; num2--)
				{
					if (str[num] == REGEX_STRING[num2])
					{
						return true;
					}
				}
			}
			return false;
		}

		protected override void onDestroy()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			btn_confirm = component.GameObjects[1].gameObject;
			inp_new_name = component.GameObjects[2].gameObject;
			m_rename_card_icon = component.GameObjects[3].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}

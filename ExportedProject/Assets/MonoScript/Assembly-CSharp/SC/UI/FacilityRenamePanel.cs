using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class FacilityRenamePanel : View
	{
		private static Action<long, string> OnClickConfirm;

		private static long InsId;

		private readonly int MaxNameLength = cfg.Consts.BUILDING_NAME_LENGTH_MAX;

		private InputField _inp;

		private string _oldName;

		private readonly string REGEX_STRING = " `~!@#$%^&*()+=|{}':;',[].<>/?~！@#￥%&*（）——+|{}【】‘；：”“’。，、？\"‥…▪•";

		private GameObject btn_close;

		private GameObject btn_confirm;

		private GameObject inp_new_name;

		protected override void onInit()
		{
			_inp = inp_new_name.GetComponent<InputField>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_confirm, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_inp.text = string.Empty;
			_oldName = param as string;
		}

		protected override void onHide(string childView = null)
		{
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

		public static void Show(Action<long, string> OnClickConfirmCallBack, string oldName, long insId)
		{
			OnClickConfirm = OnClickConfirmCallBack;
			InsId = insId;
			ViewMgr.Ins.ShowView<FacilityRenamePanel>(oldName, false);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			btn_confirm = component.GameObjects[1].gameObject;
			inp_new_name = component.GameObjects[2].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			if (string.IsNullOrEmpty(_inp.text))
			{
				AlertBox.Show(132);
				return;
			}
			if (LengthString(_inp.text) > MaxNameLength)
			{
				AlertBox.Show(256);
				return;
			}
			if (ContainSpecialCharacter(_inp.text))
			{
				AlertBox.Show(131);
				return;
			}
			if (string.IsNullOrEmpty(_oldName) || !_oldName.Equals(_inp.text.Trim()))
			{
				if (OnClickConfirm != null)
				{
					OnClickConfirm(InsId, _inp.text.Trim());
				}
				OnClickConfirm = null;
			}
			else
			{
				AlertBox.Show(111);
			}
			Hide();
		}
	}
}

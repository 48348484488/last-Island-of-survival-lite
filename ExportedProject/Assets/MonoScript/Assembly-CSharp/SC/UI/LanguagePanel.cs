using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LanguagePanel : View
	{
		[CompilerGenerated]
		private sealed class _003CInitServerCallBack_003Ec__AnonStorey0
		{
			internal int languageindex;

			internal LanguagePanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				Singleton<MultiLanguageMgr>.Ins.ChangeLanguage(languageindex);
				_0024this.Hide();
			}
		}

		private List<MultiLanguageIndexCfg> multiLanguageIndexCfgList;

		private UIScrollPanel _uIScrollPanel;

		private int _currentIndex;

		private Utils.VoidDelegate _onhide;

		private GameObject m_bg;

		private GameObject scp_language_list;

		private GLanguageItem m_cell;

		private GameObject btn_close_rec;

		protected override void onInit()
		{
			multiLanguageIndexCfgList = MultiLanguageIndexCfg.GetAllList();
			_uIScrollPanel = scp_language_list.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_close_rec, string.Empty).onClick = Close;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			if (param != null)
			{
				_onhide = param as Utils.VoidDelegate;
			}
			_uIScrollPanel.Reset(multiLanguageIndexCfgList.Count, InitServerCallBack);
		}

		protected override void onHide(string childView = null)
		{
			Utils.TriggerEvent(_onhide);
		}

		private int GetIndex(int index)
		{
			return index - 1;
		}

		private void InitServerCallBack(GameObject cell, int index)
		{
			_003CInitServerCallBack_003Ec__AnonStorey0 _003CInitServerCallBack_003Ec__AnonStorey = new _003CInitServerCallBack_003Ec__AnonStorey0();
			_003CInitServerCallBack_003Ec__AnonStorey._0024this = this;
			GLanguageItem component = cell.GetComponent<GLanguageItem>();
			MultiLanguageIndexCfg multiLanguageIndexCfg = multiLanguageIndexCfgList[index];
			_003CInitServerCallBack_003Ec__AnonStorey.languageindex = multiLanguageIndexCfg.index;
			View.SetLabelText(component.txt_language_nameText, Singleton<MultiLanguageMgr>.Ins.GetLanguageName(_003CInitServerCallBack_003Ec__AnonStorey.languageindex), false);
			component.m_select.SetActive(_003CInitServerCallBack_003Ec__AnonStorey.languageindex == Singleton<MultiLanguageMgr>.Ins.GetCurrentLanguageIndex());
			ClickListener.Get(component.gameObject, string.Empty).onClick = _003CInitServerCallBack_003Ec__AnonStorey._003C_003Em__0;
		}

		public void SetLabelText(Text label, string text)
		{
			if (!(label == null))
			{
				if (!(label.text == text))
				{
					label.text = text;
				}
			}
		}

		private void Close(GameObject go)
		{
			Hide();
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			scp_language_list = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GLanguageItem>(component.GameObjects[2].gameObject);
			btn_close_rec = component.GameObjects[3].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using gs.drop.scmsg;

namespace SC.UI
{
	public class GainPanel : View
	{
		private DropDetail _dropDetail = new DropDetail();

		private List<ItemHead> icons = new List<ItemHead>();

		public static Utils.VoidDelegate GetFinish;

		public static Utils.VoidDelegate OnHideDelegate;

		public static string TitleStr;

		private GameObject m_panel_root;

		private GameObject m_item_root;

		private GameObject btn_ok;

		private GameObject m_bg;

		private GameObject txt_title;

		private Text txt_titleText;

		protected override void onInit()
		{
			if (string.IsNullOrEmpty(TitleStr))
			{
				TitleStr = Utils.GetString(316);
			}
			UIEventListener.Get(m_bg, string.Empty).onClick = (UIEventListener.Get(btn_ok, string.Empty).onClick = OnClickOk);
		}

		public override void _SetRenderSort(int RendingSort)
		{
			base._SetRenderSort(20);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(352);
			if (ViewMgr.Ins.IsShow<LadderLevelUpPanel>())
			{
				ViewMgr.Ins.HideView<LadderLevelUpPanel>();
			}
			_dropDetail = param as DropDetail;
			View.SetLabelText(txt_titleText, TitleStr);
			if (_dropDetail == null)
			{
				Hide();
				return;
			}
			m_item_root.SetActive(false);
			m_item_root.SetActive(true);
			ShowItem(_dropDetail);
		}

		private void ShowItem(DropDetail dropDetail)
		{
			Animation();
			List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(dropDetail);
			if (dropDetailInfo.Count > 8)
			{
				ItemHead.FillList(m_item_root, dropDetailInfo, true, 170, -1, 0.85f);
			}
			else
			{
				ItemHead.FillList(m_item_root, dropDetailInfo);
			}
		}

		private void OnClickOk(GameObject go)
		{
			Hide();
			Utils.TriggerEvent(GetFinish);
			Singleton<GainMgr>.Ins.ClosePanel(_dropDetail);
		}

		protected override void onDestroy()
		{
			Singleton<GainMgr>.Ins.ClearPanel();
		}

		private void Animation()
		{
			m_panel_root.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			Tweener tweener = m_panel_root.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).OnComplete(_003CAnimation_003Em__0);
		}

		protected override void onHide(string childView = null)
		{
			TitleStr = Utils.GetString(316);
			if (_dropDetail != null && _dropDetail.dropProp.ContainsKey(17))
			{
				ViewMgr.Ins.Destroy<GainPanel>();
				ViewMgr.Ins.ShowView<LadderLevelUpPanel>(null, false);
			}
			Utils.TriggerEvent(OnHideDelegate);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_panel_root = component.GameObjects[0].gameObject;
			m_item_root = component.GameObjects[1].gameObject;
			btn_ok = component.GameObjects[2].gameObject;
			m_bg = component.GameObjects[3].gameObject;
			txt_title = component.GameObjects[4].gameObject;
			txt_titleText = txt_title.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003CAnimation_003Em__0()
		{
			UIEventListener.Get(btn_ok, string.Empty).onClick = OnClickOk;
		}
	}
}

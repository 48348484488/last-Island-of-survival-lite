using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LadderBuyLevelPanel : View
	{
		public int _buyLevel = 1;

		private List<DropMgr.DropDesInfo> _currentDropDesInfo = new List<DropMgr.DropDesInfo>();

		private UIScrollPanel _uiScrollPanel;

		private int _maxLevel;

		private int _consume;

		private GameObject btn_close;

		private GameObject scp_award;

		private GLadderBuyLevelAwardItem m_cell;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject txt_buy_level;

		private Text txt_buy_levelText;

		private GameObject btn_buy_level;

		private GameObject txt_buy_level_consume;

		private Text txt_buy_level_consumeText;

		protected override void onInit()
		{
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			_uiScrollPanel = scp_award.GetComponent<UIScrollPanel>();
			_maxLevel = LadderCfg.GetAllList().Count;
			ClickListener.Get(btn_up, string.Empty).onClick = OnClickUp;
			ClickListener.Get(btn_down, string.Empty).onClick = OnClickDown;
			ClickListener.Get(btn_buy_level, string.Empty).onClick = OnClickBuy;
			m_cell.gameObject.SetActive(false);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_buyLevel = 1;
			RefreshLevel();
			RefreshReward();
			RefreshConsume();
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevelDelegent));
		}

		private void RefreshLevelDelegent()
		{
			Hide();
		}

		protected override void onHide(string childView = null)
		{
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevelDelegent));
		}

		protected override void onDestroy()
		{
		}

		private void RefreshReward()
		{
			_currentDropDesInfo.Clear();
			_consume = 0;
			for (int i = Singleton<LadderMgr>.Ins.Info.level + 1; i < Singleton<LadderMgr>.Ins.Info.level + _buyLevel + 1; i++)
			{
				LadderCfg ladderCfg = LadderCfg.Get(i);
				_consume += ladderCfg.buyLevelPrice;
				_currentDropDesInfo.AddRange(Singleton<DropMgr>.Ins.GetDropDetailInfo(ladderCfg.freeDropId));
				if (Singleton<LadderMgr>.Ins.Info.taskType == 1 || Singleton<LadderMgr>.Ins.Info.taskType == 2)
				{
					_currentDropDesInfo.AddRange(Singleton<DropMgr>.Ins.GetDropDetailInfo(ladderCfg.payDropId));
				}
			}
			_uiScrollPanel.Clear();
			_uiScrollPanel.Reset(_currentDropDesInfo.Count, FillLadderItemData);
		}

		private void FillLadderItemData(GameObject go, int index)
		{
			GLadderBuyLevelAwardItem component = go.GetComponent<GLadderBuyLevelAwardItem>();
			DropMgr.DropDesInfo dropDesInfo = _currentDropDesInfo[index];
			View.SetItemSprite(component.m_icon, dropDesInfo.icon);
			View.SetLabelText(component.txt_numText, dropDesInfo.num);
		}

		private void OnClickBuy(GameObject go)
		{
			if (_consume > Singleton<RoleMgr>.Ins.Gold)
			{
				AlertBox.Show(155);
				return;
			}
			Singleton<LadderMgr>.Ins.BuyLadderLevel(_buyLevel);
			Hide();
		}

		private void OnClickUp(GameObject go)
		{
			if (Singleton<LadderMgr>.Ins.Info.level + _buyLevel < _maxLevel)
			{
				_buyLevel++;
				RefreshLevel();
				RefreshReward();
			}
		}

		private void OnClickDown(GameObject go)
		{
			if (_buyLevel > 1)
			{
				_buyLevel--;
				RefreshLevel();
				RefreshReward();
			}
		}

		private void RefreshLevel()
		{
			View.SetLabelText(txt_buy_levelText, Utils.GetString(150, _buyLevel));
			RefreshConsume();
		}

		private void RefreshConsume()
		{
			_consume = 0;
			for (int i = Singleton<LadderMgr>.Ins.Info.level + 1; i < Singleton<LadderMgr>.Ins.Info.level + _buyLevel + 1; i++)
			{
				LadderCfg ladderCfg = LadderCfg.Get(i);
				_consume += ladderCfg.buyLevelPrice;
			}
			View.SetLabelText(txt_buy_level_consumeText, _consume);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			scp_award = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GLadderBuyLevelAwardItem>(component.GameObjects[2].gameObject);
			btn_up = component.GameObjects[3].gameObject;
			btn_down = component.GameObjects[4].gameObject;
			txt_buy_level = component.GameObjects[5].gameObject;
			txt_buy_levelText = txt_buy_level.GetComponent<Text>();
			btn_buy_level = component.GameObjects[6].gameObject;
			txt_buy_level_consume = component.GameObjects[7].gameObject;
			txt_buy_level_consumeText = txt_buy_level_consume.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}
	}
}

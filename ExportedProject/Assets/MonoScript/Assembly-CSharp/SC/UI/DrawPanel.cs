using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.battle.scmsg;
using gs.drop.scmsg;
using gs.research.scmsg;
using gs.workbench.scmsg;

namespace SC.UI
{
	public class DrawPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillBoxBagItemData_003Ec__AnonStorey1
		{
			internal ItemCfg itemCfg;

			internal BagItem bagItem;

			internal DrawPanel _0024this;

			internal void _003C_003Em__0(GameObject g)
			{
				if (_0024this._isPress)
				{
					return;
				}
				if (_0024this._researchServerInfo.researchStatus == 2)
				{
					AlertBox.Show(209);
				}
				else if (_0024this._researchServerInfo.researchStatus == 4 || _0024this._researchServerInfo.researchStatus == 3)
				{
					AlertBox.Show(208);
				}
				else if (_0024this._researchServerInfo.researchStatus == 1)
				{
					if (_0024this.DrawOriginalIds.Contains(itemCfg.id))
					{
						_0024this.AddOriginal(bagItem);
						_0024this.AutoAddNecessary();
						_0024this.AutoAddAssist();
						_0024this.UndateBagNoposData();
					}
					else if (_0024this.DrawNecessaryIds.Contains(itemCfg.id))
					{
						AlertBox.Show(207);
					}
					else if (_0024this.DrawAssistIds.Contains(itemCfg.id))
					{
						AlertBox.Show(207);
					}
				}
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this._isPress = true;
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemCfg);
			}

			internal void _003C_003Em__2(GameObject g)
			{
				_0024this._isPress = false;
			}
		}

		private UIScrollPanel _bagScrollPanel;

		private List<BagItem> _bagItems = new List<BagItem>();

		private float _startPressTime = 1f;

		private bool _isPress;

		private SResearchInfo _researchServerInfo = new SResearchInfo();

		private SResearchInfo _researchClientInfo = new SResearchInfo();

		private int _originalInstanceId;

		private int _originalId;

		private int _necessaryNum;

		private int _assistIdNum;

		private bool _isResearching;

		private bool _isCanGet;

		public List<int> DrawOriginalIds = new List<int>();

		public List<int> DrawNecessaryIds = new List<int>();

		public List<int> DrawAssistIds = new List<int>();

		private long _researchId;

		private string _timeCountName = "Research";

		private bool isAddNecessary;

		private int _capacity = 25;

		private int _resultId;

		private bool _resultAddAssist;

		private Image _timeCountIcon;

		private bool _isStartTimeCount;

		private Tweener _tween;

		private int _showCountTime;

		private WaitForSeconds WaitSeconds1;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject scp_bag;

		private GDrawPanelBagItem m_cell;

		private GameObject m_drawing;

		private GDrawPanelOriginalItem m_original;

		private GDrawPanelNecessaryItem m_necessary;

		private GDrawPanelAssistItem m_assist;

		private GDrawPanelProdutionItem m_production;

		private GameObject txt_count_down;

		private Text txt_count_downText;

		private GameObject btn_drawing_preview;

		private GameObject btn_assist_preview;

		private GameObject btn_start;

		private GameObject btn_cancel;

		private GameObject btn_get;

		private GameObject ckb_add_assit;

		private GameObject m_time_root;

		private GameObject txt_sus_tip;

		private Text txt_sus_tipText;

		private GameObject txt_field_tip;

		private Text txt_field_tipText;

		private GameObject m_time_count_icon;

		protected override void onInit()
		{
			_timeCountIcon = m_time_count_icon.GetComponent<Image>();
			_bagScrollPanel = scp_bag.GetComponent<UIScrollPanel>();
			_bagScrollPanel.Clear();
			m_cell.gameObject.SetActiveBetter(false);
			RemoveAssist();
			RemoveNecessary();
			RemoveOriginal();
			m_production.m_icon.SetActiveBetter(false);
			m_production.btn_add.SetActiveBetter(false);
			m_production.m_icon_fail.SetActiveBetter(false);
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(m_original.gameObject, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(m_necessary.gameObject, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(m_assist.gameObject, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_start, string.Empty).onClick = OnStart;
			ClickListener.Get(btn_cancel, string.Empty).onClick = Cancel;
			ClickListener.Get(btn_get, "ui_reward").onClick = Get;
			List<ResearchCfg> allList = ResearchCfg.GetAllList();
			foreach (ResearchCfg item in allList)
			{
				DrawOriginalIds.Add(item.id);
				if (!DrawNecessaryIds.Contains(item.requisiteId))
				{
					DrawNecessaryIds.Add(item.requisiteId);
				}
				if (!DrawAssistIds.Contains(item.assistId))
				{
					DrawAssistIds.Add(item.assistId);
				}
			}
			WaitSeconds1 = Utils.WaitForSeconds(1f);
			ClickListener.Get(ckb_add_assit, string.Empty).onClick = _003ConInit_003Em__4;
			Singleton<ButtonEffectMgr>.Ins.AddGetEffect(btn_get);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SingletonMono<AudioManager>.Ins.Play2D(476);
			_researchId = (long)param;
			SResearchInfo.handler = (SResearchInfo.Handler)Delegate.Combine(SResearchInfo.handler, new SResearchInfo.Handler(SResearchInfoHandle));
			SGetResearchItem.handler = (SGetResearchItem.Handler)Delegate.Combine(SGetResearchItem.handler, new SGetResearchItem.Handler(SGetResearchItemHandle));
			SWorkbenchError.handler = (SWorkbenchError.Handler)Delegate.Combine(SWorkbenchError.handler, new SWorkbenchError.Handler(SWorkbenchErrorHandle));
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Combine(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			Singleton<DrawMgr>.Ins.ResearchInfo(_researchId);
			OnMoneyChange();
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Combine(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
		}

		private void OnSFacilityBuildingUpdate(SFacilityBuildingUpdate msg)
		{
			Singleton<DrawMgr>.Ins.ResearchInfo(_researchId);
		}

		private void SWorkbenchErrorHandle(SWorkbenchError msg)
		{
		}

		private void SGetResearchItemHandle(SGetResearchItem msg)
		{
			ClearResearchServerInfo();
			SResearchInfoHandle(_researchServerInfo);
			DropDetail dropDetail = new DropDetail();
			string @string = Utils.GetString(316);
			if (msg.drawingId > 0)
			{
				dropDetail.dropItem.Add(msg.drawingId, 1);
			}
			else
			{
				dropDetail = msg.dropDetail;
				@string = Utils.GetString(315);
			}
			GainPanel.TitleStr = @string;
			ViewMgr.Ins.ShowTopView<GainPanel>(dropDetail);
		}

		private void ClearResearchServerInfo()
		{
			_researchServerInfo.researchId = _researchId;
			_researchServerInfo.finishTime = 0;
			_researchServerInfo.researchStatus = 1;
			_researchServerInfo.bagItem = new BagItem();
		}

		private void FixResearchServerInfo()
		{
			_researchServerInfo.researchId = _researchId;
			_researchServerInfo.finishTime = 0;
			_researchServerInfo.isAddAssist = false;
			_researchServerInfo.bagItem = new BagItem();
		}

		private void SResearchInfoHandle(SResearchInfo msg)
		{
			_researchServerInfo = msg;
			if (msg.researchId == 0)
			{
				_researchClientInfo.researchId = _researchId;
			}
			else
			{
				if (msg.researchId != _researchId)
				{
					return;
				}
				_researchClientInfo.researchId = msg.researchId;
			}
			if (_researchServerInfo.researchStatus == 3 || _researchServerInfo.researchStatus == 4)
			{
				_resultId = _researchServerInfo.bagItem.itemId;
				_resultAddAssist = _researchServerInfo.isAddAssist;
			}
			if (_researchServerInfo.researchStatus == 3)
			{
				AlertBox.Show(120);
			}
			if (_researchServerInfo.researchStatus != 2)
			{
				msg.finishTime = 0;
				FixResearchServerInfo();
			}
			if (_researchServerInfo.researchStatus == 2)
			{
				SingletonMono<AudioManager>.Ins.Play2D(477);
			}
			txt_field_tip.SetActiveBetter(_researchServerInfo.researchStatus == 3);
			txt_sus_tip.SetActiveBetter(_researchServerInfo.researchStatus == 4);
			ckb_add_assit.GetComponent<Toggle>().interactable = _researchServerInfo.researchStatus != 2;
			m_time_root.SetActiveBetter(msg.finishTime > 0);
			_showCountTime = msg.finishTime;
			View.SetLabelText(txt_count_downText, Utils.GetString(116, _showCountTime));
			_bagItems = GetDrawPanelItems();
			SetBagData();
			OnMoneyChange();
			btn_start.SetActiveBetter(_researchServerInfo.researchStatus == 1);
			btn_cancel.SetActiveBetter(_researchServerInfo.researchStatus == 2);
			btn_get.SetActiveBetter(_researchServerInfo.researchStatus == 4 || _researchServerInfo.researchStatus == 3);
			SetOriginalData();
			SetNecessaryData();
			SetAssistData();
			SetProductionData();
			SetCkbAddAssist();
			if (msg.finishTime > 0)
			{
				RegisterCountDown();
			}
			else
			{
				UnregisterCountDown();
			}
		}

		protected override void onHide(string childView = null)
		{
			SResearchInfo.handler = (SResearchInfo.Handler)Delegate.Remove(SResearchInfo.handler, new SResearchInfo.Handler(SResearchInfoHandle));
			SGetResearchItem.handler = (SGetResearchItem.Handler)Delegate.Remove(SGetResearchItem.handler, new SGetResearchItem.Handler(SGetResearchItemHandle));
			SWorkbenchError.handler = (SWorkbenchError.Handler)Delegate.Remove(SWorkbenchError.handler, new SWorkbenchError.Handler(SWorkbenchErrorHandle));
			SItemChanged.handler = (SItemChanged.Handler)Delegate.Remove(SItemChanged.handler, new SItemChanged.Handler(SItemChangedHandle));
			SFacilityBuildingUpdate.handler = (SFacilityBuildingUpdate.Handler)Delegate.Remove(SFacilityBuildingUpdate.handler, new SFacilityBuildingUpdate.Handler(OnSFacilityBuildingUpdate));
			Clear();
		}

		protected override void onDestroy()
		{
		}

		private void SetBagData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_capacity, FillBoxBagItemData);
		}

		private void SItemChangedHandle(SItemChanged msg)
		{
			SetBagNoposData();
		}

		private void SetBagNoposData()
		{
			_bagScrollPanel.Clear();
			_bagScrollPanel.ResetNoPos(_capacity, FillBoxBagItemData);
		}

		private void UndateBagNoposData()
		{
			_bagScrollPanel.UpdateAllCell(UpdateCellData);
		}

		private void UpdateCellData(GameObject go, int index)
		{
			GDrawPanelBagItem component = go.GetComponent<GDrawPanelBagItem>();
			if (index >= _bagItems.Count)
			{
				component.m_nothing.SetActiveBetter(true);
				component.m_something.SetActiveBetter(false);
				return;
			}
			component.m_nothing.SetActiveBetter(false);
			component.m_something.SetActiveBetter(true);
			BagItem bagItem = _bagItems[index];
			component.m_select_pointer.SetActiveBetter(bagItem.instanceId == _researchClientInfo.bagItem.instanceId);
			component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(bagItem.instanceId));
			component.m_select_material.SetActiveBetter(bagItem.instanceId == _researchClientInfo.bagItem.instanceId);
		}

		private void RemoveAllSelect()
		{
			_bagScrollPanel.UpdateAllCell(RemoveCellSelect);
		}

		private void RemoveCellSelect(GameObject go, int index)
		{
			GDrawPanelBagItem component = go.GetComponent<GDrawPanelBagItem>();
			if (index < _bagItems.Count)
			{
				BagItem bagItem = _bagItems[index];
				component.m_select_material.SetActiveBetter(bagItem.instanceId == _researchClientInfo.bagItem.instanceId);
			}
		}

		private void FillBoxBagItemData(GameObject go, int index)
		{
			_003CFillBoxBagItemData_003Ec__AnonStorey1 _003CFillBoxBagItemData_003Ec__AnonStorey = new _003CFillBoxBagItemData_003Ec__AnonStorey1();
			_003CFillBoxBagItemData_003Ec__AnonStorey._0024this = this;
			GDrawPanelBagItem component = go.GetComponent<GDrawPanelBagItem>();
			if (index >= _bagItems.Count)
			{
				component.m_nothing.SetActiveBetter(true);
				component.m_something.SetActiveBetter(false);
				return;
			}
			component.m_nothing.SetActiveBetter(false);
			component.m_something.SetActiveBetter(true);
			_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem = _bagItems[index];
			_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg = ItemCfg.Get(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.itemId);
			View.SetItemSprite(component.m_icon, _003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.icon);
			View.SetLabelText(component.txt_num, _003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.number);
			component.m_select_pointer.SetActiveBetter(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.instanceId == _researchClientInfo.bagItem.instanceId);
			component.m_new.SetActiveBetter(Singleton<BagMgr>.Ins.IsNew(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.instanceId));
			if (_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.durability > 0)
			{
				component.m_durability.SetActiveBetter(true);
				View.SetSlider(component.m_durability, (float)_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.duration * 1f / (float)_003CFillBoxBagItemData_003Ec__AnonStorey.itemCfg.durability);
			}
			else
			{
				component.m_durability.SetActiveBetter(false);
			}
			component.m_select_material.SetActiveBetter(_003CFillBoxBagItemData_003Ec__AnonStorey.bagItem.instanceId == _researchClientInfo.bagItem.instanceId);
			ClickListener.Get(go, string.Empty).onClick = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__0;
			PressListener.Get(go).onPress = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__1;
			DownUpListener.Get(go).onDown = _003CFillBoxBagItemData_003Ec__AnonStorey._003C_003Em__2;
		}

		public void AddOriginal(BagItem bagItem)
		{
			if (_researchServerInfo.bagItem.itemId > 0)
			{
				View.SetLabelText(m_original.txt_nameText, Utils.GetString(202));
				m_necessary.txt_num.SetActiveBetter(false);
				return;
			}
			m_original.m_icon.SetActiveBetter(true);
			m_original.txt_durability.SetActiveBetter(true);
			m_original.txt_num.SetActiveBetter(true);
			m_original.btn_add.SetActiveBetter(false);
			_researchClientInfo.bagItem = bagItem;
			ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
			View.SetItemSprite(m_original.m_icon, itemCfg.icon);
			View.SetLabelText(m_original.txt_durabilityText, bagItem.duration);
			View.SetLabelText(m_original.txt_nameText, itemCfg.name);
			int itemNum = Singleton<BagMgr>.Ins.GetItemNum(bagItem.itemId);
			View.SetLabelText(m_original.txt_numText, Utils.GetString(9, itemNum, 1));
		}

		public void AddNecessary(int bagNum)
		{
			if (_researchServerInfo.bagItem.itemId > 0)
			{
				View.SetLabelText(m_necessary.txt_nameText, Utils.GetString(203));
				return;
			}
			m_necessary.m_icon.SetActiveBetter(true);
			m_necessary.txt_num.SetActiveBetter(true);
			m_necessary.btn_add.SetActiveBetter(false);
			if (_researchClientInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.requisiteId);
				View.SetItemSprite(m_necessary.m_icon, itemCfg.icon);
				View.SetLabelText(m_necessary.txt_numText, Utils.GetString(9, bagNum, researchCfg.requisiteNum));
				View.SetLabelColor(m_necessary.txt_num, (bagNum < researchCfg.requisiteNum) ? Color.red : Color.white);
				View.SetLabelText(m_necessary.txt_nameText, itemCfg.name);
			}
		}

		public void AutoAddNecessary()
		{
			if (_researchServerInfo.bagItem.itemId > 0)
			{
				View.SetLabelText(m_necessary.txt_nameText, Utils.GetString(203));
				return;
			}
			m_necessary.m_icon.SetActiveBetter(true);
			m_necessary.txt_num.SetActiveBetter(true);
			m_necessary.btn_add.SetActiveBetter(false);
			if (_researchClientInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(researchCfg.requisiteId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.requisiteId);
				View.SetItemSprite(m_necessary.m_icon, itemCfg.icon);
				View.SetLabelText(m_necessary.txt_numText, Utils.GetString(9, itemNum, researchCfg.requisiteNum));
				View.SetLabelColor(m_necessary.txt_num, (itemNum < researchCfg.requisiteNum) ? Color.red : Color.white);
				View.SetLabelText(m_necessary.txt_nameText, itemCfg.name);
			}
		}

		public void AddAssist(int bagNum)
		{
			if (_researchServerInfo.bagItem.itemId > 0)
			{
				View.SetLabelText(m_assist.txt_nameText, Utils.GetString(204));
				return;
			}
			m_assist.m_icon.SetActiveBetter(true);
			m_assist.txt_num.SetActiveBetter(true);
			m_assist.btn_add.SetActiveBetter(false);
			if (_researchClientInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.assistId);
				View.SetItemSprite(m_assist.m_icon, itemCfg.icon);
				View.SetLabelText(m_assist.txt_numText, Utils.GetString(9, bagNum, researchCfg.assistNum));
				View.SetLabelColor(m_assist.txt_num, (bagNum < researchCfg.assistNum) ? Color.red : Color.white);
				View.SetLabelText(m_assist.txt_nameText, itemCfg.name);
			}
		}

		public void AutoAddAssist()
		{
			if (_researchServerInfo.bagItem.itemId > 0)
			{
				View.SetLabelText(m_assist.txt_nameText, Utils.GetString(204));
				return;
			}
			m_assist.m_icon.SetActiveBetter(true);
			m_assist.txt_num.SetActiveBetter(true);
			m_assist.btn_add.SetActiveBetter(false);
			if (_researchClientInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(researchCfg.assistId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.assistId);
				View.SetItemSprite(m_assist.m_icon, itemCfg.icon);
				View.SetLabelText(m_assist.txt_numText, Utils.GetString(9, itemNum, researchCfg.assistNum));
				View.SetLabelColor(m_assist.txt_num, (itemNum < researchCfg.assistNum) ? Color.red : Color.white);
				View.SetLabelText(m_assist.txt_nameText, itemCfg.name);
			}
		}

		public void RemoveOriginal()
		{
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				_researchClientInfo = new SResearchInfo();
				m_original.m_icon.SetActiveBetter(false);
				m_original.txt_durability.SetActiveBetter(false);
				m_original.btn_add.SetActiveBetter(true);
				m_original.txt_num.SetActiveBetter(false);
				View.SetLabelText(m_original.txt_nameText, Utils.GetString(202));
				RemoveNecessary();
				RemoveAssist();
			}
		}

		public void RemoveNecessary()
		{
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				m_necessary.m_icon.SetActiveBetter(false);
				m_necessary.btn_add.SetActiveBetter(true);
				m_necessary.txt_num.SetActiveBetter(false);
				View.SetLabelText(m_necessary.txt_nameText, Utils.GetString(203));
			}
		}

		public void RemoveAssist()
		{
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				_researchClientInfo.isAddAssist = false;
				m_assist.m_icon.SetActiveBetter(false);
				m_assist.txt_num.SetActiveBetter(false);
				m_assist.btn_add.SetActiveBetter(true);
				View.SetLabelText(m_assist.txt_nameText, Utils.GetString(204));
			}
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		public void Clear()
		{
			UnregisterCountDown();
			_researchServerInfo = new SResearchInfo();
			_researchClientInfo = new SResearchInfo();
			RemoveOriginal();
			ClearProduction();
		}

		public void SetCkbAddAssist()
		{
			View.SetCheckbox(ckb_add_assit, _researchServerInfo.isAddAssist);
		}

		public void SetOriginalData()
		{
			View.SetSpriteAlpha(m_original.m_icon, 1f);
			if (_researchServerInfo.researchStatus == 4 || _researchServerInfo.researchStatus == 3)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.lastStartResearchId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.id);
				if (itemCfg != null)
				{
					m_original.m_icon.SetActiveBetter(true);
					m_original.btn_add.SetActive(false);
					m_original.txt_durability.SetActiveBetter(false);
					m_original.txt_num.SetActiveBetter(false);
					View.SetItemSprite(m_original.m_icon, itemCfg.icon);
					View.SetSpriteAlpha(m_original.m_icon, 0.4f);
					View.SetLabelText(m_original.txt_nameText, itemCfg.name);
					return;
				}
			}
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				m_original.m_icon.SetActiveBetter(false);
				m_original.btn_add.SetActive(true);
				m_original.txt_durability.SetActiveBetter(false);
				m_original.txt_num.SetActiveBetter(false);
				View.SetLabelText(m_original.txt_nameText, Utils.GetString(202));
				return;
			}
			ItemCfg itemCfg2 = ItemCfg.Get(_researchServerInfo.bagItem.itemId);
			if (itemCfg2 != null)
			{
				m_original.m_icon.SetActiveBetter(true);
				m_original.btn_add.SetActive(false);
				m_original.txt_durability.SetActiveBetter(true);
				m_original.txt_num.SetActiveBetter(true);
				View.SetItemSprite(m_original.m_icon, itemCfg2.icon);
				View.SetLabelText(m_original.txt_durabilityText, _researchServerInfo.bagItem.duration);
				View.SetLabelText(m_original.txt_nameText, itemCfg2.name);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(_researchServerInfo.bagItem.itemId);
				View.SetLabelText(m_original.txt_numText, Utils.GetString(9, itemNum, 1));
			}
		}

		public void SetNecessaryData()
		{
			View.SetSpriteAlpha(m_necessary.m_icon, 1f);
			if (_researchServerInfo.researchStatus == 4 || _researchServerInfo.researchStatus == 3)
			{
				ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.lastStartResearchId);
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.requisiteId);
				if (itemCfg != null)
				{
					m_necessary.m_icon.SetActiveBetter(true);
					m_necessary.btn_add.SetActive(false);
					m_necessary.txt_num.SetActiveBetter(false);
					View.SetItemSprite(m_necessary.m_icon, itemCfg.icon);
					View.SetLabelText(m_necessary.txt_nameText, itemCfg.name);
					View.SetSpriteAlpha(m_necessary.m_icon, 0.4f);
					return;
				}
			}
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				m_necessary.m_icon.SetActiveBetter(false);
				m_necessary.btn_add.SetActive(true);
				m_necessary.txt_num.SetActiveBetter(false);
				View.SetLabelText(m_necessary.txt_nameText, Utils.GetString(203));
				return;
			}
			m_necessary.m_icon.SetActiveBetter(true);
			m_necessary.btn_add.SetActive(false);
			m_necessary.txt_num.SetActiveBetter(true);
			ResearchCfg researchCfg2 = ResearchCfg.Get(_researchServerInfo.bagItem.itemId);
			ItemCfg itemCfg2 = ItemCfg.Get(researchCfg2.requisiteId);
			if (itemCfg2 != null)
			{
				View.SetItemSprite(m_necessary.m_icon, itemCfg2.icon);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(researchCfg2.requisiteId);
				View.SetLabelText(m_necessary.txt_numText, Utils.GetString(9, itemNum, researchCfg2.requisiteNum));
				View.SetLabelText(m_necessary.txt_nameText, itemCfg2.name);
			}
		}

		public void SetAssistData()
		{
			if (_researchServerInfo.bagItem.itemId <= 0)
			{
				m_assist.m_icon.SetActiveBetter(false);
				m_assist.btn_add.SetActive(true);
				m_assist.txt_num.SetActiveBetter(false);
				View.SetLabelText(m_assist.txt_nameText, Utils.GetString(204));
				return;
			}
			m_assist.m_icon.SetActiveBetter(true);
			m_assist.btn_add.SetActive(false);
			m_assist.txt_num.SetActiveBetter(true);
			ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.bagItem.itemId);
			ItemCfg itemCfg = ItemCfg.Get(researchCfg.assistId);
			if (itemCfg != null)
			{
				View.SetItemSprite(m_assist.m_icon, itemCfg.icon);
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(researchCfg.assistId);
				View.SetLabelText(m_assist.txt_numText, Utils.GetString(9, itemNum, researchCfg.assistNum));
				View.SetLabelText(m_assist.txt_nameText, itemCfg.name);
			}
		}

		public void SetProductionData()
		{
			View.SetLabelText(m_production.txt_nameText, Utils.GetString(206));
			if (_researchServerInfo.researchStatus == 4)
			{
				m_production.m_icon.SetActiveBetter(true);
				m_production.m_icon_fail.SetActive(false);
				ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.lastStartResearchId);
				int key = ((!_researchServerInfo.isAddAssist) ? researchCfg.drawingId2 : researchCfg.drawingId1);
				ItemCfg itemCfg = ItemCfg.Get(key);
				if (itemCfg == null)
				{
					return;
				}
				View.SetItemSprite(m_production.m_icon, itemCfg.icon);
				View.SetLabelText(m_production.txt_nameText, itemCfg.name);
			}
			else if (_researchServerInfo.researchStatus == 3)
			{
				m_production.m_icon.SetActiveBetter(false);
				m_production.m_icon_fail.SetActive(true);
			}
			else
			{
				m_production.m_icon.SetActiveBetter(false);
				m_production.m_icon_fail.SetActive(false);
			}
			m_production.btn_add.SetActiveBetter(_researchServerInfo.researchStatus == 2);
		}

		public void ClearProduction()
		{
			View.SetLabelText(m_production.txt_nameText, Utils.GetString(206));
			m_production.m_icon.SetActiveBetter(false);
			m_production.m_icon_fail.SetActive(false);
			m_production.btn_add.SetActiveBetter(false);
		}

		private void OnStart(GameObject go)
		{
			_researchClientInfo.isAddAssist = View.IsCheckboxChecked(ckb_add_assit);
			if (_researchClientInfo.bagItem.itemId <= 0)
			{
				AlertBox.Show(168);
				return;
			}
			ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
			if (researchCfg.requisiteNum > Singleton<BagMgr>.Ins.GetItemNum(researchCfg.requisiteId))
			{
				ItemCfg itemCfg = ItemCfg.Get(researchCfg.requisiteId);
				AlertBox.Show(Utils.GetString(317, itemCfg.name));
			}
			else if (_researchClientInfo.isAddAssist && researchCfg.assistNum > Singleton<BagMgr>.Ins.GetItemNum(researchCfg.assistId))
			{
				ItemCfg itemCfg2 = ItemCfg.Get(researchCfg.assistId);
				AlertBox.Show(Utils.GetString(348, itemCfg2.name));
			}
			else
			{
				Singleton<DrawMgr>.Ins.StartResearch(_researchClientInfo.researchId, _researchClientInfo.bagItem.instanceId, _researchClientInfo.isAddAssist);
			}
		}

		private void Cancel(GameObject go)
		{
			Singleton<DrawMgr>.Ins.CancelResearch(_researchServerInfo.researchId);
		}

		private void Get(GameObject go)
		{
			ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.lastStartResearchId);
			if (_researchServerInfo.researchStatus == 4)
			{
				int itemId = ((!_resultAddAssist) ? researchCfg.drawingId1 : researchCfg.drawingId2);
				if (!Singleton<BagMgr>.Ins.IsHasCapacity(itemId))
				{
					AlertBox.Show(210);
				}
				else
				{
					Singleton<DrawMgr>.Ins.GetResearchItem(_researchServerInfo.researchId);
				}
			}
			else
			{
				if (_researchServerInfo.researchStatus != 3)
				{
					return;
				}
				List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(researchCfg.failedDorpId);
				for (int i = 0; i < dropDetailInfo.Count; i++)
				{
					DropMgr.DropDesInfo dropDesInfo = dropDetailInfo[i];
					if (!Singleton<BagMgr>.Ins.IsHasCapacity(dropDesInfo.itemId))
					{
						AlertBox.Show(210);
						return;
					}
				}
				Singleton<DrawMgr>.Ins.GetResearchItem(_researchServerInfo.researchId);
			}
		}

		private void UnregisterCountDown()
		{
			TimeManager.UnregisterCountDown(_timeCountName);
			_isStartTimeCount = false;
			if (_tween != null)
			{
				_tween.Kill();
			}
		}

		private void RegisterCountDown()
		{
			_isStartTimeCount = true;
			TimeManager.RegisterCountDown(_timeCountName, _researchServerInfo.finishTime, TiemCountCallBack);
			ResearchCfg researchCfg = ResearchCfg.Get(_researchServerInfo.bagItem.itemId);
			if (_researchServerInfo != null && researchCfg != null)
			{
				if (_tween != null)
				{
					_tween.Kill();
				}
				_timeCountIcon.fillAmount = (float)_researchServerInfo.finishTime * 1f / (float)researchCfg.needTime;
				_tween = _timeCountIcon.DOFillAmount(0f, _researchServerInfo.finishTime).SetEase(Ease.Linear);
			}
		}

		private void TiemCountCallBack()
		{
			_isStartTimeCount = false;
			Singleton<DrawMgr>.Ins.ResearchInfo(_researchServerInfo.researchId);
			if (_tween != null)
			{
				_tween.Kill();
			}
		}

		public List<BagItem> GetDrawPanelItems()
		{
			List<BagItem> list = new List<BagItem>();
			for (int i = 0; i < DrawOriginalIds.Count; i++)
			{
				list.AddRange(Singleton<BagMgr>.Ins.GetAllItemByItemId(DrawOriginalIds[i], false));
			}
			for (int j = 0; j < DrawNecessaryIds.Count; j++)
			{
				BagItem bagItem = new BagItem();
				bagItem.itemId = DrawNecessaryIds[j];
				bagItem.instanceId = bagItem.itemId;
				bagItem.number = Singleton<BagMgr>.Ins.GetItemNum(bagItem.itemId, false);
				if (bagItem.number > 0)
				{
					list.Add(bagItem);
				}
			}
			for (int k = 0; k < DrawAssistIds.Count; k++)
			{
				BagItem bagItem2 = new BagItem();
				bagItem2.itemId = DrawAssistIds[k];
				bagItem2.instanceId = bagItem2.itemId;
				bagItem2.number = Singleton<BagMgr>.Ins.GetItemNum(bagItem2.itemId, false);
				if (bagItem2.number > 0)
				{
					list.Add(bagItem2);
				}
			}
			return list;
		}

		private IEnumerator Tick1()
		{
			while (true)
			{
				yield return WaitSeconds1;
				if (_showCountTime > 0)
				{
					_showCountTime--;
					View.SetLabelText(txt_count_downText, Utils.GetString(116, _showCountTime));
				}
			}
		}

		private void Update()
		{
			if (_isStartTimeCount)
			{
				TimeManager.ShowCountDown(_timeCountName, txt_count_downText, 116);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_blood = component.GameObjects[1].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[2].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[3].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			scp_bag = component.GameObjects[4].gameObject;
			m_cell = View.AddComponentIfNotExist<GDrawPanelBagItem>(component.GameObjects[5].gameObject);
			m_drawing = component.GameObjects[6].gameObject;
			m_original = View.AddComponentIfNotExist<GDrawPanelOriginalItem>(component.GameObjects[7].gameObject);
			m_necessary = View.AddComponentIfNotExist<GDrawPanelNecessaryItem>(component.GameObjects[8].gameObject);
			m_assist = View.AddComponentIfNotExist<GDrawPanelAssistItem>(component.GameObjects[9].gameObject);
			m_production = View.AddComponentIfNotExist<GDrawPanelProdutionItem>(component.GameObjects[10].gameObject);
			txt_count_down = component.GameObjects[11].gameObject;
			txt_count_downText = txt_count_down.GetComponent<Text>();
			btn_drawing_preview = component.GameObjects[12].gameObject;
			btn_assist_preview = component.GameObjects[13].gameObject;
			btn_start = component.GameObjects[14].gameObject;
			btn_cancel = component.GameObjects[15].gameObject;
			btn_get = component.GameObjects[16].gameObject;
			ckb_add_assit = component.GameObjects[17].gameObject;
			m_time_root = component.GameObjects[18].gameObject;
			txt_sus_tip = component.GameObjects[19].gameObject;
			txt_sus_tipText = txt_sus_tip.GetComponent<Text>();
			txt_field_tip = component.GameObjects[20].gameObject;
			txt_field_tipText = txt_field_tip.GetComponent<Text>();
			m_time_count_icon = component.GameObjects[21].gameObject;
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
			if (_researchServerInfo.researchStatus == 1 || _researchServerInfo.researchStatus == 3)
			{
				RemoveOriginal();
				RemoveAllSelect();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (_researchServerInfo.researchStatus == 1 || _researchServerInfo.researchStatus == 3)
			{
				if (_researchClientInfo.bagItem.itemId > 0)
				{
					ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(researchCfg.requisiteId));
				}
				else
				{
					RemoveNecessary();
				}
			}
			else if (_researchServerInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg2 = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(researchCfg2.requisiteId));
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (_researchServerInfo.researchStatus == 1 || _researchServerInfo.researchStatus == 3)
			{
				if (_researchClientInfo.bagItem.itemId > 0)
				{
					ResearchCfg researchCfg = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(researchCfg.assistId));
				}
			}
			else if (_researchServerInfo.bagItem.itemId > 0)
			{
				ResearchCfg researchCfg2 = ResearchCfg.Get(_researchClientInfo.bagItem.itemId);
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(researchCfg2.assistId));
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			if (_researchServerInfo.researchStatus == 2)
			{
				AlertBox.Show(226);
			}
		}
	}
}

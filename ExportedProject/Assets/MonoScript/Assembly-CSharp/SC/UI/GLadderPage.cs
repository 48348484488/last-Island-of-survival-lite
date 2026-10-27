using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.ladder.scmsg;

namespace SC.UI
{
	public class GLadderPage : MonoBehaviour, IBagAndBuildPage
	{
		[CompilerGenerated]
		private sealed class _003CFillLadderItemData_003Ec__AnonStorey0
		{
			internal LadderCfg ladderCfg;

			internal GLadderPage _0024this;
		}

		[CompilerGenerated]
		private sealed class _003CFillLadderItemData_003Ec__AnonStorey1
		{
			internal DropMgr.DropDesInfo juniorDropDesInfo;

			internal _003CFillLadderItemData_003Ec__AnonStorey0 _003C_003Ef__ref_00240;

			internal void _003C_003Em__0(GameObject o)
			{
				_003C_003Ef__ref_00240._0024this.SetSelect(_003C_003Ef__ref_00240.ladderCfg.id, 0, juniorDropDesInfo.itemId);
				_003C_003Ef__ref_00240._0024this.SetItemDescData(juniorDropDesInfo, _003C_003Ef__ref_00240.ladderCfg.id, true);
				_003C_003Ef__ref_00240._0024this.UpdateLadderData();
				if (_003C_003Ef__ref_00240._0024this._info.level >= _003C_003Ef__ref_00240.ladderCfg.id && !_003C_003Ef__ref_00240._0024this._info.rewardedNormalTaskIds.Contains(_003C_003Ef__ref_00240.ladderCfg.id))
				{
					Singleton<LadderMgr>.Ins.GetLadderReward(0, _003C_003Ef__ref_00240.ladderCfg.id, juniorDropDesInfo.itemId, juniorDropDesInfo.num, juniorDropDesInfo.type);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillLadderItemData_003Ec__AnonStorey2
		{
			internal DropMgr.DropDesInfo dropDesInfo;

			internal _003CFillLadderItemData_003Ec__AnonStorey0 _003C_003Ef__ref_00240;

			internal void _003C_003Em__0(GameObject o)
			{
				_003C_003Ef__ref_00240._0024this.SetSelect(_003C_003Ef__ref_00240.ladderCfg.id, 1, dropDesInfo.itemId);
				_003C_003Ef__ref_00240._0024this.SetItemDescData(dropDesInfo, _003C_003Ef__ref_00240.ladderCfg.id, false);
				_003C_003Ef__ref_00240._0024this.UpdateLadderData();
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_003C_003Ef__ref_00240._0024this.SetItemDescData(dropDesInfo, _003C_003Ef__ref_00240.ladderCfg.id, false);
				_003C_003Ef__ref_00240._0024this.SetSelect(_003C_003Ef__ref_00240.ladderCfg.id, 1, dropDesInfo.itemId);
				_003C_003Ef__ref_00240._0024this.UpdateLadderData();
				if (_003C_003Ef__ref_00240._0024this._info.level >= _003C_003Ef__ref_00240.ladderCfg.id && !_003C_003Ef__ref_00240._0024this._info.rewardedBuyedTaskIds.Contains(_003C_003Ef__ref_00240.ladderCfg.id))
				{
					Singleton<LadderMgr>.Ins.GetLadderReward(_003C_003Ef__ref_00240._0024this._info.taskType, _003C_003Ef__ref_00240.ladderCfg.id, dropDesInfo.itemId, dropDesInfo.num, dropDesInfo.type);
				}
			}
		}

		private LadderInfo _info;

		private UIScrollPanel _scrollPanel;

		private ScrollRect _scrollRect;

		private List<LadderCfg> _ladderCfgs;

		private List<LadderCfg> _showAwardladderCfgs;

		private Camera _camera;

		private RenderTexture _renderTexture;

		public int _selectLevel;

		public int _selectType;

		public int _selectId;

		private int _currentId;

		private readonly float _gunLength = 2.5f;

		private readonly Vector3 _initEulerAngle = Vector3.zero;

		private GameObject _currentGunGameObject;

		public GameObject btn_all_get;

		public GameObject btn_buy_ladder;

		public GameObject btn_up_ladder;

		public GameObject m_btn;

		public GameObject m_camera;

		public GLadderLevelAwardItem m_cell;

		public GameObject m_desc;

		public GameObject m_desc_icon;

		public GameObject m_hero_tex;

		public GameObject m_ladder_buy_suo;

		public GameObject m_ladder_notice;

		public GameObject m_laddrer_item_desc;

		public GameObject m_left;

		public GameObject m_model;

		public GameObject m_model_role;

		public GameObject m_model_root;

		public GameObject m_reward;

		public GameObject m_right;

		public GameObject m_role_bg;

		public GameObject m_unlock_condition;

		public GameObject scp_ladder_lavel_award;

		public GameObject txt_desc;

		public Text txt_descText;

		public GameObject txt_name_num;

		public Text txt_name_numText;

		public GameObject txt_unlock_condition;

		public Text txt_unlock_conditionText;

		public object context;

		public bool IsShow
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public void OnInit()
		{
			_showAwardladderCfgs = new List<LadderCfg>();
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			_info = Singleton<LadderMgr>.Ins.Info;
			_scrollPanel = scp_ladder_lavel_award.GetComponent<UIScrollPanel>();
			_scrollRect = scp_ladder_lavel_award.GetComponent<ScrollRect>();
			_ladderCfgs = LadderCfg.GetAllList();
			PrepareLadderData();
			ClickListener.Get(btn_buy_ladder, string.Empty).onClick = OnClickBuyLadder;
			ClickListener.Get(btn_up_ladder, string.Empty).onClick = OnClickBuyLadder;
			ClickListener.Get(m_ladder_buy_suo, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener.Get(btn_all_get, string.Empty).onClick = OnClickGetAll;
		}

		public void OnShow(object param = null)
		{
			LadderEvent.RefreshLadder = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLadder, new Utils.VoidDelegate(RefreshLadder));
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevel));
			LadderEvent.RefreshTaskType = (Utils.VoidDelegate)Delegate.Combine(LadderEvent.RefreshTaskType, new Utils.VoidDelegate(RefreshTaskType));
			m_ladder_notice.SetActiveBetter(true);
			m_laddrer_item_desc.SetActiveBetter(false);
			SetLadderData();
			base.gameObject.SetActiveBetter(true);
			m_btn.SetActiveBetter(_info.taskType == 0);
			m_ladder_notice.SetActiveBetter(_info.taskType == 0);
			btn_all_get.SetActiveBetter(CheckCanReward());
		}

		private void RefreshTaskType()
		{
			SetLadderData();
		}

		private void RefreshLevel()
		{
			SetLadderData();
		}

		private void RefreshLadder()
		{
			UpdateLadderData();
			btn_all_get.SetActiveBetter(CheckCanReward());
		}

		public void OnHide()
		{
			LadderEvent.RefreshLadder = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLadder, new Utils.VoidDelegate(RefreshLadder));
			LadderEvent.RefreshLevel = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshLevel, new Utils.VoidDelegate(RefreshLevel));
			LadderEvent.RefreshTaskType = (Utils.VoidDelegate)Delegate.Remove(LadderEvent.RefreshTaskType, new Utils.VoidDelegate(RefreshTaskType));
			base.gameObject.SetActiveBetter(false);
		}

		private void PrepareLadderData()
		{
			_showAwardladderCfgs.Clear();
			for (int i = 0; i < _ladderCfgs.Count; i++)
			{
				if (_ladderCfgs[i].freeDropId >= 0 || _ladderCfgs[i].payDropId >= 0)
				{
					_showAwardladderCfgs.Add(_ladderCfgs[i]);
				}
			}
		}

		private void SetLadderData()
		{
			_scrollPanel.Clear();
			_scrollPanel.Reset(_showAwardladderCfgs.Count, FillLadderItemData);
			m_ladder_buy_suo.SetActiveBetter(_info.taskType == 0);
		}

		public void OnDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		public void SetSelect(int selectLevel, int selectType, int selectId)
		{
			_selectLevel = selectLevel;
			_selectType = selectType;
			_selectId = selectId;
		}

		public bool IsSelect(int selectLevel, int selectType, int selectId)
		{
			return _selectLevel == selectLevel && _selectType == selectType && _selectId == selectId;
		}

		private void FillLadderItemData(GameObject go, int index)
		{
			_003CFillLadderItemData_003Ec__AnonStorey0 _003CFillLadderItemData_003Ec__AnonStorey = new _003CFillLadderItemData_003Ec__AnonStorey0();
			_003CFillLadderItemData_003Ec__AnonStorey._0024this = this;
			GLadderLevelAwardItem component = go.GetComponent<GLadderLevelAwardItem>();
			component.m_junior_item.m_quality.SetActiveBetter(false);
			_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg = _showAwardladderCfgs[index];
			View.SetLabelText(component.txt_levelText, Utils.GetString(248, _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id));
			component.m_level_mask.SetActiveBetter(_info.level < _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id);
			if (_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.freeDropId <= 0)
			{
				component.m_junior_item.m_suo.SetActiveBetter(false);
				component.m_junior_item.m_already_get.SetActiveBetter(false);
				component.m_junior_item.m_red.SetActiveBetter(false);
				component.m_junior_item.m_select.SetActiveBetter(false);
				component.m_junior_item.m_icon.SetActiveBetter(false);
				component.m_junior_item.txt_name.SetActiveBetter(false);
				component.m_junior_item.m_bind.SetActive(false);
			}
			else
			{
				_003CFillLadderItemData_003Ec__AnonStorey1 _003CFillLadderItemData_003Ec__AnonStorey2 = new _003CFillLadderItemData_003Ec__AnonStorey1();
				_003CFillLadderItemData_003Ec__AnonStorey2._003C_003Ef__ref_00240 = _003CFillLadderItemData_003Ec__AnonStorey;
				List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.freeDropId);
				_003CFillLadderItemData_003Ec__AnonStorey2.juniorDropDesInfo = dropDetailInfo[0];
				component.m_junior_item.m_icon.SetActiveBetter(true);
				component.m_junior_item.txt_name.SetActiveBetter(true);
				View.SetItemSprite(component.m_junior_item.m_icon, _003CFillLadderItemData_003Ec__AnonStorey2.juniorDropDesInfo.icon);
				View.SetLabelText(component.m_junior_item.txt_nameText, _003CFillLadderItemData_003Ec__AnonStorey2.juniorDropDesInfo.num);
				component.m_junior_item.m_suo.SetActiveBetter(_info.level < _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id);
				component.m_junior_item.m_already_get.SetActiveBetter(_info.rewardedNormalTaskIds.Contains(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id));
				component.m_junior_item.m_red.SetActiveBetter(_info.level >= _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id && !_info.rewardedNormalTaskIds.Contains(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id));
				component.m_junior_item.m_select.SetActiveBetter(IsSelect(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id, 0, _003CFillLadderItemData_003Ec__AnonStorey2.juniorDropDesInfo.itemId));
				component.m_junior_item.m_bind.SetActive(dropDetailInfo[0].isBinding);
				ClickListener.Get(component.m_junior_item.gameObject, string.Empty).onClick = _003CFillLadderItemData_003Ec__AnonStorey2._003C_003Em__0;
			}
			if (_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.payDropId <= 0)
			{
				component.m_seniorsObj.SetActiveBetter(false);
				return;
			}
			component.m_seniorsObj.SetActiveBetter(true);
			List<DropMgr.DropDesInfo> dropDetailInfo2 = Singleton<DropMgr>.Ins.GetDropDetailInfo(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.payDropId);
			for (int i = 0; i < component.m_seniors.Length; i++)
			{
				_003CFillLadderItemData_003Ec__AnonStorey2 _003CFillLadderItemData_003Ec__AnonStorey3 = new _003CFillLadderItemData_003Ec__AnonStorey2();
				_003CFillLadderItemData_003Ec__AnonStorey3._003C_003Ef__ref_00240 = _003CFillLadderItemData_003Ec__AnonStorey;
				if (i >= dropDetailInfo2.Count)
				{
					component.m_seniors[i].SetActiveBetter(false);
					continue;
				}
				component.m_seniors[i].SetActiveBetter(true);
				GLadderLevelItem component2 = component.m_seniors[i].GetComponent<GLadderLevelItem>();
				component2.m_quality.SetActiveBetter(false);
				_003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo = dropDetailInfo2[i];
				component2.m_bind.SetActiveBetter(_003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo.isBinding);
				View.SetItemSprite(component2.m_icon, _003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo.icon);
				View.SetLabelText(component2.txt_nameText, _003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo.num);
				if (_info.taskType == 0)
				{
					component2.m_suo.SetActiveBetter(true);
					component2.m_red.SetActiveBetter(false);
					component2.m_already_get.SetActiveBetter(false);
					component2.m_select.SetActiveBetter(IsSelect(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id, 1, _003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo.itemId));
					ClickListener.Get(component2.gameObject, string.Empty).onClick = _003CFillLadderItemData_003Ec__AnonStorey3._003C_003Em__0;
				}
				else
				{
					component2.m_suo.SetActiveBetter(_info.level < _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id);
					component2.m_already_get.SetActiveBetter(_info.rewardedBuyedTaskIds.Contains(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id));
					component2.m_red.SetActiveBetter(_info.level >= _003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id && !_info.rewardedBuyedTaskIds.Contains(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id));
					component2.m_select.SetActiveBetter(IsSelect(_003CFillLadderItemData_003Ec__AnonStorey.ladderCfg.id, 1, _003CFillLadderItemData_003Ec__AnonStorey3.dropDesInfo.itemId));
					ClickListener.Get(component2.gameObject, string.Empty).onClick = _003CFillLadderItemData_003Ec__AnonStorey3._003C_003Em__1;
				}
			}
		}

		private bool CheckCanReward()
		{
			foreach (LadderCfg showAwardladderCfg in _showAwardladderCfgs)
			{
				if (showAwardladderCfg.freeDropId > 0)
				{
					List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(showAwardladderCfg.freeDropId);
					DropMgr.DropDesInfo dropDesInfo = dropDetailInfo[0];
					if (_info.level >= showAwardladderCfg.id && !_info.rewardedNormalTaskIds.Contains(showAwardladderCfg.id))
					{
						return true;
					}
				}
				if (showAwardladderCfg.payDropId <= 0 || _info.taskType == 0)
				{
					continue;
				}
				List<DropMgr.DropDesInfo> dropDetailInfo2 = Singleton<DropMgr>.Ins.GetDropDetailInfo(showAwardladderCfg.payDropId);
				for (int i = 0; i < dropDetailInfo2.Count; i++)
				{
					DropMgr.DropDesInfo dropDesInfo2 = dropDetailInfo2[i];
					if (_info.level >= showAwardladderCfg.id && !_info.rewardedBuyedTaskIds.Contains(showAwardladderCfg.id))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void OnClickGetAll(GameObject go)
		{
			bool flag = false;
			foreach (LadderCfg showAwardladderCfg in _showAwardladderCfgs)
			{
				if (showAwardladderCfg.freeDropId > 0)
				{
					List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(showAwardladderCfg.freeDropId);
					DropMgr.DropDesInfo dropDesInfo = dropDetailInfo[0];
					if (_info.level >= showAwardladderCfg.id && !_info.rewardedNormalTaskIds.Contains(showAwardladderCfg.id))
					{
						flag = true;
						if (!Singleton<LadderMgr>.Ins.GetLadderReward(0, showAwardladderCfg.id, dropDesInfo.itemId, dropDesInfo.num, dropDesInfo.type))
						{
							break;
						}
					}
				}
				if (showAwardladderCfg.payDropId <= 0 || _info.taskType == 0)
				{
					continue;
				}
				List<DropMgr.DropDesInfo> dropDetailInfo2 = Singleton<DropMgr>.Ins.GetDropDetailInfo(showAwardladderCfg.payDropId);
				for (int i = 0; i < dropDetailInfo2.Count; i++)
				{
					DropMgr.DropDesInfo dropDesInfo2 = dropDetailInfo2[i];
					if (_info.level >= showAwardladderCfg.id && !_info.rewardedBuyedTaskIds.Contains(showAwardladderCfg.id))
					{
						flag = true;
						if (!Singleton<LadderMgr>.Ins.GetLadderReward(_info.taskType, showAwardladderCfg.id, dropDesInfo2.itemId, dropDesInfo2.num, dropDesInfo2.type))
						{
							return;
						}
					}
				}
			}
		}

		private void UpdateLadderData()
		{
			_scrollPanel.UpdateAllCell(UpdateLadderItemData);
		}

		private void UpdateLadderItemData(GameObject go, int index)
		{
			GLadderLevelAwardItem component = go.GetComponent<GLadderLevelAwardItem>();
			LadderCfg ladderCfg = _showAwardladderCfgs[index];
			if (ladderCfg.freeDropId > 0)
			{
				component.m_junior.SetActiveBetter(true);
				List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(ladderCfg.freeDropId);
				DropMgr.DropDesInfo dropDesInfo = dropDetailInfo[0];
				component.m_junior_item.m_suo.SetActiveBetter(_info.level < ladderCfg.id);
				component.m_junior_item.m_already_get.SetActiveBetter(_info.rewardedNormalTaskIds.Contains(ladderCfg.id));
				component.m_junior_item.m_red.SetActiveBetter(_info.level >= ladderCfg.id && !_info.rewardedNormalTaskIds.Contains(ladderCfg.id));
				component.m_junior_item.m_select.SetActiveBetter(IsSelect(ladderCfg.id, 0, dropDesInfo.itemId));
			}
			if (ladderCfg.payDropId <= 0)
			{
				return;
			}
			List<DropMgr.DropDesInfo> dropDetailInfo2 = Singleton<DropMgr>.Ins.GetDropDetailInfo(ladderCfg.payDropId);
			for (int i = 0; i < component.m_seniors.Length; i++)
			{
				if (i >= dropDetailInfo2.Count)
				{
					component.m_seniors[i].SetActiveBetter(false);
					continue;
				}
				DropMgr.DropDesInfo dropDesInfo2 = dropDetailInfo2[i];
				component.m_seniors[i].SetActiveBetter(true);
				GLadderLevelItem component2 = component.m_seniors[i].GetComponent<GLadderLevelItem>();
				if (_info.taskType == 0)
				{
					component2.m_suo.SetActiveBetter(true);
					component2.m_red.SetActiveBetter(false);
					component2.m_already_get.SetActiveBetter(false);
					component2.m_select.SetActiveBetter(IsSelect(ladderCfg.id, 1, dropDesInfo2.itemId));
				}
				else
				{
					component2.m_suo.SetActiveBetter(_info.level < ladderCfg.id);
					component2.m_already_get.SetActiveBetter(_info.rewardedBuyedTaskIds.Contains(ladderCfg.id));
					component2.m_red.SetActiveBetter(_info.level >= ladderCfg.id && !_info.rewardedBuyedTaskIds.Contains(ladderCfg.id));
					component2.m_select.SetActiveBetter(IsSelect(ladderCfg.id, 1, dropDesInfo2.itemId));
				}
			}
		}

		public void SetItemDescData(DropMgr.DropDesInfo dropDesInfo, int needLevel, bool isJunior)
		{
			m_laddrer_item_desc.SetActiveBetter(true);
			m_ladder_notice.SetActiveBetter(false);
			View.SetLabelText(txt_name_numText, Utils.GetString(145, dropDesInfo.name, dropDesInfo.num));
			if (isJunior)
			{
				m_unlock_condition.SetActiveBetter(_info.level < needLevel);
				if (_info.level < needLevel)
				{
					LadderCfg ladderCfg = LadderCfg.Get(needLevel);
					View.SetLabelText(txt_unlock_conditionText, ladderCfg.unlockJuniorContition);
				}
			}
			else if (_info.level < needLevel || _info.taskType == 0)
			{
				LadderCfg ladderCfg2 = LadderCfg.Get(needLevel);
				View.SetLabelText(txt_unlock_conditionText, ladderCfg2.unlockSeniorContition);
				m_unlock_condition.SetActiveBetter(true);
			}
			else
			{
				m_unlock_condition.SetActiveBetter(false);
			}
			ItemCfg itemCfg = ItemCfg.Get(dropDesInfo.itemId);
			if (itemCfg == null)
			{
				m_desc_icon.SetActiveBetter(true);
				m_model_root.SetActiveBetter(false);
				View.SetItemSprite(m_desc_icon, dropDesInfo.icon);
				txt_desc.SetActiveBetter(false);
				return;
			}
			txt_desc.SetActiveBetter(true);
			View.SetLabelText(txt_descText, itemCfg.desc);
			if (itemCfg.type == 13)
			{
				m_desc_icon.SetActiveBetter(false);
				m_model_root.SetActiveBetter(true);
				ShowMode(dropDesInfo.itemId);
			}
			else
			{
				m_desc_icon.SetActiveBetter(true);
				m_model_root.SetActiveBetter(false);
				View.SetItemSprite(m_desc_icon, itemCfg.icon);
			}
		}

		public void ShowMode(int itemId)
		{
			m_role_bg.SetActiveBetter(false);
			if (_currentId != itemId)
			{
				_currentId = itemId;
				m_model_root.transform.localEulerAngles = _initEulerAngle;
				ItemCfg itemCfg = ItemCfg.Get(_currentId);
				ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CShowMode_003Em__1);
			}
		}

		public void SetBasicData()
		{
		}

		private void OnClickBuyLadder(GameObject go)
		{
			ViewMgr.Ins.ShowView<LadderBuyTypePanel>();
		}

		public void OnDragEnd()
		{
		}

		private void Awake()
		{
			btn_all_get = base.transform.Find("btn_all_get").gameObject;
			btn_buy_ladder = base.transform.Find("m_btn/btn_buy_ladder").gameObject;
			btn_up_ladder = base.transform.Find("m_desc/m_ladder_notice/btn_up_ladder").gameObject;
			m_btn = base.transform.Find("m_btn").gameObject;
			m_camera = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root/m_model_role/m_camera").gameObject;
			m_cell = View.AddComponentIfNotExist<GLadderLevelAwardItem>(base.transform.Find("GameObject (1)/scp_ladder_lavel_award/content/m_cell").gameObject);
			m_desc = base.transform.Find("m_desc").gameObject;
			m_desc_icon = base.transform.Find("m_desc/m_laddrer_item_desc/m_desc_icon").gameObject;
			m_hero_tex = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root/m_hero_tex").gameObject;
			m_ladder_buy_suo = base.transform.Find("GameObject (1)/GameObject/gaoji/m_ladder_buy_suo").gameObject;
			m_ladder_notice = base.transform.Find("m_desc/m_ladder_notice").gameObject;
			m_laddrer_item_desc = base.transform.Find("m_desc/m_laddrer_item_desc").gameObject;
			m_left = base.transform.Find("GameObject (1)/Image (1)/m_left").gameObject;
			m_model = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root/m_model_role/m_model").gameObject;
			m_model_role = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root/m_model_role").gameObject;
			m_model_root = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root").gameObject;
			m_reward = base.transform.Find("m_reward").gameObject;
			m_right = base.transform.Find("GameObject (1)/Image/m_right").gameObject;
			m_role_bg = base.transform.Find("m_desc/m_laddrer_item_desc/m_model_root/m_role_bg").gameObject;
			m_unlock_condition = base.transform.Find("m_desc/m_laddrer_item_desc/GameObject/m_unlock_condition").gameObject;
			scp_ladder_lavel_award = base.transform.Find("GameObject (1)/scp_ladder_lavel_award").gameObject;
			txt_desc = base.transform.Find("m_desc/m_laddrer_item_desc/GameObject/txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_name_num = base.transform.Find("m_desc/m_laddrer_item_desc/GameObject/txt_name_num").gameObject;
			txt_name_numText = txt_name_num.GetComponent<Text>();
			txt_unlock_condition = base.transform.Find("m_desc/m_laddrer_item_desc/GameObject/m_unlock_condition/txt_unlock_condition").gameObject;
			txt_unlock_conditionText = txt_unlock_condition.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject o)
		{
			m_laddrer_item_desc.SetActiveBetter(false);
			m_ladder_notice.SetActiveBetter(true);
		}

		[CompilerGenerated]
		private void _003CShowMode_003Em__1(GameObject go)
		{
			if ((bool)_currentGunGameObject)
			{
				UnityEngine.Object.DestroyImmediate(_currentGunGameObject);
			}
			go.transform.SetParent(m_model.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			_currentGunGameObject = go;
			Transform transform = go.transform.Find("ef");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(true);
			}
			if (_gunLength > 0f)
			{
				SkinnedMeshRenderer componentInChildren = go.transform.GetComponentInChildren<SkinnedMeshRenderer>();
				float num = _gunLength / componentInChildren.bounds.size.x;
				go.transform.localScale = new Vector3(num, num, num);
				go.transform.localPosition -= go.transform.InverseTransformPoint(componentInChildren.bounds.center) * num;
			}
			else
			{
				go.transform.localPosition -= go.transform.InverseTransformPoint(go.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center);
			}
		}
	}
}

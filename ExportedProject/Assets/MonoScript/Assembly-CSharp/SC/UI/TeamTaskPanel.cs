using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class TeamTaskPanel : View
	{
		private enum TabTypeEnum
		{
			None = 0,
			Task = 1,
			GuideTask = 2,
			Team = 3
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal TabTypeEnum tab;

			internal TeamTaskPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this.OnClickBtn(tab, null);
			}
		}

		private readonly Vector2 ShowBtnsPos = new Vector2(560f, 134f);

		private readonly Vector2 HideBtnsPos = new Vector2(400f, 134f);

		private Dictionary<TabTypeEnum, GameObject> _dicTab2Btn = new Dictionary<TabTypeEnum, GameObject>();

		private Dictionary<TabTypeEnum, ITeamTaskPage> _dicTab2Page = new Dictionary<TabTypeEnum, ITeamTaskPage>();

		private TabTypeEnum _curTab;

		private int _order;

		private RectTransform _rectTransform;

		private float _defaultPosX;

		private float _movePosX;

		private GameObject btn_task;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_team;

		private TeamPage m_team;

		private LitterTaskPage m_task;

		private GameObject txt_num;

		private Text txt_numText;

		private GuidePage m_guide;

		private GameObject btn_show;

		private GameObject m_panel;

		private GameObject btn_hide;

		private GameObject btn_wenhao;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			base.onInit();
			_rectTransform = m_panel.transform as RectTransform;
			_defaultPosX = _rectTransform.anchoredPosition.x;
			m_task.gameObject.SetActiveBetter(false);
			m_team.gameObject.SetActiveBetter(false);
			m_guide.gameObject.SetActiveBetter(false);
			_dicTab2Btn[TabTypeEnum.Task] = btn_task;
			_dicTab2Page[TabTypeEnum.Task] = m_task;
			_dicTab2Btn[TabTypeEnum.GuideTask] = btn_task;
			_dicTab2Page[TabTypeEnum.GuideTask] = m_guide;
			_dicTab2Btn[TabTypeEnum.Team] = btn_team;
			_dicTab2Page[TabTypeEnum.Team] = m_team;
			for (int i = 0; i < Enum.GetNames(typeof(TabTypeEnum)).Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.tab = (TabTypeEnum)i;
				GameObject value;
				if (_dicTab2Btn.TryGetValue(_003ConInit_003Ec__AnonStorey.tab, out value))
				{
					if (_003ConInit_003Ec__AnonStorey.tab != TabTypeEnum.GuideTask)
					{
						ClickListener.Get(value, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
					}
					ITeamTaskPage value2;
					if (_dicTab2Page.TryGetValue(_003ConInit_003Ec__AnonStorey.tab, out value2))
					{
						value2.OnInit();
					}
				}
			}
			ClickListener.Get(btn_hide, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_show, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener clickListener = ClickListener.Get(btn_wenhao, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__2;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			BattlePanel view = ViewMgr.Ins.GetView<BattlePanel>();
			if ((bool)view)
			{
				Canvas component = view.GetComponent<Canvas>();
				if ((bool)component)
				{
					_order = component.sortingOrder + 8;
				}
			}
		}

		public void RefreshShowBtn()
		{
			if (m_team.gameObject.activeSelf && m_team.m_btns.activeSelf)
			{
				btn_show.GetComponent<RectTransform>().anchoredPosition = new Vector2(557f, btn_show.GetComponent<RectTransform>().anchoredPosition.y);
				btn_hide.GetComponent<RectTransform>().anchoredPosition = new Vector2(557f, btn_show.GetComponent<RectTransform>().anchoredPosition.y);
			}
			else
			{
				btn_show.GetComponent<RectTransform>().anchoredPosition = new Vector2(400f, btn_show.GetComponent<RectTransform>().anchoredPosition.y);
				btn_hide.GetComponent<RectTransform>().anchoredPosition = new Vector2(400f, btn_show.GetComponent<RectTransform>().anchoredPosition.y);
			}
		}

		private void OnClickBtn(TabTypeEnum tabType, object param)
		{
			ITeamTaskPage value;
			if (_dicTab2Page.TryGetValue(_curTab, out value) && value.ThisGo.activeSelf)
			{
				value.ThisGo.SetActive(false);
				value.OnHide();
			}
			_curTab = ((tabType != TabTypeEnum.Task) ? tabType : ((Singleton<GuideMgr>.Ins.CurStepCfgInfo == null) ? TabTypeEnum.Task : TabTypeEnum.GuideTask));
			if (_dicTab2Page.TryGetValue(_curTab, out value) && !value.ThisGo.activeSelf)
			{
				RadioButton.ChooseBtn(_dicTab2Btn[_curTab]);
				value.ThisGo.SetActive(true);
				value.OnShow(param);
			}
			RefreshShowBtn();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			OnClickBtn(TabTypeEnum.Task, null);
			btn_show.SetActiveBetter(false);
			btn_hide.SetActiveBetter(true);
			UpdateTeammateNum();
			TeamTaskEvent.HideTeamTaskPanelDelegate = (Utils.VoidDelegate)Delegate.Combine(TeamTaskEvent.HideTeamTaskPanelDelegate, new Utils.VoidDelegate(base.Hide));
			TeamScEvent.UpdateLeaveOrAddTroopEvent = (Utils.VoidDelegate)Delegate.Combine(TeamScEvent.UpdateLeaveOrAddTroopEvent, new Utils.VoidDelegate(UpdateTeammateNum));
			STroopInfo.handler = (STroopInfo.Handler)Delegate.Combine(STroopInfo.handler, new STroopInfo.Handler(OnSTroopInfo));
			GuideEvent.GuideTotalFinishAction = (Action)Delegate.Combine(GuideEvent.GuideTotalFinishAction, new Action(OnGuideTotalFinish));
			TeamTaskEvent.RefreshShowBtnsDelegate = (Utils.VoidDelegate)Delegate.Combine(TeamTaskEvent.RefreshShowBtnsDelegate, new Utils.VoidDelegate(RefreshShowBtn));
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			ITeamTaskPage value;
			if (_dicTab2Page.TryGetValue(_curTab, out value) && value.ThisGo.activeSelf)
			{
				value.ThisGo.SetActive(false);
				value.OnHide();
			}
			TeamTaskEvent.HideTeamTaskPanelDelegate = (Utils.VoidDelegate)Delegate.Remove(TeamTaskEvent.HideTeamTaskPanelDelegate, new Utils.VoidDelegate(base.Hide));
			TeamScEvent.UpdateLeaveOrAddTroopEvent = (Utils.VoidDelegate)Delegate.Remove(TeamScEvent.UpdateLeaveOrAddTroopEvent, new Utils.VoidDelegate(UpdateTeammateNum));
			STroopInfo.handler = (STroopInfo.Handler)Delegate.Remove(STroopInfo.handler, new STroopInfo.Handler(OnSTroopInfo));
			TeamTaskEvent.RefreshShowBtnsDelegate = (Utils.VoidDelegate)Delegate.Remove(TeamTaskEvent.RefreshShowBtnsDelegate, new Utils.VoidDelegate(RefreshShowBtn));
			GuideEvent.GuideTotalFinishAction = (Action)Delegate.Remove(GuideEvent.GuideTotalFinishAction, new Action(OnGuideTotalFinish));
		}

		private void OnGuideTotalFinish()
		{
			if (_curTab == TabTypeEnum.GuideTask)
			{
				OnClickBtn(TabTypeEnum.Task, null);
			}
		}

		private void OnSTroopInfo(STroopInfo msg)
		{
			UpdateTeammateNum();
		}

		private void UpdateTeammateNum()
		{
			View.SetLabelText(txt_numText, Utils.GetString(9, Singleton<TeamScMgr>.Ins.GetTeamates().Count, cfg.Consts.MAX_TEAMATE_NUM));
		}

		public override void _SetRenderSort(int order)
		{
			if (_order <= 0)
			{
				base._SetRenderSort(order);
				return;
			}
			Canvas component = GetComponent<Canvas>();
			if (component == null)
			{
				Debug.LogError(base.gameObject.name + "canvas is null");
				return;
			}
			component.pixelPerfect = false;
			component.overrideSorting = true;
			component.sortingOrder = _order;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_task = component.GameObjects[0].gameObject;
			txt_on = component.GameObjects[1].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[2].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_team = component.GameObjects[3].gameObject;
			m_team = View.AddComponentIfNotExist<TeamPage>(component.GameObjects[4].gameObject);
			m_task = View.AddComponentIfNotExist<LitterTaskPage>(component.GameObjects[5].gameObject);
			txt_num = component.GameObjects[6].gameObject;
			txt_numText = txt_num.GetComponent<Text>();
			m_guide = View.AddComponentIfNotExist<GuidePage>(component.GameObjects[7].gameObject);
			btn_show = component.GameObjects[8].gameObject;
			m_panel = component.GameObjects[9].gameObject;
			btn_hide = component.GameObjects[10].gameObject;
			btn_wenhao = component.GameObjects[11].gameObject;
			txt_on_0 = component.GameObjects[12].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[13].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			_movePosX = btn_show.GetComponent<RectTransform>().anchoredPosition.x;
			_rectTransform.DOAnchorPosX(0f - _movePosX, 0.5f).OnComplete(_003ConInit_003Em__3);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			_rectTransform.DOAnchorPosX(_defaultPosX, 0.5f).OnComplete(_003ConInit_003Em__4);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__2(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<TeamTaskExplainPanel>();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3()
		{
			btn_hide.SetActiveBetter(false);
			btn_show.SetActiveBetter(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4()
		{
			btn_show.SetActiveBetter(false);
			btn_hide.SetActiveBetter(true);
		}
	}
}

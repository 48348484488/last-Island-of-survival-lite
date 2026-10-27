using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.workbench.scmsg;

namespace SC.UI
{
	public class WorkbenchUpgradePage : MonoBehaviour, IWorkbenchPage
	{
		[CompilerGenerated]
		private sealed class _003COnClickBtn_003Ec__AnonStorey1
		{
			internal DrawingNeedMaterial material;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(material.itemId));
			}
		}

		public GameObject btn_cancel;

		public GameObject btn_preview;

		public GameObject btn_upgrage;

		public List<GWorkbenchUpgradeCell> m_materiallist = new List<GWorkbenchUpgradeCell>();

		public GameObject[] m_material;

		public GameObject m_materialObj;

		public GameObject m_workbench_icon;

		public GameObject txt_count_down;

		public Text txt_count_downText;

		public GameObject txt_material_title;

		public Text txt_material_titleText;

		public GameObject txt_upgrage_title;

		public Text txt_upgrage_titleText;

		public object context;

		private int _curLevel;

		private HashSet<int> _notEnoughItemId = new HashSet<int>();

		private float _curUpgradeTime;

		private const float Alpha = 0.5f;

		private int _upgradingSoundId;

		private bool _isUpgrading;

		private bool _onHideUpgradingState;

		private WaitForSeconds _oneWait = new WaitForSeconds(1f);

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache2;

		private bool IsUpgrading
		{
			get
			{
				return _isUpgrading;
			}
			set
			{
				_isUpgrading = value;
				if (value && _upgradingSoundId <= 0)
				{
					_upgradingSoundId = SingletonMono<AudioManager>.Ins.Play2DLoop(475);
				}
				else if (!value && _upgradingSoundId > 0)
				{
					SingletonMono<AudioManager>.Ins.StopMusic(_upgradingSoundId);
					_upgradingSoundId = 0;
				}
			}
		}

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		private void Awake()
		{
			btn_cancel = base.transform.Find("GameObject (2)/btn_cancel").gameObject;
			btn_preview = base.transform.Find("GameObject (2)/btn_preview").gameObject;
			btn_upgrage = base.transform.Find("GameObject (2)/btn_upgrage").gameObject;
			m_material = base.transform.Find("m_material").gameObject.GetComponent<UIGameObjectList>().objects;
			m_materialObj = base.transform.Find("m_material").gameObject;
			if (m_materiallist.Count <= 0)
			{
				for (int i = 0; i < m_material.Length; i++)
				{
					m_materiallist.Add(View.AddComponentIfNotExist<GWorkbenchUpgradeCell>(m_material[i].gameObject));
				}
			}
			m_workbench_icon = base.transform.Find("GameObject/m_workbench_icon").gameObject;
			txt_count_down = base.transform.Find("GameObject (2)/Image (2)/txt_count_down").gameObject;
			txt_count_downText = txt_count_down.GetComponent<Text>();
			txt_material_title = base.transform.Find("m_material/txt_material_title").gameObject;
			txt_material_titleText = txt_material_title.GetComponent<Text>();
			txt_upgrage_title = base.transform.Find("m_material/Image (3)/txt_upgrage_title").gameObject;
			txt_upgrage_titleText = txt_upgrage_title.GetComponent<Text>();
		}

		public void OnInit()
		{
			ClickListener.Get(btn_upgrage, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener clickListener = ClickListener.Get(btn_cancel, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__1;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
		}

		private void OnClickBtn(int level)
		{
			_curLevel = level;
			bool isPrimary2High;
			if (!Singleton<WorkbenchPanelMgr>.Ins.IsLevelToUpgrade(level, out isPrimary2High))
			{
				if (!isPrimary2High && WorkbenchEvent.ChangePageDelegate != null)
				{
					WorkbenchEvent.ChangePageDelegate(WorkbenchPanel.WorkbenchPageType.Study, level);
				}
				else if (isPrimary2High)
				{
					OnClickBtn(2);
				}
				return;
			}
			_notEnoughItemId.Clear();
			IsUpgrading = Singleton<WorkbenchPanelMgr>.Ins.IsCurOpenUpgrading();
			WorkbenchLevelUpCfg workbenchLevelUpCfg = WorkbenchLevelUpCfg.Get(level);
			View.SetLabelText(txt_upgrage_titleText, Utils.GetString((level != 2) ? 154 : 153));
			View.SetLabelText(txt_material_titleText, Utils.GetString((level != 2) ? 200 : 199));
			View.SetTexture(m_workbench_icon, Singleton<WorkbenchPanelMgr>.Ins.GetWorkbenchIcon(level));
			if (!IsUpgrading)
			{
				View.SetLabelText(txt_count_downText, Utils.GetCountDownTime(workbenchLevelUpCfg.needTime));
			}
			btn_cancel.SetActiveBetter(IsUpgrading);
			btn_upgrage.SetActiveBetter(!IsUpgrading);
			List<DrawingNeedMaterial> material = workbenchLevelUpCfg.material;
			int i = 0;
			for (int num = m_material.Length; i < num; i++)
			{
				UIEventListener uIEventListener = UIEventListener.Get(m_material[i], string.Empty);
				bool flag = material.Count > i;
				GWorkbenchUpgradeCell gWorkbenchUpgradeCell = m_materiallist[i];
				gWorkbenchUpgradeCell.m_icon.SetActiveBetter(flag);
				if (flag)
				{
					_003COnClickBtn_003Ec__AnonStorey1 _003COnClickBtn_003Ec__AnonStorey = new _003COnClickBtn_003Ec__AnonStorey1();
					m_material[i].SetActiveBetter(true);
					_003COnClickBtn_003Ec__AnonStorey.material = material[i];
					int itemNum = Singleton<BagMgr>.Ins.GetItemNum(_003COnClickBtn_003Ec__AnonStorey.material.itemId);
					int num2 = _003COnClickBtn_003Ec__AnonStorey.material.num;
					bool flag2 = itemNum >= num2;
					bool flag3 = itemNum > 0;
					gWorkbenchUpgradeCell.txt_num_enough.SetActiveBetter(flag2);
					gWorkbenchUpgradeCell.txt_num_not_enough.SetActiveBetter(!flag2);
					gWorkbenchUpgradeCell.btn_add.SetActiveBetter(!flag2);
					Image component = gWorkbenchUpgradeCell.m_icon.GetComponent<Image>();
					Color color = component.color;
					color.a = ((!flag3) ? 0.5f : 1f);
					component.color = color;
					View.SetItemSprite(gWorkbenchUpgradeCell.m_icon, ItemCfg.Get(_003COnClickBtn_003Ec__AnonStorey.material.itemId).icon);
					if (flag2)
					{
						View.SetLabelText(gWorkbenchUpgradeCell.txt_num_enoughText, Utils.GetString(9, itemNum, num2));
					}
					else
					{
						_notEnoughItemId.Add(_003COnClickBtn_003Ec__AnonStorey.material.itemId);
						View.SetLabelText(gWorkbenchUpgradeCell.txt_num_not_enoughText, Utils.GetString(9, itemNum, num2));
					}
					if (_003C_003Ef__am_0024cache1 == null)
					{
						_003C_003Ef__am_0024cache1 = _003COnClickBtn_003Em__2;
					}
					uIEventListener.onEnter = _003C_003Ef__am_0024cache1;
					if (_003C_003Ef__am_0024cache2 == null)
					{
						_003C_003Ef__am_0024cache2 = _003COnClickBtn_003Em__3;
					}
					uIEventListener.onExit = _003C_003Ef__am_0024cache2;
					uIEventListener.onClick = _003COnClickBtn_003Ec__AnonStorey._003C_003Em__0;
				}
				else
				{
					m_material[i].SetActiveBetter(false);
					uIEventListener.onEnter = null;
					uIEventListener.onExit = null;
					uIEventListener.onClick = null;
				}
			}
		}

		public void OnShow(int level)
		{
			_curLevel = level;
			OnClickBtn(level);
			if (_onHideUpgradingState != IsUpgrading && !IsUpgrading)
			{
				Singleton<WorkbenchPanelMgr>.Ins.SendRequireWorkbenchInfoMsg();
			}
			WorkbenchEvent.RefreshPageDelegate = (Action<SWorkbenchInfo>)Delegate.Combine(WorkbenchEvent.RefreshPageDelegate, new Action<SWorkbenchInfo>(RefreshPage));
		}

		public void OnHide()
		{
			if (_upgradingSoundId > 0)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(_upgradingSoundId);
				_upgradingSoundId = 0;
			}
			WorkbenchEvent.RefreshPageDelegate = (Action<SWorkbenchInfo>)Delegate.Remove(WorkbenchEvent.RefreshPageDelegate, new Action<SWorkbenchInfo>(RefreshPage));
			_onHideUpgradingState = IsUpgrading;
		}

		public void OnClickBagItem(BagItem bagItemInfo, int num)
		{
		}

		private void RefreshPage(SWorkbenchInfo workbenchInfo)
		{
			if ((float)workbenchInfo.finishTime <= Time.realtimeSinceStartup)
			{
				if (Singleton<WorkbenchPanelMgr>.Ins.WorkbenchLevel >= _curLevel && WorkbenchEvent.ChangePageDelegate != null)
				{
					WorkbenchEvent.ChangePageDelegate(WorkbenchPanel.WorkbenchPageType.Study, _curLevel);
				}
				bool isPrimary2High;
				if (!Singleton<WorkbenchPanelMgr>.Ins.IsLevelToUpgrade(_curLevel, out isPrimary2High))
				{
					AlertBox.Show(124, GetLevel());
				}
			}
			else
			{
				btn_cancel.SetActiveBetter(true);
				btn_upgrage.SetActiveBetter(false);
				IsUpgrading = true;
				_curUpgradeTime = workbenchInfo.finishTime;
				OnClickBtn(_curLevel);
			}
		}

		private string GetLevel()
		{
			switch (_curLevel)
			{
			case 2:
				return Utils.GetString(125);
			case 3:
				return Utils.GetString(126);
			default:
				return string.Empty;
			}
		}

		protected void Update()
		{
			if (IsUpgrading)
			{
				if (_curUpgradeTime <= Time.realtimeSinceStartup)
				{
					IsUpgrading = false;
					StartCoroutine(WaitRequireInfo());
				}
				View.SetLabelText(txt_count_downText, Utils.GetCountDownTime((int)(_curUpgradeTime - Time.realtimeSinceStartup)));
			}
		}

		private IEnumerator WaitRequireInfo()
		{
			yield return _oneWait;
			Singleton<WorkbenchPanelMgr>.Ins.SendRequireWorkbenchInfoMsg();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			if (_notEnoughItemId.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (int item in _notEnoughItemId)
				{
					ItemCfg itemCfg = ItemCfg.Get(item);
					if (itemCfg != null)
					{
						stringBuilder.Append(itemCfg.name);
						stringBuilder.Append(",");
					}
				}
				AlertBox.Show(10, stringBuilder.ToString().TrimEnd(','));
			}
			else if (_notEnoughItemId.Count == 0 && !IsUpgrading)
			{
				Singleton<WorkbenchPanelMgr>.Ins.SendUpgradeWorkbenchMsg();
				AlertBox.Show(123);
			}
			else if (IsUpgrading)
			{
				AlertBox.Show(82);
			}
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__1(GameObject go)
		{
			Singleton<WorkbenchPanelMgr>.Ins.SendCancelUpgradeMsg();
			AlertBox.Show(128);
		}

		[CompilerGenerated]
		private static void _003COnClickBtn_003Em__2(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.Repair);
			}
		}

		[CompilerGenerated]
		private static void _003COnClickBtn_003Em__3(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.None);
			}
		}
	}
}

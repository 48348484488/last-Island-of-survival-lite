using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.workbench.scmsg;

namespace SC.UI
{
	public class WorkbenchStudyPage : MonoBehaviour, IWorkbenchPage
	{
		public GameObject btn_assist_preview;

		public GameObject btn_cancel;

		public GameObject btn_drawing_preview;

		public GameObject btn_get;

		public GameObject btn_start;

		public GameObject ckb_add_assist;

		public GWorkbenchStudyCell m_assist;

		public GameObject m_count_down;

		public GWorkbenchStudyCell m_necessary;

		public GWorkbenchStudyCell m_production;

		public GameObject m_round_count_down;

		public GameObject m_workbench_icon;

		public GameObject txt_count_down;

		public Text txt_count_downText;

		public GameObject txt_study_finish;

		public Text txt_study_finishText;

		public GameObject txt_study_level;

		public Text txt_study_levelText;

		public object context;

		private int _curLevel;

		private float _developTotalTime;

		private Development _developInfo;

		private int _useAssistNum;

		private int _useNecessaryNum;

		private ItemCfg _assistMaterial;

		private ItemCfg _necessaryMaterial;

		private Image _productionFillAmount;

		private Image _assistImage;

		private Image _necessaryImage;

		private Toggle _useAssist;

		private const float Alpha = 0.5f;

		private int _studySoundId;

		private bool _isStudying;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache2;

		[CompilerGenerated]
		private static UIEventListener.VoidDelegate _003C_003Ef__am_0024cache3;

		private bool IsStudying
		{
			get
			{
				return _isStudying;
			}
			set
			{
				_isStudying = value;
				if (value && _studySoundId <= 0)
				{
					_studySoundId = SingletonMono<AudioManager>.Ins.Play2DLoop(475);
				}
				else if (!value)
				{
					if (_studySoundId > 0)
					{
						SingletonMono<AudioManager>.Ins.StopMusic(_studySoundId);
						_studySoundId = 0;
					}
					_productionFillAmount.fillAmount = 1f;
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
			btn_assist_preview = base.transform.Find("GameObject (2)/btn_assist_preview").gameObject;
			btn_cancel = base.transform.Find("GameObject (2)/btn_cancel").gameObject;
			btn_drawing_preview = base.transform.Find("GameObject (2)/btn_drawing_preview").gameObject;
			btn_get = base.transform.Find("GameObject (2)/btn_get").gameObject;
			btn_start = base.transform.Find("GameObject (2)/btn_start").gameObject;
			ckb_add_assist = base.transform.Find("GameObject (1)/ckb_add_assist").gameObject;
			m_assist = View.AddComponentIfNotExist<GWorkbenchStudyCell>(base.transform.Find("GameObject (1)/m_assist").gameObject);
			m_count_down = base.transform.Find("GameObject (2)/Image (2)/m_count_down").gameObject;
			m_necessary = View.AddComponentIfNotExist<GWorkbenchStudyCell>(base.transform.Find("GameObject (1)/m_necessary").gameObject);
			m_production = View.AddComponentIfNotExist<GWorkbenchStudyCell>(base.transform.Find("GameObject (1)/m_production").gameObject);
			m_round_count_down = base.transform.Find("GameObject (2)/Image (2)/m_count_down/m_round_count_down").gameObject;
			m_workbench_icon = base.transform.Find("GameObject (1)/m_workbench_icon").gameObject;
			txt_count_down = base.transform.Find("GameObject (2)/Image (2)/m_count_down/txt_count_down").gameObject;
			txt_count_downText = txt_count_down.GetComponent<Text>();
			txt_study_finish = base.transform.Find("GameObject (2)/Image (2)/txt_study_finish").gameObject;
			txt_study_finishText = txt_study_finish.GetComponent<Text>();
			txt_study_level = base.transform.Find("GameObject (1)/Image (1)/txt_study_level").gameObject;
			txt_study_levelText = txt_study_level.GetComponent<Text>();
		}

		public void OnInit()
		{
			_productionFillAmount = m_round_count_down.GetComponent<Image>();
			_assistImage = m_assist.m_icon.GetComponent<Image>();
			_necessaryImage = m_necessary.m_icon.GetComponent<Image>();
			_useAssist = ckb_add_assist.GetComponent<Toggle>();
			UIEventListener uIEventListener = UIEventListener.Get(m_necessary.gameObject, string.Empty);
			UIEventListener uIEventListener2 = uIEventListener;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__0;
			}
			uIEventListener2.onEnter = _003C_003Ef__am_0024cache0;
			UIEventListener uIEventListener3 = uIEventListener;
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003COnInit_003Em__1;
			}
			uIEventListener3.onExit = _003C_003Ef__am_0024cache1;
			uIEventListener.onClick = _003COnInit_003Em__2;
			uIEventListener = UIEventListener.Get(m_assist.gameObject, string.Empty);
			UIEventListener uIEventListener4 = uIEventListener;
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003COnInit_003Em__3;
			}
			uIEventListener4.onEnter = _003C_003Ef__am_0024cache2;
			UIEventListener uIEventListener5 = uIEventListener;
			if (_003C_003Ef__am_0024cache3 == null)
			{
				_003C_003Ef__am_0024cache3 = _003COnInit_003Em__4;
			}
			uIEventListener5.onExit = _003C_003Ef__am_0024cache3;
			uIEventListener.onClick = _003COnInit_003Em__5;
			ClickListener.Get(btn_start, string.Empty).onClick = _003COnInit_003Em__6;
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003COnInit_003Em__7;
			ClickListener.Get(btn_get, string.Empty).onClick = _003COnInit_003Em__8;
			ClickListener.Get(ckb_add_assist, string.Empty).onClick = _003COnInit_003Em__9;
			Singleton<ButtonEffectMgr>.Ins.AddGetEffect(btn_get);
		}

		private void OnClickBtn(int level)
		{
			bool isPrimary2High;
			if (Singleton<WorkbenchPanelMgr>.Ins.IsLevelToUpgrade(level, out isPrimary2High))
			{
				if (!isPrimary2High && WorkbenchEvent.ChangePageDelegate != null)
				{
					WorkbenchEvent.ChangePageDelegate(WorkbenchPanel.WorkbenchPageType.Upgrade, level);
				}
				else if (isPrimary2High)
				{
					WorkbenchEvent.ChangePageDelegate(WorkbenchPanel.WorkbenchPageType.Upgrade, 2);
				}
				return;
			}
			_curLevel = level;
			WorkbenchDevelopmentCfg workbenchDevelopmentCfg = WorkbenchDevelopmentCfg.Get(level);
			_assistMaterial = ItemCfg.Get(workbenchDevelopmentCfg.assistId);
			_necessaryMaterial = ItemCfg.Get(workbenchDevelopmentCfg.requisiteId);
			_useNecessaryNum = 0;
			_useAssistNum = 0;
			View.SetTexture(m_workbench_icon, Singleton<WorkbenchPanelMgr>.Ins.GetWorkbenchIcon(level));
			List<BagItem> bagItems = Singleton<BagMgr>.Ins.BagItems;
			bool flag = false;
			bool flag2 = false;
			int i = 0;
			for (int count = bagItems.Count; i < count; i++)
			{
				BagItem bagItem = bagItems[i];
				if (bagItem.itemId == _necessaryMaterial.id && !flag)
				{
					ItemCfg itemCfg = ItemCfg.Get(bagItem.itemId);
					OnClickBagItem(bagItem, (!itemCfg.isPileAble) ? bagItem.number : Singleton<BagMgr>.Ins.GetItemNum(itemCfg.id));
					flag = true;
				}
				if (bagItem.itemId == _assistMaterial.id && !flag2)
				{
					ItemCfg itemCfg2 = ItemCfg.Get(bagItem.itemId);
					AddAssistMaterial((!itemCfg2.isPileAble) ? bagItem.number : Singleton<BagMgr>.Ins.GetItemNum(itemCfg2.id));
					flag2 = true;
				}
				if (flag2 && flag)
				{
					break;
				}
			}
			if (!flag)
			{
				AddNecessaryMaterialInfo(0);
			}
			if (!flag2)
			{
				AddAssistMaterial(0);
			}
			int num = 0;
			switch (level)
			{
			case 3:
				num = 36;
				break;
			case 2:
				num = 37;
				break;
			case 1:
				num = 38;
				break;
			}
			if (num > 0)
			{
				View.SetLabelText(txt_study_levelText, Utils.GetString(num));
			}
			if (Singleton<WorkbenchPanelMgr>.Ins.GetDevelopInfo(Singleton<WorkbenchPanelMgr>.Ins.CurOpenWorkbench, level, out _developInfo))
			{
				IsStudying = (float)_developInfo.finishTime > Time.realtimeSinceStartup;
				if (IsStudying)
				{
					SetItemInDeveloping(level);
					_useAssist.isOn = _developInfo.isUsedAssist;
				}
				else if (!_developInfo.itemIsGet)
				{
					SetItemCanGet();
				}
				else if (_developInfo.itemIsGet && (float)_developInfo.finishTime <= Time.realtimeSinceStartup)
				{
					SetItemAfterGot();
				}
			}
			else
			{
				IsStudying = false;
				SetItemAfterGot();
			}
		}

		public void OnShow(int level)
		{
			_curLevel = level;
			if (_curLevel > 3 || _curLevel < 1)
			{
				_curLevel = 1;
			}
			_useAssist.isOn = false;
			OnClickBtn(_curLevel);
			WorkbenchEvent.RefreshPageDelegate = (Action<SWorkbenchInfo>)Delegate.Combine(WorkbenchEvent.RefreshPageDelegate, new Action<SWorkbenchInfo>(RefreshPage));
			WorkbenchEvent.ChangeStudyLevelDelegate = (Action<int>)Delegate.Combine(WorkbenchEvent.ChangeStudyLevelDelegate, new Action<int>(OnClickBtn));
			SGetItem.handler = (SGetItem.Handler)Delegate.Combine(SGetItem.handler, new SGetItem.Handler(OnSGetItem));
			SCancelDevelopment.handler = (SCancelDevelopment.Handler)Delegate.Combine(SCancelDevelopment.handler, new SCancelDevelopment.Handler(OnSCancelDevelopment));
		}

		public void OnHide()
		{
			if (_studySoundId > 0)
			{
				SingletonMono<AudioManager>.Ins.StopMusic(_studySoundId);
				_studySoundId = 0;
			}
			WorkbenchEvent.RefreshPageDelegate = (Action<SWorkbenchInfo>)Delegate.Remove(WorkbenchEvent.RefreshPageDelegate, new Action<SWorkbenchInfo>(RefreshPage));
			WorkbenchEvent.ChangeStudyLevelDelegate = (Action<int>)Delegate.Remove(WorkbenchEvent.ChangeStudyLevelDelegate, new Action<int>(OnClickBtn));
			SGetItem.handler = (SGetItem.Handler)Delegate.Remove(SGetItem.handler, new SGetItem.Handler(OnSGetItem));
			SCancelDevelopment.handler = (SCancelDevelopment.Handler)Delegate.Remove(SCancelDevelopment.handler, new SCancelDevelopment.Handler(OnSCancelDevelopment));
		}

		private void OnSCancelDevelopment(SCancelDevelopment msg)
		{
			AlertBox.Show(127);
		}

		public void OnClickBagItem(BagItem bagItemInfo, int num)
		{
			if (_assistMaterial.id == bagItemInfo.itemId)
			{
				AddAssistMaterial(num);
			}
			else if (_necessaryMaterial.id == bagItemInfo.itemId)
			{
				AddNecessaryMaterialInfo(num);
			}
			else
			{
				AlertBox.Show(84);
			}
		}

		protected void Update()
		{
			if (IsStudying && _developInfo != null)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num = _developInfo.finishTime;
				if (num < realtimeSinceStartup)
				{
					IsStudying = false;
					SetItemCanGet();
					AlertBox.Show(122);
				}
				else
				{
					float num2 = num - realtimeSinceStartup;
					_productionFillAmount.fillAmount = (num - realtimeSinceStartup) / _developTotalTime;
					View.SetLabelText(txt_count_downText, Utils.GetCountDownTime((int)num2));
				}
			}
		}

		private void RefreshPage(SWorkbenchInfo workbenchInfo)
		{
			OnClickBtn(_curLevel);
		}

		private void AddNecessaryMaterialInfo(int num)
		{
			_useNecessaryNum += num;
			Color color = _necessaryImage.color;
			color.a = ((_useNecessaryNum <= 0) ? 0.5f : 1f);
			_necessaryImage.color = color;
			WorkbenchDevelopmentCfg workbenchDevelopmentCfg = WorkbenchDevelopmentCfg.Get(_curLevel);
			ItemCfg itemCfg = ItemCfg.Get(workbenchDevelopmentCfg.requisiteId);
			View.SetItemSprite(m_necessary.m_icon, itemCfg.icon);
			View.SetLabelText(m_necessary.txt_nameText, itemCfg.name);
			m_necessary.btn_add.SetActiveBetter(_useNecessaryNum <= 0);
			View.SetLabelText(m_necessary.txt_numText, Utils.GetString(9, _useNecessaryNum, workbenchDevelopmentCfg.requisiteNum));
			Color color2;
			if (_useNecessaryNum < workbenchDevelopmentCfg.requisiteNum)
			{
				ColorUtility.TryParseHtmlString("#E93F3F", out color2);
			}
			else
			{
				color2 = Color.white;
			}
			View.SetLabelColor(m_necessary.txt_numText, color2);
		}

		private void AddAssistMaterial(int useNum)
		{
			_useAssistNum = useNum;
			WorkbenchDevelopmentCfg workbenchDevelopmentCfg = WorkbenchDevelopmentCfg.Get(_curLevel);
			ItemCfg itemCfg = ItemCfg.Get(workbenchDevelopmentCfg.assistId);
			View.SetItemSprite(m_assist.m_icon, itemCfg.icon);
			View.SetLabelText(m_assist.txt_nameText, itemCfg.name);
			Color color = _assistImage.color;
			color.a = ((_useAssistNum <= 0) ? 0.5f : 1f);
			_assistImage.color = color;
			m_assist.btn_add.SetActiveBetter(_useAssistNum <= 0);
			View.SetLabelText(m_assist.txt_numText, Utils.GetString(9, _useAssistNum, workbenchDevelopmentCfg.assistNum));
			Color color2;
			if (_useAssistNum < workbenchDevelopmentCfg.assistNum)
			{
				ColorUtility.TryParseHtmlString("#E93F3F", out color2);
			}
			else
			{
				color2 = Color.white;
			}
			View.SetLabelColor(m_assist.txt_numText, color2);
		}

		private void SetItemCanGet()
		{
			if (_developInfo != null)
			{
				m_count_down.SetActiveBetter(false);
				txt_study_finish.SetActiveBetter(true);
				m_production.btn_add.SetActiveBetter(false);
				btn_get.SetActiveBetter(true);
				btn_cancel.SetActiveBetter(false);
				btn_start.SetActiveBetter(false);
				m_production.m_icon.SetActiveBetter(true);
			}
		}

		private void SetItemInDeveloping(int level)
		{
			WorkbenchDevelopmentCfg workbenchDevelopmentCfg = WorkbenchDevelopmentCfg.Get(_curLevel);
			if (level != _curLevel)
			{
				SetItemAfterGot();
				return;
			}
			m_count_down.SetActiveBetter(true);
			txt_count_down.SetActiveBetter(true);
			txt_study_finish.SetActiveBetter(false);
			btn_get.SetActiveBetter(false);
			btn_cancel.SetActiveBetter(true);
			btn_start.SetActiveBetter(false);
			_developTotalTime = workbenchDevelopmentCfg.needTime;
			Development developInfo;
			if (Singleton<WorkbenchPanelMgr>.Ins.GetDevelopInfo(Singleton<WorkbenchPanelMgr>.Ins.CurOpenWorkbench, _curLevel, out developInfo))
			{
				IsStudying = true;
				m_production.m_icon.SetActiveBetter(false);
				m_production.btn_add.SetActiveBetter(true);
			}
			else
			{
				IsStudying = false;
				m_production.m_icon.SetActiveBetter(false);
				m_production.btn_add.SetActiveBetter(false);
			}
		}

		private void SetItemAfterGot()
		{
			m_count_down.SetActiveBetter(false);
			txt_study_finish.SetActiveBetter(false);
			btn_get.SetActiveBetter(false);
			btn_cancel.SetActiveBetter(false);
			btn_start.SetActiveBetter(true);
			IsStudying = false;
			m_production.m_icon.SetActiveBetter(false);
			m_production.btn_add.SetActiveBetter(false);
			View.SetLabelText(txt_count_downText, Utils.GetCountDownTime(WorkbenchDevelopmentCfg.Get(_curLevel).needTime));
		}

		private void OnSGetItem(SGetItem msg)
		{
			if (msg.developmentId == _curLevel)
			{
				SetItemAfterGot();
				Singleton<GainMgr>.Ins.Add(msg.dropDetail);
			}
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__0(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.Study);
			}
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__1(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.None);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(WorkbenchDevelopmentCfg.Get(_curLevel).requisiteId));
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__3(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.Study);
			}
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__4(GameObject go)
		{
			if (WorkbenchEvent.SetDragTargetDelegate != null)
			{
				WorkbenchEvent.SetDragTargetDelegate(WorkbenchPanel.WorkbenchPageType.None);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(WorkbenchDevelopmentCfg.Get(_curLevel).assistId));
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__6(GameObject go)
		{
			WorkbenchDevelopmentCfg workbenchDevelopmentCfg = WorkbenchDevelopmentCfg.Get(_curLevel);
			if (_useNecessaryNum < workbenchDevelopmentCfg.requisiteNum)
			{
				AlertBox.Show(10, ItemCfg.Get(workbenchDevelopmentCfg.requisiteId).name);
			}
			else if (_useAssist.isOn && (workbenchDevelopmentCfg.assistNum > _useAssistNum || _useAssistNum > Singleton<BagMgr>.Ins.GetItemNum(workbenchDevelopmentCfg.assistId)))
			{
				AlertBox.Show(10, ItemCfg.Get(workbenchDevelopmentCfg.assistId).name);
			}
			else
			{
				Singleton<WorkbenchPanelMgr>.Ins.SendDevelopMsg(_curLevel, _useAssist.isOn);
				AlertBox.Show(121);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__7(GameObject go)
		{
			Singleton<WorkbenchPanelMgr>.Ins.SendCancelDevelop(_curLevel);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__8(GameObject go)
		{
			Singleton<WorkbenchPanelMgr>.Ins.SendGetItemMsg(_curLevel);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__9(GameObject go)
		{
			if (_isStudying)
			{
				AlertBox.Show(213);
				_useAssist.isOn = !_useAssist.isOn;
			}
		}
	}
}

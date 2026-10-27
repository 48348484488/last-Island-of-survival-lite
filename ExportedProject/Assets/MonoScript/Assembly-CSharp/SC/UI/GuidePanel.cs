using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class GuidePanel : View
	{
		private enum CfgDirEnum
		{
			Left = 1,
			Right = 2,
			Down = 3,
			Up = 4
		}

		private enum PanelIndexDirEnum
		{
			Left = 0,
			Right = 1,
			Down = 2,
			Up = 3
		}

		private const string NormalLayer = "Layer_0/";

		private const string TopLayer = "Layer_1/";

		private readonly Dictionary<CfgDirEnum, PanelIndexDirEnum> _dicCfgDir2PanelDir = new Dictionary<CfgDirEnum, PanelIndexDirEnum>();

		private readonly Dictionary<string, GameObject> _dicName2PicGo = new Dictionary<string, GameObject>();

		private readonly Dictionary<string, Transform> _dicName2PicTextPoses = new Dictionary<string, Transform>();

		private Transform _uiRoot;

		private RectTransform _picTextRect;

		private Slider _bloodSlider;

		private readonly Transform[] _effectRectTrans = new Transform[4];

		private RectTransform _effectClickArea;

		private Transform _selectEffectTrans;

		private Transform _selectEffectOldParent;

		private readonly Transform[] _arrowTrans = new Transform[4];

		private Coroutine _hideProtect;

		private string _curViewName = string.Empty;

		private Coroutine _hidePicTextParent;

		private float _noviceFinishTime;

		private float _picTextGuideHideTime;

		private const float IntervalTime = 1f;

		private float _passTime;

		private GameObject m_pic_parent;

		private GameObject m_bg;

		private GameObject[] m_pic_guide_parent;

		private GameObject m_pic_guide_parentObj;

		private GameObject btn_confirm;

		private GameObject m_pic_text_guide_parent;

		private GameObject txt_guide_text;

		private Text txt_guide_textText;

		private GameObject txt_text_hide_time;

		private Text txt_text_hide_timeText;

		private GameObject btn_close_pic_text;

		private GameObject txt_novice_time;

		private Text txt_novice_timeText;

		private GameObject m_mybloodSlilder;

		private GameObject m_mybloodFillImage;

		private GameObject m_novice_des;

		private GameObject m_select_effect_parent;

		private GameObject[] m_select_effect;

		private GameObject m_select_effectObj;

		private GameObject m_drag_screen_effect;

		private List<GGuideArrowTextCell> m_arrow_text_parentlist = new List<GGuideArrowTextCell>();

		private GameObject[] m_arrow_text_parent;

		private GameObject m_arrow_text_parentObj;

		private GameObject m_novice;

		private GameObject[] m_pic_text_guide_pos_parent;

		private GameObject m_pic_text_guide_pos_parentObj;

		private GameObject m_pic_text;

		private GameObject m_select_effect_lu;

		private GameObject m_select_effect_ld;

		private GameObject m_select_effect_ru;

		private GameObject m_select_effect_rd;

		private GameObject m_effect_click_area;

		protected override void onInit()
		{
			base.onInit();
			_dicCfgDir2PanelDir.Add(CfgDirEnum.Left, PanelIndexDirEnum.Left);
			_dicCfgDir2PanelDir.Add(CfgDirEnum.Right, PanelIndexDirEnum.Right);
			_dicCfgDir2PanelDir.Add(CfgDirEnum.Down, PanelIndexDirEnum.Down);
			_dicCfgDir2PanelDir.Add(CfgDirEnum.Up, PanelIndexDirEnum.Up);
			_uiRoot = Singleton<GuideMgr>.Ins.UIRoot;
			_picTextRect = m_pic_text.transform as RectTransform;
			_bloodSlider = m_mybloodSlilder.GetComponent<Slider>();
			_bloodSlider.maxValue = ConstsBs.HpPlayer;
			_effectClickArea = m_effect_click_area.transform as RectTransform;
			_selectEffectTrans = m_select_effect_parent.transform;
			_selectEffectOldParent = _selectEffectTrans.parent;
			m_novice_des.SetActiveBetter(false);
			int i = 0;
			for (int num = m_pic_guide_parent.Length; i < num; i++)
			{
				_dicName2PicGo.Add(m_pic_guide_parent[i].name, m_pic_guide_parent[i]);
			}
			int j = 0;
			for (int num2 = m_pic_text_guide_pos_parent.Length; j < num2; j++)
			{
				_dicName2PicTextPoses.Add(m_pic_text_guide_pos_parent[j].name, m_pic_text_guide_pos_parent[j].transform);
			}
			int k = 0;
			for (int num3 = m_select_effect.Length; k < num3; k++)
			{
				_effectRectTrans[k] = m_select_effect[k].transform;
			}
			int l = 0;
			for (int num4 = m_arrow_text_parent.Length; l < num4; l++)
			{
				_arrowTrans[l] = m_arrow_text_parent[l].transform;
			}
			ClickListener.Get(btn_confirm, string.Empty).onClick = (ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0);
			ClickListener.Get(btn_close_pic_text, string.Empty).onClick = _003ConInit_003Em__1;
			DownUpListener.Get(m_select_effectObj).onDown = _003ConInit_003Em__2;
			DownUpListener.Get(m_drag_screen_effect).onDown = _003ConInit_003Em__3;
			ClickListener.Get(m_novice, string.Empty).onClick = _003ConInit_003Em__4;
		}

		public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
		{
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(data, list);
			GameObject gameObject = data.pointerCurrentRaycast.gameObject;
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject2 = list[i].gameObject;
				if (!(gameObject != gameObject2))
				{
					continue;
				}
				ClickListener componentInParent = gameObject2.GetComponentInParent<ClickListener>();
				if ((bool)componentInParent)
				{
					GameObject gameObject3 = componentInParent.gameObject;
					if (componentInParent.onClick == null)
					{
						break;
					}
					componentInParent.onClick(gameObject3);
					Toggle component = gameObject3.GetComponent<Toggle>();
					if ((bool)component)
					{
						component.isOn = true;
					}
					else if (gameObject2.name.EndsWith("on") || gameObject2.name.EndsWith("off"))
					{
						RadioButton componentInParent2 = gameObject2.GetComponentInParent<RadioButton>();
						if ((bool)componentInParent2)
						{
							RadioButton.ChooseBtn(componentInParent2.gameObject);
						}
					}
					break;
				}
				DownUpListener componentInParent3 = gameObject2.GetComponentInParent<DownUpListener>();
				if ((bool)componentInParent3)
				{
					if (componentInParent3.onDown != null)
					{
						componentInParent3.onDown(componentInParent3.gameObject);
						componentInParent3.onUp(componentInParent3.gameObject);
					}
				}
				else if (gameObject2.name.EndsWith("on") || gameObject2.name.EndsWith("off"))
				{
					RadioButton componentInParent4 = gameObject2.GetComponentInParent<RadioButton>();
					if ((bool)componentInParent4)
					{
						RadioButton.ChooseBtn(componentInParent4.gameObject);
					}
				}
				break;
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			HideAll();
			OnBloodChange();
			StepInCurStep();
			UpdateProtectTime(Singleton<GuideMgr>.Ins.NoviceProtectFinishTime);
			GuideEvent.StepForwardAction = (Action)Delegate.Combine(GuideEvent.StepForwardAction, new Action(StepInCurStep));
			GuideEvent.GuideEffectActiveNeedChangeAction = (Action<bool>)Delegate.Combine(GuideEvent.GuideEffectActiveNeedChangeAction, new Action<bool>(EffectShowOrHide));
			GuideEvent.UpdateProtectTimeAction = (Action<float>)Delegate.Combine(GuideEvent.UpdateProtectTimeAction, new Action<float>(UpdateProtectTime));
			GuideEvent.GuideTotalFinishAction = (Action)Delegate.Combine(GuideEvent.GuideTotalFinishAction, new Action(GuideTotalFinish));
		}

		private void UpdateProtectTime(float finishTime)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (finishTime > realtimeSinceStartup)
			{
				_noviceFinishTime = finishTime;
				if (_hideProtect != null)
				{
					StopCoroutine(_hideProtect);
				}
				_hideProtect = StartCoroutine(HideNoviceDes(finishTime - realtimeSinceStartup));
				RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnBloodChange));
			}
			else
			{
				m_novice.SetActiveBetter(false);
			}
		}

		private void EffectShowOrHide(bool isShow)
		{
			m_select_effect_parent.SetActiveBetter(isShow);
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			if ((bool)this)
			{
				StopAllCoroutines();
			}
			GuideEvent.StepForwardAction = (Action)Delegate.Remove(GuideEvent.StepForwardAction, new Action(StepInCurStep));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnBloodChange));
			GuideEvent.GuideEffectActiveNeedChangeAction = (Action<bool>)Delegate.Remove(GuideEvent.GuideEffectActiveNeedChangeAction, new Action<bool>(EffectShowOrHide));
			GuideEvent.UpdateProtectTimeAction = (Action<float>)Delegate.Remove(GuideEvent.UpdateProtectTimeAction, new Action<float>(UpdateProtectTime));
			GuideEvent.GuideTotalFinishAction = (Action)Delegate.Remove(GuideEvent.GuideTotalFinishAction, new Action(GuideTotalFinish));
		}

		private void GuideTotalFinish()
		{
			_selectEffectTrans.SetParent(_selectEffectOldParent);
			m_select_effect_parent.SetActiveBetter(false);
		}

		private void OnBloodChange()
		{
			View.SetSlider(_bloodSlider, Singleton<RoleMgr>.Ins.Blood);
		}

		private IEnumerator HideNoviceDes(float noviceRemainTime)
		{
			yield return new WaitForSeconds(noviceRemainTime);
			m_novice.SetActiveBetter(false);
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnBloodChange));
			if (Singleton<GuideMgr>.Ins.IsAllNoviceTaskFinish())
			{
				ViewMgr.Ins.Destroy<GuidePanel>();
			}
		}

		private void HideAll()
		{
			m_pic_parent.SetActiveBetter(false);
			m_pic_text_guide_parent.SetActiveBetter(false);
			m_select_effect_parent.SetActiveBetter(false);
		}

		private IEnumerator WaitHide()
		{
			yield return Utils.WaitForSeconds(0.1f);
			Hide();
		}

		private void StepInCurStep()
		{
			GuideCfg curStepCfgInfo = Singleton<GuideMgr>.Ins.CurStepCfgInfo;
			if (curStepCfgInfo == null)
			{
				StartCoroutine(WaitHide());
				return;
			}
			if (!string.IsNullOrEmpty(_curViewName))
			{
				ViewMgr.Ins.RemoveOnShowEvent(_curViewName, OnViewShowSetEffectPos);
				_curViewName = string.Empty;
			}
			m_drag_screen_effect.SetActiveBetter(curStepCfgInfo.id == 7);
			if (curStepCfgInfo.taskCfgId == 3015)
			{
				Singleton<GuideMgr>.Ins.SendRequestNewbeeMonsterMsg();
			}
			if (!Singleton<GuideMgr>.Ins.IsGuideEffectAttachToBtn(curStepCfgInfo.effectBtnName))
			{
				m_select_effect_parent.SetActiveBetter(false);
			}
			else
			{
				string text = (_curViewName = curStepCfgInfo.effectBtnName.Substring(0, curStepCfgInfo.effectBtnName.IndexOf('/')));
				View view = ViewMgr.Ins.GetView(text);
				if (ViewMgr.Ins.IsShow(text) && (!view || ((bool)view && !view.gameObject)))
				{
					GuideCondition value;
					if (curStepCfgInfo.guideConditions.TryGetValue(16, out value) && value.conditionStrArg.Equals(text))
					{
						ViewMgr.Ins.AddOnShowEvent(text, OnViewShowSetEffectPos);
					}
				}
				else
				{
					SetSelectEffectPos(curStepCfgInfo);
				}
			}
			if (curStepCfgInfo.arrowGuideDir != 0)
			{
				m_arrow_text_parentObj.SetActiveBetter(true);
				List<float> effectScale = curStepCfgInfo.effectScale;
				SetArrowText(curStepCfgInfo.arrowGuideDir, curStepCfgInfo.arrowGuideText, effectScale);
			}
			else
			{
				m_arrow_text_parentObj.SetActiveBetter(false);
			}
			if (string.IsNullOrEmpty(curStepCfgInfo.guideText))
			{
				m_pic_text_guide_parent.SetActiveBetter(false);
			}
			else
			{
				m_pic_text_guide_parent.SetActiveBetter(true);
				SetPicTextGuide(curStepCfgInfo.guideText, curStepCfgInfo.guideTextParent);
			}
			if (string.IsNullOrEmpty(curStepCfgInfo.guidePicParent))
			{
				m_pic_parent.SetActiveBetter(false);
				return;
			}
			m_pic_parent.SetActiveBetter(true);
			SetPicGuide(curStepCfgInfo.guidePicParent);
		}

		private void OnViewShowSetEffectPos()
		{
			ViewMgr.Ins.RemoveOnShowEvent(_curViewName, OnViewShowSetEffectPos);
			GuideCfg curStepCfgInfo = Singleton<GuideMgr>.Ins.CurStepCfgInfo;
			GuideCondition value;
			if (curStepCfgInfo != null && curStepCfgInfo.guideConditions.TryGetValue(16, out value) && value.conditionStrArg.Equals(_curViewName))
			{
				SetSelectEffectPos(curStepCfgInfo);
			}
			_curViewName = string.Empty;
		}

		private void SetSelectEffectPos(GuideCfg curStepInfo)
		{
			Transform transform = _uiRoot.Find("Layer_0/" + curStepInfo.effectBtnName);
			m_select_effect_parent.SetActiveBetter(false);
			if (!transform)
			{
				_selectEffectTrans.SetParent(_selectEffectOldParent);
				return;
			}
			_selectEffectTrans.SetParent(transform);
			m_select_effect_parent.SetActiveBetter(true);
			(_selectEffectTrans as RectTransform).anchoredPosition = Vector3.zero;
			List<float> effectScale = curStepInfo.effectScale;
			if (effectScale.Count < 3)
			{
				m_select_effect_parent.SetActiveBetter(false);
				Debug.LogError(string.Concat("[GuidePanel.cs]Wrong Scale : ", curStepInfo.effectScale, " guideCfgId : ", curStepInfo.id));
			}
			float num = effectScale[0];
			float num2 = effectScale[1];
			float num3 = effectScale[2];
			int i = 0;
			for (int num4 = _effectRectTrans.Length; i < num4; i++)
			{
				_effectRectTrans[i].localScale = new Vector3(num, num, num);
				_effectRectTrans[i].localPosition = new Vector3(num2 * (float)((i >= 2) ? 1 : (-1)), num3 * (float)((i % 2 == 0) ? 1 : (-1)), 0f);
			}
			_effectClickArea.sizeDelta = new Vector2(2f * num2, 2f * num3);
		}

		private void SetPicGuide(string guidePicParent)
		{
			foreach (KeyValuePair<string, GameObject> item in _dicName2PicGo)
			{
				if (guidePicParent.Equals(item.Key))
				{
					item.Value.SetActiveBetter(true);
				}
				else
				{
					item.Value.SetActiveBetter(false);
				}
			}
		}

		private void SetPicTextGuide(string guideText, string guideTextParent)
		{
			_picTextGuideHideTime = Time.realtimeSinceStartup + (float)cfg.Consts.GUIDE_PIC_TEXT_SHOW_TIME;
			View.SetLabelText(txt_guide_textText, guideText);
			View.SetLabelText(txt_text_hide_timeText, Utils.GetString(322, cfg.Consts.GUIDE_PIC_TEXT_SHOW_TIME));
			if (_hidePicTextParent != null)
			{
				StopCoroutine(_hidePicTextParent);
			}
			_hidePicTextParent = StartCoroutine(HidePicTextParent());
			Transform value;
			if (_dicName2PicTextPoses.TryGetValue(guideTextParent, out value))
			{
				_picTextRect.anchoredPosition = (value as RectTransform).anchoredPosition;
			}
		}

		private IEnumerator HidePicTextParent()
		{
			yield return Utils.WaitForSeconds(cfg.Consts.GUIDE_PIC_TEXT_SHOW_TIME);
			m_pic_text_guide_parent.SetActiveBetter(false);
		}

		protected void Update()
		{
			_passTime += Time.deltaTime;
			if (_passTime > 1f)
			{
				_passTime = 0f;
				if (_picTextGuideHideTime > Time.realtimeSinceStartup)
				{
					View.SetLabelText(txt_text_hide_timeText, Utils.GetString(322, (int)(_picTextGuideHideTime - Time.realtimeSinceStartup)));
				}
				if (_noviceFinishTime > Time.realtimeSinceStartup)
				{
					View.SetLabelText(txt_novice_timeText, Utils.GetNoHourTimeString((int)(_noviceFinishTime - Time.realtimeSinceStartup)));
				}
			}
		}

		private void SetArrowText(int arrowGuideDir, string arrowGuideText, List<float> offset)
		{
			int num = (int)_dicCfgDir2PanelDir[(CfgDirEnum)arrowGuideDir];
			int i = 0;
			for (int num2 = m_arrow_text_parent.Length; i < num2; i++)
			{
				m_arrow_text_parent[i].SetActiveBetter(num == i);
			}
			switch (arrowGuideDir)
			{
			case 3:
				ChangeLocalYPos(_arrowTrans[num], offset[2]);
				break;
			case 4:
				ChangeLocalYPos(_arrowTrans[num], 0f - offset[2]);
				break;
			case 2:
				ChangeLocalXPos(_arrowTrans[num], 0f - offset[1]);
				break;
			case 1:
				ChangeLocalXPos(_arrowTrans[num], offset[1]);
				break;
			}
			View.SetLabelText(m_arrow_text_parentlist[num].txt_guideText, arrowGuideText);
		}

		private void ChangeLocalXPos(Transform trans, float localX)
		{
			Vector3 localPosition = trans.localPosition;
			localPosition.x = localX;
			trans.localPosition = localPosition;
		}

		private void ChangeLocalYPos(Transform trans, float localY)
		{
			Vector3 localPosition = trans.localPosition;
			localPosition.y = localY;
			trans.localPosition = localPosition;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_pic_parent = component.GameObjects[0].gameObject;
			m_bg = component.GameObjects[1].gameObject;
			m_pic_guide_parent = component.GameObjects[2].gameObject.GetComponent<UIGameObjectList>().objects;
			m_pic_guide_parentObj = component.GameObjects[2].gameObject;
			btn_confirm = component.GameObjects[3].gameObject;
			m_pic_text_guide_parent = component.GameObjects[4].gameObject;
			txt_guide_text = component.GameObjects[5].gameObject;
			txt_guide_textText = txt_guide_text.GetComponent<Text>();
			txt_text_hide_time = component.GameObjects[6].gameObject;
			txt_text_hide_timeText = txt_text_hide_time.GetComponent<Text>();
			btn_close_pic_text = component.GameObjects[7].gameObject;
			txt_novice_time = component.GameObjects[8].gameObject;
			txt_novice_timeText = txt_novice_time.GetComponent<Text>();
			m_mybloodSlilder = component.GameObjects[9].gameObject;
			m_mybloodFillImage = component.GameObjects[10].gameObject;
			m_novice_des = component.GameObjects[11].gameObject;
			m_select_effect_parent = component.GameObjects[12].gameObject;
			m_select_effect = component.GameObjects[13].gameObject.GetComponent<UIGameObjectList>().objects;
			m_select_effectObj = component.GameObjects[13].gameObject;
			m_drag_screen_effect = component.GameObjects[14].gameObject;
			m_arrow_text_parent = component.GameObjects[15].gameObject.GetComponent<UIGameObjectList>().objects;
			m_arrow_text_parentObj = component.GameObjects[15].gameObject;
			if (m_arrow_text_parentlist.Count <= 0)
			{
				for (int i = 0; i < m_arrow_text_parent.Length; i++)
				{
					m_arrow_text_parentlist.Add(View.AddComponentIfNotExist<GGuideArrowTextCell>(m_arrow_text_parent[i].gameObject));
				}
			}
			m_novice = component.GameObjects[16].gameObject;
			m_pic_text_guide_pos_parent = component.GameObjects[17].gameObject.GetComponent<UIGameObjectList>().objects;
			m_pic_text_guide_pos_parentObj = component.GameObjects[17].gameObject;
			m_pic_text = component.GameObjects[18].gameObject;
			m_select_effect_lu = component.GameObjects[19].gameObject;
			m_select_effect_ld = component.GameObjects[20].gameObject;
			m_select_effect_ru = component.GameObjects[21].gameObject;
			m_select_effect_rd = component.GameObjects[22].gameObject;
			m_effect_click_area = component.GameObjects[23].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			m_pic_parent.SetActiveBetter(false);
			GuideCfg curStepCfgInfo = Singleton<GuideMgr>.Ins.CurStepCfgInfo;
			if (curStepCfgInfo != null && curStepCfgInfo.specialFinishCondition == 17 && GuideEvent.StepCompleteAction != null)
			{
				GuideEvent.StepCompleteAction();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			_picTextGuideHideTime = 0f;
			m_pic_text_guide_parent.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			PassEvent(DownUpListener.pointEventData, ExecuteEvents.pointerClickHandler);
			if (GuideEvent.StepCompleteAction != null)
			{
				GuideEvent.StepCompleteAction();
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			m_drag_screen_effect.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			m_novice_des.SetActiveBetter(!m_novice_des.activeSelf);
		}
	}
}

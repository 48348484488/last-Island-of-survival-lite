using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class SettingGBasicPage : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003CSetSlide_003Ec__AnonStorey0
		{
			internal Slider slider;

			internal Utils.FloatDelegate callBack;

			internal SettingGBasicSlideItem settingGBasicSlideItem;

			internal SettingGBasicPage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.OnUpSlider(slider);
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_0024this.OnDownSlider(slider);
			}

			internal void _003C_003Em__2(float value)
			{
				Utils.TriggerEvent(callBack, value);
				_0024this.ShowValue(slider, settingGBasicSlideItem.txt_valueText);
			}
		}

		public GameObject btn_language;

		public GameObject m_leftshoot_mode;

		public GameObject m_model1;

		public GameObject m_model2;

		public GameObject m_model3;

		public SettingGBasicSlideItem m_music;

		public SettingGBasicSlideItem m_sensitivity_2_aim;

		public SettingGBasicSlideItem m_sensitivity_4_aim;

		public SettingGBasicSlideItem m_sensitivity_horizontal;

		public SettingGBasicSlideItem m_sensitivity_red_aim;

		public SettingGBasicSlideItem m_sensitivity_vertical;

		public SettingGBasicSlideItem m_sound;

		public GameObject m_zudui;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		public void OnInit()
		{
			InitSetting();
			SetSlide(m_music, OnMusicChange);
			SetSlide(m_sound, OnSoundChange);
			SetSlide(m_sensitivity_vertical, OnSensitivityV);
			SetSlide(m_sensitivity_horizontal, OnSensitivityH);
			SetSlide(m_sensitivity_red_aim, OnSensitivityRed);
			SetSlide(m_sensitivity_2_aim, OnSensitivity2);
			SetSlide(m_sensitivity_4_aim, OnSensitivity4);
			Toggle component = m_zudui.GetComponent<Toggle>();
			component.isOn = SettingMgr.ZuDui;
			ClickListener.Get(m_model1, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener.Get(m_model2, string.Empty).onClick = _003COnInit_003Em__1;
			ClickListener.Get(m_model3, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener.Get(m_zudui, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener clickListener = ClickListener.Get(btn_language, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__4;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
		}

		private void OnLeftShoot(int value)
		{
			SettingMgr.LeftShootMode = value;
		}

		private void OnMusicChange(float value)
		{
			SettingMgr.MusicValume = value;
		}

		private void OnSoundChange(float value)
		{
			SettingMgr.SoundEffectValume = value;
		}

		private void OnSensitivityH(float value)
		{
			SettingMgr.SensitivityHorizontal = value;
		}

		private void OnSensitivityV(float value)
		{
			SettingMgr.SensitivityVertical = value;
		}

		private void OnSensitivityRed(float value)
		{
			SettingMgr.SensitivityAimRed = value;
		}

		private void OnSensitivityHolo(float value)
		{
			SettingMgr.SensitivityAimHolo = value;
		}

		private void OnSensitivity2(float value)
		{
			SettingMgr.SensitivityAim2 = value;
		}

		private void OnSensitivity4(float value)
		{
			SettingMgr.SensitivityAim4 = value;
		}

		private void OnDownSlider(Slider slider)
		{
			if (!(slider.value <= 0f))
			{
				if (slider.value - 0.01f <= 0f)
				{
					slider.value = 0f;
				}
				else
				{
					slider.value -= 0.01f;
				}
			}
		}

		private void OnUpSlider(Slider slider)
		{
			if (!(slider.value >= 1f))
			{
				if (slider.value + 0.01f > 1f)
				{
					slider.value = 1f;
				}
				else
				{
					slider.value += 0.01f;
				}
			}
		}

		private void SetSlide(SettingGBasicSlideItem settingGBasicSlideItem, Utils.FloatDelegate callBack)
		{
			_003CSetSlide_003Ec__AnonStorey0 _003CSetSlide_003Ec__AnonStorey = new _003CSetSlide_003Ec__AnonStorey0();
			_003CSetSlide_003Ec__AnonStorey.callBack = callBack;
			_003CSetSlide_003Ec__AnonStorey.settingGBasicSlideItem = settingGBasicSlideItem;
			_003CSetSlide_003Ec__AnonStorey._0024this = this;
			_003CSetSlide_003Ec__AnonStorey.slider = _003CSetSlide_003Ec__AnonStorey.settingGBasicSlideItem.m_slide.GetComponent<Slider>();
			ClickListener.Get(_003CSetSlide_003Ec__AnonStorey.settingGBasicSlideItem.btn_up, string.Empty).onClick = _003CSetSlide_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(_003CSetSlide_003Ec__AnonStorey.settingGBasicSlideItem.btn_down, string.Empty).onClick = _003CSetSlide_003Ec__AnonStorey._003C_003Em__1;
			_003CSetSlide_003Ec__AnonStorey.slider.onValueChanged.AddListener(_003CSetSlide_003Ec__AnonStorey._003C_003Em__2);
		}

		private void ShowValue(Slider slider, Text text)
		{
			View.SetLabelText(text, Utils.GetString(268, (int)(slider.value * 100f)));
		}

		private void InitSlide(SettingGBasicSlideItem settingGBasicSlideItem, float slideValue)
		{
			Slider component = settingGBasicSlideItem.m_slide.GetComponent<Slider>();
			component.value = slideValue;
			ShowValue(component, settingGBasicSlideItem.txt_valueText);
		}

		private void InitSetting()
		{
			InitSlide(m_music, SettingMgr.MusicValume);
			InitSlide(m_sound, SettingMgr.SoundEffectValume);
			InitSlide(m_sensitivity_vertical, SettingMgr.SensitivityVertical);
			InitSlide(m_sensitivity_horizontal, SettingMgr.SensitivityHorizontal);
			InitSlide(m_sensitivity_red_aim, SettingMgr.SensitivityAimRed);
			InitSlide(m_sensitivity_2_aim, SettingMgr.SensitivityAim2);
			InitSlide(m_sensitivity_4_aim, SettingMgr.SensitivityAim4);
			View.SetCheckbox(m_model1, SettingMgr.LeftShootMode == 0);
			View.SetCheckbox(m_model2, SettingMgr.LeftShootMode == 1);
			View.SetCheckbox(m_model3, SettingMgr.LeftShootMode == 2);
		}

		public void OnShow()
		{
			SettingEvent.ResertSettingDelegate = (Utils.VoidDelegate)Delegate.Combine(SettingEvent.ResertSettingDelegate, new Utils.VoidDelegate(ResertSettingDelegate));
		}

		public void OnHide()
		{
			SettingEvent.ResertSettingDelegate = (Utils.VoidDelegate)Delegate.Remove(SettingEvent.ResertSettingDelegate, new Utils.VoidDelegate(ResertSettingDelegate));
		}

		private void ResertSettingDelegate()
		{
			InitSetting();
		}

		private void Awake()
		{
			btn_language = base.transform.Find("GameObject (2)/btn_language").gameObject;
			m_leftshoot_mode = base.transform.Find("GameObject (2)/mode/m_leftshoot_mode").gameObject;
			m_model1 = base.transform.Find("GameObject (2)/mode/m_leftshoot_mode/m_model1").gameObject;
			m_model2 = base.transform.Find("GameObject (2)/mode/m_leftshoot_mode/m_model2").gameObject;
			m_model3 = base.transform.Find("GameObject (2)/mode/m_leftshoot_mode/m_model3").gameObject;
			m_music = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/music/m_music").gameObject);
			m_sensitivity_2_aim = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/sensityvity/m_sensitivity_2_aim").gameObject);
			m_sensitivity_4_aim = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/sensityvity/m_sensitivity_4_aim").gameObject);
			m_sensitivity_horizontal = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/sensityvity/m_sensitivity_horizontal").gameObject);
			m_sensitivity_red_aim = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/sensityvity/m_sensitivity_red_aim").gameObject);
			m_sensitivity_vertical = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/sensityvity/m_sensitivity_vertical").gameObject);
			m_sound = View.AddComponentIfNotExist<SettingGBasicSlideItem>(base.transform.Find("GameObject (2)/music/m_sound").gameObject);
			m_zudui = base.transform.Find("GameObject (2)/mode/m_leftshoot_mode/m_zudui").gameObject;
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			OnLeftShoot(0);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__1(GameObject go)
		{
			OnLeftShoot(1);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			OnLeftShoot(2);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__3(GameObject go)
		{
			Toggle component = m_zudui.GetComponent<Toggle>();
			SettingMgr.ZuDui = component.isOn;
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__4(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<LanguagePanel>();
		}
	}
}

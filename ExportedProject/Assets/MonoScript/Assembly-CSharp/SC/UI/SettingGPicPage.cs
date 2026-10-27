using System.Runtime.CompilerServices;
using UnityEngine;

namespace SC.UI
{
	public class SettingGPicPage : MonoBehaviour
	{
		public GameObject btn_reset;

		public GameObject ckb_30;

		public GameObject ckb_48;

		public GameObject ckb_60;

		public GameObject ckb_high;

		public GameObject ckb_light1;

		public GameObject ckb_light2;

		public GameObject ckb_light3;

		public GameObject ckb_light4;

		public GameObject ckb_light5;

		public GameObject ckb_low;

		public GameObject ckb_medium;

		public GameObject ckb_resolution1;

		public GameObject ckb_resolution2;

		public GameObject ckb_resolution3;

		public GameObject ckb_shadow1;

		public GameObject ckb_shadow2;

		public GameObject ckb_shadow3;

		public GameObject ckb_shadow4;

		public GameObject ckb_shadow5;

		public GameObject ckb_zidingyi;

		public GameObject m_change;

		public object context;

		public void OnInit()
		{
			ClickListener.Get(ckb_low, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener.Get(ckb_medium, string.Empty).onClick = _003COnInit_003Em__1;
			ClickListener.Get(ckb_high, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener.Get(ckb_zidingyi, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener.Get(ckb_shadow1, string.Empty).onClick = _003COnInit_003Em__4;
			ClickListener.Get(ckb_shadow2, string.Empty).onClick = _003COnInit_003Em__5;
			ClickListener.Get(ckb_shadow3, string.Empty).onClick = _003COnInit_003Em__6;
			ClickListener.Get(ckb_shadow4, string.Empty).onClick = _003COnInit_003Em__7;
			ClickListener.Get(ckb_shadow5, string.Empty).onClick = _003COnInit_003Em__8;
			ClickListener.Get(ckb_light1, string.Empty).onClick = _003COnInit_003Em__9;
			ClickListener.Get(ckb_light2, string.Empty).onClick = _003COnInit_003Em__A;
			ClickListener.Get(ckb_light3, string.Empty).onClick = _003COnInit_003Em__B;
			ClickListener.Get(ckb_light4, string.Empty).onClick = _003COnInit_003Em__C;
			ClickListener.Get(ckb_light5, string.Empty).onClick = _003COnInit_003Em__D;
			ClickListener.Get(ckb_30, string.Empty).onClick = _003COnInit_003Em__E;
			ClickListener.Get(ckb_48, string.Empty).onClick = _003COnInit_003Em__F;
			ClickListener.Get(ckb_60, string.Empty).onClick = _003COnInit_003Em__10;
			ClickListener.Get(ckb_resolution1, string.Empty).onClick = _003COnInit_003Em__11;
			ClickListener.Get(ckb_resolution2, string.Empty).onClick = _003COnInit_003Em__12;
			ClickListener.Get(ckb_resolution3, string.Empty).onClick = _003COnInit_003Em__13;
		}

		public void OnShow()
		{
			RefreshTopQualityCheckBox();
			RefreshShadowQualityCheckBox();
			RefreshRenderQualityCheckBox();
			RefreshFrameCheckBox();
			RefreshresolutionCheckBox();
			m_change.SetActiveBetter(false);
		}

		public void OnHide()
		{
		}

		private void ClickQuality()
		{
			m_change.SetActiveBetter(true);
			DelayInvoker.DelayInvoke("setting", 0.1f, _003CClickQuality_003Em__14);
			DelayInvoker.DelayInvoke("settingFinish", 1.1f, _003CClickQuality_003Em__15);
		}

		private void OnQualityLevel(int value)
		{
			SettingMgr.QualityLevel = value;
			RefreshTopQualityCheckBox();
		}

		private void OnShadowQuality(int value)
		{
			SettingMgr.ShadowQuality = value;
		}

		private void RefreshTopQualityCheckBox()
		{
			View.SetCheckbox(ckb_low, SettingMgr.QualityLevel == 0);
			View.SetCheckbox(ckb_medium, SettingMgr.QualityLevel == 1);
			View.SetCheckbox(ckb_high, SettingMgr.QualityLevel == 2);
			View.SetCheckbox(ckb_zidingyi, SettingMgr.QualityLevel == 3);
		}

		private void RefreshShadowQualityCheckBox()
		{
			View.SetCheckbox(ckb_shadow1, SettingMgr.ShadowQuality == 1);
			View.SetCheckbox(ckb_shadow2, SettingMgr.ShadowQuality == 2);
			View.SetCheckbox(ckb_shadow3, SettingMgr.ShadowQuality == 3);
			View.SetCheckbox(ckb_shadow4, SettingMgr.ShadowQuality == 4);
			View.SetCheckbox(ckb_shadow5, SettingMgr.ShadowQuality == 5);
		}

		private void RefreshRenderQualityCheckBox()
		{
			View.SetCheckbox(ckb_light1, SettingMgr.RenderQuality == 1);
			View.SetCheckbox(ckb_light2, SettingMgr.RenderQuality == 2);
			View.SetCheckbox(ckb_light3, SettingMgr.RenderQuality == 3);
			View.SetCheckbox(ckb_light4, SettingMgr.RenderQuality == 4);
			View.SetCheckbox(ckb_light5, SettingMgr.RenderQuality == 5);
		}

		private void RefreshFrameCheckBox()
		{
			View.SetCheckbox(ckb_30, SettingMgr.FrameRateQuality == 0);
			View.SetCheckbox(ckb_48, SettingMgr.FrameRateQuality == 1);
			View.SetCheckbox(ckb_60, SettingMgr.FrameRateQuality == 2);
		}

		private void RefreshresolutionCheckBox()
		{
			View.SetCheckbox(ckb_resolution1, SettingMgr.ResolutionLevel == 0);
			View.SetCheckbox(ckb_resolution2, SettingMgr.ResolutionLevel == 1);
			View.SetCheckbox(ckb_resolution3, SettingMgr.ResolutionLevel == 2);
		}

		private void Awake()
		{
			btn_reset = base.transform.Find("btn_reset").gameObject;
			ckb_30 = base.transform.Find("Image (1)/items/content/zhenshu/GameObject/ckb_30").gameObject;
			ckb_48 = base.transform.Find("Image (1)/items/content/zhenshu/GameObject/ckb_48").gameObject;
			ckb_60 = base.transform.Find("Image (1)/items/content/zhenshu/GameObject/ckb_60").gameObject;
			ckb_high = base.transform.Find("Image (1)/items/content/pinzhi/GameObject/ckb_high").gameObject;
			ckb_light1 = base.transform.Find("Image (1)/items/content/guangying/GameObject/ckb_light1").gameObject;
			ckb_light2 = base.transform.Find("Image (1)/items/content/guangying/GameObject/ckb_light2").gameObject;
			ckb_light3 = base.transform.Find("Image (1)/items/content/guangying/GameObject/ckb_light3").gameObject;
			ckb_light4 = base.transform.Find("Image (1)/items/content/guangying/GameObject/ckb_light4").gameObject;
			ckb_light5 = base.transform.Find("Image (1)/items/content/guangying/GameObject/ckb_light5").gameObject;
			ckb_low = base.transform.Find("Image (1)/items/content/pinzhi/GameObject/ckb_low").gameObject;
			ckb_medium = base.transform.Find("Image (1)/items/content/pinzhi/GameObject/ckb_medium").gameObject;
			ckb_resolution1 = base.transform.Find("Image (1)/items/content/fbl/GameObject/ckb_resolution1").gameObject;
			ckb_resolution2 = base.transform.Find("Image (1)/items/content/fbl/GameObject/ckb_resolution2").gameObject;
			ckb_resolution3 = base.transform.Find("Image (1)/items/content/fbl/GameObject/ckb_resolution3").gameObject;
			ckb_shadow1 = base.transform.Find("Image (1)/items/content/yinying/GameObject/ckb_shadow1").gameObject;
			ckb_shadow2 = base.transform.Find("Image (1)/items/content/yinying/GameObject/ckb_shadow2").gameObject;
			ckb_shadow3 = base.transform.Find("Image (1)/items/content/yinying/GameObject/ckb_shadow3").gameObject;
			ckb_shadow4 = base.transform.Find("Image (1)/items/content/yinying/GameObject/ckb_shadow4").gameObject;
			ckb_shadow5 = base.transform.Find("Image (1)/items/content/yinying/GameObject/ckb_shadow5").gameObject;
			ckb_zidingyi = base.transform.Find("Image (1)/items/content/pinzhi/GameObject/ckb_zidingyi").gameObject;
			m_change = base.transform.Find("m_change").gameObject;
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			OnQualityLevel(0);
			ClickQuality();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__1(GameObject go)
		{
			OnQualityLevel(1);
			ClickQuality();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			OnQualityLevel(2);
			ClickQuality();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__3(GameObject go)
		{
			OnQualityLevel(3);
			ClickQuality();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__4(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetShadow(1);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetShadow(2);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__6(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetShadow(3);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__7(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetShadow(4);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__8(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetShadow(5);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__9(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetRender(1);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__A(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetRender(2);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__B(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetRender(3);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__C(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetRender(4);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__D(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetRender(5);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__E(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetFrame(0);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__F(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetFrame(1);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__10(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetFrame(2);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__11(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetResolution(0);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__12(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetResolution(1);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__13(GameObject go)
		{
			OnQualityLevel(3);
			PerformanceUtils.SetResolution(2);
		}

		[CompilerGenerated]
		private void _003CClickQuality_003Em__14(object[] o)
		{
			PerformanceUtils.SetQualityLevel();
			RefreshShadowQualityCheckBox();
			RefreshRenderQualityCheckBox();
			RefreshFrameCheckBox();
			RefreshresolutionCheckBox();
		}

		[CompilerGenerated]
		private void _003CClickQuality_003Em__15(object[] o)
		{
			m_change.SetActiveBetter(false);
		}
	}
}

using UnityEngine;

public class PerformanceUtils : MonoBehaviour
{
	public static void SetCloseShadow()
	{
		OnShadowQuality(1);
		QualitySettings.shadows = ShadowQuality.Disable;
	}

	public static void SetLowShadow()
	{
		OnShadowQuality(2);
		QualitySettings.shadows = ShadowQuality.HardOnly;
		QualitySettings.shadowDistance = 30f;
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowResolution = ShadowResolution.Medium;
	}

	public static void SetMeShadow()
	{
		OnShadowQuality(3);
		QualitySettings.shadows = ShadowQuality.HardOnly;
		QualitySettings.shadowDistance = 30f;
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowResolution = ShadowResolution.High;
	}

	public static void SetHighShadow()
	{
		OnShadowQuality(4);
		QualitySettings.shadows = ShadowQuality.HardOnly;
		QualitySettings.shadowDistance = 40f;
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowCascade2Split = 0.1f;
		QualitySettings.shadowResolution = ShadowResolution.High;
	}

	public static void SetVeryHighShadow()
	{
		OnShadowQuality(5);
		QualitySettings.shadows = ShadowQuality.All;
		QualitySettings.shadowDistance = 50f;
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowCascade4Split = new Vector3(0.067f, 0.2f, 0.467f);
		QualitySettings.shadowResolution = ShadowResolution.High;
	}

	public static void SetShadow(int level)
	{
		switch (level)
		{
		case 1:
			SetCloseShadow();
			break;
		case 2:
			SetLowShadow();
			break;
		case 3:
			SetMeShadow();
			break;
		case 4:
			SetHighShadow();
			break;
		case 5:
			SetVeryHighShadow();
			break;
		default:
			SetMeShadow();
			break;
		}
	}

	public static void SetRender(int level)
	{
		switch (level)
		{
		case 1:
			SettingMgr.RenderQuality = 1;
			QualitySettings.masterTextureLimit = 2;
			QualitySettings.pixelLightCount = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			QualitySettings.lodBias = 0.6f;
			QualitySettings.blendWeights = BlendWeights.TwoBones;
			break;
		case 2:
			SettingMgr.RenderQuality = 2;
			QualitySettings.masterTextureLimit = 2;
			QualitySettings.pixelLightCount = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			QualitySettings.lodBias = 0.6f;
			QualitySettings.blendWeights = BlendWeights.TwoBones;
			break;
		case 3:
			SettingMgr.RenderQuality = 3;
			QualitySettings.masterTextureLimit = 1;
			QualitySettings.pixelLightCount = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.lodBias = 0.8f;
			QualitySettings.blendWeights = BlendWeights.TwoBones;
			break;
		case 4:
			SettingMgr.RenderQuality = 4;
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.pixelLightCount = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.lodBias = 1f;
			QualitySettings.blendWeights = BlendWeights.TwoBones;
			break;
		case 5:
			SettingMgr.RenderQuality = 5;
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.pixelLightCount = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.lodBias = 1f;
			QualitySettings.blendWeights = BlendWeights.TwoBones;
			break;
		}
	}

	public static void SetQualityLevel()
	{
		QualitySettings.SetQualityLevel(SettingMgr.QualityLevel);
		switch (SettingMgr.QualityLevel)
		{
		case 0:
			SetCloseShadow();
			SetRender(2);
			SetFrame(0);
			SetResolution(0);
			break;
		case 1:
			SetMeShadow();
			SetRender(3);
			SetFrame(1);
			SetResolution(1);
			break;
		case 2:
			SetHighShadow();
			SetRender(4);
			SetFrame(2);
			SetResolution(2);
			break;
		case 3:
			break;
		default:
			SetMeShadow();
			SetRender(3);
			SetFrame(1);
			SetResolution(2);
			break;
		}
	}

	private static void OnShadowQuality(int value)
	{
		SettingMgr.ShadowQuality = value;
	}

	public static void SetFrame(int level)
	{
		switch (level)
		{
		case 0:
			SettingMgr.FrameRateQuality = 0;
			Application.targetFrameRate = 30;
			break;
		case 1:
			SettingMgr.FrameRateQuality = 1;
			Application.targetFrameRate = 48;
			break;
		case 2:
			SettingMgr.FrameRateQuality = 2;
			Application.targetFrameRate = 60;
			break;
		default:
			SettingMgr.FrameRateQuality = 1;
			Application.targetFrameRate = 60;
			break;
		}
	}

	public static void SetResolution(int level)
	{
		switch (level)
		{
		case 0:
			SettingMgr.ResolutionLevel = 0;
			break;
		case 1:
			SettingMgr.ResolutionLevel = 1;
			break;
		case 2:
			SettingMgr.ResolutionLevel = 2;
			break;
		default:
			SettingMgr.ResolutionLevel = 1;
			break;
		}
	}
}

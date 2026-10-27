using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using SC.LargeScene;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LoadingPanel : View
	{
		private Slider _slider;

		private bool updateing;

		private Coroutine corountWaitUpdate;

		private readonly string[] preloadAbs = new string[5] { "fonts/fonts.ab", "model/newdrop_mat.ab", "role/role.ab", "partinfo_material/component-mat.ab", "partinfo_material/component01-mat.ab" };

		private RawImage _bgRawImage;

		private bool _deceive;

		private float _sliderLength;

		private float _loadtime = 0.3f;

		private float _timeout = 5f;

		private bool _notReachable;

		private GameObject m_main_bg;

		private GameObject m_bg;

		private GameObject txt_version;

		private Text txt_versionText;

		private GameObject txt_loding_name;

		private Text txt_loding_nameText;

		private GameObject m_loding_slider;

		private GameObject btn_repair;

		[CompilerGenerated]
		private static Utils.VoidDelegate _003C_003Ef__am_0024cache0;

		private IEnumerator LoadPreFont(Utils.VoidDelegate callback)
		{
			yield return ResMgr.Ins.LoadAB("fonts/fonts.ab", null, false);
			int language = ((!Singleton<PlatformMgr>.Ins.IsOverSea()) ? (-1) : 0);
			Singleton<MultiLanguageMgr>.Ins.SetSystemLanguage(language);
			Utils.TriggerEvent(callback);
		}

		private IEnumerator LoadPre()
		{
			string[] array = preloadAbs;
			foreach (string ab in array)
			{
				yield return ResMgr.Ins.LoadAB(ab, null, false);
			}
			yield return StartCoroutine(LoadAllCfg());
			yield return LargeSceneManager.LoadSceneInfoFile();
			yield return LargeSceneManager.LoadSceneCellInfoFile();
			yield return ScenePreloadAb.LoadPreloadAbInfoFile();
			yield return ScenePreloadAb.LoadPartsPreloadAbInfoFile();
			yield return Singleton<ServerMgr>.Ins.LoadServer();
			FinishSlider();
		}

		private IEnumerator LoadAllCfg()
		{
			foreach (KeyValuePair<string, CfgManager.OnLoaded> pair in CfgManager.GetAll())
			{
				WWW www = new WWW(Utils.GetFilePathForWWW("cfg/" + pair.Key));
				yield return www;
				if (string.IsNullOrEmpty(www.error))
				{
					try
					{
						pair.Value(www.bytes);
					}
					catch (Exception)
					{
						Debug.LogError("oc error, to delete it:" + pair.Key);
						UdpSession.Ins.Send("[LoadingPanel] oc error:" + pair.Key);
						Singleton<UpdateMgr>.Ins.ToRepairClient();
						MessageBoxPanel.ShowConfirm(411, AndroidSDKInterface.Instance.RestartApplication);
					}
					continue;
				}
				Debug.LogError(www.error);
				break;
			}
		}

		private IEnumerator WaitUpdateFinish()
		{
			yield return Utils.WaitForSeconds(10f);
			if (!Singleton<UpdateMgr>.Ins.bUpdateNormal)
			{
				Singleton<UpdateMgr>.Ins.SetUpdateFinishCallback(null);
				FinishUpdate();
				UdpSession.Ins.Send("[update] wait 10 sceconds not finish");
			}
		}

		protected override void onInit()
		{
			m_main_bg.SetActive(false);
			SingletonMono<AudioManager>.Ins.EnableBgAudio = true;
			_slider = m_loding_slider.GetComponent<Slider>();
			_loadtime = ((!GameConst.IsCheck) ? 0.5f : 0.33f);
			_slider.value = 0f;
			SingletonMono<AudioManager>.Ins.BGVolume = SettingMgr.MusicValume;
			SingletonMono<AudioManager>.Ins.EffectVolume = SettingMgr.SoundEffectValume;
			SingletonMono<AudioManager>.Ins.PlayMusicBg("Bgm_loading");
			ClickListener.Get(btn_repair, string.Empty).onClick = OnRepair;
		}

		private void OnRepair(GameObject go)
		{
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnRepair_003Em__0;
			}
			MessageBoxPanel.Show(413, _003C_003Ef__am_0024cache0);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			m_main_bg.SetActive(true);
			txt_loding_name.SetActive(false);
			txt_version.SetActive(false);
			StartCoroutine(LoadPreFont(Load));
		}

		public void Load()
		{
			View.SetLabelText(txt_loding_name, Utils.GetString(435));
			txt_loding_name.SetActive(true);
			txt_version.SetActive(true);
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				MessageBoxPanel.ShowConfirm(342, _003CLoad_003Em__1);
				return;
			}
			updateing = true;
			Singleton<UpdateMgr>.Ins.Update(FinishUpdate);
		}

		private bool IsLowEndPhone()
		{
			if (SystemInfo.systemMemorySize < 1024)
			{
				return true;
			}
			return false;
		}

		protected override void onHide(string childView = null)
		{
			Singleton<UpdateMgr>.Ins.SetUpdateFinishCallback(null);
			ViewMgr.Ins.Destroy<LoadingPanel>();
		}

		private void SetProgerss(int cuttent, int size)
		{
			if (size != 0)
			{
				_slider.DoSlider((float)cuttent * 1f / (float)size, 0.5f);
				string @string;
				string string2;
				if (size > 1048576)
				{
					@string = Utils.GetString(445, string.Format("{0:N2}", (float)size * 1f / 1048576f));
					string2 = Utils.GetString(445, string.Format("{0:N2}", (float)cuttent * 1f / 1048576f));
				}
				else
				{
					@string = Utils.GetString(446, string.Format("{0:N2}", (float)size * 1f / 1024f));
					string2 = Utils.GetString(446, string.Format("{0:N2}", (float)cuttent * 1f / 1024f));
				}
				View.SetLabelText(txt_loding_name, Utils.GetString(436, string2, @string));
			}
		}

		private void FinishUpdate()
		{
			updateing = false;
			if (Singleton<UpdateMgr>.Ins._serverVersionFile != null)
			{
				GameConst.Version = Singleton<UpdateMgr>.Ins._serverVersionFile.strV;
				View.SetLabelText(txt_version, GameConst.Version);
				if (!string.IsNullOrEmpty(Singleton<UpdateMgr>.Ins._serverVersionFile.codeVersion))
				{
					GameConst.CodeVersion = Singleton<UpdateMgr>.Ins._serverVersionFile.codeVersion;
				}
				UdpSession.strLogHead = UdpSession.strLogHead + " " + GameConst.Version;
			}
			if (UpdateMgr.bRestart)
			{
				MessageBoxPanel.ShowConfirm(412, AndroidSDKInterface.Instance.RestartApplication);
				return;
			}
			_sliderLength = 0f;
			_deceive = true;
			if (Singleton<UpdateMgr>.Ins.bUpdateSucess)
			{
				StartCoroutine(LoadPre());
				View.SetLabelText(txt_loding_name, Utils.GetString(435));
			}
		}

		private void FinishSlider()
		{
			Login();
		}

		private void Login()
		{
			if (!Singleton<PlatformMgr>.Ins.IsTest)
			{
				ViewMgr.Ins.ShowView<LoginPanel>();
			}
			else
			{
				ViewMgr.Ins.ShowView<LoginAccoutPanel>();
			}
		}

		private void Update()
		{
			if (updateing)
			{
				SetProgerss(Singleton<UpdateMgr>.Ins.GetHadDownloadSize(), Singleton<UpdateMgr>.Ins.nToDownloadTotalSize);
			}
			if (_deceive)
			{
				if (_sliderLength < 1f)
				{
					_sliderLength += _loadtime * Time.deltaTime;
					_slider.value = _sliderLength;
				}
				else
				{
					_deceive = false;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_main_bg = component.GameObjects[0].gameObject;
			m_bg = component.GameObjects[1].gameObject;
			txt_version = component.GameObjects[2].gameObject;
			txt_versionText = txt_version.GetComponent<Text>();
			txt_loding_name = component.GameObjects[3].gameObject;
			txt_loding_nameText = txt_loding_name.GetComponent<Text>();
			m_loding_slider = component.GameObjects[4].gameObject;
			btn_repair = component.GameObjects[5].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private static void _003COnRepair_003Em__0()
		{
			Singleton<UpdateMgr>.Ins.ToRepairClient();
			AndroidSDKInterface.Instance.RestartApplication();
		}

		[CompilerGenerated]
		private void _003CLoad_003Em__1()
		{
			onShow();
		}
	}
}

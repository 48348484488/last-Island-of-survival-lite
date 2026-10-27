using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.online.scmsg;

namespace SC.UI
{
	public class LoginPanel : View
	{
		private ServerCfg _currentCfg;

		private Utils.VoidDelegate _languagePanelhideDelegate;

		private string logFilePath;

		private string deviceUniqueIdentifier;

		private bool bUpdateLogSuccess;

		private bool bUpdateLogFinish;

		private GameObject btn_choose_server;

		private GameObject txt_server_name;

		private Text txt_server_nameText;

		private GameObject btn_start;

		private GameObject m_healthy;

		private GameObject btn_repair;

		private GameObject btn_customer_service;

		private GameObject btn_upload;

		private GameObject m_feng_effect;

		private GameObject m_logo;

		private GameObject m_over_sea_logo;

		private GameObject m_loading;

		[CompilerGenerated]
		private static Utils.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			m_loading.SetActiveBetter(false);
			ClickListener.Get(btn_start, string.Empty).onClick = Login;
			ClickListener.Get(btn_choose_server, string.Empty).onClick = ChooseServer;
			ClickListener.Get(btn_customer_service, string.Empty).onClick = OnCustomerService;
			ClickListener.Get(btn_repair, string.Empty).onClick = OnRepair;
			ClickListener.Get(btn_upload, string.Empty).onClick = OnUploadLog;
			m_feng_effect.SetActiveBetter(false);
		}

		private void OnUploadLog(GameObject go)
		{
			logFilePath = Application.persistentDataPath + "/hero_log.txt";
			deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			bUpdateLogSuccess = false;
			bUpdateLogFinish = false;
			StartCoroutine(WaitUploadLogFile());
			Worker.Ins.Execute(UploadLogFile);
		}

		protected override void onShow(object param = null, string childView = null)
		{
			Singleton<ServerMgr>.Ins.StopPing();
			m_feng_effect.GetComponent<RenderQueueFixerInUIForParticle>().StartSetting();
			_currentCfg = Singleton<ServerMgr>.Ins.GetCurrentServerCfg();
			ChooseServer(_currentCfg);
			SNoCreateRole.handler = (SNoCreateRole.Handler)Delegate.Combine(SNoCreateRole.handler, new SNoCreateRole.Handler(SNoCreateRoleHandle));
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinish));
			MultiLanguageEvent.RefreshLableDelegate = (Utils.VoidDelegate)Delegate.Combine(MultiLanguageEvent.RefreshLableDelegate, new Utils.VoidDelegate(RefreshLableDelegate));
			OnlineEvent.onConnectedGs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectedGs, new Utils.VoidDelegate(OnConnectedGs));
			if (!Singleton<MultiLanguageMgr>.Ins.IsSetLanguage())
			{
				_languagePanelhideDelegate = ShowNotice;
			}
			else
			{
				ShowNotice();
			}
		}

		private void OnConnectedGs()
		{
			m_loading.SetActiveBetter(false);
		}

		private void ShowNotice()
		{
		}

		private void RefreshLableDelegate()
		{
			ChooseServer(_currentCfg);
		}

		private void OnSLoginFinish(SLoginFinished s)
		{
			TweenTime.Quit(btn_start);
		}

		private void SNoCreateRoleHandle(SNoCreateRole msg)
		{
			ViewMgr.Ins.ShowView<EstablishPanel>(1);
		}

		protected override void onHide(string childView = null)
		{
			SNoCreateRole.handler = (SNoCreateRole.Handler)Delegate.Remove(SNoCreateRole.handler, new SNoCreateRole.Handler(SNoCreateRoleHandle));
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Remove(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinish));
			MultiLanguageEvent.RefreshLableDelegate = (Utils.VoidDelegate)Delegate.Remove(MultiLanguageEvent.RefreshLableDelegate, new Utils.VoidDelegate(RefreshLableDelegate));
			OnlineEvent.onConnectedGs = (Utils.VoidDelegate)Delegate.Remove(OnlineEvent.onConnectedGs, new Utils.VoidDelegate(OnConnectedGs));
			ViewMgr.Ins.Destroy<LoginPanel>();
		}

		public void ChooseServer(GameObject serverCfg)
		{
			ServerMgr.OnChoose param = ChooseServer;
			ViewMgr.Ins.ShowView<LoginServerPanel>(param, false);
		}

		public void OnCustomerService(GameObject serverCfg)
		{
			Application.OpenURL("http://kf.yingxiong.com/Mobile/checkOption?Gid=157");
		}

		private IEnumerator WaitUploadLogFile()
		{
			do
			{
				yield return Utils.WaitForSeconds(0.1f);
			}
			while (!bUpdateLogFinish);
		}

		private void UploadLogFile()
		{
			bUpdateLogSuccess = FtpOperation.UploadFile(logFilePath, ServerPath.FtpServerIp, ServerPath.FtpUserName, ServerPath.FtpUserPwd, deviceUniqueIdentifier);
			bUpdateLogFinish = true;
		}

		private void OnRepair(GameObject go)
		{
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnRepair_003Em__0;
			}
			MessageBoxPanel.Show(413, _003C_003Ef__am_0024cache0);
		}

		public void ChooseServer(ServerCfg serverCfg)
		{
			_currentCfg = serverCfg;
			View.SetLabelText(txt_server_name, serverCfg.name);
			Singleton<PlatformMgr>.Ins.onSelectServer(_currentCfg.id);
		}

		public void Login(GameObject go)
		{
			TweenTime.Begin(go, cfg.Consts.LOGIN_SDK_TIME, _003CLogin_003Em__1, _003CLogin_003Em__2);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_choose_server = component.GameObjects[0].gameObject;
			txt_server_name = component.GameObjects[1].gameObject;
			txt_server_nameText = txt_server_name.GetComponent<Text>();
			btn_start = component.GameObjects[2].gameObject;
			m_healthy = component.GameObjects[3].gameObject;
			btn_repair = component.GameObjects[4].gameObject;
			btn_customer_service = component.GameObjects[5].gameObject;
			btn_upload = component.GameObjects[6].gameObject;
			m_feng_effect = component.GameObjects[7].gameObject;
			m_logo = component.GameObjects[8].gameObject;
			m_over_sea_logo = component.GameObjects[9].gameObject;
			m_loading = component.GameObjects[10].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private static void _003COnRepair_003Em__0()
		{
			Singleton<UpdateMgr>.Ins.ToRepairClient();
			AndroidSDKInterface.Instance.RestartApplication();
		}

		[CompilerGenerated]
		private void _003CLogin_003Em__1()
		{
			Singleton<LoginMgr>.Ins.SetIpAddress(_currentCfg.host, _currentCfg.port);
			Singleton<LoginMgr>.Ins.IsReConnectGs = false;
			Singleton<PlatformMgr>.Ins.Login();
			m_loading.SetActiveBetter(true);
		}

		[CompilerGenerated]
		private void _003CLogin_003Em__2()
		{
			m_loading.SetActiveBetter(false);
		}
	}
}

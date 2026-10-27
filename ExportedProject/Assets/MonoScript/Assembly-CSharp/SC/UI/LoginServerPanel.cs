using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using auth.msg;

namespace SC.UI
{
	public class LoginServerPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CServerRegionItemCallBack_003Ec__AnonStorey0
		{
			internal ServerRegionCfg item;

			internal LoginServerPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._currentRegionId = item.id;
				_0024this.SetServer(item.ServerList);
				_0024this.UpdateServerRegionList();
			}
		}

		[CompilerGenerated]
		private sealed class _003CServerItemCallBack_003Ec__AnonStorey1
		{
			internal int serverStatus;

			internal ServerCfg item;

			internal LoginServerPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (serverStatus == 3)
				{
					AlertBox.Show(363);
					return;
				}
				Singleton<ServerMgr>.Ins.SaveServerId(item.id);
				_0024this._onChoose(item);
				_0024this.Hide();
			}
		}

		private List<ServerCfg> _serverCfgList;

		private ServerMgr.OnChoose _onChoose;

		private List<ServerCfg> _currentServerList = new List<ServerCfg>();

		private HashSet<int> _historyGs;

		private HashSet<int> _openGs;

		private List<ServerRegionCfg> _currentServerRegionCfgList = new List<ServerRegionCfg>();

		private UIScrollPanel _regioScrollPanel;

		private UIScrollPanel _serverScrollPanel;

		private int _currentRegionId;

		private int _currentServerId;

		private SPlatformLogin _platformLogin;

		private List<int> _serverList = new List<int>();

		private GameObject m_bg;

		private GameObject btn_close;

		private GameObject scp_server_list;

		private GServerItem m_cell;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_title;

		private Text txt_titleText;

		private GameObject scp_sever_btns;

		private GServerBtnItem m_cell_0;

		protected override void onInit()
		{
			m_cell.gameObject.SetActive(false);
			m_cell_0.gameObject.SetActive(false);
			_regioScrollPanel = scp_sever_btns.GetComponent<UIScrollPanel>();
			_serverScrollPanel = scp_server_list.GetComponent<UIScrollPanel>();
			_platformLogin = Singleton<AuthMgr>.Ins.PlatformLogin;
			if (_platformLogin == null)
			{
				_platformLogin = new SPlatformLogin();
			}
			_historyGs = _platformLogin.historyGs;
			_openGs = _platformLogin.openGs;
			_serverCfgList = Singleton<ServerMgr>.Ins.GetServerList();
			if (_historyGs.Count > 0)
			{
				ServerRegionCfg serverRegionCfg = new ServerRegionCfg();
				serverRegionCfg.id = 0;
				serverRegionCfg.name = Utils.GetString(361);
				serverRegionCfg.ServerList = new List<int>(_historyGs);
				_currentServerRegionCfgList.Add(serverRegionCfg);
			}
			_currentServerRegionCfgList.AddRange(Singleton<ServerMgr>.Ins.GetRegionList());
			UIEventListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0;
			UIEventListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_onChoose = param as ServerMgr.OnChoose;
			_currentServerId = Singleton<ServerMgr>.Ins.GetCurrentServerId();
			Singleton<PlatformMgr>.Ins.onSelectServer(_currentServerId);
			ServerRegionCfg serverRegionCfg = _currentServerRegionCfgList[0];
			_currentRegionId = serverRegionCfg.id;
			SetServer(serverRegionCfg.ServerList);
			SetServerRegionList();
		}

		protected override void onHide(string childView = null)
		{
			ViewMgr.Ins.Destroy<LoginServerPanel>();
		}

		private void SetServerRegionList()
		{
			_regioScrollPanel.Clear();
			_regioScrollPanel.Reset(_currentServerRegionCfgList.Count, ServerRegionItemCallBack);
		}

		private void ServerRegionItemCallBack(GameObject cell, int index)
		{
			_003CServerRegionItemCallBack_003Ec__AnonStorey0 _003CServerRegionItemCallBack_003Ec__AnonStorey = new _003CServerRegionItemCallBack_003Ec__AnonStorey0();
			_003CServerRegionItemCallBack_003Ec__AnonStorey._0024this = this;
			GServerBtnItem component = cell.GetComponent<GServerBtnItem>();
			_003CServerRegionItemCallBack_003Ec__AnonStorey.item = _currentServerRegionCfgList[index];
			View.SetLabelText(component.txt_name_onText, _003CServerRegionItemCallBack_003Ec__AnonStorey.item.name);
			View.SetLabelText(component.txt_name_offText, _003CServerRegionItemCallBack_003Ec__AnonStorey.item.name);
			component.m_select.SetActiveBetter(_003CServerRegionItemCallBack_003Ec__AnonStorey.item.id == _currentRegionId);
			ClickListener.Get(component.gameObject, string.Empty).onClick = _003CServerRegionItemCallBack_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateServerRegionList()
		{
			_regioScrollPanel.UpdateAllCell(UpdateServerRegionItem);
		}

		private void UpdateServerRegionItem(GameObject cell, int index)
		{
			GServerBtnItem component = cell.GetComponent<GServerBtnItem>();
			ServerRegionCfg serverRegionCfg = _currentServerRegionCfgList[index];
			component.m_select.SetActiveBetter(serverRegionCfg.id == _currentRegionId);
		}

		private void SetServer(List<int> serverList)
		{
			_serverList = serverList;
			_serverScrollPanel.Clear();
			_serverScrollPanel.Reset(_serverList.Count, ServerItemCallBack);
		}

		private void ServerItemCallBack(GameObject cell, int index)
		{
			_003CServerItemCallBack_003Ec__AnonStorey1 _003CServerItemCallBack_003Ec__AnonStorey = new _003CServerItemCallBack_003Ec__AnonStorey1();
			_003CServerItemCallBack_003Ec__AnonStorey._0024this = this;
			GServerItem component = cell.GetComponent<GServerItem>();
			_003CServerItemCallBack_003Ec__AnonStorey.item = Singleton<ServerMgr>.Ins.GetServerCfg(_serverList[index]);
			cell.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.item != null);
			if (_003CServerItemCallBack_003Ec__AnonStorey.item != null)
			{
				View.SetLabelText(component.txt_server_nameText, _003CServerItemCallBack_003Ec__AnonStorey.item.name);
				View.SetLabelText(component.txt_server_name_no_selectText, _003CServerItemCallBack_003Ec__AnonStorey.item.name);
				component.m_no_select.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.item.id != _currentServerId);
				component.m_select.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.item.id == _currentServerId);
				_003CServerItemCallBack_003Ec__AnonStorey.serverStatus = 0;
				_platformLogin.gsStatus.TryGetValue(_003CServerItemCallBack_003Ec__AnonStorey.item.id, out _003CServerItemCallBack_003Ec__AnonStorey.serverStatus);
				component.m_state_1.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.serverStatus == 0);
				component.m_state_2.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.serverStatus == 2);
				component.m_state_3.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.serverStatus == 3);
				component.m_state_4.SetActiveBetter(_003CServerItemCallBack_003Ec__AnonStorey.serverStatus == 1);
				ClickListener.Get(component.gameObject, string.Empty).onClick = _003CServerItemCallBack_003Ec__AnonStorey._003C_003Em__0;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			btn_close = component.GameObjects[1].gameObject;
			scp_server_list = component.GameObjects[2].gameObject;
			m_cell = View.AddComponentIfNotExist<GServerItem>(component.GameObjects[3].gameObject);
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_title = component.GameObjects[5].gameObject;
			txt_titleText = txt_title.GetComponent<Text>();
			scp_sever_btns = component.GameObjects[6].gameObject;
			m_cell_0 = View.AddComponentIfNotExist<GServerBtnItem>(component.GameObjects[7].gameObject);
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			Hide();
		}
	}
}

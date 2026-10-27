using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerMgr : Singleton<ServerMgr>
{
	public delegate void OnChoose(ServerCfg item);

	public static readonly string txt_address = "gameconfig/serverList.xml";

	private readonly List<ServerCfg> mListServerCfg = new List<ServerCfg>();

	private readonly Dictionary<int, ServerCfg> mDicServerCfg = new Dictionary<int, ServerCfg>();

	private readonly List<ServerRegionCfg> _listServerRegionCfg = new List<ServerRegionCfg>();

	private readonly Dictionary<int, ServerRegionCfg> _serverRegionDic = new Dictionary<int, ServerRegionCfg>();

	private int mCurrentServerId = 1;

	private string mLocalClientVersion = string.Empty;

	private Dictionary<int, Ping> PingDic = new Dictionary<int, Ping>();

	private bool _isPing;

	private int _fastestServerId;

	private Coroutine _pingCoroutine;

	private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	public bool SelectedServer { get; set; }

	public void Init()
	{
		mListServerCfg.Clear();
		mDicServerCfg.Clear();
		mCurrentServerId = PlayerPrefsData.ServerId;
	}

	public Coroutine LoadServer()
	{
		return ResMgr.Ins.LoadTextLocal(txt_address, OnLoaded);
	}

	public List<ServerCfg> GetServerList()
	{
		return mListServerCfg;
	}

	public List<ServerRegionCfg> GetRegionList()
	{
		return _listServerRegionCfg;
	}

	public void SaveServerId(int serverId)
	{
		mCurrentServerId = serverId;
		PlayerPrefsData.ServerId = serverId;
		Singleton<PlatformMgr>.Ins.onSelectServer(serverId);
	}

	public ServerCfg GetCurrentServerCfg()
	{
		ServerCfg value;
		mDicServerCfg.TryGetValue(GetCurrentServerId(), out value);
		return value;
	}

	public int GetCurrentServerId()
	{
		if (!mDicServerCfg.ContainsKey(mCurrentServerId))
		{
			if (mListServerCfg.Count > 0)
			{
				mCurrentServerId = mListServerCfg[0].id;
			}
			else
			{
				mCurrentServerId = 1;
			}
		}
		return mCurrentServerId;
	}

	public void Add(ServerCfg serverCfg)
	{
		mListServerCfg.Add(serverCfg);
		mDicServerCfg[serverCfg.id] = serverCfg;
	}

	public ServerCfg GetServerCfg(int serverId)
	{
		ServerCfg value;
		mDicServerCfg.TryGetValue(serverId, out value);
		return value;
	}

	private void OnLoaded(string obj)
	{
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(obj);
		XMLNodeList nodeList = xMLNode.GetNodeList("root>0>server");
		IEnumerator enumerator = nodeList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				XMLNode xMLNode2 = (XMLNode)enumerator.Current;
				ServerCfg serverCfg = new ServerCfg();
				serverCfg.id = int.Parse(xMLNode2.GetValue("@id"));
				serverCfg.type = int.Parse(xMLNode2.GetValue("@type"));
				serverCfg.name = xMLNode2.GetValue("@name");
				serverCfg.port = int.Parse(xMLNode2.GetValue("@port"));
				serverCfg.status = int.Parse(xMLNode2.GetValue("@status"));
				serverCfg.host = xMLNode2.GetValue("@host");
				Add(serverCfg);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		nodeList = xMLNode.GetNodeList("root>0>region");
		IEnumerator enumerator2 = nodeList.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				XMLNode xMLNode3 = (XMLNode)enumerator2.Current;
				ServerRegionCfg serverRegionCfg = new ServerRegionCfg();
				serverRegionCfg.id = int.Parse(xMLNode3.GetValue("@id"));
				serverRegionCfg.name = xMLNode3.GetValue("@name");
				string value = xMLNode3.GetValue("@serverList");
				serverRegionCfg.ServerList = new List<int>();
				string[] array = value.Split(',');
				foreach (string text in array)
				{
					if (text.Contains("-"))
					{
						string[] array2 = text.Split('-');
						for (int j = int.Parse(array2[0]); j < int.Parse(array2[1]); j++)
						{
							serverRegionCfg.ServerList.Add(j);
						}
					}
					else
					{
						int item = int.Parse(text);
						serverRegionCfg.ServerList.Add(item);
					}
				}
				_listServerRegionCfg.Add(serverRegionCfg);
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = enumerator2 as IDisposable) != null)
			{
				disposable2.Dispose();
			}
		}
		GetPings();
	}

	private void ChooseFirstServer()
	{
	}

	private void GetPings()
	{
		if (!PlayerPrefsData.IsHasServerId && mListServerCfg.Count >= 2)
		{
			for (int i = 0; i < mListServerCfg.Count; i++)
			{
				ServerCfg serverCfg = mListServerCfg[i];
				Ping value = new Ping(serverCfg.host);
				PingDic.Add(serverCfg.id, value);
			}
			_isPing = true;
			_pingCoroutine = Utils.StartConroutine(CheckPing());
		}
	}

	public void StopPing()
	{
		if (_pingCoroutine != null)
		{
			Utils.StopConroutine(_pingCoroutine);
		}
	}

	private void UpdateCheckPing()
	{
		if (!_isPing || PingDic.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<int, Ping> item in PingDic)
		{
			if (item.Value.isDone)
			{
				_fastestServerId = item.Key;
				mCurrentServerId = _fastestServerId;
				_isPing = false;
				break;
			}
		}
	}

	private IEnumerator CheckPing()
	{
		while (_isPing)
		{
			yield return _waitForEndOfFrame;
			foreach (KeyValuePair<int, Ping> item in PingDic)
			{
				if (item.Value.isDone)
				{
					_fastestServerId = item.Key;
					mCurrentServerId = _fastestServerId;
					_isPing = false;
					yield break;
				}
			}
		}
	}
}

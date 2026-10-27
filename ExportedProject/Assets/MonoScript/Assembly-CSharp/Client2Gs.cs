using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Net;
using SC.UI;
using Share;
using UnityEngine;
using gs.online.scmsg;
using online;

public sealed class Client2Gs : Manager
{
	public class MainThreadFuncs
	{
		public Utils.ObjectDelegate fanc;

		public object obj;

		public MainThreadFuncs(Utils.ObjectDelegate fan, object o)
		{
			fanc = fan;
			obj = o;
		}
	}

	public static Client2Gs Ins = new Client2Gs();

	private string mHost;

	private int mPort;

	private Session mSession;

	private readonly MessageQueue<Message> mQueue = new MessageQueue<Message>();

	private readonly BetterList<MainThreadFuncs> mFuncs = new BetterList<MainThreadFuncs>();

	private readonly WaitForEndOfFrame mWait = new WaitForEndOfFrame();

	private bool mConnecting;

	private bool mClosebySelf;

	public static bool isServerInReview;

	[CompilerGenerated]
	private static SAlert.Handler _003C_003Ef__am_0024cache0;

	public bool IsSessionExist
	{
		get
		{
			return mSession != null;
		}
	}

	private Client2Gs()
	{
		foreach (Message item in AllClientMsgs.All)
		{
			addMessageType(item);
		}
	}

	public bool IsConnecting()
	{
		return mConnecting;
	}

	private void OnSIsServerInReview(SIsServerInReview sIsServerInReview)
	{
		isServerInReview = sIsServerInReview.isServerInReview;
	}

	public void Init()
	{
		Utils.StartConroutine(DoFlush());
		mConnecting = false;
		SIsServerInReview.handler = (SIsServerInReview.Handler)Delegate.Combine(SIsServerInReview.handler, new SIsServerInReview.Handler(OnSIsServerInReview));
		SAlert.Handler handler = SAlert.handler;
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CInit_003Em__0;
		}
		SAlert.handler = (SAlert.Handler)Delegate.Combine(handler, _003C_003Ef__am_0024cache0);
	}

	public void SetHost(string host, int port)
	{
		mHost = host;
		mPort = port;
	}

	public string GetHost()
	{
		return mHost;
	}

	public int GetPort()
	{
		return mPort;
	}

	public void Connect()
	{
		mClosebySelf = false;
		DoConnect();
	}

	private void DoConnect()
	{
		if (!string.IsNullOrEmpty(mHost) && !mConnecting)
		{
			mConnecting = true;
			CreateClientSession(mHost, mPort);
			Debug.Log("[online]Connect Gs" + mHost + ":" + mPort);
		}
	}

	public void Close()
	{
		mClosebySelf = true;
		if (mSession != null)
		{
			mSession.Close();
		}
		mSession = null;
		mConnecting = false;
		mQueue.Clear();
	}

	public void Send(Message msg)
	{
		if (mSession != null)
		{
			mSession.Send(msg);
		}
	}

	public void Update()
	{
		if (mFuncs.size > 0)
		{
			lock (mFuncs)
			{
				int i = 0;
				for (int size = mFuncs.size; i < size; i++)
				{
					Utils.ObjectDelegate fanc = mFuncs[i].fanc;
					try
					{
						fanc(mFuncs[i].obj);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
				mFuncs.Clear();
			}
		}
		while (true)
		{
			Message message = mQueue.Dequeue();
			if (message == null)
			{
				break;
			}
			try
			{
				message.handle();
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
	}

	private IEnumerator DoFlush()
	{
		while (true)
		{
			yield return mWait;
			if (mSession != null)
			{
				mSession.Flush();
			}
		}
	}

	private void OnConnected(object o)
	{
		Session session = o as Session;
		if (mSession != null && mSession.id != session.id)
		{
			mSession.Close();
		}
		mSession = session;
		mConnecting = true;
		if (OnlineEvent.onConnectedGs != null)
		{
			OnlineEvent.onConnectedGs();
		}
		Debug.Log("[online]OnAddSession" + mSession.id);
	}

	private void OnDisConnected(object session)
	{
		mConnecting = false;
		if (mSession != null)
		{
			if (mSession == session)
			{
				Debug.Log("OnDelSession " + mSession.id);
				mSession = null;
				if (OnlineEvent.onDisconnectedGs != null)
				{
					OnlineEvent.onDisconnectedGs();
				}
			}
		}
		else
		{
			Debug.Log("connect gs failed");
			Utils.TriggerEvent(OnlineEvent.onConnectGsFailed);
		}
	}

	private void AddMainThreadFunc(Utils.ObjectDelegate func, Session session)
	{
		lock (mFuncs)
		{
			mFuncs.Add(new MainThreadFuncs(func, session));
		}
	}

	protected override void OnAddSession(Session session)
	{
		AddMainThreadFunc(OnConnected, session);
	}

	protected override void OnDelSession(Session session)
	{
		if (!mClosebySelf)
		{
			AddMainThreadFunc(OnDisConnected, session);
		}
	}

	public override void dispatchMessage(Session session, Message msg)
	{
		mQueue.Enqueue(msg);
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__0(SAlert msg)
	{
		if (msg.strId > 0 && msg.args.Count == 0)
		{
			AlertBox.Show(msg.strId);
		}
		else if (msg.strId > 0 && msg.args.Count > 0)
		{
			List<string> args = msg.args;
			object[] array = new object[args.Count];
			int i = 0;
			for (int count = args.Count; i < count; i++)
			{
				array[i] = args[i];
			}
			AlertBox.Show(msg.strId, array);
		}
		else if (msg.strId <= 0 && msg.args.Count > 0)
		{
			AlertBox.Show(msg.args[0]);
		}
	}
}

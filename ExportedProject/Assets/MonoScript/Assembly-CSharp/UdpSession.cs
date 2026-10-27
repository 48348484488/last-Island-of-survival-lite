using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Net;
using Share;
using UnityEngine;

public class UdpSession
{
	private UdpClient mUdpClient;

	private Manager mManager;

	private IPEndPoint mServerEndPoint;

	private long mSessionId = -1L;

	private Octets obuffer;

	public static UdpSession Ins = new UdpSession(0L, null);

	public static string strLogHead = string.Empty;

	public long SessionId
	{
		get
		{
			return mSessionId;
		}
	}

	public UdpSession(long sessionId, Manager mgr)
	{
		mSessionId = sessionId;
		mManager = mgr;
		obuffer = new Octets(1024);
	}

	public void Connect(string host, string port)
	{
		int port2 = Convert.ToInt32(port);
		Connect(host, port2);
	}

	public void Connect(string host, int port)
	{
		try
		{
			mServerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
			mUdpClient = new UdpClient(AddressFamily.InterNetwork);
			mUdpClient.Connect(mServerEndPoint);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void Send(Message message)
	{
		try
		{
			obuffer.clear();
			obuffer.push(message.getType());
			obuffer.push(0);
			int size = obuffer.Size;
			obuffer.push(message);
			int num = obuffer.Size - size;
			obuffer.push_rollback(num + 4);
			obuffer.push(num);
			obuffer.push_count(num);
			mUdpClient.BeginSend(obuffer.getBytes(), obuffer.Size, OnSendResult, obuffer);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void Send(string message)
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes(strLogHead + " " + message);
			mUdpClient.BeginSend(bytes, bytes.Length, OnSendResult, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void Close()
	{
		try
		{
			if (mUdpClient != null)
			{
				mUdpClient.Close();
				mUdpClient = null;
			}
			mSessionId = -1L;
			mManager = null;
			mServerEndPoint = null;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void OnSendResult(IAsyncResult result)
	{
		try
		{
			mUdpClient.EndSend(result);
		}
		catch (Exception)
		{
		}
	}

	private void OnReceiveResult(IAsyncResult result)
	{
		if (mUdpClient == null)
		{
			return;
		}
		try
		{
			byte[] buffer = mUdpClient.EndReceive(result, ref mServerEndPoint);
			Octets octets = mManager.AllocateOctets();
			octets.Set(buffer, int.MaxValue);
			long num = octets.pop_long();
			if (num == mSessionId)
			{
				Decode(octets);
			}
			mManager.RecycleOctets(octets);
			mUdpClient.BeginReceive(OnReceiveResult, null);
		}
		catch (Exception)
		{
		}
	}

	private void Decode(Octets oc)
	{
		int type;
		while (true)
		{
			if (oc.Size < 8)
			{
				return;
			}
			type = oc.pop_int();
			int num = oc.pop_int();
			Message message = mManager.CreateMessage(type);
			if (message == null)
			{
				break;
			}
			message.unmarshal(oc);
			try
			{
				mManager.dispatchMessage(null, message);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		mManager.onUnknownMessage(null, type, 0, null);
	}
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Share;

namespace Net
{
	public abstract class Manager
	{
		private readonly int iBufferCapacity;

		protected Dictionary<int, Message> messages;

		protected Dictionary<long, Session> sessions = new Dictionary<long, Session>();

		protected ObjectPool<Octets> octetsPool;

		[CompilerGenerated]
		private static ObjectPool<Octets>.CreateObject<Octets> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ObjectPool<Octets>.DestroyObject<Octets> _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static ObjectPool<Octets>.RecycleObject<Octets> _003C_003Ef__am_0024cache2;

		public int IBufferCapacity
		{
			get
			{
				return iBufferCapacity;
			}
		}

		protected Manager()
			: this(1048576)
		{
		}

		protected Manager(int iBufferCapacity)
		{
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CoctetsPool_003Em__0;
			}
			ObjectPool<Octets>.CreateObject<Octets> createFun = _003C_003Ef__am_0024cache0;
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003CoctetsPool_003Em__1;
			}
			ObjectPool<Octets>.DestroyObject<Octets> destroyFun = _003C_003Ef__am_0024cache1;
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003CoctetsPool_003Em__2;
			}
			octetsPool = new ObjectPool<Octets>(100, createFun, destroyFun, _003C_003Ef__am_0024cache2);
			base._002Ector();
			this.iBufferCapacity = iBufferCapacity;
			messages = new Dictionary<int, Message>();
		}

		public bool Send(long sessionID, Message msg)
		{
			Session value;
			lock (sessions)
			{
				if (!sessions.TryGetValue(sessionID, out value))
				{
					return false;
				}
			}
			value.Send(msg);
			return true;
		}

		public void SendAll(Message msg)
		{
			lock (sessions)
			{
				foreach (Session value in sessions.Values)
				{
					value.Send(msg);
				}
			}
		}

		public Session GetSession(long sessionID)
		{
			lock (sessions)
			{
				Session value;
				sessions.TryGetValue(sessionID, out value);
				return value;
			}
		}

		public void CloseSession(long sessionID)
		{
			Session value;
			lock (sessions)
			{
				if (!sessions.TryGetValue(sessionID, out value))
				{
					return;
				}
			}
			value.Close();
		}

		public void AddSession(Session session)
		{
			lock (sessions)
			{
				sessions[session.id] = session;
			}
			try
			{
				OnAddSession(session);
			}
			catch
			{
			}
		}

		public Session CreateClientSession(string host, int port)
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Session session = new Session(this, socket);
			session.Connect(host, port);
			return session;
		}

		public virtual void dispatchMessage(Session session, Message msg)
		{
			try
			{
				msg.handle();
			}
			catch
			{
			}
		}

		public virtual void onUnknownMessage(Session session, int type, int size, Octets buffer)
		{
		}

		public virtual Message CreateMessage(int type)
		{
			try
			{
				Message message = messages[type];
				return (message != null) ? ((Message)Activator.CreateInstance(message.GetType())) : null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void addMessageType(Message msg)
		{
			if (messages.ContainsKey(msg.getType()))
			{
				throw new Exception("duplicate message type: " + msg.getType());
			}
			messages[msg.getType()] = msg;
		}

		public Octets AllocateOctets()
		{
			lock (octetsPool)
			{
				return octetsPool.Get();
			}
		}

		public void RecycleOctets(Octets oc)
		{
			lock (octetsPool)
			{
				octetsPool.Recycle(oc);
			}
		}

		public void DelSession(Session session)
		{
			try
			{
				OnDelSession(session);
			}
			catch
			{
			}
			lock (sessions)
			{
				sessions.Remove(session.id);
			}
		}

		protected abstract void OnAddSession(Session session);

		protected abstract void OnDelSession(Session session);

		[CompilerGenerated]
		private static Octets _003CoctetsPool_003Em__0()
		{
			return new Octets();
		}

		[CompilerGenerated]
		private static void _003CoctetsPool_003Em__1(Octets oc)
		{
		}

		[CompilerGenerated]
		private static void _003CoctetsPool_003Em__2(Octets oc)
		{
			oc.clear();
		}
	}
}

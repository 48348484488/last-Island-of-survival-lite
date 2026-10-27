using System;
using System.Net;
using System.Net.Sockets;
using Share;

namespace Net
{
	public sealed class Session
	{
		private static readonly AtomicLong nextId = new AtomicLong(1L);

		public readonly long id = nextId.GetAndAdd(1L);

		private Octets ibuffer;

		private Octets obuffer;

		private readonly Socket socket;

		private readonly Manager manager;

		private readonly AtomicInteger closed = new AtomicInteger(0);

		public Manager Manager
		{
			get
			{
				return manager;
			}
		}

		public Session(Manager manager, Socket socket)
		{
			this.manager = manager;
			this.socket = socket;
			ibuffer = manager.AllocateOctets();
			obuffer = manager.AllocateOctets();
		}

		public void Connect(string host, int port)
		{
			socket.BeginConnect(host, port, OnConnectResult, null);
		}

		public void Connect(IPAddress[] address, int port)
		{
			socket.BeginConnect(address, port, OnConnectResult, null);
		}

		public void Send(Message message)
		{
			try
			{
				obuffer.push(message.getType());
				obuffer.push(0);
				int size = obuffer.Size;
				obuffer.push(message);
				int num = obuffer.Size - size;
				obuffer.push_rollback(num + 4);
				obuffer.push(num);
				obuffer.push_count(num);
			}
			catch (Exception)
			{
				Close();
			}
		}

		public void Flush()
		{
			try
			{
				if (obuffer.Size > 0)
				{
					socket.BeginSend(obuffer.getBytes(), 0, obuffer.Size, SocketFlags.None, OnSendResult, obuffer);
					obuffer = manager.AllocateOctets();
				}
			}
			catch (Exception)
			{
				Close();
			}
		}

		public void Close()
		{
			if (!closed.CompareAndSet(0, 1))
			{
				return;
			}
			try
			{
				manager.RecycleOctets(ibuffer);
				manager.RecycleOctets(obuffer);
				ibuffer = null;
				obuffer = null;
				socket.Shutdown(SocketShutdown.Receive);
				socket.Close(1);
			}
			catch
			{
			}
			try
			{
				manager.DelSession(this);
			}
			catch
			{
			}
		}

		private void Decode()
		{
			while (true)
			{
				if (ibuffer.Size < 8)
				{
					return;
				}
				int type = ibuffer.pop_int();
				int num = ibuffer.pop_int();
				if (num > ibuffer.Capacity - 8)
				{
					throw new Exception("message size error: " + num);
				}
				if (num > ibuffer.Size)
				{
					break;
				}
				int size = ibuffer.Size;
				int num2 = 0;
				Message message = manager.CreateMessage(type);
				if (message == null)
				{
					manager.onUnknownMessage(this, type, num, ibuffer);
					num2 = size - ibuffer.Size;
					if (num2 < num)
					{
						ibuffer.pop_count(num - num2);
					}
					continue;
				}
				message.unmarshal(ibuffer);
				num2 = size - ibuffer.Size;
				if (num2 < num)
				{
					ibuffer.pop_count(num - num2);
				}
				message.setSession(this);
				try
				{
					message.dispatch();
				}
				catch
				{
				}
			}
			ibuffer.pop_rollback(8);
		}

		private void OnConnectResult(IAsyncResult result)
		{
			try
			{
				socket.EndConnect(result);
				manager.AddSession(this);
				int offset;
				int remainSize;
				byte[] bytesForWrite = ibuffer.getBytesForWrite(out offset, out remainSize);
				socket.BeginReceive(bytesForWrite, offset, remainSize, SocketFlags.None, onReceiveResult, null);
			}
			catch (Exception)
			{
				Close();
			}
		}

		private void onReceiveResult(IAsyncResult result)
		{
			try
			{
				int num = socket.EndReceive(result);
				if (num == 0)
				{
					Close();
					return;
				}
				ibuffer.push_count(num);
				Decode();
				int offset;
				int remainSize;
				byte[] bytesForWrite = ibuffer.getBytesForWrite(out offset, out remainSize);
				socket.BeginReceive(bytesForWrite, offset, remainSize, SocketFlags.None, onReceiveResult, null);
			}
			catch (Exception)
			{
				Close();
			}
		}

		private void OnSendResult(IAsyncResult result)
		{
			try
			{
				socket.EndSend(result);
				manager.RecycleOctets(result.AsyncState as Octets);
			}
			catch (Exception)
			{
				Close();
			}
		}
	}
}

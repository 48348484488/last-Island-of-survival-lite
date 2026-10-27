using System.Collections.Generic;

namespace Share
{
	public class MessageQueue<T>
	{
		private readonly Queue<T> m_queue = new Queue<T>();

		public bool IsEmpty
		{
			get
			{
				lock (m_queue)
				{
					return m_queue.Count == 0;
				}
			}
		}

		public void Enqueue(T message)
		{
			lock (m_queue)
			{
				m_queue.Enqueue(message);
			}
		}

		public T Dequeue()
		{
			lock (m_queue)
			{
				if (m_queue.Count == 0)
				{
					return default(T);
				}
				return m_queue.Dequeue();
			}
		}

		public void Clear()
		{
			lock (m_queue)
			{
				m_queue.Clear();
			}
		}
	}
}

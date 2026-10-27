using System;
using System.Threading;

namespace Share
{
	public class AtomicInteger
	{
		private int value;

		public AtomicInteger(int initialValue)
		{
			value = initialValue;
		}

		public AtomicInteger()
			: this(0)
		{
		}

		public int Get()
		{
			return value;
		}

		public void Set(int newValue)
		{
			value = newValue;
		}

		public int GetAndSet(int newValue)
		{
			int num;
			do
			{
				num = Get();
			}
			while (!CompareAndSet(num, newValue));
			return num;
		}

		public bool CompareAndSet(int expect, int update)
		{
			return Interlocked.CompareExchange(ref value, update, expect) == expect;
		}

		public int GetAndIncrement()
		{
			int num;
			int update;
			do
			{
				num = Get();
				update = num + 1;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public int GetAndDecrement()
		{
			int num;
			int update;
			do
			{
				num = Get();
				update = num - 1;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public int GetAndAdd(int delta)
		{
			int num;
			int update;
			do
			{
				num = Get();
				update = num + delta;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public int IncrementAndGet()
		{
			int num;
			int num2;
			do
			{
				num = Get();
				num2 = num + 1;
			}
			while (!CompareAndSet(num, num2));
			return num2;
		}

		public int DecrementAndGet()
		{
			int num;
			int num2;
			do
			{
				num = Get();
				num2 = num - 1;
			}
			while (!CompareAndSet(num, num2));
			return num2;
		}

		public int AddAndGet(int delta)
		{
			int num;
			int num2;
			do
			{
				num = Get();
				num2 = num + delta;
			}
			while (!CompareAndSet(num, num2));
			return num2;
		}

		public override string ToString()
		{
			return Convert.ToString(Get());
		}
	}
}

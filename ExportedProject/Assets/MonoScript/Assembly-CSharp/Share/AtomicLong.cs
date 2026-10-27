using System;
using System.Threading;

namespace Share
{
	public class AtomicLong
	{
		private long value;

		public AtomicLong(long initialValue)
		{
			value = initialValue;
		}

		public AtomicLong()
			: this(0L)
		{
		}

		public long Get()
		{
			return value;
		}

		public void Set(long newValue)
		{
			value = newValue;
		}

		public long GetAndSet(long newValue)
		{
			long num;
			do
			{
				num = Get();
			}
			while (!CompareAndSet(num, newValue));
			return num;
		}

		public bool CompareAndSet(long expect, long update)
		{
			return Interlocked.CompareExchange(ref value, update, expect) == expect;
		}

		public long GetAndIncrement()
		{
			long num;
			long update;
			do
			{
				num = Get();
				update = num + 1;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public long GetAndDecrement()
		{
			long num;
			long update;
			do
			{
				num = Get();
				update = num - 1;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public long GetAndAdd(long delta)
		{
			long num;
			long update;
			do
			{
				num = Get();
				update = num + delta;
			}
			while (!CompareAndSet(num, update));
			return num;
		}

		public long IncrementAndGet()
		{
			long num;
			long num2;
			do
			{
				num = Get();
				num2 = num + 1;
			}
			while (!CompareAndSet(num, num2));
			return num2;
		}

		public long DecrementAndGet()
		{
			long num;
			long num2;
			do
			{
				num = Get();
				num2 = num - 1;
			}
			while (!CompareAndSet(num, num2));
			return num2;
		}

		public long AddAndGet(long delta)
		{
			long num;
			long num2;
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

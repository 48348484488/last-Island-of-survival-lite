using System;
using System.Text;

namespace Share
{
	public sealed class Octets
	{
		private int capacity;

		private byte[] buffer;

		private int start;

		private int size;

		private readonly byte[] tmp = new byte[8];

		private static readonly byte[] numArray = new byte[4];

		public int Size
		{
			get
			{
				return size;
			}
		}

		public int Capacity
		{
			get
			{
				return capacity;
			}
		}

		public Octets()
		{
			capacity = int.MaxValue;
			buffer = new byte[16];
		}

		public Octets(Octets oc)
		{
			buffer = oc.buffer;
			capacity = oc.capacity;
			start = oc.start;
			size = oc.size;
		}

		public Octets(int capacity)
		{
			if (capacity < 16)
			{
				capacity = 16;
			}
			this.capacity = capacity;
			buffer = new byte[16];
		}

		public Octets(int capacity, int initialSize)
		{
			if (capacity < 16)
			{
				capacity = 16;
			}
			if (initialSize < 16)
			{
				initialSize = 16;
			}
			if (initialSize > capacity)
			{
				initialSize = capacity;
			}
			this.capacity = capacity;
			buffer = new byte[initialSize];
		}

		public Octets(byte[] buffer, int capacity)
		{
			Set(buffer, capacity);
		}

		public Octets Set(byte[] buffer, int capacity)
		{
			if (capacity < buffer.Length)
			{
				throw new ArgumentException("wrong arg");
			}
			this.capacity = capacity;
			this.buffer = buffer;
			size = buffer.Length;
			return this;
		}

		public bool is_empty()
		{
			return size == 0;
		}

		public bool is_full()
		{
			return size == capacity;
		}

		public void clear()
		{
			start = 0;
			size = 0;
		}

		public Octets roundup()
		{
			if (buffer.Length < capacity)
			{
				int num = buffer.Length;
				num <<= 1;
				if (num > capacity)
				{
					num = capacity;
				}
				byte[] dstBuffer = new byte[num];
				copy_to(dstBuffer, 0);
				start = 0;
				buffer = dstBuffer;
				return this;
			}
			throw new Exception("buffer overflow");
		}

		public void copy_to(byte[] dstBuffer, int destPos)
		{
			int num = buffer.Length - start;
			if (num < size)
			{
				Array.Copy(buffer, start, dstBuffer, destPos, num);
				Array.Copy(buffer, 0, dstBuffer, destPos + num, size - num);
			}
			else
			{
				Array.Copy(buffer, start, dstBuffer, destPos, size);
			}
		}

		public void copy_from(Octets x, int size)
		{
			if (size < 0 || size > x.Size)
			{
				throw new Exception("wrong arg");
			}
			if (size != 0)
			{
				x.arrange(false);
				push(x.buffer, x.start, size);
			}
		}

		public void reserve(int size)
		{
			if (size > buffer.Length)
			{
				if (size > capacity)
				{
					throw new Exception("buffer overflow");
				}
				int num;
				for (num = 16; num < size; num <<= 1)
				{
				}
				if (num > capacity)
				{
					num = capacity;
				}
				byte[] dstBuffer = new byte[num];
				copy_to(dstBuffer, 0);
				buffer = dstBuffer;
				start = 0;
			}
		}

		public byte[] getBytes()
		{
			return buffer;
		}

		public byte[] getBytesForWrite(out int offset, out int remainSize)
		{
			reserve(size + 1);
			int num = start + size;
			if (num < buffer.Length)
			{
				offset = num;
				remainSize = buffer.Length - num;
			}
			else
			{
				offset = num - buffer.Length;
				remainSize = buffer.Length - size;
			}
			return buffer;
		}

		public byte[] getBytesForRead(out int offset, out int remainSize)
		{
			int num = start + size;
			int num2 = ((num > buffer.Length) ? (buffer.Length - start) : size);
			offset = start;
			remainSize = num2;
			return buffer;
		}

		public Octets push_rollback(int count)
		{
			if (count < 0 || count > size)
			{
				throw new Exception("wrong argument");
			}
			size -= count;
			return this;
		}

		public Octets push_count(int count)
		{
			if (count < 0)
			{
				throw new Exception("wrong argument");
			}
			size += count;
			reserve(size);
			return this;
		}

		public Octets pop_count(int count)
		{
			if (count < 0)
			{
				throw new Exception("wrong argument");
			}
			if (count > size)
			{
				throw new Exception("buffer empty");
			}
			start += count;
			if (start >= buffer.Length)
			{
				start -= buffer.Length;
			}
			size -= count;
			return this;
		}

		public Octets pop_rollback(int count)
		{
			if (count < 0)
			{
				throw new Exception("wrong argument");
			}
			if (count == 0)
			{
				return this;
			}
			size += count;
			reserve(size);
			start -= count;
			if (start < 0)
			{
				start += buffer.Length;
			}
			return this;
		}

		public Octets push(bool x)
		{
			return push((byte)(x ? 1u : 0u));
		}

		public Octets push(byte x)
		{
			reserve(size + 1);
			return push_byte(x);
		}

		public Octets push(short x)
		{
			reserve(size + 2);
			return push_byte((byte)x).push_byte((byte)(x >> 8));
		}

		public Octets push(int x)
		{
			reserve(size + 4);
			return push_byte((byte)x).push_byte((byte)(x >> 8)).push_byte((byte)(x >> 16)).push_byte((byte)(x >> 24));
		}

		public Octets push(long x)
		{
			tmp[0] = (byte)x;
			tmp[1] = (byte)(x >> 8);
			tmp[2] = (byte)(x >> 16);
			tmp[3] = (byte)(x >> 24);
			tmp[4] = (byte)(x >> 32);
			tmp[5] = (byte)(x >> 40);
			tmp[6] = (byte)(x >> 48);
			tmp[7] = (byte)(x >> 56);
			return push(tmp, 0, 8);
		}

		private unsafe static byte[] GetBytes(float value)
		{
			//IL_0023: Incompatible stack types: I vs Ref
			fixed (byte* ptr = &(numArray != null && numArray.Length != 0 ? ref numArray[0] : ref *(byte*)null))
			{
				*(float*)ptr = value;
			}
			return numArray;
		}

		public Octets push(float value)
		{
			byte[] bytes = GetBytes(value);
			int x = BitConverter.ToInt32(bytes, 0);
			return push(x);
		}

		public Octets push(double value)
		{
			return push(BitConverter.DoubleToInt64Bits(value));
		}

		public Octets push(string x)
		{
			try
			{
				if (x == null)
				{
					x = string.Empty;
				}
				byte[] bytes = Encoding.UTF8.GetBytes(x);
				push(bytes.Length);
				if (bytes.Length > 0)
				{
					return push(bytes);
				}
				return this;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw new Exception("wrong charset name");
			}
		}

		public Octets push(Octets x)
		{
			x.arrange(false);
			push(x.size);
			if (x.size > 0)
			{
				return push(x.buffer, x.start, x.size);
			}
			return this;
		}

		public Octets push(Marshal x)
		{
			return x.marshal(this);
		}

		public bool pop_bool()
		{
			return pop_byte() == 1;
		}

		public bool pop_boolean()
		{
			return pop_byte() == 1;
		}

		public byte pop_byte()
		{
			if (size <= 0)
			{
				throw new Exception("buffer empty");
			}
			byte result = buffer[start];
			size--;
			start++;
			if (start >= buffer.Length)
			{
				start = 0;
			}
			return result;
		}

		public short pop_short()
		{
			byte b = pop_byte();
			byte b2 = pop_byte();
			return (short)((b & 0xFF) | (b2 << 8));
		}

		public int pop_int()
		{
			byte b = pop_byte();
			byte b2 = pop_byte();
			byte b3 = pop_byte();
			byte b4 = pop_byte();
			return (b & 0xFF) | ((b2 & 0xFF) << 8) | ((b3 & 0xFF) << 16) | (b4 << 24);
		}

		public long pop_long()
		{
			pop(tmp, 0, 8);
			return ((long)(int)tmp[0] & 0xFFL) | (long)(((ulong)(int)tmp[1] & 0xFFuL) << 8) | (long)(((ulong)(int)tmp[2] & 0xFFuL) << 16) | (long)(((ulong)(int)tmp[3] & 0xFFuL) << 24) | (long)(((ulong)(int)tmp[4] & 0xFFuL) << 32) | (long)(((ulong)(int)tmp[5] & 0xFFuL) << 40) | (long)(((ulong)(int)tmp[6] & 0xFFuL) << 48) | ((long)(int)tmp[7] << 56);
		}

		public float pop_float()
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(pop_int()), 0);
		}

		public double pop_double()
		{
			return BitConverter.Int64BitsToDouble(pop_long());
		}

		public string pop_string()
		{
			int num = pop_int();
			if (num == 0)
			{
				return string.Empty;
			}
			if (num > 1048576)
			{
				throw new Exception("wrong string length : " + num);
			}
			byte[] array = new byte[num];
			pop(array);
			try
			{
				return Encoding.UTF8.GetString(array);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				throw new Exception("wrong charset name");
			}
		}

		public Octets pop_octets()
		{
			int num = pop_int();
			if (num == 0)
			{
				return new Octets();
			}
			byte[] dst = new byte[num];
			pop(dst);
			return new Octets(dst, int.MaxValue);
		}

		public void pop_octets(Octets oc)
		{
			int num = pop_int();
			if (num != 0)
			{
				oc.reserve(oc.size + num);
				oc.arrange(true);
				pop(oc.getBytes(), oc.size, num);
				oc.push_count(num);
			}
		}

		public Octets pop(Marshal x)
		{
			return x.unmarshal(this);
		}

		public Octets arrange(bool strict)
		{
			if (start == 0)
			{
				return this;
			}
			int num = start + size;
			if (!strict && num <= buffer.Length)
			{
				return this;
			}
			byte[] dstBuffer = new byte[(size >= 16) ? size : 16];
			copy_to(dstBuffer, 0);
			buffer = dstBuffer;
			start = 0;
			return this;
		}

		public Octets push(byte[] src, int offset, int length)
		{
			reserve(size + length);
			int num = start + size;
			if (num >= buffer.Length)
			{
				num -= buffer.Length;
			}
			int num2 = buffer.Length - num;
			if (num2 < length)
			{
				Array.Copy(src, offset, buffer, num, num2);
				Array.Copy(src, offset + num2, buffer, 0, length - num2);
			}
			else
			{
				Array.Copy(src, offset, buffer, num, length);
			}
			return push_count(length);
		}

		public Octets copy()
		{
			Octets octets = new Octets(this);
			octets.buffer = new byte[buffer.Length];
			Array.Copy(buffer, 0, octets.buffer, 0, buffer.Length);
			return octets;
		}

		private Octets push_byte(byte b)
		{
			int num = start + size;
			if (num >= buffer.Length)
			{
				num -= buffer.Length;
			}
			buffer[num] = b;
			size++;
			return this;
		}

		private Octets push(byte[] src)
		{
			return push(src, 0, src.Length);
		}

		private Octets pop(byte[] dst, int offset, int length)
		{
			if (length > size)
			{
				throw new Exception("buffer underflow");
			}
			int num = buffer.Length - start;
			if (length > num)
			{
				Array.Copy(buffer, start, dst, offset, num);
				Array.Copy(buffer, 0, dst, offset + num, length - num);
			}
			else
			{
				Array.Copy(buffer, start, dst, offset, length);
			}
			return pop_count(length);
		}

		public Octets pop(byte[] dst)
		{
			return pop(dst, 0, dst.Length);
		}

		public override int GetHashCode()
		{
			return (int)hash();
		}

		public long hash()
		{
			arrange(true);
			long num = MurmurHash.hash64(buffer, size);
			if (num < 0)
			{
				num = -num;
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			Octets octets = (Octets)obj;
			if (size != octets.size)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < size; i++)
			{
				num = start + i;
				num2 = octets.start + i;
				if (num >= buffer.Length)
				{
					num = 0;
				}
				if (num2 >= octets.buffer.Length)
				{
					num2 = 0;
				}
				if (buffer[num] != octets.buffer[num2])
				{
					return false;
				}
			}
			return true;
		}
	}
}

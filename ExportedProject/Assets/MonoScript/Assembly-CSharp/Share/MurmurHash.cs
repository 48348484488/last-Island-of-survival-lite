using System.Text;

namespace Share
{
	public sealed class MurmurHash
	{
		public static int hash32(byte[] data, int length, int seed)
		{
			int num = 1540483477;
			int shift = 24;
			int num2 = seed ^ length;
			int num3 = length / 4;
			for (int i = 0; i < num3; i++)
			{
				int num4 = i * 4;
				int num5 = (data[num4] & 0xFF) + ((data[num4 + 1] & 0xFF) << 8) + ((data[num4 + 2] & 0xFF) << 16) + ((data[num4 + 3] & 0xFF) << 24);
				num5 *= num;
				num5 ^= rightShiftWithoutSign(num5, shift);
				num5 *= num;
				num2 *= num;
				num2 ^= num5;
			}
			switch (length % 4)
			{
			case 3:
				num2 ^= (data[(length & -4) + 2] & 0xFF) << 16;
				num2 ^= (data[(length & -4) + 1] & 0xFF) << 8;
				num2 ^= data[length & -4] & 0xFF;
				num2 *= num;
				break;
			case 2:
				num2 ^= (data[(length & -4) + 1] & 0xFF) << 8;
				num2 ^= data[length & -4] & 0xFF;
				num2 *= num;
				break;
			case 1:
				num2 ^= data[length & -4] & 0xFF;
				num2 *= num;
				break;
			}
			num2 ^= rightShiftWithoutSign(num2, 13);
			num2 *= num;
			return num2 ^ rightShiftWithoutSign(num2, 15);
		}

		public static int hash32(byte[] data, int length)
		{
			return hash32(data, length, -1756908916);
		}

		public static int hash32(string text)
		{
			byte[] bytes = Encoding.Default.GetBytes(text);
			return hash32(bytes, bytes.Length);
		}

		public static int hash32(string text, int from, int length)
		{
			return hash32(text.Substring(from, length));
		}

		public static long hash64(byte[] data, int length, int seed)
		{
			long num = -4132994306676758123L;
			int shift = 47;
			long num2 = (seed & 0xFFFFFFFFu) ^ (length * num);
			int num3 = length / 8;
			for (int i = 0; i < num3; i++)
			{
				int num4 = i * 8;
				long num5 = (long)(((ulong)(int)data[num4] & 0xFFuL) + (((ulong)(int)data[num4 + 1] & 0xFFuL) << 8) + (((ulong)(int)data[num4 + 2] & 0xFFuL) << 16) + (((ulong)(int)data[num4 + 3] & 0xFFuL) << 24) + (((ulong)(int)data[num4 + 4] & 0xFFuL) << 32) + (((ulong)(int)data[num4 + 5] & 0xFFuL) << 40) + (((ulong)(int)data[num4 + 6] & 0xFFuL) << 48) + (((ulong)(int)data[num4 + 7] & 0xFFuL) << 56));
				num5 *= num;
				num5 ^= rightShiftWithoutSign(num5, shift);
				num5 *= num;
				num2 ^= num5;
				num2 *= num;
			}
			switch (length % 8)
			{
			case 7:
				num2 ^= (long)(data[(length & -8) + 6] & 0xFF) << 48;
				num2 ^= (long)(data[(length & -8) + 5] & 0xFF) << 40;
				num2 ^= (long)(data[(length & -8) + 4] & 0xFF) << 32;
				num2 ^= (long)(data[(length & -8) + 3] & 0xFF) << 24;
				num2 ^= (long)(data[(length & -8) + 2] & 0xFF) << 16;
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 6:
				num2 ^= (long)(data[(length & -8) + 5] & 0xFF) << 40;
				num2 ^= (long)(data[(length & -8) + 4] & 0xFF) << 32;
				num2 ^= (long)(data[(length & -8) + 3] & 0xFF) << 24;
				num2 ^= (long)(data[(length & -8) + 2] & 0xFF) << 16;
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 5:
				num2 ^= (long)(data[(length & -8) + 4] & 0xFF) << 32;
				num2 ^= (long)(data[(length & -8) + 3] & 0xFF) << 24;
				num2 ^= (long)(data[(length & -8) + 2] & 0xFF) << 16;
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 4:
				num2 ^= (long)(data[(length & -8) + 3] & 0xFF) << 24;
				num2 ^= (long)(data[(length & -8) + 2] & 0xFF) << 16;
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 3:
				num2 ^= (long)(data[(length & -8) + 2] & 0xFF) << 16;
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 2:
				num2 ^= (long)(data[(length & -8) + 1] & 0xFF) << 8;
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			case 1:
				num2 ^= data[length & -8] & 0xFF;
				num2 *= num;
				break;
			}
			num2 ^= rightShiftWithoutSign(num2, shift);
			num2 *= num;
			return num2 ^ rightShiftWithoutSign(num2, shift);
		}

		public static long hash64(byte[] data, int length)
		{
			return hash64(data, length, -512093083);
		}

		public static long hash64(string text)
		{
			byte[] bytes = Encoding.Default.GetBytes(text);
			return hash64(bytes, bytes.Length);
		}

		public static long hash64(string text, int from, int length)
		{
			return hash64(text.Substring(from, length));
		}

		private static int rightShiftWithoutSign(int i, int shift)
		{
			if (i >= 0)
			{
				return i >> shift;
			}
			shift %= 32;
			int num = int.MaxValue;
			for (int j = 0; j < shift; j++)
			{
				i >>= 1;
				i &= num;
			}
			return i;
		}

		private static long rightShiftWithoutSign(long i, int shift)
		{
			if (i >= 0)
			{
				return i >> shift;
			}
			shift %= 64;
			long num = long.MaxValue;
			for (int j = 0; j < shift; j++)
			{
				i >>= 1;
				i &= num;
			}
			return i;
		}
	}
}

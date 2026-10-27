namespace Share
{
	public class Common
	{
		public static int roundUp(int value)
		{
			if (value > 1073741824)
			{
				return int.MaxValue;
			}
			int num;
			for (num = 1; num < value; num <<= 1)
			{
			}
			return num;
		}
	}
}

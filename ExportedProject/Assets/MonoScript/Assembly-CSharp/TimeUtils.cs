using System;

public class TimeUtils
{
	private static readonly DateTime date_1970 = new DateTime(1970, 1, 1);

	public static bool TheTimeIsToday(int nSecond)
	{
		long num = nSecond;
		num *= 10000000;
		num += date_1970.Ticks;
		DateTime date = new DateTime(num).Date;
		return DateTime.UtcNow.Date == date;
	}

	public static int GetCurrentSeconds()
	{
		long num = DateTime.Now.Ticks - date_1970.Ticks;
		num /= 10000000;
		return (int)num;
	}
}

public class Singleton<T> where T : class, new()
{
	private static T ins = new T();

	public static T Ins
	{
		get
		{
			return ins;
		}
	}

	protected Singleton()
	{
	}
}

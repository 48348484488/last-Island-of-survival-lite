namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class StringExtension
	{
		public static string Truncate(string s, int maxLength)
		{
			return (s == null || s.Length <= maxLength) ? s : s.Substring(0, maxLength);
		}
	}
}

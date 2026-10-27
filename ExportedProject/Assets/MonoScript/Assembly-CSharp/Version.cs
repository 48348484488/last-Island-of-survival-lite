using System.Text;

internal class Version
{
	private int[] nVersion = new int[4];

	public static Version Parse(string strVersion)
	{
		Version version = new Version();
		string[] array = strVersion.Split('.');
		if (array.Length != 4)
		{
			return version;
		}
		for (int i = 0; i < 4; i++)
		{
			version.nVersion[i] = int.Parse(array[i]);
		}
		return version;
	}

	public int Compare(Version version)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = nVersion[i] - version.nVersion[i];
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}

	public int Compare(string strVersion)
	{
		Version version = Parse(strVersion);
		return Compare(version);
	}

	public static int Compare(string first, string second)
	{
		Version version = Parse(first);
		return version.Compare(second);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("{0}.{1}.{2}.{3}", nVersion[0], nVersion[1], nVersion[2], nVersion[3]);
		return stringBuilder.ToString();
	}

	public int CompareOnlyMainSub(Version version)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = nVersion[i] - version.nVersion[i];
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}
}

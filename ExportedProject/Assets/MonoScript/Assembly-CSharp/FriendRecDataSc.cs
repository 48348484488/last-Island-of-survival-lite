using System.Collections.Generic;

public class FriendRecDataSc : Singleton<FriendRecDataSc>
{
	private int emptyIndex;

	private bool isRound;

	private Dictionary<int, string> mNews = new Dictionary<int, string>();

	private readonly byte MAX_REC_NUM = 100;

	public void AddNews(string news)
	{
		if (news != null)
		{
			mNews[emptyIndex] = news;
			if (emptyIndex < MAX_REC_NUM)
			{
				emptyIndex++;
				return;
			}
			isRound = true;
			emptyIndex = 0;
		}
	}

	public string GetNews(int index)
	{
		int num = (emptyIndex - 1 + MAX_REC_NUM - index) % (int)MAX_REC_NUM;
		if (num < 0)
		{
			return string.Empty;
		}
		return mNews[num];
	}

	public int GetNewsCount()
	{
		if (isRound)
		{
			return MAX_REC_NUM;
		}
		return emptyIndex;
	}

	public void CleanAllNews()
	{
		mNews.Clear();
		isRound = false;
		emptyIndex = 0;
	}
}

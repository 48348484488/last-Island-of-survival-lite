using Share;

public class ziyuanShuaxin
{
	public string ziyuanid;

	public string poscount;

	public string countPerPos;

	public string safeRound;

	public string inter;

	public string shuaxintime;

	public ziyuanShuaxin(Octets oc)
	{
		ziyuanid = oc.pop_string();
		poscount = oc.pop_string();
		countPerPos = oc.pop_string();
		safeRound = oc.pop_string();
		inter = oc.pop_string();
		shuaxintime = oc.pop_string();
	}
}

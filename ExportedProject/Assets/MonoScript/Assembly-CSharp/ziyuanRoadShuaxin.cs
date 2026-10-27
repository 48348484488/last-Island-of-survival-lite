using Share;

public class ziyuanRoadShuaxin
{
	public int rand;

	public string ziyuanid;

	public ziyuanRoadShuaxin(Octets oc)
	{
		rand = oc.pop_int();
		ziyuanid = oc.pop_string();
	}
}

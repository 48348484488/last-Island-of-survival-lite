using Share;

public class lajidropInfo
{
	public int id;

	public int dropRandom;

	public lajidropInfo(Octets oc)
	{
		id = oc.pop_int();
		dropRandom = oc.pop_int();
	}
}

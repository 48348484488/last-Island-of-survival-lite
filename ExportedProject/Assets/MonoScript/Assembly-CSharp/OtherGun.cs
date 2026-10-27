using System.Collections.Generic;

public class OtherGun : BaseGun
{
	public OtherGun(BasePlayerController owner, int gunId, int skinid, HashSet<int> parts, int bulletNum)
		: base(owner, gunId, skinid, parts)
	{
	}
}

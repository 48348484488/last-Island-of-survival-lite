using Share;

namespace gs.battle.map.board.scmsg
{
	public class BoardInfo : Marshal
	{
		public string info = string.Empty;

		public string extraInfo = string.Empty;

		public Octets marshal(Octets oc)
		{
			oc.push(info);
			oc.push(extraInfo);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			info = oc.pop_string();
			extraInfo = oc.pop_string();
			return oc;
		}
	}
}

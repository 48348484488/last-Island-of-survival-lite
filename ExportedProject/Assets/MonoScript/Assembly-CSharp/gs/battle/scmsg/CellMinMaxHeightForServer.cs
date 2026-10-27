using Share;

namespace gs.battle.scmsg
{
	public class CellMinMaxHeightForServer : Marshal
	{
		public int row;

		public int col;

		public float min;

		public float max;

		public Octets marshal(Octets oc)
		{
			oc.push(row);
			oc.push(col);
			oc.push(min);
			oc.push(max);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			row = oc.pop_int();
			col = oc.pop_int();
			min = oc.pop_float();
			max = oc.pop_float();
			return oc;
		}
	}
}

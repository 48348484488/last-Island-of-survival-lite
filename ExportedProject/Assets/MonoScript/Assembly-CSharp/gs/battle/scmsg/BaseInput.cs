using Share;

namespace gs.battle.scmsg
{
	public class BaseInput : Marshal
	{
		public float horizontalInput;

		public float verticalInput;

		public Octets marshal(Octets oc)
		{
			oc.push(horizontalInput);
			oc.push(verticalInput);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			horizontalInput = oc.pop_float();
			verticalInput = oc.pop_float();
			return oc;
		}
	}
}

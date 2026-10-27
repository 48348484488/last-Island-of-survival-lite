using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CAISyncLayerWeight : Message
	{
		public delegate void Handler(CAISyncLayerWeight msg);

		public const int TYPE = 11537441;

		public static Handler handler;

		public long roleId;

		public byte layer;

		public float value;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537441;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(layer);
			oc.push(value);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			layer = oc.pop_byte();
			value = oc.pop_float();
			return oc;
		}
	}
}

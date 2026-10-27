using Net;
using Share;
using gs.battle.drop.scmsg;

namespace gs.battle.scmsg
{
	public class SWitchBagGun : Message
	{
		public delegate void Handler(SWitchBagGun msg);

		public const int TYPE = 11537485;

		public static Handler handler;

		public long roleId;

		public int instanceId;

		public BagGun gun = new BagGun();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537485;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(instanceId);
			oc.push(gun);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			instanceId = oc.pop_int();
			oc.pop(gun);
			return oc;
		}
	}
}

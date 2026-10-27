using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SPlantInfo : Message
	{
		public delegate void Handler(SPlantInfo msg);

		public const int TYPE = 11537499;

		public static Handler handler;

		public long instanceId;

		public int plantId;

		public bool isGrownUp;

		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537499;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(plantId);
			oc.push(isGrownUp);
			oc.push(pos);
			oc.push(orientation);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			plantId = oc.pop_int();
			isGrownUp = oc.pop_bool();
			oc.pop(pos);
			oc.pop(orientation);
			return oc;
		}
	}
}

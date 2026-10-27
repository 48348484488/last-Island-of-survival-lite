using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SReBuildJiaju : Message
	{
		public delegate void Handler(SReBuildJiaju msg);

		public const int TYPE = 11537476;

		public static Handler handler;

		public long insId;

		public float posX;

		public float posY;

		public float posZ;

		public float eulerY;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537476;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(posX);
			oc.push(posY);
			oc.push(posZ);
			oc.push(eulerY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			posX = oc.pop_float();
			posY = oc.pop_float();
			posZ = oc.pop_float();
			eulerY = oc.pop_float();
			return oc;
		}
	}
}

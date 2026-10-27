using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CBuildJiaju : Message
	{
		public delegate void Handler(CBuildJiaju msg);

		public const int TYPE = 11537474;

		public static Handler handler;

		public int typeId;

		public long parentInsId;

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
			return 11537474;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(typeId);
			oc.push(parentInsId);
			oc.push(posX);
			oc.push(posY);
			oc.push(posZ);
			oc.push(eulerY);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			typeId = oc.pop_int();
			parentInsId = oc.pop_long();
			posX = oc.pop_float();
			posY = oc.pop_float();
			posZ = oc.pop_float();
			eulerY = oc.pop_float();
			return oc;
		}
	}
}

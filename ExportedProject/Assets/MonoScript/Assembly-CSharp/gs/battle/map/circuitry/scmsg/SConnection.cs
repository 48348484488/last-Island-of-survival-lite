using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SConnection : Message
	{
		public delegate void Handler(SConnection msg);

		public const int TYPE = 27265985;

		public static Handler handler;

		public const int IS_HAVE_CYCLE = 1;

		public long parentId;

		public long childId;

		public int errorCode;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265985;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(parentId);
			oc.push(childId);
			oc.push(errorCode);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			parentId = oc.pop_long();
			childId = oc.pop_long();
			errorCode = oc.pop_int();
			return oc;
		}
	}
}

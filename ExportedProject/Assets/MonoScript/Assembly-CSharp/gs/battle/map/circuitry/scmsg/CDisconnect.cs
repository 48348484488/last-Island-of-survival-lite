using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CDisconnect : Message
	{
		public delegate void Handler(CDisconnect msg);

		public const int TYPE = 27265986;

		public static Handler handler;

		public long parentId;

		public long childId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265986;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(parentId);
			oc.push(childId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			parentId = oc.pop_long();
			childId = oc.pop_long();
			return oc;
		}
	}
}

using Net;
using Share;

namespace gs.battle.scmsg
{
	public class COpenDoor : Message
	{
		public delegate void Handler(COpenDoor msg);

		public const int TYPE = 11537515;

		public static Handler handler;

		public long id;

		public bool direction;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537515;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(direction);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			direction = oc.pop_bool();
			return oc;
		}
	}
}

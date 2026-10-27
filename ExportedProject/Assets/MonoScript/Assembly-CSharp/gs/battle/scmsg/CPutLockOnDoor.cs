using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CPutLockOnDoor : Message
	{
		public delegate void Handler(CPutLockOnDoor msg);

		public const int TYPE = 11537522;

		public static Handler handler;

		public long id;

		public string password = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537522;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(password);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_long();
			password = oc.pop_string();
			return oc;
		}
	}
}

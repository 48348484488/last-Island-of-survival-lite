using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SRoleLoadBullet : Message
	{
		public delegate void Handler(SRoleLoadBullet msg);

		public const int TYPE = 11537559;

		public static Handler handler;

		public long roleId;

		public int gunInstanceId;

		public int bulletNumber;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537559;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(gunInstanceId);
			oc.push(bulletNumber);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			gunInstanceId = oc.pop_int();
			bulletNumber = oc.pop_int();
			return oc;
		}
	}
}

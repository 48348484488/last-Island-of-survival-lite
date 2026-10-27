using Net;
using Share;
using gs.role.scmsg;

namespace gs.troop.scmsg
{
	public class SRoleApplyTroop : Message
	{
		public delegate void Handler(SRoleApplyTroop msg);

		public const int TYPE = 15731653;

		public static Handler handler;

		public RoleVersion role = new RoleVersion();

		public int score;

		public bool scoreIsTop500;

		public int ladderLevel;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731653;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(role);
			oc.push(score);
			oc.push(scoreIsTop500);
			oc.push(ladderLevel);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(role);
			score = oc.pop_int();
			scoreIsTop500 = oc.pop_bool();
			ladderLevel = oc.pop_int();
			return oc;
		}
	}
}

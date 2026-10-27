using Net;
using Share;

namespace gs.troop.scmsg
{
	public class STeamateChangeName : Message
	{
		public delegate void Handler(STeamateChangeName msg);

		public const int TYPE = 15731659;

		public static Handler handler;

		public long roleId;

		public string name = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731659;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(name);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			name = oc.pop_string();
			return oc;
		}
	}
}

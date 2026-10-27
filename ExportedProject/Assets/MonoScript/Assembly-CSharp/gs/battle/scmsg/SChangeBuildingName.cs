using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SChangeBuildingName : Message
	{
		public delegate void Handler(SChangeBuildingName msg);

		public const int TYPE = 11537544;

		public static Handler handler;

		public long insId;

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
			return 11537544;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(insId);
			oc.push(name);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			insId = oc.pop_long();
			name = oc.pop_string();
			return oc;
		}
	}
}

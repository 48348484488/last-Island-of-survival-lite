using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SEnterBuilding : Message
	{
		public delegate void Handler(SEnterBuilding msg);

		public const int TYPE = 11537480;

		public static Handler handler;

		public BuildPartInfo buildPartInfo = new BuildPartInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537480;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(buildPartInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(buildPartInfo);
			return oc;
		}
	}
}

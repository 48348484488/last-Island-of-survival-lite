using Net;
using Share;

namespace gs.role.scmsg
{
	public class CChangeRoleSex : Message
	{
		public delegate void Handler(CChangeRoleSex msg);

		public const int TYPE = 4197312;

		public static Handler handler;

		public bool sex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197312;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(sex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			sex = oc.pop_bool();
			return oc;
		}
	}
}

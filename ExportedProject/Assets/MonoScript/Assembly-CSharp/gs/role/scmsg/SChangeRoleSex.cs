using Net;
using Share;

namespace gs.role.scmsg
{
	public class SChangeRoleSex : Message
	{
		public delegate void Handler(SChangeRoleSex msg);

		public const int TYPE = 4197313;

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
			return 4197313;
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

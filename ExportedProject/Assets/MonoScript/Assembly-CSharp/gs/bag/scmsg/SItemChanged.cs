using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SItemChanged : Message
	{
		public delegate void Handler(SItemChanged msg);

		public const int TYPE = 8391608;

		public static Handler handler;

		public int instanceId;

		public int itemId;

		public int num;

		public int duration;

		public bool isBind;

		public Octets extraInfo = new Octets();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391608;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(itemId);
			oc.push(num);
			oc.push(duration);
			oc.push(isBind);
			oc.push(extraInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_int();
			itemId = oc.pop_int();
			num = oc.pop_int();
			duration = oc.pop_int();
			isBind = oc.pop_bool();
			extraInfo = oc.pop_octets();
			return oc;
		}
	}
}

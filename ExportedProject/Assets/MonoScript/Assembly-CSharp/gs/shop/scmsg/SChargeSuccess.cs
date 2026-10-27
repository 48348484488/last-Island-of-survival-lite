using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SChargeSuccess : Message
	{
		public delegate void Handler(SChargeSuccess msg);

		public const int TYPE = 6294456;

		public static Handler handler;

		public int cfgId;

		public int amount;

		public string orderId = string.Empty;

		public string currency = string.Empty;

		public int goldType;

		public int gain;

		public int extra;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294456;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cfgId);
			oc.push(amount);
			oc.push(orderId);
			oc.push(currency);
			oc.push(goldType);
			oc.push(gain);
			oc.push(extra);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cfgId = oc.pop_int();
			amount = oc.pop_int();
			orderId = oc.pop_string();
			currency = oc.pop_string();
			goldType = oc.pop_int();
			gain = oc.pop_int();
			extra = oc.pop_int();
			return oc;
		}
	}
}

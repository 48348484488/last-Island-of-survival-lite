using Net;
using Share;

namespace gs.shop.scmsg
{
	public class CReportCharge : Message
	{
		public delegate void Handler(CReportCharge msg);

		public const int TYPE = 6294457;

		public static Handler handler;

		public string userId = string.Empty;

		public long roleId;

		public int amount;

		public string orderId = string.Empty;

		public string currencytype = string.Empty;

		public string paytype = string.Empty;

		public string deviceId = string.Empty;

		public string idfa = string.Empty;

		public string idfv = string.Empty;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294457;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(userId);
			oc.push(roleId);
			oc.push(amount);
			oc.push(orderId);
			oc.push(currencytype);
			oc.push(paytype);
			oc.push(deviceId);
			oc.push(idfa);
			oc.push(idfv);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			userId = oc.pop_string();
			roleId = oc.pop_long();
			amount = oc.pop_int();
			orderId = oc.pop_string();
			currencytype = oc.pop_string();
			paytype = oc.pop_string();
			deviceId = oc.pop_string();
			idfa = oc.pop_string();
			idfv = oc.pop_string();
			return oc;
		}
	}
}

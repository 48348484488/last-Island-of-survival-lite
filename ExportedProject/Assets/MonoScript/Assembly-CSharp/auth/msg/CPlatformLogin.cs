using Share;

namespace auth.msg
{
	public class CPlatformLogin : Marshal
	{
		public int platform;

		public Octets data = new Octets();

		public string deviceUniqueID = string.Empty;

		public string deviceModel = string.Empty;

		public string mac = string.Empty;

		public Octets marshal(Octets oc)
		{
			oc.push(platform);
			oc.push(data);
			oc.push(deviceUniqueID);
			oc.push(deviceModel);
			oc.push(mac);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			platform = oc.pop_int();
			data = oc.pop_octets();
			deviceUniqueID = oc.pop_string();
			deviceModel = oc.pop_string();
			mac = oc.pop_string();
			return oc;
		}
	}
}

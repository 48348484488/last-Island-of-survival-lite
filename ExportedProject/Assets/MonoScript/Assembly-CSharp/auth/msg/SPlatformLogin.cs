using System.Collections.Generic;
using Share;

namespace auth.msg
{
	public class SPlatformLogin : Marshal
	{
		public const int SUCCESS = 0;

		public const int FAILED = 1;

		public const int FORBID = 2;

		public const int ERROR = 3;

		public const int NO_SERVER_AVAILABLE = 4;

		public const int DEVICE_IS_FORBID = 5;

		public int code;

		public string userId = string.Empty;

		public string session = string.Empty;

		public Octets data = new Octets();

		public HashSet<int> historyGs = new HashSet<int>();

		public HashSet<int> openGs = new HashSet<int>();

		public Dictionary<int, int> gsStatus = new Dictionary<int, int>();

		public Octets marshal(Octets oc)
		{
			oc.push(code);
			oc.push(userId);
			oc.push(session);
			oc.push(data);
			oc.push(historyGs.Count);
			foreach (int historyG in historyGs)
			{
				oc.push(historyG);
			}
			oc.push(openGs.Count);
			foreach (int openG in openGs)
			{
				oc.push(openG);
			}
			oc.push(gsStatus.Count);
			foreach (KeyValuePair<int, int> item in gsStatus)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			userId = oc.pop_string();
			session = oc.pop_string();
			data = oc.pop_octets();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				historyGs.Add(oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				openGs.Add(oc.pop_int());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				gsStatus.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}

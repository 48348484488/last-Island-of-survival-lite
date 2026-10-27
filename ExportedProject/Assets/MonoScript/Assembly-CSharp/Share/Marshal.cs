namespace Share
{
	public interface Marshal
	{
		Octets marshal(Octets oc);

		Octets unmarshal(Octets oc);
	}
}

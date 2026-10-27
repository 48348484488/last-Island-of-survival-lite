using Share;

namespace gs.battle.scmsg
{
	public class LightmapInfo : Marshal
	{
		public byte lightmapIndex;

		public Vec4 lightmapScaleOffset = new Vec4();

		public Octets marshal(Octets oc)
		{
			oc.push(lightmapIndex);
			oc.push(lightmapScaleOffset);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			lightmapIndex = oc.pop_byte();
			oc.pop(lightmapScaleOffset);
			return oc;
		}
	}
}

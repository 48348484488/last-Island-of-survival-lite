using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncSound : Message
	{
		public delegate void Handler(CSyncSound msg);

		public const int TYPE = 11537386;

		public static Handler handler;

		public long roleId;

		public int id;

		public int soundId;

		public Vec3 pos = new Vec3();

		public float minDist;

		public float maxDist;

		public bool loop;

		public bool playMoreThanOne;

		public float volume;

		public float pitch;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537386;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(id);
			oc.push(soundId);
			oc.push(pos);
			oc.push(minDist);
			oc.push(maxDist);
			oc.push(loop);
			oc.push(playMoreThanOne);
			oc.push(volume);
			oc.push(pitch);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			id = oc.pop_int();
			soundId = oc.pop_int();
			oc.pop(pos);
			minDist = oc.pop_float();
			maxDist = oc.pop_float();
			loop = oc.pop_bool();
			playMoreThanOne = oc.pop_bool();
			volume = oc.pop_float();
			pitch = oc.pop_float();
			return oc;
		}
	}
}

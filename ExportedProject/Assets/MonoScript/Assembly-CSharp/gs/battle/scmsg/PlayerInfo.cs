using System.Collections.Generic;
using Share;
using gs.battle.drop.scmsg;

namespace gs.battle.scmsg
{
	public class PlayerInfo : Marshal
	{
		public const int InIsland = 1;

		public const int InBigPlane = 2;

		public const int LeavePlane = 3;

		public const int OpenSan = 4;

		public const int OnGround = 5;

		public const int Die = 11;

		public byte curStatus;

		public long roleId;

		public long insId;

		public string name = string.Empty;

		public bool sex;

		public float yaw;

		public bool isTanshen;

		public int groupId;

		public Vec3 pos = new Vec3();

		public ShortVec3 orientation = new ShortVec3();

		public ShortVec3 velocity = new ShortVec3();

		public BaseInput input = new BaseInput();

		public int ep;

		public int hp;

		public int hpMax;

		public bool isSecondHp;

		public bool isInBuildState;

		public BagInfo bagInfo = new BagInfo();

		public List<Vec3> markList = new List<Vec3>();

		public bool isDriver;

		public int vehicleId;

		public int headId;

		public int headFrameId;

		public Dictionary<byte, int> animatiorInfo = new Dictionary<byte, int>();

		public Dictionary<int, short> animatiorWeightInfo = new Dictionary<int, short>();

		public bool newPlayer;

		public Octets marshal(Octets oc)
		{
			oc.push(curStatus);
			oc.push(roleId);
			oc.push(insId);
			oc.push(name);
			oc.push(sex);
			oc.push(yaw);
			oc.push(isTanshen);
			oc.push(groupId);
			oc.push(pos);
			oc.push(orientation);
			oc.push(velocity);
			oc.push(input);
			oc.push(ep);
			oc.push(hp);
			oc.push(hpMax);
			oc.push(isSecondHp);
			oc.push(isInBuildState);
			oc.push(bagInfo);
			oc.push(markList.Count);
			foreach (Vec3 mark in markList)
			{
				oc.push(mark);
			}
			oc.push(isDriver);
			oc.push(vehicleId);
			oc.push(headId);
			oc.push(headFrameId);
			oc.push(animatiorInfo.Count);
			foreach (KeyValuePair<byte, int> item in animatiorInfo)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(animatiorWeightInfo.Count);
			foreach (KeyValuePair<int, short> item2 in animatiorWeightInfo)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			oc.push(newPlayer);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			curStatus = oc.pop_byte();
			roleId = oc.pop_long();
			insId = oc.pop_long();
			name = oc.pop_string();
			sex = oc.pop_bool();
			yaw = oc.pop_float();
			isTanshen = oc.pop_bool();
			groupId = oc.pop_int();
			oc.pop(pos);
			oc.pop(orientation);
			oc.pop(velocity);
			oc.pop(input);
			ep = oc.pop_int();
			hp = oc.pop_int();
			hpMax = oc.pop_int();
			isSecondHp = oc.pop_bool();
			isInBuildState = oc.pop_bool();
			oc.pop(bagInfo);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Vec3 vec = new Vec3();
				oc.pop(vec);
				markList.Add(vec);
			}
			isDriver = oc.pop_bool();
			vehicleId = oc.pop_int();
			headId = oc.pop_int();
			headFrameId = oc.pop_int();
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				animatiorInfo.Add(oc.pop_byte(), oc.pop_int());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				animatiorWeightInfo.Add(oc.pop_int(), oc.pop_short());
			}
			newPlayer = oc.pop_bool();
			return oc;
		}
	}
}

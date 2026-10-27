using System.Collections.Generic;
using UnityEngine;

public class Decal : MonoBehaviour
{
	public enum DecalType
	{
		Shuini = 0,
		JinShu = 1,
		Wood = 2,
		Ground = 3,
		Shuhen = 4,
		ShiTouhen = 5,
		Default = 6,
		Null = 7
	}

	public List<GameObject> Decals;

	public void SetDecal(DecalType decalType)
	{
		switch (decalType)
		{
		case DecalType.Null:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.JinShu:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(true);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.Wood:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(true);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.Ground:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(true);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.Default:
			Decals[0].SetActiveBetter(true);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.Shuhen:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(true);
			Decals[5].SetActiveBetter(false);
			break;
		case DecalType.ShiTouhen:
			Decals[0].SetActiveBetter(false);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(true);
			break;
		default:
			Decals[0].SetActiveBetter(true);
			Decals[1].SetActiveBetter(false);
			Decals[2].SetActiveBetter(false);
			Decals[3].SetActiveBetter(false);
			Decals[4].SetActiveBetter(false);
			Decals[5].SetActiveBetter(false);
			break;
		}
	}
}

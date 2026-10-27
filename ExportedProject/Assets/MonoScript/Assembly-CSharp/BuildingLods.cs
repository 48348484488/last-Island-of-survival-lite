using UnityEngine;

public class BuildingLods : MonoBehaviour
{
	private Transform mainCamTransform;

	private int lodLevel;

	private Transform lod0;

	private static float distance_0 = 2500f;

	private Transform lod1;

	private float distance_1 = 10000f;

	private Transform lod2;

	private static float distance_2 = 90000f;

	private Transform lod3;

	private Transform lod1ShowObject;

	private static float distance_hide = 360000f;

	private int nTotalLods;

	public static bool builidLod;

	private void Start()
	{
		nTotalLods = 1;
		mainCamTransform = Camera.main.transform;
		lod0 = base.transform.Find("LOD0");
		lod1 = base.transform.Find("LOD1");
		lod2 = base.transform.Find("LOD2");
		lod3 = base.transform.Find("LOD3");
		if (lod0 == null)
		{
			return;
		}
		string text = base.gameObject.name;
		int num = text.IndexOf(" (");
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		lod1ShowObject = lod0.Find(text);
		if (lod1 == null)
		{
			return;
		}
		nTotalLods++;
		if (!(lod2 == null))
		{
			nTotalLods++;
			if (!(lod3 == null))
			{
				nTotalLods++;
			}
		}
	}

	private void Update()
	{
		if (lod0 == null)
		{
			return;
		}
		float sqrMagnitude = (base.gameObject.transform.position - mainCamTransform.position).sqrMagnitude;
		if (sqrMagnitude > distance_hide)
		{
			if (lodLevel != -1)
			{
				HideAll();
				lodLevel = -1;
			}
			return;
		}
		int num = 0;
		if (sqrMagnitude > distance_2)
		{
			num = 3;
		}
		else if (sqrMagnitude > distance_0)
		{
			num = 1;
		}
		if (num >= nTotalLods)
		{
			num = nTotalLods - 1;
		}
		if (lodLevel != num)
		{
			lodLevel = num;
			ChangeLod();
		}
	}

	private void HideAll()
	{
		if (lod0 != null)
		{
			lod0.gameObject.SetActive(false);
		}
		if (lod1 != null)
		{
			lod1.gameObject.SetActive(false);
		}
		if (lod2 != null)
		{
			lod2.gameObject.SetActive(false);
		}
		if (lod3 != null)
		{
			lod3.gameObject.SetActive(false);
		}
	}

	private void ChangeLod()
	{
		lod0.gameObject.SetActive(lodLevel == 0);
		if (lod2 != null)
		{
			lod2.gameObject.SetActive(lodLevel == 2);
		}
		if (lod3 != null)
		{
			lod3.gameObject.SetActive(lodLevel == 3);
		}
		if (lodLevel == 1)
		{
			lod0.gameObject.SetActive(true);
			for (int i = 0; i < lod0.childCount; i++)
			{
				Transform child = lod0.GetChild(i);
				if (child != lod1ShowObject)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
		else if (lodLevel == 0)
		{
			for (int j = 0; j < lod0.childCount; j++)
			{
				Transform child2 = lod0.GetChild(j);
				child2.gameObject.SetActive(true);
			}
		}
	}
}

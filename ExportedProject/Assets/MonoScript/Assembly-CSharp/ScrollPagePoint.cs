using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPagePoint : MonoBehaviour
{
	public ScrollPage scrollPage;

	public ToggleGroup toggleGroup;

	public Toggle togglePrefab;

	private List<Toggle> toggleList = new List<Toggle>();

	private void Awake()
	{
		ScrollPage obj = scrollPage;
		obj.OnPageChanged = (Action<int, int>)Delegate.Combine(obj.OnPageChanged, new Action<int, int>(OnScrollPageChanged));
	}

	public void OnScrollPageChanged(int pageCount, int currentPageIndex)
	{
		if (pageCount != toggleList.Count)
		{
			if (pageCount > toggleList.Count)
			{
				int num = pageCount - toggleList.Count;
				for (int i = 0; i < num; i++)
				{
					toggleList.Add(CreateToggle());
				}
			}
			else if (pageCount < toggleList.Count)
			{
				while (toggleList.Count > pageCount)
				{
					Toggle toggle = toggleList[toggleList.Count - 1];
					toggleList.Remove(toggle);
					UnityEngine.Object.DestroyImmediate(toggle.gameObject);
				}
			}
		}
		if (currentPageIndex >= 0)
		{
			toggleList[currentPageIndex].isOn = true;
		}
	}

	private Toggle CreateToggle()
	{
		togglePrefab.gameObject.SetActive(false);
		Toggle toggle = UnityEngine.Object.Instantiate(togglePrefab);
		toggle.gameObject.SetActive(true);
		toggle.transform.SetParent(toggleGroup.transform);
		toggle.transform.localScale = Vector3.one;
		toggle.transform.localPosition = Vector3.zero;
		return toggle;
	}
}

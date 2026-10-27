using System.Collections.Generic;
using UnityEngine;
using cfg;

public class MultiLanguageMgr : Singleton<MultiLanguageMgr>
{
	private int CurrentLanguageIndex = -1;

	private List<MultiLanguageCfg> MultiLanguageCfgList;

	private List<MultiLanguageIndexCfg> multiLanguageIndexCfgList;

	public void ChangeLanguage(int index)
	{
		if (index == CurrentLanguageIndex)
		{
			SetLanguage();
			return;
		}
		CurrentLanguageIndex = index;
		SetLanguage();
		Utils.TriggerEvent(MultiLanguageEvent.RefreshFixedLableDelegate);
		Utils.TriggerEvent(MultiLanguageEvent.RefreshLableDelegate);
	}

	public int GetCurrentLanguageIndex()
	{
		return CurrentLanguageIndex;
	}

	public string GetLanguage(string oldStr)
	{
		if (string.IsNullOrEmpty(oldStr))
		{
			return string.Empty;
		}
		if (CurrentLanguageIndex < 0)
		{
			return oldStr;
		}
		if (oldStr.Contains("\r\n"))
		{
			oldStr = oldStr.Replace("\r\n", "\n");
		}
		MultiLanguageCfg multiLanguageCfg = MultiLanguageCfg.Get(oldStr);
		if (multiLanguageCfg == null)
		{
			return oldStr;
		}
		if (multiLanguageCfg.languages.Count > CurrentLanguageIndex)
		{
			return multiLanguageCfg.languages[CurrentLanguageIndex];
		}
		return oldStr;
	}

	public void SetMultiLanguage(GameObject gameObject)
	{
		if (!gameObject.GetComponent<MultiLanguage>())
		{
			gameObject.AddComponent<MultiLanguage>();
		}
	}

	public string GetLanguageName(int index)
	{
		List<MultiLanguageCfg> allList = MultiLanguageCfg.GetAllList();
		MultiLanguageCfg multiLanguageCfg = allList[0];
		if (index < 0)
		{
			return multiLanguageCfg.id;
		}
		return multiLanguageCfg.languages[index];
	}

	public void SetSystemLanguage(int index)
	{
		if (IsSetLanguage())
		{
			ChangeLanguage(GetLanguage());
			return;
		}
		MultiLanguageIndexCfg multiLanguageIndexCfg = MultiLanguageIndexCfg.Get((int)Application.systemLanguage);
		if (multiLanguageIndexCfg == null)
		{
			if (index > -1)
			{
				ChangeLanguage(0);
			}
		}
		else
		{
			ChangeLanguage(multiLanguageIndexCfg.index);
		}
	}

	public int GetNameIndex(out int length)
	{
		length = 0;
		multiLanguageIndexCfgList = MultiLanguageIndexCfg.GetAllList();
		for (int i = 0; i < multiLanguageIndexCfgList.Count; i++)
		{
			if (multiLanguageIndexCfgList[i].index == CurrentLanguageIndex)
			{
				length = multiLanguageIndexCfgList[i].nameLength;
				return multiLanguageIndexCfgList[i].nameIndex;
			}
		}
		return 0;
	}

	public bool IsSetLanguage()
	{
		return PlayerPrefs.HasKey("multiname");
	}

	private void SetLanguage()
	{
		PlayerPrefs.SetInt("multiname", Singleton<MultiLanguageMgr>.Ins.GetCurrentLanguageIndex());
		PlayerPrefs.Save();
	}

	private int GetLanguage()
	{
		return PlayerPrefs.GetInt("multiname");
	}
}

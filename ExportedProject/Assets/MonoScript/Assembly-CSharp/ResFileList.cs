using System;
using System.Collections.Generic;

[Serializable]
public class ResFileList
{
	public List<ResFileInfo> files;

	public ResFileList()
	{
		files = new List<ResFileInfo>();
	}

	public ResFileList(List<ResFileInfo> res)
	{
		files = res;
	}

	public void UpdateFileInfo(ResFileInfo info)
	{
		foreach (ResFileInfo file in files)
		{
			if (file.path.Equals(info.path, StringComparison.OrdinalIgnoreCase))
			{
				file.md5 = info.md5;
				file.size = info.size;
				return;
			}
		}
		files.Add(info);
	}

	public List<ResFileInfo> CheckDifference(ResFileList compList)
	{
		if (compList == null)
		{
			return files;
		}
		List<ResFileInfo> list = new List<ResFileInfo>();
		foreach (ResFileInfo file in files)
		{
			bool flag = false;
			for (int i = 0; i < compList.files.Count; i++)
			{
				ResFileInfo resFileInfo = compList.files[i];
				if (file.path.Equals(resFileInfo.path, StringComparison.OrdinalIgnoreCase))
				{
					if (file.md5.Equals(resFileInfo.md5, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
					compList.files.RemoveAt(i);
					break;
				}
			}
			if (!flag)
			{
				list.Add(file);
			}
		}
		return list;
	}
}

using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class FileOperation
{
	public static void DeleteFile(string fileName)
	{
		if (fileName.Length == 0)
		{
			return;
		}
		if (fileName[0] != '/')
		{
			fileName = Utils.GetPersistentPath("res/" + fileName);
		}
		try
		{
			File.Delete(fileName);
		}
		catch (Exception ex)
		{
			string text = "ReplaceLocalRes error: " + fileName + ex.ToString();
			Debug.LogError(text);
			if (text.Length > 600)
			{
				text = text.Substring(0, 600);
			}
			UdpSession.Ins.Send(text);
		}
	}

	public static bool WirteToFile(string fileName, string text)
	{
		if (fileName.Length == 0)
		{
			return false;
		}
		if (fileName[0] != '/')
		{
			fileName = Utils.GetPersistentPath("res/" + fileName);
		}
		try
		{
			string directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
			{
				CreateDirectory(directoryName);
			}
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(text);
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
			return true;
		}
		catch (Exception ex)
		{
			string text2 = "ReplaceLocalRes error: " + fileName + ex.ToString();
			Debug.LogError(text2);
			if (text2.Length > 600)
			{
				text2 = text2.Substring(0, 600);
			}
			UdpSession.Ins.Send(text2);
			return false;
		}
	}

	public static bool WirteToFile(string fileName, byte[] data, int nLength = -1)
	{
		if (fileName.Length == 0)
		{
			return false;
		}
		if (fileName[0] != '/')
		{
			fileName = Utils.GetPersistentPath("res/" + fileName);
		}
		int num = nLength;
		if (num < 0)
		{
			num = data.Length;
		}
		try
		{
			string directoryName = Path.GetDirectoryName(fileName);
			if (!Directory.Exists(directoryName))
			{
				CreateDirectory(directoryName);
			}
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			fileStream.Write(data, 0, num);
			fileStream.Flush();
			fileStream.Close();
			return true;
		}
		catch (Exception ex)
		{
			string text = "ReplaceLocalRes error: " + fileName + ex.ToString();
			Debug.LogError(text);
			if (text.Length > 600)
			{
				text = text.Substring(0, 600);
			}
			UdpSession.Ins.Send(text);
			return false;
		}
	}

	public static void DeleteExistPath(string path)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
			FileSystemInfo[] array = fileSystemInfos;
			foreach (FileSystemInfo fileSystemInfo in array)
			{
				if (fileSystemInfo is DirectoryInfo)
				{
					DirectoryInfo directoryInfo2 = new DirectoryInfo(fileSystemInfo.FullName);
					directoryInfo2.Delete(true);
				}
				else
				{
					File.Delete(fileSystemInfo.FullName);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void CreateDirectory(string path)
	{
		try
		{
			string persistentPath = Utils.GetPersistentPath(string.Empty);
			int length = persistentPath.Length;
			for (int num = path.IndexOf('/', length); num > 0; num = path.IndexOf('/', length))
			{
				length = num + 1;
				string path2 = path.Substring(0, length);
				if (!Directory.Exists(path2))
				{
					Directory.CreateDirectory(path2);
				}
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch (Exception ex)
		{
			string text = "CreateDirectory error: " + path + ex.ToString();
			Debug.LogError(text);
			if (text.Length > 600)
			{
				text = text.Substring(0, 600);
			}
			UdpSession.Ins.Send(text);
		}
	}

	public static Coroutine LoadText(string abPath, Utils.StringDelegate callback)
	{
		return Utils.StartConroutine(_LoadText(abPath, callback));
	}

	private static IEnumerator _LoadText(string abPath, Utils.StringDelegate callback)
	{
		WWW www = new WWW(abPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (callback != null)
			{
				callback(www.text);
			}
		}
		else
		{
			callback(string.Empty);
			Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
		}
	}

	public static string GetFileName(string path)
	{
		int num = path.LastIndexOf("/");
		if (num < 0)
		{
			num = path.LastIndexOf("\\");
		}
		int num2 = path.LastIndexOf(".");
		return path.Substring(num + 1, num2 - num - 1);
	}

	public static string GetFileNameIncludeExt(string path)
	{
		int num = path.LastIndexOf("\\");
		if (num < 0)
		{
			num = path.LastIndexOf("/");
		}
		return path.Substring(num + 1);
	}

	public static string GetFileExt(string path)
	{
		int num = path.LastIndexOf(".");
		if (num < 0)
		{
			return string.Empty;
		}
		return path.Substring(num + 1);
	}

	public static string GetFileLastSubPath(string path)
	{
		int num = path.LastIndexOf("/");
		if (num >= 0)
		{
			string text = path.Substring(0, num);
			num = text.LastIndexOf("/");
			if (num >= 0)
			{
				return text.Substring(num + 1);
			}
			return text;
		}
		num = path.LastIndexOf("\\");
		if (num >= 0)
		{
			string text2 = path.Substring(0, num);
			num = text2.LastIndexOf("\\");
			if (num >= 0)
			{
				return text2.Substring(num + 1);
			}
			return text2;
		}
		return string.Empty;
	}

	public static string ReadTextFromFile(string path)
	{
		if (File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		return string.Empty;
	}

	public static byte[] ReadByteFromFile(string path)
	{
		if (File.Exists(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
	}

	public static void CheckPathEnd(ref string path)
	{
		int length = path.Length;
		if (length != 0 && path[length - 1] != '/')
		{
			path += "/";
		}
	}

	public static string GetFilePath(string path)
	{
		path = path.Replace('\\', '/');
		int num = path.LastIndexOf("/");
		if (num < 0)
		{
			return string.Empty;
		}
		return path.Substring(0, num);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using cfg;

internal class UpdateMgr : Singleton<UpdateMgr>
{
	public delegate void TextCallback(string text, bool bSuccess);

	public delegate void BinCallback(byte[] text, bool bSuccess);

	public delegate void DownloadCallback(object obj, byte[] bytes);

	public static readonly string _strVersionPath = "version/version.txt";

	public static readonly string _strFileMd5List = "version/FileList.txt";

	public static readonly string _strCheckMd5List = "version/CheckFiles.txt";

	private Utils.VoidDelegate onUpdateComplete;

	private Utils.VoidDelegate onCheckComplete;

	private VersionFile _persistentVersionFile;

	public VersionFile _localVersionFile;

	public VersionFile _serverVersionFile;

	public static bool bRestart;

	private bool bUpdateServerList;

	private bool bUpdateRes;

	public int nHadDownloadSize;

	public int nToDownloadTotalSize;

	private int nHadFinishFileSize;

	private List<ResFileInfo> _toDownloadList;

	private ResFileList _localResList;

	private ResFileList _serverResList;

	private string _strResFileListJsonFromServer;

	private bool bXiufuDontWriteFile;

	private WWW _www;

	public bool bUpdateNormal;

	public bool bUpdateSucess = true;

	private float checkUpdateStartTime;

	private float checkUpdateFinishTime;

	private float updateStartTime;

	private float updateFinishTime;

	private ResFileList _checkFileList;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__am_0024cache1;

	public void Update(Utils.VoidDelegate updateComplete)
	{
		checkUpdateStartTime = Time.fixedTime;
		Singleton<PlatformMgr>.Ins.onUpdateGameResource(201, 1, 0);
		Debug.LogError("version " + Application.version);
		bXiufuDontWriteFile = false;
		bUpdateNormal = false;
		bRestart = false;
		bUpdateSucess = true;
		onUpdateComplete = updateComplete;
		LoadLocalVersion();
	}

	public void SetUpdateFinishCallback(Utils.VoidDelegate updateComplete)
	{
		onUpdateComplete = updateComplete;
	}

	private void LoadLocalVersion()
	{
		string persistentPath = Utils.GetPersistentPath("res/" + _strVersionPath);
		if (File.Exists(persistentPath))
		{
			string text = FileOperation.ReadTextFromFile(persistentPath);
			ToLoadStreamLocalVersion(text, text.Length > 0);
		}
		else
		{
			LoadText(Utils.GetStreamingAssetPathForWWW(_strVersionPath), ToCompareLocalVersion);
		}
	}

	private void ToLoadStreamLocalVersion(string versionStr, bool bSuccess)
	{
		if (bSuccess)
		{
			try
			{
				_persistentVersionFile = JsonUtility.FromJson<VersionFile>(versionStr);
			}
			catch (Exception)
			{
				string persistentPath = Utils.GetPersistentPath("res/" + _strVersionPath);
				File.Delete(persistentPath);
				Utils.TriggerEvent(onUpdateComplete);
				Debug.LogError("[update] ToLoadStreamLocalVersion error:" + versionStr);
				return;
			}
		}
		LoadText(Utils.GetStreamingAssetPathForWWW(_strVersionPath), ToCompareLocalVersion);
	}

	private void ToCompareLocalVersion(string text, bool bSuccess)
	{
		if (!bSuccess)
		{
			Utils.TriggerEvent(onUpdateComplete);
			return;
		}
		try
		{
			_localVersionFile = JsonUtility.FromJson<VersionFile>(text);
		}
		catch (Exception)
		{
			Utils.TriggerEvent(onUpdateComplete);
			Debug.LogError("[update] ToCompareLocalVersion error:" + text);
			return;
		}
		if (_localVersionFile == null)
		{
			Utils.TriggerEvent(onUpdateComplete);
			return;
		}
		if (_persistentVersionFile != null)
		{
			if (_persistentVersionFile.code < _localVersionFile.code)
			{
				string persistentPath = Utils.GetPersistentPath(ServerMgr.txt_address);
				if (File.Exists(persistentPath))
				{
					File.Delete(persistentPath);
				}
				_persistentVersionFile.code = _localVersionFile.code;
			}
			if (_persistentVersionFile.channels == null && _localVersionFile.channels != null)
			{
				_persistentVersionFile.channels = _localVersionFile.channels;
			}
			if (Version.Compare(_persistentVersionFile.strV, _localVersionFile.strV) < 0)
			{
				DeleteExistPath(Utils.GetPersistentPath("res/"));
				try
				{
					string environmentVariable = Environment.GetEnvironmentVariable("EXTERNAL_STORAGE");
					string path = environmentVariable + "/Android/data/" + Application.identifier + "/files/res/";
					if (Directory.Exists(path))
					{
						Directory.Delete(path);
						bRestart = true;
					}
					environmentVariable = Environment.GetEnvironmentVariable("SECONDARY_STORAGE");
					path = environmentVariable + "/Android/data/" + Application.identifier + "/files/res/";
					if (Directory.Exists(path))
					{
						Directory.Delete(path);
						bRestart = true;
					}
				}
				catch (Exception)
				{
				}
			}
			else
			{
				_localVersionFile = _persistentVersionFile;
			}
		}
		else
		{
			FileOperation.WirteToFile(_strVersionPath, text);
		}
		LoadText(ServerPath.ServerVisionPath + "?" + DateTime.Now.Ticks, CompareToServer);
	}

	private void CompareToServer(string text, bool bSuccess)
	{
		if (!bSuccess)
		{
			_serverVersionFile = _localVersionFile;
			bUpdateSucess = false;
			Utils.TriggerEvent(onUpdateComplete);
			UdpSession.Ins.Send("[update] CompareToServer get version file error");
			Debug.LogError("[update] CompareToServer get version file error");
			CheckVersion(false);
			return;
		}
		try
		{
			_serverVersionFile = JsonUtility.FromJson<VersionFile>(text);
		}
		catch (Exception)
		{
			bUpdateSucess = false;
			Utils.TriggerEvent(onUpdateComplete);
			Debug.LogError("[update] CompareToServer error:" + text);
			UdpSession.Ins.Send("[update] CompareToServer parse error" + text);
			CheckVersion(false);
			return;
		}
		if (_serverVersionFile == null)
		{
			Utils.TriggerEvent(onUpdateComplete);
			return;
		}
		Version version = Version.Parse(_serverVersionFile.strV);
		Version version2 = Version.Parse(_localVersionFile.strV);
		if (version.CompareOnlyMainSub(version2) > 0)
		{
			Debug.LogError("delete in compare");
			DeleteExistPath(Utils.GetPersistentPath("res/"));
			bUpdateNormal = true;
			Utils.VoidDelegate onOk = ToOpenChannelUrl;
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = Application.Quit;
			}
			MessageBoxPanel.Show(404, onOk, _003C_003Ef__mg_0024cache0);
			return;
		}
		if (_serverVersionFile.code > _localVersionFile.code)
		{
			LoadText(ServerPath.GsServerListAddr + "?" + _serverVersionFile.strV, OnGetServerList);
		}
		else
		{
			_serverVersionFile.code = _localVersionFile.code;
			bUpdateServerList = true;
			if (version.Compare(version2) <= 0)
			{
				_serverVersionFile.strV = _localVersionFile.strV;
				if (!string.IsNullOrEmpty(_localVersionFile.codeVersion))
				{
					_serverVersionFile.codeVersion = _localVersionFile.codeVersion;
				}
				Utils.TriggerEvent(onUpdateComplete);
				return;
			}
		}
		if (version.Compare(version2) > 0)
		{
			LoadText(ServerPath.ServerFileListpath + "?" + _serverVersionFile.strV, OnGetFileList);
			return;
		}
		_serverVersionFile.strV = _localVersionFile.strV;
		if (!string.IsNullOrEmpty(_localVersionFile.codeVersion))
		{
			_serverVersionFile.codeVersion = _localVersionFile.codeVersion;
		}
		bUpdateRes = true;
		CheckUpdateComplete();
	}

	private void ToOpenChannelUrl()
	{
		ToDownloadApk();
		Utils.TriggerEvent(onUpdateComplete);
	}

	private bool ToDownloadApk()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageName", new object[0]);
		string text = androidJavaObject.Call<string>("toString", new object[0]);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		Debug.LogError("[update] packname" + text);
		int num = text.IndexOf('-');
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		List<QrCodeCfg> allList = QrCodeCfg.GetAllList();
		QrCodeCfg qrCodeCfg = null;
		foreach (QrCodeCfg item in allList)
		{
			if (item.packageName.Equals(text, StringComparison.OrdinalIgnoreCase))
			{
				qrCodeCfg = item;
				break;
			}
		}
		if (qrCodeCfg == null)
		{
			string message = string.Format("[upate] packname not exist:" + text);
			Debug.LogError(message);
			UdpSession.Ins.Send(message);
			return false;
		}
		Application.OpenURL(qrCodeCfg.url);
		return true;
	}

	private void OnGetFileList(string text, bool bSuccess)
	{
		bUpdateNormal = true;
		if (!bSuccess)
		{
			bUpdateSucess = false;
			bUpdateRes = true;
			_serverVersionFile.strV = _localVersionFile.strV;
			CheckUpdateComplete();
			UdpSession.Ins.Send("[update] OnGetFileList get error");
			Debug.LogError("[update] OnGetFileList get error");
			CheckVersion(false);
			return;
		}
		_strResFileListJsonFromServer = text;
		try
		{
			_serverResList = JsonUtility.FromJson<ResFileList>(text);
		}
		catch (Exception)
		{
			bUpdateSucess = false;
			bUpdateRes = true;
			_serverVersionFile.strV = _localVersionFile.strV;
			CheckUpdateComplete();
			Debug.LogError("[update] OnLoadLocalFileMds error:" + text);
			UdpSession.Ins.Send("[update] OnGetFileList paser error");
			CheckVersion(false);
			return;
		}
		if (_serverResList == null)
		{
			bUpdateRes = true;
			_serverVersionFile.strV = _localVersionFile.strV;
			CheckUpdateComplete();
		}
		else
		{
			LoadText(Utils.GetFilePathForWWW(_strFileMd5List), OnLoadLocalFileMds);
		}
	}

	private void OnLoadLocalFileMds(string strJson, bool bSuccess)
	{
		ResFileList resFileList = null;
		try
		{
			resFileList = JsonUtility.FromJson<ResFileList>(strJson);
		}
		catch (Exception)
		{
			bUpdateRes = true;
			bUpdateSucess = false;
			CheckUpdateComplete();
			Debug.LogError("[update] OnLoadLocalFileMds error:" + strJson);
			UdpSession.Ins.Send("[update] OnLoadLocalFileMds error");
			CheckVersion(false);
			return;
		}
		_toDownloadList = _serverResList.CheckDifference(resFileList);
		Debug.Log("[update] should update file count :" + _toDownloadList.Count);
		if (_toDownloadList.Count == 0)
		{
			bUpdateRes = true;
			CheckUpdateComplete();
			return;
		}
		nToDownloadTotalSize = 0;
		foreach (ResFileInfo toDownload in _toDownloadList)
		{
			nToDownloadTotalSize += toDownload.size;
		}
		Debug.LogError("update size is:" + nToDownloadTotalSize);
		_localResList = JsonUtility.FromJson<ResFileList>(strJson);
		if (nToDownloadTotalSize > 10485760)
		{
			Singleton<PlatformMgr>.Ins.onUpdateGameResource(203, 1, 0);
			MessageBoxPanel.ShowConfirm(Utils.GetString(410, nToDownloadTotalSize / 1048576), DownloadResFiles);
		}
		else
		{
			DownloadResFiles();
		}
	}

	private void DownloadResFiles()
	{
		if (bXiufuDontWriteFile)
		{
			return;
		}
		updateStartTime = Time.fixedTime;
		Singleton<PlatformMgr>.Ins.onUpdateGameResource(204, 1, 0);
		if (_toDownloadList.Count == 0)
		{
			_www = null;
			bUpdateRes = true;
			bool flag = FileOperation.WirteToFile(_strFileMd5List, _strResFileListJsonFromServer);
			CheckWriteFile(flag, _strFileMd5List);
			if (flag)
			{
				CheckUpdateComplete();
			}
		}
		else
		{
			ResFileInfo fileInfo = _toDownloadList[_toDownloadList.Count - 1];
			DownloadResFile(fileInfo, DownloadFileFinish);
		}
	}

	private void DownloadFileFinish(object obj, byte[] bytes)
	{
		if (bXiufuDontWriteFile)
		{
			return;
		}
		ResFileInfo resFileInfo = obj as ResFileInfo;
		if (bytes == null)
		{
			Debug.LogError("[update] download " + resFileInfo.path + " error");
			return;
		}
		string fileMd = GetFileMd5(bytes);
		if (!resFileInfo.md5.Equals(fileMd, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogError("[update] download " + resFileInfo.path + " md5 error");
			nHadFinishFileSize -= resFileInfo.size;
			Utils.VoidDelegate onOk = DownloadResFiles;
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = Application.Quit;
			}
			MessageBoxPanel.Show(405, onOk, _003C_003Ef__mg_0024cache1);
			return;
		}
		if (resFileInfo.path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
		{
			bRestart = true;
		}
		bool flag = FileOperation.WirteToFile(resFileInfo.path, bytes);
		CheckWriteFile(flag, resFileInfo.path);
		if (!flag)
		{
			FileOperation.DeleteFile(resFileInfo.path);
			return;
		}
		_localResList.UpdateFileInfo(resFileInfo);
		string text = JsonUtility.ToJson(_localResList);
		flag = FileOperation.WirteToFile(_strFileMd5List, text);
		CheckWriteFile(flag, _strFileMd5List);
		if (flag)
		{
			_toDownloadList.RemoveAt(_toDownloadList.Count - 1);
			DownloadResFiles();
		}
	}

	public void CheckWriteFile(bool bWrite, string path)
	{
		if (!bWrite)
		{
			bUpdateNormal = true;
			UdpSession.Ins.Send("[update] write file error:" + path);
			Debug.LogError("[update] write file error:" + path);
			MessageBoxPanel.ShowConfirm(406, AndroidSDKInterface.Instance.RestartApplication);
		}
	}

	public void CheckVersion(bool bSuccess)
	{
		bUpdateNormal = true;
		UdpSession.Ins.Send("[update] CheckVersion error");
		MessageBoxPanel.ShowConfirm(407, AndroidSDKInterface.Instance.RestartApplication);
	}

	public void DownloadResFile(ResFileInfo fileInfo, DownloadCallback callback)
	{
		Utils.StartConroutine(_Download(fileInfo, callback));
	}

	private IEnumerator _Download(ResFileInfo fileInfo, DownloadCallback callback)
	{
		string fileName2 = string.Empty;
		int nPos = fileInfo.path.IndexOf('/');
		string url2 = string.Empty;
		if (nPos > 0)
		{
			fileName2 = fileInfo.path.Substring(nPos + 1);
			fileName2 = WWW.EscapeURL(fileName2);
			fileName2 = fileName2.Replace("+", "%20");
			url2 = ServerPath.ServerResPath + fileInfo.path.Substring(0, nPos + 1) + fileName2 + "?" + fileInfo.md5;
		}
		else
		{
			fileName2 = WWW.EscapeURL(fileInfo.path);
			url2 = string.Concat(str1: fileName2.Replace("+", "%20"), str0: ServerPath.ServerResPath, str2: "?", str3: fileInfo.md5);
		}
		_www = new WWW(url2);
		yield return _www;
		if (string.IsNullOrEmpty(_www.error))
		{
			nHadFinishFileSize += _www.bytesDownloaded;
			if (callback != null)
			{
				callback(fileInfo, _www.bytes);
			}
		}
		else
		{
			Debug.LogError("Download error:" + url2);
			UdpSession.Ins.Send("[update] download error:" + url2);
			CheckWriteFile(false, fileInfo.path);
		}
	}

	public int GetHadDownloadSize()
	{
		if (nToDownloadTotalSize == 0)
		{
			return 0;
		}
		nHadDownloadSize = nHadFinishFileSize;
		if (_www != null)
		{
			int num = (int)(_www.progress * (float)_toDownloadList[_toDownloadList.Count - 1].size);
			nHadDownloadSize += num;
		}
		return nHadDownloadSize;
	}

	private void CheckUpdateComplete()
	{
		if (bUpdateServerList && bUpdateRes)
		{
			string text = JsonUtility.ToJson(_serverVersionFile);
			FileOperation.WirteToFile(_strVersionPath, text);
			Singleton<PlatformMgr>.Ins.OnResourceLoaded();
			Utils.TriggerEvent(onUpdateComplete);
			OnDownloadResourceFinish(true);
		}
	}

	private void OnGetServerList(string text, bool bSuccess)
	{
		bUpdateServerList = true;
		if (!bSuccess)
		{
			_serverVersionFile.code = _localVersionFile.code;
		}
		else
		{
			FileOperation.WirteToFile(ServerMgr.txt_address, text);
		}
		CheckUpdateComplete();
	}

	public void LoadText(string abPath, TextCallback callback)
	{
		Utils.StartConroutine(_LoadText(abPath, callback));
	}

	private static IEnumerator _LoadText(string abPath, TextCallback callback)
	{
		WWW www = new WWW(abPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (callback != null)
			{
				callback(www.text, true);
			}
		}
		else
		{
			Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
			callback(string.Empty, false);
		}
	}

	private void DeleteExistPath(string path)
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

	public static string GetFileMd5(byte[] bytes)
	{
		try
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
		catch (FileNotFoundException ex)
		{
			Console.WriteLine(ex.Message);
			return string.Empty;
		}
	}

	public void ServerListFileError()
	{
		if (_serverVersionFile != null)
		{
			try
			{
				File.Delete(Utils.GetFilePath(ServerMgr.txt_address));
			}
			catch (Exception)
			{
			}
			LoadText(Utils.GetStreamingAssetPathForWWW(_strVersionPath), LoadVersionCallback);
		}
	}

	private void LoadVersionCallback(string text, bool bSuccess)
	{
		if (bSuccess)
		{
			_localVersionFile = JsonUtility.FromJson<VersionFile>(text);
			_serverVersionFile.code = _localVersionFile.code;
			string text2 = JsonUtility.ToJson(_serverVersionFile);
			FileOperation.WirteToFile(_strVersionPath, text2);
		}
	}

	public void ToRepairClient()
	{
		DeleteExistPath(Utils.GetPersistentPath("res/"));
	}

	public void CheckFilesCorrent(Utils.VoidDelegate checkComplete)
	{
		Debug.LogError("to check  ");
		onCheckComplete = checkComplete;
		string abPath = ServerPath.ServerResPath + _strCheckMd5List + "?" + _serverVersionFile.strV;
		LoadText(abPath, GetCheckFileCallback);
	}

	private void GetCheckFileCallback(string text, bool bSuccess)
	{
		if (!bSuccess || string.IsNullOrEmpty(text))
		{
			Debug.LogError("get check filemd5 error");
			Utils.TriggerEvent(onCheckComplete);
			return;
		}
		try
		{
			_checkFileList = JsonUtility.FromJson<ResFileList>(text);
		}
		catch (Exception)
		{
			Debug.LogError("get check filemd5 filelist error");
			Utils.TriggerEvent(onCheckComplete);
			return;
		}
		CheckMd5();
	}

	private void LoadFileBin(string abPath, BinCallback callback)
	{
		Utils.StartConroutine(_LoadFileBin(abPath, callback));
	}

	private static IEnumerator _LoadFileBin(string abPath, BinCallback callback)
	{
		WWW www = new WWW(abPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (callback != null)
			{
				callback(www.bytes, true);
			}
		}
		else
		{
			Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
			callback(null, false);
		}
	}

	private void CheckMd5()
	{
		if (_checkFileList.files.Count == 0)
		{
			Debug.LogError("check md5 finish");
			return;
		}
		string filePathForWWW = Utils.GetFilePathForWWW(_checkFileList.files[_checkFileList.files.Count - 1].path);
		LoadFileBin(filePathForWWW, CheckOneFileMd5CallBack);
	}

	private void CheckOneFileMd5CallBack(byte[] text, bool bSuccess)
	{
		ResFileInfo resFileInfo = _checkFileList.files[_checkFileList.files.Count - 1];
		if (!bSuccess)
		{
			Debug.LogError(resFileInfo.path + " not in apk, can not play");
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CCheckOneFileMd5CallBack_003Em__0;
			}
			MessageBoxPanel.ShowConfirm(408, _003C_003Ef__am_0024cache0);
			return;
		}
		string fileMd = GetFileMd5(text);
		if (!resFileInfo.md5.Equals(fileMd, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogError(resFileInfo.path + " md5 error, can not play");
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003CCheckOneFileMd5CallBack_003Em__1;
			}
			MessageBoxPanel.ShowConfirm(408, _003C_003Ef__am_0024cache1);
		}
		else
		{
			_checkFileList.files.RemoveAt(_checkFileList.files.Count - 1);
			CheckMd5();
		}
	}

	private void OnDownloadResourceFinish(bool success)
	{
		if (updateStartTime > 0f)
		{
			updateFinishTime = Time.fixedTime;
			float num = updateFinishTime - updateStartTime;
			if (num <= 0f)
			{
				num = 1f;
			}
			else if (num > 300f)
			{
				num = Utils.Random(1, 300);
			}
			int status = (success ? 1 : 2);
			Singleton<PlatformMgr>.Ins.onUpdateGameResource(212, status, (int)num);
		}
	}

	[CompilerGenerated]
	private static void _003CCheckOneFileMd5CallBack_003Em__0()
	{
		Application.Quit();
	}

	[CompilerGenerated]
	private static void _003CCheckOneFileMd5CallBack_003Em__1()
	{
		Application.Quit();
	}
}

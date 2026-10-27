using System.IO;
using System.Runtime.CompilerServices;
using Share;
using UnityEngine;

internal class ServerPath
{
	public static string ServerVisionPath;

	public static string ServerFileListpath;

	public static string ServerResPath;

	public static string GsServerListAddr;

	public static string ImageUpLoadPath;

	public static string ImageDownLoadPath;

	public static string FeedbackPath;

	public static string GameLoadPath;

	public static string AuthServerAddress = "http://47.74.155.110:80/login";

	public static string UdpServerAddress;

	public static string FtpServerIp;

	public static string FtpUserName;

	public static string FtpUserPwd;

	private static string addressconfigPath = "gameconfig/addressconfig.txt";

	[CompilerGenerated]
	private static Utils.StringDelegate _003C_003Ef__mg_0024cache0;

	public static Coroutine LoadServerPath()
	{
		string filePathForWWW = Utils.GetFilePathForWWW(addressconfigPath);
		if (_003C_003Ef__mg_0024cache0 == null)
		{
			_003C_003Ef__mg_0024cache0 = LoadCallback;
		}
		return FileOperation.LoadText(filePathForWWW, _003C_003Ef__mg_0024cache0);
	}

	private static void LoadCallback(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			StringReader sr = new StringReader(text);
			Conf conf = new Conf(sr);
			ServerVisionPath = conf.getProperty("AndroidVersionAddr");
			ServerFileListpath = conf.getProperty("AndroidFileListAddr");
			ServerResPath = conf.getProperty("AndroidResAddr");
			GsServerListAddr = conf.getProperty("GsServerListAddr");
			FileOperation.CheckPathEnd(ref ServerResPath);
			ImageUpLoadPath = conf.getProperty("ImageUpLoadPath");
			ImageDownLoadPath = conf.getProperty("ImageDownLoadPath");
			FileOperation.CheckPathEnd(ref ImageDownLoadPath);
			FeedbackPath = conf.getProperty("FeedbackPath");
			GameLoadPath = conf.getProperty("AndroidGameLoadPath");
			AuthServerAddress = conf.getProperty("AuthServerAddress");
			UdpServerAddress = conf.getProperty("UpdServerAddress");
			FtpServerIp = conf.getProperty("uploadpath");
			FtpUserName = conf.getProperty("pathParam1");
			FtpUserPwd = conf.getProperty("pathParam2");
		}
	}
}

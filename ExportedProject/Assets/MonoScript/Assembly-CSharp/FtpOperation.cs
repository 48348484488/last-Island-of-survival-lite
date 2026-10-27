using System;
using System.IO;
using System.Net;
using UnityEngine;

public class FtpOperation
{
	private static void MakeDir(string dirName, string ftpHostIP, string username, string password)
	{
		try
		{
			string uRI = "ftp://" + ftpHostIP + dirName;
			FtpWebRequest request = GetRequest(uRI, username, password);
			request.Method = "MKD";
			FtpWebResponse ftpWebResponse = (FtpWebResponse)request.GetResponse();
			ftpWebResponse.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	private static FtpWebRequest GetRequest(string URI, string username, string password)
	{
		FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(URI);
		ftpWebRequest.Credentials = new NetworkCredential(username, password);
		ftpWebRequest.KeepAlive = false;
		return ftpWebRequest;
	}

	public static bool UploadFile(string filePath, string ftpServerIP, string ftpUserName, string ftpPassword, string deviceUniqueIdentifier)
	{
		if (ftpServerIP[ftpServerIP.Length - 1] != '/')
		{
			ftpServerIP += "/";
		}
		FileInfo fileInfo = new FileInfo(filePath);
		if (fileInfo == null || fileInfo.Length == 0)
		{
			Debug.LogError(filePath + " not exist");
			return false;
		}
		string text = DateTime.Now.ToString("yyyyMMdd");
		MakeDir(text, ftpServerIP, ftpUserName, ftpPassword);
		string uRI = "ftp://" + ftpServerIP + text + "/" + deviceUniqueIdentifier + "_" + DateTime.Now.Ticks + ".txt";
		FtpWebRequest request = GetRequest(uRI, ftpUserName, ftpPassword);
		request.Method = "STOR";
		request.ContentLength = fileInfo.Length;
		request.UsePassive = true;
		request.UseBinary = true;
		int num = 2048;
		byte[] buffer = new byte[num];
		int num2 = 0;
		FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		try
		{
			Stream requestStream = request.GetRequestStream();
			for (num2 = fileStream.Read(buffer, 0, num); num2 != 0; num2 = fileStream.Read(buffer, 0, num))
			{
				requestStream.Write(buffer, 0, num2);
			}
			requestStream.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			fileStream.Close();
			Debug.LogError(ex.ToString());
			return false;
		}
		Debug.LogError("finish upload log.txt");
		return true;
	}
}

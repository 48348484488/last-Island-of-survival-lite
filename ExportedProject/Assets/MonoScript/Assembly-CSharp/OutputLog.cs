using System;
using System.IO;
using System.Text;
using UnityEngine;

public class OutputLog : MonoBehaviour
{
	public static OutputLog Ins;

	private readonly BetterList<string> mLogsToWrite = new BetterList<string>();

	private StreamWriter mWriter;

	private void Start()
	{
		Ins = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		try
		{
			string empty = string.Empty;
			empty = Application.persistentDataPath + "/hero_log.txt";
			FileInfo fileInfo = new FileInfo(empty);
			if (fileInfo.Exists && fileInfo.Length > 5242880)
			{
				try
				{
					File.Delete(empty);
				}
				catch (Exception)
				{
				}
			}
			mWriter = new StreamWriter(empty, true, Encoding.UTF8);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		Application.logMessageReceived += LogHandler;
	}

	private void Update()
	{
		if (mWriter != null && mLogsToWrite.size > 0)
		{
			for (int num = mLogsToWrite.size - 1; num >= 0; num--)
			{
				string value = mLogsToWrite[num];
				mWriter.WriteLine(value);
			}
			mLogsToWrite.Clear();
			mWriter.Flush();
		}
	}

	private void OnDestroy()
	{
		if (mWriter != null)
		{
			mWriter.Close();
			mWriter = null;
		}
		Ins = null;
	}

	private void LogHandler(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			mLogsToWrite.Add(string.Concat("[", DateTime.Now, "]", logString));
			if (!string.IsNullOrEmpty(stackTrace))
			{
				mLogsToWrite.Add(stackTrace);
			}
			Flush();
		}
	}

	public void Flush()
	{
		Update();
	}
}

using System.Collections;
using Share;
using UnityEngine;

public class HttpUtil
{
	public delegate void OnHttpRequestDone(byte[] response);

	public static void Post(string url, Marshal mar, OnHttpRequestDone callback)
	{
		Utils.StartConroutine(doPost(url, mar, callback));
	}

	public static void Post(string url, byte[] raw, OnHttpRequestDone callback)
	{
		Utils.StartConroutine(doPost(url, raw, callback));
	}

	private static IEnumerator doPost(string url, Marshal mar, OnHttpRequestDone callback)
	{
		Octets octects = new Octets();
		octects.push(mar);
		WWW www = new WWW(url, octects.getBytes());
		yield return www;
		if (www.error != null)
		{
			callback(null);
		}
		else
		{
			callback(www.bytes);
		}
	}

	private static IEnumerator doPost(string url, byte[] raw, OnHttpRequestDone callback)
	{
		WWW www = new WWW(url, raw);
		yield return www;
		if (www.error != null)
		{
			callback(null);
		}
		else
		{
			callback(www.bytes);
		}
	}

	public static void Get(string url, OnHttpRequestDone callback)
	{
		Utils.StartConroutine(doGet(url, callback));
	}

	private static IEnumerator doGet(string url, OnHttpRequestDone callback)
	{
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
			callback(null);
		}
		else
		{
			callback(www.bytes);
		}
	}
}

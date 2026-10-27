using System;
using System.Collections.Generic;
using UnityEngine;

internal class VersionFile
{
	public string strV;

	public int code;

	public string codeVersion;

	public List<ChannelUpdate> channels;

	public static bool operator <(VersionFile a, VersionFile b)
	{
		if (a.code < b.code)
		{
			return true;
		}
		if (a.code > b.code)
		{
			return false;
		}
		return Version.Compare(a.strV, b.strV) < 0;
	}

	public static bool operator >(VersionFile a, VersionFile b)
	{
		if (b.code < a.code)
		{
			return true;
		}
		if (b.code > a.code)
		{
			return false;
		}
		return Version.Compare(b.strV, a.strV) < 0;
	}

	public bool NeedForceUpdate(string channelName)
	{
		if (channels != null)
		{
			foreach (ChannelUpdate channel in channels)
			{
				if (channel.channel.Equals(channelName, StringComparison.OrdinalIgnoreCase))
				{
					if (channel.forceUpdateApkVer.CompareTo(Application.version) > 0)
					{
						Debug.LogError("channel " + channelName + " need forceupdate");
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}
}

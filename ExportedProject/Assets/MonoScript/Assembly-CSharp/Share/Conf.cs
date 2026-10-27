using System;
using System.Collections.Generic;
using System.IO;

namespace Share
{
	public sealed class Conf
	{
		private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

		private readonly Dictionary<string, Dictionary<string, string>> groups = new Dictionary<string, Dictionary<string, string>>();

		public Conf(string filePath)
		{
			FileStream fileStream = new FileStream(filePath, FileMode.Open);
			StreamReader sr = new StreamReader(fileStream);
			Parse(sr);
			fileStream.Close();
		}

		public Conf(StreamReader sr)
		{
			Parse(sr);
		}

		public Conf(StringReader sr)
		{
			Parse(sr);
		}

		private void Parse(StreamReader sr)
		{
			string text = null;
			string text2 = null;
			while ((text2 = sr.ReadLine()) != null)
			{
				text2 = text2.Trim();
				if (text2.Equals(string.Empty) || text2.StartsWith("#"))
				{
					continue;
				}
				if (text2.StartsWith("[") && text2.EndsWith("]"))
				{
					text = text2.Substring(1, text2.Length - 2);
					continue;
				}
				int num = text2.IndexOf('=');
				if (num <= 0 || num == text2.Length - 1)
				{
					throw new Exception("wrong line: " + text2);
				}
				string key = text2.Substring(0, num).Trim();
				string value = text2.Substring(num + 1).Trim();
				if (text != null)
				{
					Dictionary<string, string> dictionary = null;
					if (!groups.ContainsKey(text))
					{
						dictionary = new Dictionary<string, string>();
						groups.Add(text, dictionary);
					}
					else
					{
						dictionary = groups[text];
					}
					dictionary.Add(key, value);
				}
				else
				{
					properties.Add(key, value);
				}
			}
			sr.Close();
		}

		private void Parse(StringReader sr)
		{
			string text = null;
			string text2 = null;
			while ((text2 = sr.ReadLine()) != null)
			{
				text2 = text2.Trim();
				if (text2.Equals(string.Empty) || text2.StartsWith("#"))
				{
					continue;
				}
				if (text2.StartsWith("[") && text2.EndsWith("]"))
				{
					text = text2.Substring(1, text2.Length - 2);
					continue;
				}
				int num = text2.IndexOf('=');
				if (num <= 0 || num == text2.Length - 1)
				{
					throw new Exception("wrong line: " + text2);
				}
				string key = text2.Substring(0, num).Trim();
				string value = text2.Substring(num + 1).Trim();
				if (text != null)
				{
					Dictionary<string, string> dictionary = null;
					if (!groups.ContainsKey(text))
					{
						dictionary = new Dictionary<string, string>();
						groups.Add(text, dictionary);
					}
					else
					{
						dictionary = groups[text];
					}
					dictionary.Add(key, value);
				}
				else
				{
					properties.Add(key, value);
				}
			}
			sr.Close();
		}

		public string getProperty(string name)
		{
			if (properties.ContainsKey(name))
			{
				return properties[name];
			}
			return string.Empty;
		}

		public string getProperty(string group, string name)
		{
			Dictionary<string, string> dictionary = groups[group];
			return (dictionary != null) ? dictionary[name] : null;
		}

		public int getIntProperty(string group, string name)
		{
			return Convert.ToInt32(getProperty(group, name));
		}

		public long getLongProperty(string group, string name)
		{
			return Convert.ToInt64(getProperty(group, name));
		}

		public float getFloatProperty(string group, string name)
		{
			return (float)Convert.ToDouble(getProperty(group, name));
		}

		public bool getBooleanProperty(string group, string name)
		{
			return Convert.ToBoolean(getProperty(group, name));
		}

		public int getIntProperty(string group, string name, int defaultValue)
		{
			try
			{
				return Convert.ToInt32(getProperty(group, name));
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return defaultValue;
			}
		}

		public long getLongProperty(string group, string name, long defaultValue)
		{
			try
			{
				return Convert.ToInt64(getProperty(group, name));
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return defaultValue;
			}
		}

		public float getFloatProperty(string group, string name, float defaultValue)
		{
			try
			{
				return (float)Convert.ToDouble(getProperty(group, name));
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return defaultValue;
			}
		}

		public bool getBooleanProperty(string group, string name, bool defaultValue)
		{
			try
			{
				return Convert.ToBoolean(getProperty(group, name));
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return defaultValue;
			}
		}
	}
}

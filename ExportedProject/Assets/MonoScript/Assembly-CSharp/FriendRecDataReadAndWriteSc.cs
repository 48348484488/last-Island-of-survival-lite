using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FriendRecDataReadAndWriteSc : Singleton<FriendRecDataReadAndWriteSc>
{
	public static readonly string PathURL = "jar:file://" + Application.dataPath + "!/assets/";

	public void CreateTextFile(string fileName, string strFileData, bool isEncryption, bool isCover)
	{
		string text = ((!isEncryption) ? strFileData : Encrypt(strFileData));
		Singleton<FriendRecDataSc>.Ins.AddNews(text);
		StreamWriter streamWriter = ((!File.Exists(fileName) || isCover) ? File.CreateText(fileName) : File.AppendText(fileName));
		streamWriter.WriteLine(text);
		streamWriter.Close();
	}

	public void ClearTextFile(string fileName)
	{
		File.Delete(fileName);
	}

	public void LoadTextFile(string fileName, bool isEncryption)
	{
		if (!File.Exists(fileName))
		{
			File.CreateText(fileName).Close();
			return;
		}
		StreamReader streamReader = File.OpenText(fileName);
		int num = 0;
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			num++;
			Singleton<FriendRecDataSc>.Ins.AddNews((!isEncryption) ? text : Decrypt(text));
		}
		streamReader.Close();
		if (num >= 400)
		{
			RewriteTextFile(fileName);
		}
	}

	private void RewriteTextFile(string fileName)
	{
		if (Singleton<FriendRecDataSc>.Ins.GetNewsCount() > 0)
		{
			StreamWriter streamWriter = File.CreateText(fileName);
			for (int num = Singleton<FriendRecDataSc>.Ins.GetNewsCount(); num >= 0; num--)
			{
				streamWriter.WriteLine(Singleton<FriendRecDataSc>.Ins.GetNews(num));
			}
			streamWriter.Close();
		}
	}

	public string Encrypt(string toE)
	{
		byte[] bytes = Encoding.UTF8.GetBytes("12348578902223367877623456789012");
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] bytes2 = Encoding.UTF8.GetBytes(toE);
		byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public string Decrypt(string toD)
	{
		byte[] bytes = Encoding.UTF8.GetBytes("12348578902223367877623456789012");
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] array = Convert.FromBase64String(toD);
		byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes2);
	}
}

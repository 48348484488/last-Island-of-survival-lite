using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ResMgr : MonoBehaviour
{
	public delegate void ABCallback(AssetBundle ab);

	public delegate void TextCallback(string text);

	public delegate void Texture2DCallback(Texture2D texture);

	public delegate void SpriteCallback(Sprite texture);

	private sealed class ABRef : MonoBehaviour
	{
		[SerializeField]
		private readonly List<string> abPathList = new List<string>(2);

		private void Awake()
		{
			if (abPathList.Count > 0)
			{
				int i = 0;
				for (int count = abPathList.Count; i < count; i++)
				{
					AddABRef(abPathList[i]);
				}
			}
		}

		private void OnDestroy()
		{
			if (abPathList.Count > 0)
			{
				int i = 0;
				for (int count = abPathList.Count; i < count; i++)
				{
					ReduceABRef(abPathList[i]);
				}
				abPathList.Clear();
			}
		}

		public void SetABPath(string path)
		{
			if (!string.IsNullOrEmpty(path) && !abPathList.Contains(path) && AddABRef(path))
			{
				abPathList.Add(path);
			}
		}

		private bool ReduceABRef(string abPath)
		{
			ABInfo aBInfo = Ins.FindABInfo(abPath);
			if (aBInfo != null)
			{
				aBInfo.refCount--;
				return true;
			}
			return false;
		}

		private bool AddABRef(string abPath)
		{
			ABInfo aBInfo = Ins.FindABInfo(abPath);
			if (aBInfo != null)
			{
				aBInfo.refCount++;
				return true;
			}
			return false;
		}
	}

	private sealed class ABInfo
	{
		public string path;

		public AssetBundle ab;

		public bool canUnload = true;

		public int refCount;
	}

	public static ResMgr Ins;

	private const int AssetBundleCacheSize = 5;

	private string mStreamingAssetPathForWWW = string.Empty;

	private readonly LRU<string, ABInfo> mAssetBundles = new LRU<string, ABInfo>(5);

	private readonly HashSet<string> mLoading = new HashSet<string>();

	public static void Init()
	{
		GameObject gameObject = new GameObject("ResMgr");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		Ins = gameObject.AddComponent<ResMgr>();
	}

	private void Awake()
	{
		mStreamingAssetPathForWWW = Utils.GetStreamingAssetPathForWWW(string.Empty);
		mAssetBundles.onRemoveEntry = OnCacheOverflow;
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public Coroutine LoadAB(string abPath, ABCallback callback = null, bool canAutoUnload = true)
	{
		return StartCoroutine(_LoadAB(abPath, callback, canAutoUnload, false));
	}

	public Coroutine LoadAssetFromAB<T>(string abPath, string resName, GameObject go, Utils.UnityObjectDelegate callback = null)
	{
		return StartCoroutine(_LoadAssetFromAB(abPath, resName, go, typeof(T), callback));
	}

	public Coroutine CreateFromAB(string abPath, string resName, Utils.GameObjectDelegate callback = null)
	{
		return StartCoroutine(_CreateFromAB(abPath, resName, callback));
	}

	public Coroutine UpLoadPic(string name, byte[] bytes, Utils.VoidDelegate callback = null, Utils.VoidDelegate failCallback = null)
	{
		return StartCoroutine(_UpdateLoad(name, bytes, callback, failCallback));
	}

	public void AddRef(GameObject go, string abPath)
	{
		abPath = abPath.ToLower();
		ABRef aBRef = go.GetComponent<ABRef>();
		if (!aBRef)
		{
			aBRef = go.AddComponent<ABRef>();
		}
		aBRef.SetABPath(abPath);
	}

	public void CleanRef(GameObject go)
	{
		ABRef component = go.GetComponent<ABRef>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	public void CleanCahce()
	{
		List<ABInfo> values = mAssetBundles.GetValues();
		foreach (ABInfo item in values)
		{
			if (item.canUnload && item.refCount <= 0 && !mLoading.Contains(item.path))
			{
				item.ab.Unload(true);
				mAssetBundles.Remove(item.path);
			}
		}
		GC.Collect();
	}

	public void CleanUp()
	{
		StopAllCoroutines();
		mLoading.Clear();
		List<ABInfo> values = mAssetBundles.GetValues();
		foreach (ABInfo item in values)
		{
			if (item.canUnload)
			{
				item.ab.Unload(true);
				mAssetBundles.Remove(item.path);
			}
		}
		GC.Collect();
	}

	public string Dump()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<ABInfo> values = mAssetBundles.GetValues();
		foreach (ABInfo item in values)
		{
			stringBuilder.Append("{");
			stringBuilder.Append(item.path);
			stringBuilder.Append(":");
			stringBuilder.Append(item.refCount);
			stringBuilder.Append("}");
		}
		stringBuilder.Append("loading{");
		foreach (string item2 in mLoading)
		{
			stringBuilder.Append(item2);
			stringBuilder.Append(",");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private IEnumerator _LoadAB(string abPath, ABCallback callback, bool canUnload, bool addRef)
	{
		ABInfo abInfo2;
		if (mAssetBundles.TryGetValue(abPath, out abInfo2))
		{
			abInfo2.canUnload = abInfo2.canUnload && canUnload;
			if (addRef)
			{
				abInfo2.refCount++;
			}
			if (callback != null)
			{
				callback(abInfo2.ab);
			}
			yield break;
		}
		if (mLoading.Contains(abPath))
		{
			while (mLoading.Contains(abPath))
			{
				yield return null;
			}
			if (mAssetBundles.TryGetValue(abPath, out abInfo2))
			{
				abInfo2.canUnload = abInfo2.canUnload && canUnload;
				if (addRef)
				{
					abInfo2.refCount++;
				}
				if (callback != null)
				{
					callback(abInfo2.ab);
				}
			}
			yield break;
		}
		mLoading.Add(abPath);
		abInfo2 = new ABInfo
		{
			path = abPath,
			canUnload = canUnload
		};
		if (addRef)
		{
			abInfo2.refCount++;
		}
		AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync(Utils.GetFilePath(abPath));
		yield return assetRequest;
		mLoading.Remove(abPath);
		if (assetRequest.isDone && (bool)assetRequest.assetBundle)
		{
			abInfo2.ab = assetRequest.assetBundle;
			mAssetBundles.Set(abPath, abInfo2);
			if (callback != null)
			{
				callback(abInfo2.ab);
			}
		}
		else
		{
			Debug.LogError("ResMgr load " + abPath + " failed.");
		}
	}

	public Coroutine LoadTextLocal(string abPath, TextCallback callback)
	{
		string abPath2 = ((!File.Exists(Utils.GetPersistentPath("res/" + abPath))) ? Utils.GetStreamingAssetPathForWWW(abPath) : Utils.GetPersistentPathForWWW("res/" + abPath));
		return StartCoroutine(_LoadText(abPath2, callback));
	}

	public Coroutine LoadTextServer(string abPath, TextCallback callback)
	{
		return StartCoroutine(_LoadText(abPath, callback));
	}

	private IEnumerator _LoadText(string abPath, TextCallback callback)
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

	private void GetTextureFromServer(string abPath, int vision, Texture2DCallback callback)
	{
		WWW wWW = WWW.LoadFromCacheOrDownload(abPath, vision);
		if (wWW.texture != null)
		{
			callback(wWW.texture);
		}
	}

	private IEnumerator _LoadAssetFromAB(string abPath, string resName, GameObject go, Type type, Utils.UnityObjectDelegate callback)
	{
		CorrectPath(ref abPath, ref resName);
		yield return Utils.StartConroutine(_LoadAB(abPath, null, true, true));
		ABInfo abInfo;
		if (!mAssetBundles.TryGetValue(abPath, out abInfo))
		{
			yield break;
		}
		AssetBundleRequest assetRequest2 = abInfo.ab.LoadAssetAsync(resName, type);
		yield return assetRequest2;
		abInfo.refCount--;
		UnityEngine.Object asset = null;
		if (assetRequest2.isDone)
		{
			asset = assetRequest2.asset;
		}
		if (asset == null)
		{
			if (type != typeof(Mesh))
			{
				Debug.LogError("ResMgr load res " + resName + " from " + abPath + " failed");
				yield break;
			}
			abInfo.refCount++;
			assetRequest2 = abInfo.ab.LoadAllAssetsAsync<Mesh>();
			yield return assetRequest2;
			abInfo.refCount--;
			if (assetRequest2.allAssets != null && assetRequest2.allAssets.Length > 0)
			{
				int i = 0;
				for (int num = assetRequest2.allAssets.Length; i < num; i++)
				{
					if (assetRequest2.allAssets[i].name == resName)
					{
						asset = assetRequest2.allAssets[i];
						break;
					}
				}
			}
			if (asset == null)
			{
				Debug.LogError("ResMgr load res " + resName + " from " + abPath + " failed");
				yield break;
			}
		}
		if (!go && asset is GameObject)
		{
			go = asset as GameObject;
		}
		if ((bool)go)
		{
			ABRef aBRef = go.GetComponent<ABRef>();
			if (!aBRef)
			{
				aBRef = go.AddComponent<ABRef>();
			}
			aBRef.SetABPath(abPath);
		}
		if (callback != null)
		{
			callback(asset);
		}
	}

	private IEnumerator _CreateFromAB(string abPath, string resName, Utils.GameObjectDelegate callback)
	{
		CorrectPath(ref abPath, ref resName);
		yield return Utils.StartConroutine(_LoadAB(abPath, null, true, true));
		ABInfo abInfo;
		if (!mAssetBundles.TryGetValue(abPath, out abInfo))
		{
			yield break;
		}
		AssetBundleRequest assetRequeat = abInfo.ab.LoadAssetAsync(resName, typeof(GameObject));
		yield return assetRequeat;
		abInfo.refCount--;
		GameObject go2 = null;
		if (assetRequeat.isDone)
		{
			go2 = assetRequeat.asset as GameObject;
		}
		if (!go2)
		{
			Debug.LogError("ResMgr create res " + resName + " from " + abPath + " failed:");
		}
		else
		{
			go2 = UnityEngine.Object.Instantiate(go2);
			ABRef abRef = go2.AddComponent<ABRef>();
			abRef.SetABPath(abPath);
			if (callback != null)
			{
				callback(go2);
			}
		}
	}

	private bool OnCacheOverflow(string abPath, ABInfo abInfo)
	{
		if (abInfo == null)
		{
			return false;
		}
		if (!abInfo.canUnload)
		{
			return false;
		}
		if (abInfo.refCount > 0)
		{
			return false;
		}
		if (mLoading.Contains(abPath))
		{
			return false;
		}
		abInfo.ab.Unload(true);
		abInfo.ab = null;
		return true;
	}

	private ABInfo FindABInfo(string abPath)
	{
		ABInfo value;
		mAssetBundles.TryGetValue(abPath, out value);
		return value;
	}

	private string GetAssetBundlePathForWWW(string abPath)
	{
		return mStreamingAssetPathForWWW + abPath;
	}

	private static void CorrectPath(ref string abPath, ref string resName)
	{
		abPath = abPath.ToLower();
		if (!string.IsNullOrEmpty(resName))
		{
			return;
		}
		int num = abPath.LastIndexOf('/');
		if (num >= 0)
		{
			int num2 = abPath.LastIndexOf(".");
			if (num2 < 0)
			{
				resName = abPath.Substring(num + 1);
				abPath = abPath.Substring(0, num) + ".ab";
			}
			else
			{
				resName = abPath.Substring(num + 1, num2 - num - 1);
			}
		}
	}

	public void GetTextureFromServer(long roleId, string vision, Texture2DCallback callback)
	{
		string textureName = vision + ".jpg";
		GetTextureFromServer("photofile/", textureName, callback);
	}

	public void GetHeadFromServer(long roleId, string vision, Texture2DCallback callback)
	{
		string textureName = "_head_" + vision + ".jpg";
		GetTextureFromServer("headfile/", textureName, callback);
	}

	public void GetTextureFromServer(string folder, string textureName, Texture2DCallback callback)
	{
		if (!Directory.Exists(Utils.GetPersistentPath(folder)))
		{
			Directory.CreateDirectory(Utils.GetPersistentPath(folder));
		}
		string persistentPath = Utils.GetPersistentPath(folder + textureName);
		persistentPath = ((!File.Exists(persistentPath)) ? (ServerPath.ImageDownLoadPath + folder + textureName) : Utils.GetPersistentPathForWWW(folder + textureName));
		StartCoroutine(_LoadTexture(persistentPath, folder, textureName, callback));
	}

	private IEnumerator _LoadTexture(string abPath, string folder, string textureName, Texture2DCallback callback)
	{
		WWW www = new WWW(abPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (callback != null)
			{
				callback(www.texture);
				byte[] bytes = www.bytes;
				string persistentPath = Utils.GetPersistentPath(folder + textureName);
				Stream stream = new FileStream(persistentPath, FileMode.OpenOrCreate);
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				binaryWriter.Write(bytes);
				binaryWriter.Close();
				stream.Close();
			}
		}
		else
		{
			Debug.LogError("ResMgr load " + abPath + " failed:" + www.error);
		}
	}

	private IEnumerator _UpdateLoad(string name, byte[] bytes, Utils.VoidDelegate callback, Utils.VoidDelegate failCallback)
	{
		string content = "name=" + name + "&data=" + Convert.ToBase64String(bytes);
		WWW www = new WWW(ServerPath.ImageUpLoadPath, Encoding.UTF8.GetBytes(content));
		yield return www;
		if (www.error != null)
		{
			failCallback();
			yield break;
		}
		string @string = Encoding.UTF8.GetString(www.bytes);
		if (@string.Equals("SUCCESS"))
		{
			callback();
		}
		else
		{
			failCallback();
		}
	}
}

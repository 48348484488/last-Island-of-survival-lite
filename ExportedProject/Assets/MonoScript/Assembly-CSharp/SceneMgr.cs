using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : SingletonMono<SceneMgr>
{
	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadSceneCoroutine(sceneName));
	}

	public IEnumerator LoadSceneCoroutine(string sceneName)
	{
		WWW www = new WWW(Utils.GetStreamingAssetPathForWWW("scene/" + sceneName + ".ab"));
		yield return www;
		if (www.error != null)
		{
			Debug.LogError("加载失败" + sceneName + "==" + www.error);
		}
		else
		{
			AssetBundle bundle = www.assetBundle;
			yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			bundle.Unload(false);
		}
		www.Dispose();
	}
}

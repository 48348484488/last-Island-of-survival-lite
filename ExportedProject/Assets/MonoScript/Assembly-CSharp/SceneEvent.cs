using UnityEngine.SceneManagement;

public class SceneEvent
{
	public delegate void SceneCallBack(Scene scene);

	public static Utils.VoidDelegate InitLoadSceneFinish;

	public static Utils.FloatDelegate LoadSceneProcess;

	public static SceneCallBack SceneLoadFinished;
}

using Share;
using UnityEngine;

public abstract class SceneObject
{
	public static readonly Vector3 HidePos = new Vector3(0f, -6000f, 0f);

	public Vector3 Pos = Vector3.zero;

	public Vector3 Scale = Vector3.one;

	public Vector3 Angles = Vector3.zero;

	public string PrefabName;

	public ObjectHolder Holder;

	public string sceneName;

	public abstract void Recycle();

	public abstract void AddToScene(SmallScene scene);

	public abstract void RemoveFromScene(SmallScene scene);

	public virtual void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		Pos.x = oc.pop_float();
		Pos.y = oc.pop_float();
		Pos.z = oc.pop_float();
		Scale.x = oc.pop_float();
		Scale.y = oc.pop_float();
		Scale.z = oc.pop_float();
		Angles.x = oc.pop_float();
		Angles.y = oc.pop_float();
		Angles.z = oc.pop_float();
		int i = oc.pop_short();
		PrefabName = prefabNames[i];
	}
}

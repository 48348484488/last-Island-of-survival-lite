using UnityEngine;
using UnityEngine.UI;

public class WorldData : MonoBehaviour
{
	public static WorldData Ins;

	public Camera BattleCamera;

	public RectTransform Background;

	public RectTransform DeadRoot;

	public RectTransform LingdiRoot;

	public RectTransform SelfLingdiRoot;

	public RectTransform PhotoRoot;

	public RectTransform SelfPhotoRoot;

	public RectTransform StepRoot;

	public RectTransform FootprintRoot;

	public RectTransform PlayerRoot;

	public RectTransform PoolRoot;

	public RectTransform EffectRoot;

	public RectTransform WorldParent;

	public Image BlackMask;

	private void Awake()
	{
		GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
		Ins = this;
		BlackMask = new GameObject("BlackMask").AddComponent<Image>();
		BlackMask.color = new Color32(0, 0, 0, 0);
		BlackMask.gameObject.transform.SetParent(base.transform);
		BlackMask.gameObject.AddComponent<Canvas>().overrideSorting = true;
		BlackMask.gameObject.GetComponent<Canvas>().sortingOrder = 1000;
	}

	private void OnDestroy()
	{
		Ins = null;
	}
}

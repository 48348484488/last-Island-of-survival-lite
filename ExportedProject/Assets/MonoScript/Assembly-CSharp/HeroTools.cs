using UnityEngine;
using UnityEngine.UI;

public class HeroTools : MonoBehaviour
{
	public static RenderTexture CreatShowHero(Camera camera, GameObject herotex, int antiAliasing = 1)
	{
		RectTransform rectTransform = herotex.transform as RectTransform;
		int width = (int)rectTransform.sizeDelta.x;
		int height = (int)rectTransform.sizeDelta.y;
		RenderTexture renderTexture = new RenderTexture(width, height, 1);
		renderTexture.name = "herotex";
		renderTexture.antiAliasing = antiAliasing;
		renderTexture.anisoLevel = 1;
		herotex.GetComponent<RawImage>().texture = renderTexture;
		camera.targetTexture = renderTexture;
		return renderTexture;
	}
}

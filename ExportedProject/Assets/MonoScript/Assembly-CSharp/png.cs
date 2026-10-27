using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class png : MonoBehaviour
{
	public string folder = "RolePng";

	public int frameRate = 25;

	public float Times = 1f;

	private float frameCount = 100f;

	public int _Width = 128;

	public int _Height = 128;

	private Vector3 cameraPosition = Vector3.zero;

	private Vector3 cameraRotation = Vector3.zero;

	private string realFolder = string.Empty;

	private float originaltimescaleTime;

	private float currentTime;

	private bool over;

	private int currentIndex;

	private Camera exportCamera;

	public void Start()
	{
		frameCount = (float)frameRate * Times;
		Time.captureFramerate = frameRate;
		realFolder = Application.dataPath + "/" + Path.Combine(folder, base.name);
		Debug.LogError(realFolder);
		if (!Directory.Exists(realFolder))
		{
			Directory.CreateDirectory(realFolder);
		}
		originaltimescaleTime = Time.timeScale;
		GameObject gameObject = Camera.main.gameObject;
		if (cameraPosition != Vector3.zero)
		{
			gameObject.transform.position = cameraPosition;
		}
		if (cameraRotation != Vector3.zero)
		{
			gameObject.transform.rotation = Quaternion.Euler(cameraRotation);
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		exportCamera = gameObject2.GetComponent<Camera>();
		currentTime = 0f;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (!over && (float)currentIndex >= frameCount)
		{
			over = true;
			Cleanup();
			Debug.Log("Finish");
		}
		else
		{
			StartCoroutine(CaptureFrame());
		}
	}

	private void Cleanup()
	{
		UnityEngine.Object.DestroyImmediate(exportCamera);
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	private IEnumerator CaptureFrame()
	{
		Time.timeScale = 0f;
		yield return new WaitForEndOfFrame();
		string filename = string.Format("{0}/{1:D04}.png", realFolder, ++currentIndex);
		Debug.Log(filename);
		int width = Screen.width;
		int height = Screen.height;
		RenderTexture blackCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
		RenderTexture whiteCamRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
		exportCamera.targetTexture = blackCamRenderTexture;
		exportCamera.backgroundColor = Color.black;
		exportCamera.Render();
		RenderTexture.active = blackCamRenderTexture;
		Texture2D texb3 = GetTex2D();
		exportCamera.targetTexture = whiteCamRenderTexture;
		exportCamera.backgroundColor = Color.white;
		exportCamera.Render();
		RenderTexture.active = whiteCamRenderTexture;
		Texture2D texw = GetTex2D();
		if (!texb3)
		{
			yield break;
		}
		Texture2D texture2D = new Texture2D(_Width, _Height, TextureFormat.ARGB32, false);
		for (int i = 0; i < texture2D.height; i++)
		{
			for (int j = 0; j < texture2D.width; j++)
			{
				float num = texw.GetPixel(j, i).r - texb3.GetPixel(j, i).r;
				num = 1f - num;
				Color color = ((num != 0f) ? texb3.GetPixel(j, i) : Color.clear);
				color.a = num;
				texture2D.SetPixel(j, i, color);
			}
		}
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(filename, bytes);
		RenderTexture.active = null;
		UnityEngine.Object.DestroyImmediate(texture2D);
		UnityEngine.Object.DestroyImmediate(blackCamRenderTexture);
		blackCamRenderTexture = null;
		UnityEngine.Object.DestroyImmediate(texb3);
		texb3 = null;
		texb3 = null;
		GC.Collect();
		Time.timeScale = originaltimescaleTime;
	}

	private Texture2D GetTex2D()
	{
		int width = Screen.width;
		int height = Screen.height;
		Texture2D texture2D = new Texture2D(_Width, _Height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, _Width, _Height), 0, 0);
		texture2D.Apply();
		return texture2D;
	}
}

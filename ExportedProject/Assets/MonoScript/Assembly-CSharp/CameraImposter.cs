using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraImposter : MonoBehaviour
{
	private RenderTexture texture;

	public int size = 4096;

	public int antialiasing = 2;

	public Transform usercamera;

	private List<Renderer> renderers;

	private Camera renderingCamera;

	private bool toImposter;

	private void Start()
	{
		renderers = new List<Renderer>();
		texture = new RenderTexture(size, size, 16);
		texture.antiAliasing = antialiasing;
		if (usercamera == null)
		{
			usercamera = Camera.main.transform;
		}
		renderingCamera = usercamera.gameObject.GetComponent<Camera>();
		GameObject[] array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Renderer component = gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				renderers.Add(component);
			}
		}
	}

	private void Update()
	{
		if (toImposter)
		{
			renderingCamera.targetTexture = texture;
			renderingCamera.Render();
			renderingCamera.targetTexture = null;
			SavePng();
			toImposter = false;
		}
	}

	private void OnGUI()
	{
		if (GUILayout.Button("拍 照", GUILayout.Width(100f), GUILayout.Height(50f)))
		{
			toImposter = true;
		}
	}

	private void SavePng()
	{
		int width = texture.width;
		int height = texture.height;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
		RenderTexture.active = texture;
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		string text = "Assets/Scenes/TerrainScenes/";
		int num = 0;
		string path;
		while (true)
		{
			path = text + num + ".png";
			if (File.Exists(path))
			{
				num++;
				continue;
			}
			break;
		}
		File.WriteAllBytes(path, bytes);
	}
}

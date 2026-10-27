using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OutlineFilter : MonoBehaviour
{
	private static OutlineFilter Ins;

	[Header("Post Process")]
	public Color m_Color = Color.green;

	[Range(0.2f, 2.2f)]
	public float m_BlurPixelOffset = 1.2f;

	[Range(1f, 6f)]
	public float m_GlowIntensity = 3f;

	public string OutlineLayerName = "Outline";

	[Header("Internal")]
	private Shader m_SdrGlowFlatColor;

	private Material m_MatGlowHalo;

	private Material m_MatGlowBlur;

	private Camera m_Camera;

	private Camera m_RTCam;

	private int m_OutlineLayer;

	private int currentSelectedOldLayer;

	private GameObject currentSelected;

	public static void Select(GameObject go, Color selectedColor)
	{
		CancelSelect();
		if (go != null)
		{
			Ins.currentSelectedOldLayer = go.layer;
			Ins.currentSelected = go;
			Ins.m_Color = selectedColor;
			go.layer = Ins.m_OutlineLayer;
		}
	}

	public static void CancelSelect()
	{
		if (Ins.currentSelected != null)
		{
			Ins.currentSelected.layer = Ins.currentSelectedOldLayer;
			Ins.currentSelected = null;
		}
	}

	private void Start()
	{
		Ins = this;
		m_Camera = GetComponent<Camera>();
		m_RTCam = new GameObject().AddComponent<Camera>();
		m_RTCam.name = "RTCam";
		m_RTCam.transform.parent = m_Camera.gameObject.transform;
		m_RTCam.enabled = false;
		m_OutlineLayer = LayerMask.NameToLayer(OutlineLayerName);
	}

	private void OnEnable()
	{
		m_SdrGlowFlatColor = Shader.Find("Selected Effect --- Outline/Post Process/Flat Color");
		Shader shader = Shader.Find("Selected Effect --- Outline/Post Process/Halo");
		m_MatGlowHalo = new Material(shader);
		shader = Shader.Find("Selected Effect --- Outline/Post Process/Blur");
		m_MatGlowBlur = new Material(shader);
	}

	private void OnDisable()
	{
		if ((bool)m_MatGlowHalo)
		{
			Object.DestroyImmediate(m_MatGlowHalo);
			m_MatGlowHalo = null;
		}
		if ((bool)m_MatGlowBlur)
		{
			Object.DestroyImmediate(m_MatGlowBlur);
			m_MatGlowBlur = null;
		}
	}

	private void DoBlurPass(RenderTexture input, RenderTexture output, bool vertical)
	{
		if (vertical)
		{
			m_MatGlowBlur.SetVector("_Offsets", new Vector4(0f, m_BlurPixelOffset, 0f, 0f));
			Graphics.Blit(input, output, m_MatGlowBlur);
		}
		else
		{
			m_MatGlowBlur.SetVector("_Offsets", new Vector4(m_BlurPixelOffset, 0f, 0f, 0f));
			Graphics.Blit(input, output, m_MatGlowBlur);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, dst);
		if (!(Ins.currentSelected == null))
		{
			m_RTCam.CopyFrom(m_Camera);
			m_RTCam.clearFlags = CameraClearFlags.Nothing;
			RenderTexture temporary = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.R8);
			Graphics.Blit(Texture2D.blackTexture, temporary);
			m_RTCam.cullingMask = 1 << m_OutlineLayer;
			m_RTCam.SetTargetBuffers(temporary.colorBuffer, src.depthBuffer);
			m_RTCam.RenderWithShader(m_SdrGlowFlatColor, "RenderType");
			RenderTexture temporary2 = RenderTexture.GetTemporary(Screen.width / 4, Screen.height / 4, 0);
			RenderTexture temporary3 = RenderTexture.GetTemporary(Screen.width / 4, Screen.height / 4, 0);
			DoBlurPass(temporary, temporary2, true);
			DoBlurPass(temporary2, temporary3, false);
			m_MatGlowHalo.SetTexture("_GlowObjectTex", temporary);
			m_MatGlowHalo.SetColor("_GlowColor", m_Color);
			m_MatGlowHalo.SetFloat("_GlowIntensity", m_GlowIntensity);
			Graphics.Blit(temporary3, dst, m_MatGlowHalo);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary3);
		}
	}

	private void OnDestroy()
	{
		Ins = null;
	}
}

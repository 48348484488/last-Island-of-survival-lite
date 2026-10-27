using UnityEngine;
using UnityEngine.UI;

public class AlphaAnimator : MonoBehaviour
{
	private RawImage m_Image;

	private Color m_Color;

	private bool m_AddUp;

	private float m_RemainTime;

	private void Awake()
	{
		m_Image = GetComponent<RawImage>();
		m_Color = m_Image.color;
		m_AddUp = true;
		m_Color.a = 0f;
	}

	public void Play(float period)
	{
		base.gameObject.SetActive(true);
		m_RemainTime = period;
	}

	private void Update()
	{
		if (m_RemainTime < 0f)
		{
			base.gameObject.SetActive(false);
			return;
		}
		m_RemainTime -= Time.deltaTime;
		if (m_AddUp)
		{
			m_Color.a += Time.deltaTime;
		}
		else
		{
			m_Color.a -= Time.deltaTime;
		}
		if (m_Color.a > 1f)
		{
			m_AddUp = false;
		}
		else if (m_Color.a < 0f)
		{
			m_AddUp = true;
		}
		m_Image.color = m_Color;
	}
}

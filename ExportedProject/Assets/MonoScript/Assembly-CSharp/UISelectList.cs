using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectList : MonoBehaviour
{
	public delegate void ClickEvent();

	public enum UISelectListType
	{
		Up = 0,
		Down = 1
	}

	public ClickEvent clickEvent;

	public UISelectListType m_type = UISelectListType.Down;

	public Text m_curText;

	public GameObject m_listPanel;

	public RectTransform m_listBg;

	public float m_listBgOffset = 4f;

	public Transform m_parent;

	public GameObject m_item;

	public int m_itemHeight = 20;

	private int m_curIndex;

	public List<string> m_list = new List<string>();

	private void Start()
	{
		m_item.SetActive(false);
		Hide();
		UIEventListener.Get(m_curText.gameObject, string.Empty).onClick = OnClickBtn;
		UIEventListener.Get(m_listPanel, string.Empty).onEnter = OnHoverPanel;
		UIEventListener.Get(m_listPanel, string.Empty).onExit = OnExictPanel;
		SetList(m_list);
	}

	public void Show()
	{
		m_listPanel.SetActive(true);
	}

	public void Hide()
	{
		m_listPanel.SetActive(false);
	}

	public void SetText(int index)
	{
		m_curIndex = index;
		m_curText.text = m_list[index];
	}

	public int GetIndex()
	{
		return m_curIndex;
	}

	public List<GameObject> SetList(List<string> list)
	{
		List<GameObject> list2 = new List<GameObject>();
		m_list = list;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			Transform transform = m_parent.Find(i.ToString());
			if (transform == null)
			{
				transform = Object.Instantiate(m_item).transform;
				transform.SetParent(m_parent.transform);
				transform.name = i.ToString();
				transform.localScale = Vector3.one;
				transform.gameObject.SetActive(true);
			}
			Text component = transform.GetComponent<Text>();
			if (!component)
			{
				transform.gameObject.AddComponent<Text>();
			}
			component.text = list[i].ToString();
			if (m_type == UISelectListType.Down)
			{
				transform.localPosition = new Vector2(0f, -1 * i * m_itemHeight);
			}
			else
			{
				transform.localPosition = new Vector2(0f, i * m_itemHeight);
			}
			UIEventListener.Get(transform.gameObject, string.Empty).onEnter = OnHoverItem;
			UIEventListener.Get(transform.gameObject, string.Empty).onClick = OnClickItem;
			UIEventListener.Get(transform.gameObject, string.Empty).parameter = i;
			list2.Add(transform.gameObject);
		}
		m_listBg.sizeDelta = new Vector2(m_listBg.sizeDelta.x, (float)(count * m_itemHeight) + 2f * m_listBgOffset);
		if (m_type == UISelectListType.Down)
		{
			m_listPanel.transform.localPosition = new Vector2(0f, -1f * base.gameObject.GetComponent<RectTransform>().sizeDelta.y);
			m_listBg.pivot = new Vector2(0.5f, 1f);
			m_listBg.localPosition = new Vector3(0f, base.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f, 0f);
		}
		else
		{
			m_listPanel.transform.localPosition = new Vector2(0f, base.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f);
			m_listBg.pivot = new Vector2(0.5f, 0f);
			m_listBg.localPosition = new Vector3(0f, -1f * base.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f, 0f);
		}
		return list2;
	}

	private void OnClickBtn(GameObject go)
	{
		if (m_listPanel.activeSelf)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	private void OnHoverItem(GameObject go)
	{
	}

	private void OnClickItem(GameObject go)
	{
		int text = (int)UIEventListener.Get(go, string.Empty).parameter;
		SetText(text);
		Hide();
		if (clickEvent != null)
		{
			clickEvent();
		}
	}

	private void OnHoverPanel(GameObject go)
	{
		Show();
	}

	private void OnExictPanel(GameObject go)
	{
		Hide();
	}
}

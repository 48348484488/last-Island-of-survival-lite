using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour
{
	[SerializeField]
	private GameObject m_YaoganTrigger;

	[SerializeField]
	private GameObject m_YaoganCircle;

	[SerializeField]
	private GameObject m_LittleYaogan;

	[SerializeField]
	private GameObject m_YaoganArrow;

	[SerializeField]
	private GameObject m_LittleYaoganParent;

	private Coroutine sendYaoganAngleCoroutine;

	private int m_CircleMaxMoveDistance = 400;

	public Action OnDownJoystick;

	public static Action OnUpJoystick;

	private float m_Distance;

	private Vector2 m_TouchPos = Vector2.zero;

	private RectTransform m_YaoganCircleRect;

	private RectTransform m_LittleYaoganRect;

	private RectTransform m_YaoganArrowRect;

	public static bool IsDown;

	public string horizontalAxisName = KeyName.Horizontal;

	public string verticalAxisName = KeyName.Vertical;

	private Camera UICamrea;

	public void Awake()
	{
		YaoganDragListener.Get(m_YaoganTrigger).onDown = OnDown;
		YaoganDragListener.Get(m_YaoganTrigger).onUp = OnUp;
		YaoganDragListener.Get(m_YaoganTrigger).onDrag = OnDrag;
		m_YaoganCircleRect = m_YaoganCircle.transform as RectTransform;
		m_LittleYaoganRect = m_LittleYaogan.transform as RectTransform;
		m_CircleMaxMoveDistance = (int)(m_LittleYaoganRect.sizeDelta.x * 0.5f);
		m_YaoganArrowRect = m_YaoganArrow.transform as RectTransform;
		m_YaoganArrow.SetActive(false);
		m_YaoganCircle.transform.localPosition = Vector2.zero;
		m_LittleYaogan.transform.localPosition = Vector2.zero;
		YaoganDragListener.pointEventData = null;
		UICamrea = GameObject.Find("UICamera").GetComponent<Camera>();
	}

	private void OnDown(GameObject go)
	{
		IsDown = true;
		Vector2 localPoint = Vector2.zero;
		Vector2 position = YaoganDragListener.pointEventData.position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(m_LittleYaoganParent.transform as RectTransform, position, YaoganDragListener.pointEventData.pressEventCamera, out localPoint);
		if (localPoint.x < -120f)
		{
			localPoint.x = -120f;
		}
		if (localPoint.y < -125f)
		{
			localPoint.y = -125f;
		}
		m_LittleYaoganRect.anchoredPosition = localPoint;
		SetYaoganCirclePos();
		if (OnDownJoystick != null)
		{
			OnDownJoystick();
		}
	}

	private void OnUp(GameObject go)
	{
		IsDown = false;
		m_YaoganCircle.transform.localPosition = Vector2.zero;
		m_LittleYaogan.transform.localPosition = Vector2.zero;
		m_YaoganArrow.SetActive(false);
		UpdateVirtualAxes(Vector3.zero);
		YaoganDragListener.pointEventData = null;
		if (OnUpJoystick != null)
		{
			OnUpJoystick();
		}
	}

	private void OnDrag(GameObject go)
	{
		IsDown = true;
		SetYaoganCirclePos();
		if (!(m_Distance > 0f))
		{
		}
	}

	private void SetYaoganCirclePos()
	{
		m_TouchPos = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(m_LittleYaoganRect, YaoganDragListener.pointEventData.position, YaoganDragListener.pointEventData.pressEventCamera, out m_TouchPos);
		m_Distance = Vector2.Distance(Vector2.zero, m_TouchPos);
		if (m_Distance > (float)m_CircleMaxMoveDistance)
		{
			m_Distance = m_CircleMaxMoveDistance;
			m_TouchPos = m_TouchPos.normalized * m_CircleMaxMoveDistance;
		}
		m_YaoganCircleRect.anchoredPosition = m_TouchPos;
		UpdateVirtualAxes(m_TouchPos);
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		value.x /= m_CircleMaxMoveDistance;
		value.y /= m_CircleMaxMoveDistance;
		CrossPlatformInputManager.SetAxis(horizontalAxisName, value.x);
		CrossPlatformInputManager.SetAxis(verticalAxisName, value.y);
	}

	public void Stop()
	{
		OnUp(m_YaoganTrigger.gameObject);
	}

	private void OnEnable()
	{
		OnUp(m_YaoganTrigger.gameObject);
	}
}

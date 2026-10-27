using System.Collections;
using UnityEngine;

public class ClimbState : FSMState
{
	private bool m_TopToLow;

	private bool m_CanLeaveTop;

	private bool m_CanInput;

	private float m_ClimbedLength;

	private float m_StartHeight;

	private Transform tiziTrans;

	private RaycastHit raycastHitInfo;

	private WaitForSeconds m_OneZero = new WaitForSeconds(0.1f);

	private float m_speedUpNum;

	public ClimbState()
	{
		stateID = StateID.Climb;
	}

	public override void DoBeforeEntering(object[] args)
	{
		tiziTrans = null;
		m_CanLeaveTop = false;
		m_TopToLow = false;
		m_CanInput = false;
		Player.Climbing = true;
		m_ClimbedLength = 0f;
		ClimbCapsule(Player);
		if (args.Length > 0)
		{
			Player.PlayChangeWeapen(-1, false);
			raycastHitInfo = (RaycastHit)args[0];
			m_TopToLow = (bool)args[1];
			tiziTrans = raycastHitInfo.transform;
			Player.PlayerTransform.position = raycastHitInfo.point;
			m_StartHeight = Player.PlayerTransform.position.y;
			Player.transform.forward = tiziTrans.forward;
			Player.PlayerRigidbody.useGravity = false;
			BoxCollider component = tiziTrans.GetComponent<BoxCollider>();
			component.isTrigger = true;
			if (m_TopToLow)
			{
				Player.transform.SetPositionY(raycastHitInfo.point.y - 0.8f);
			}
		}
		SpeedUpSkill();
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		while (true)
		{
			if (m_CanLeaveTop)
			{
				yield return new WaitForSeconds(1.08f);
				Player.FSM.SwitchState(StateID.Stand);
			}
			if (m_TopToLow && !m_CanInput)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.kongshou_zhan_patizi_down_idle02");
				yield return new WaitForSeconds(1f);
				m_CanInput = true;
			}
			if (!m_TopToLow && !m_CanInput)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.kongshou_zhan_patizi_up_idle01");
				yield return new WaitForSeconds(0.3f);
				m_CanInput = true;
			}
			yield return m_OneZero;
		}
	}

	public override void DoBeforeLeaving()
	{
		Player.PlayerRigidbody.useGravity = true;
		Battle.Ins.QuickLooking = false;
		Player.Climbing = false;
		ResetSkillSpeedUp();
		Player.transform.eulerAngles = new Vector3(0f, Player.transform.eulerAngles.y, 0f);
	}

	public override void Act()
	{
		Player.CloseAllIK();
		Battle.Ins.QuickLooking = true;
		if (!m_CanLeaveTop)
		{
			Vector3 vector = tiziTrans.TransformPoint(Vector3.zero);
			if (tiziTrans.localEulerAngles.x < 1f)
			{
				Player.PlayerTransform.position = new Vector3(vector.x, Player.PlayerTransform.position.y, vector.z);
			}
		}
		if (!m_CanLeaveTop && m_CanInput)
		{
			if (Player.Input.y > 0f)
			{
				Player.SetAnimatorYValue(1f);
			}
			else if (Player.Input.y == 0f)
			{
				Player.PlayerRigidbody.velocity = Vector3.zero;
				Player.SetAnimatorYValue(0f);
			}
			else if (Player.Input.y < 0f)
			{
				Player.SetAnimatorYValue(-1f);
			}
			if (tiziTrans.localEulerAngles.x > 9f)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.c94");
			}
			else
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.c");
			}
			CheckLeaveTopTizi();
		}
	}

	public static void ClimbCapsule(PlayerController Player)
	{
		Player.PlayerCollider.height = 1.8f;
		Player.PlayerCollider.radius = 0.02f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, 1.24f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.direction = 1;
		Player.PlayerCollider.transform.rotation = Player.PlayerCollider.transform.rotation;
	}

	public override void LateUpdate()
	{
		Player.transform.forward = tiziTrans.forward;
	}

	public override void Reason()
	{
		if (!m_CanLeaveTop && m_CanInput)
		{
			if (Player.GroundDistance < 0.5f && Player.PlayerRigidbody.velocity.y < -0.01f)
			{
				Player.FSM.SwitchState(StateID.Stand);
			}
			m_ClimbedLength = Player.PlayerTransform.position.y - m_StartHeight;
		}
	}

	private void CheckLeaveTopTizi()
	{
		RaycastHit hitInfo;
		if (m_CanInput && Physics.Raycast(Player.HeadTop.position - Vector3.up * 0.4f - Player.PlayerTransform.forward * 0.7f, Player.PlayerTransform.forward, out hitInfo, 1.7f, 1))
		{
			if (!hitInfo.collider.CompareTag("Tizi"))
			{
				if (!Physics.Raycast(Player.HeadTop.position + Vector3.up * 0.5f - Player.PlayerTransform.forward * 0.7f, Player.PlayerTransform.forward, out hitInfo, 1.7f, 1, QueryTriggerInteraction.Ignore))
				{
					Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.kongshou_zhan_patizi_up_idle02");
					m_CanLeaveTop = true;
				}
				else
				{
					Player.FSM.SwitchState(StateID.Fall);
				}
			}
		}
		else if (m_CanInput)
		{
			if (!Physics.Raycast(Player.HeadTop.position + Vector3.up * 0.5f - Player.PlayerTransform.forward * 0.7f, Player.PlayerTransform.forward, out hitInfo, 1.7f, 1, QueryTriggerInteraction.Ignore))
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Climb.kongshou_zhan_patizi_up_idle02");
				m_CanLeaveTop = true;
			}
			else
			{
				Player.FSM.SwitchState(StateID.Fall);
			}
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(100);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
	}
}

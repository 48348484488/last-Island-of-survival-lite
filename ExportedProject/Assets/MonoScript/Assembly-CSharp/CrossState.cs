using System.Collections;
using UnityEngine;

public class CrossState : FSMState
{
	public Vector3 TargetPos;

	private float m_PlayerStartY;

	private bool m_CrossUpFinish;

	public static bool CanPlayHighCross;

	public static bool CanPlayLowCross;

	public static bool CanPlayStandCross;

	private static float m_CrossHeight;

	public CrossState()
	{
		stateID = StateID.Cross;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_PlayerStartY = Player.Pos.y;
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		Player.PlayerCollider.isTrigger = false;
		if (Player.CurGun != null)
		{
			Player.CurGun.StopShoot();
		}
	}

	public override void DoBeforeLeaving()
	{
		Player.PlayerRigidbody.useGravity = true;
		Player.PlayerCollider.isTrigger = false;
		Player.CanShoot = true;
	}

	public override void Act()
	{
		Player.CanShoot = false;
		Player.CloseAllIK();
	}

	public override void Reason()
	{
	}

	public override IEnumerator ActCoroutine()
	{
		float detime3 = 0f;
		if (CanPlayLowCross)
		{
		}
		if (CanPlayStandCross)
		{
			detime3 = 0.8f;
			Player.ChangeAnimatorStates(Player.BaseLayer, "Cross.kongshou_zhan_fanyue02");
			yield return new WaitForSeconds(0.1f);
			if ((double)m_CrossHeight > 0.8)
			{
				Player.transform.SetPositionY(Player.transform.position.y + m_CrossHeight - 1.2f);
			}
			yield return new WaitForSeconds(detime3 - 0.1f);
		}
		if (CanPlayHighCross)
		{
			detime3 = 2.5f;
			Player.ChangeAnimatorStates(Player.BaseLayer, "Cross.kongshou_zhan_fanyue03");
			yield return new WaitForSeconds(0.1f);
			if ((double)m_CrossHeight > 1.8)
			{
				Player.transform.SetPositionY(Player.transform.position.y + m_CrossHeight - 1.8f);
			}
			yield return new WaitForSeconds(detime3 - 0.1f);
		}
		m_CrossUpFinish = false;
		Player.FSM.SwitchState(StateID.Stand);
	}

	public override void LateUpdate()
	{
	}

	public static void CheckCrossHeight(PlayerController Player)
	{
		float num = 0f;
		GameObject gameObject = null;
		CanPlayHighCross = false;
		CanPlayLowCross = false;
		CanPlayStandCross = false;
		RaycastHit hitInfo;
		if (Physics.SphereCast(Player.Pos + Vector3.up * 1.2f, 0.1f, Vector3.up, out hitInfo, 1.5f, 1))
		{
			return;
		}
		Vector3 vector = Player.Pos + Player.transform.forward * 0.62f;
		if (Physics.SphereCast(vector + Vector3.up * 2.1f, 0.2f, Vector3.down, out hitInfo, 1.7f, 5, QueryTriggerInteraction.Ignore) && !(Vector3.Angle(Vector3.up, hitInfo.normal) > 50f) && !hitInfo.collider.name.Contains("shitou") && !hitInfo.collider.name.Contains("shatanyanshi") && !hitInfo.collider.name.Contains("Projet_001_dakache") && !hitInfo.collider.CompareTag("Ground") && !hitInfo.collider.CompareTag("Tingjiping") && !hitInfo.collider.CompareTag("tree") && !hitInfo.collider.CompareTag("Tizi"))
		{
			float num2 = hitInfo.collider.bounds.max.y - hitInfo.transform.position.y;
			m_CrossHeight = hitInfo.point.y - Player.transform.position.y;
			if (m_CrossHeight <= 0.7f && m_CrossHeight > 0.4f)
			{
				CanPlayLowCross = true;
			}
			else if (m_CrossHeight <= 1.8f && m_CrossHeight > 1f)
			{
				CanPlayStandCross = true;
			}
		}
	}
}

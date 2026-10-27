using System.Collections;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using SC.UI;
using UnityEngine;
using cfg;

public class PlantMgr : SingletonMono<PlantMgr>
{
	public PlantInfo CurPreviewPlant;

	public bool AllowPlacement;

	private WaitForSeconds second1 = new WaitForSeconds(1f);

	private void Update()
	{
		if (CurPreviewPlant != null)
		{
			UpdatePlantPreviewCollisions();
			UpdateFreeMovement();
			CurPreviewPlant.gameObject.ChangeAllMaterialsOutLineInChildren(CurPreviewPlant.Renderers[0], (!AllowPlacement) ? SingletonMono<BuildManager>.Ins.PreviewDeniedColor : SingletonMono<BuildManager>.Ins.PreviewAllowedColor, true);
		}
	}

	public IEnumerator DownTime()
	{
		while (true)
		{
			if (SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject != null)
			{
				PlantInfo plantInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as PlantInfo;
				if ((bool)plantInfo && (float)plantInfo.GrowFinishTime > Time.time)
				{
					Battle.Ins.MyBattlePanel.SetAimedPlantGrowFinishTime(plantInfo);
				}
			}
			yield return second1;
		}
	}

	public void CreatePreviewPrefab(int id)
	{
		if (CurPreviewPlant != null)
		{
			if (CurPreviewPlant.CfgId == id)
			{
				Object.Destroy(CurPreviewPlant.gameObject);
				return;
			}
			Object.Destroy(CurPreviewPlant.gameObject);
		}
		if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Pa)
		{
			AlertBox.Show(Utils.GetString(285));
			return;
		}
		if (!Battle.Ins.SelfPlayer.InPlantState)
		{
			Battle.Ins.SelfPlayer.FSMUpBody.SwitchState(StateID.Plant);
		}
		Vector3 position = Vector3.zero;
		RaycastHit hitInfo;
		if (Physics.Raycast(SingletonMono<BuilderBehaviour>.Ins.GetRay(), out hitInfo, float.PositiveInfinity, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
		{
			position = hitInfo.point;
		}
		CurPreviewPlant = Object.Instantiate(Battle.Ins.PlantPoolDic[(int)ItemCfg.Get(id).extras[0]].Get(), position, Quaternion.identity).GetComponent<PlantInfo>();
		CurPreviewPlant.gameObject.SetActive(true);
		CurPreviewPlant.CfgId = id;
	}

	private void UpdatePlantPreviewCollisions()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(CurPreviewPlant.transform.position + Vector3.up, Vector3.down, out hitInfo, 1.1f, SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
		{
			AllowPlacement = true;
		}
		else
		{
			AllowPlacement = false;
		}
	}

	public void UpdateFreeMovement()
	{
		if (!(CurPreviewPlant == null))
		{
			float actionDistance = SingletonMono<BuilderBehaviour>.Ins.ActionDistance;
			RaycastHit hitInfo;
			if (Physics.Raycast(SingletonMono<BuilderBehaviour>.Ins.GetRay(), out hitInfo, actionDistance, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
			{
				CurPreviewPlant.transform.position = hitInfo.point;
				CurPreviewPlant.transform.rotation = Quaternion.Euler(Vector3.zero);
				CurPreviewPlant.transform.rotation *= Quaternion.Euler(new Vector3(0f, SingletonMono<BuilderBehaviour>.Ins.GetTransform.localEulerAngles.y, 0f));
				return;
			}
			Transform getTransform = SingletonMono<BuilderBehaviour>.Ins.GetTransform;
			Vector3 position = getTransform.position + getTransform.forward * actionDistance;
			position.y = Mathf.Clamp(position.y, SingletonMono<BuilderBehaviour>.Ins.GetTransform.position.y - 3f, SingletonMono<BuilderBehaviour>.Ins.GetTransform.position.y + 3f);
			CurPreviewPlant.transform.position = position;
			CurPreviewPlant.transform.rotation = Quaternion.Euler(Vector3.zero);
			CurPreviewPlant.transform.rotation *= Quaternion.Euler(new Vector3(0f, SingletonMono<BuilderBehaviour>.Ins.GetTransform.localEulerAngles.y, 0f));
		}
	}
}

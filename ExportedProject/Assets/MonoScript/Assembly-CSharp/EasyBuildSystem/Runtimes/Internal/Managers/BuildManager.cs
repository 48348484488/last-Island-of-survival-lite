using System;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

namespace EasyBuildSystem.Runtimes.Internal.Managers
{
	public class BuildManager : SingletonMono<BuildManager>
	{
		public SupportType BuildingSupport;

		public bool UsePhysics;

		public Dictionary<int, PartBehaviour> PartsCollections = new Dictionary<int, PartBehaviour>();

		public bool UseDefaultPreviewMaterial = true;

		public Material CustomPreviewMaterial;

		public Shader OutLineShader;

		public Color AimColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public Color PreviewAllowedColor = new Color32(0, 0, byte.MaxValue, byte.MaxValue);

		public Color PreviewDeniedColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public bool Online = true;

		[HideInInspector]
		public List<PartBehaviour> Parts = new List<PartBehaviour>();

		[HideInInspector]
		public List<SocketBehaviour> Sockets = new List<SocketBehaviour>();

		[HideInInspector]
		public StateType DefaultState = StateType.Placed;

		[HideInInspector]
		public List<BuildPart> PartCfgList;

		public Dictionary<long, PartBehaviour> PartDic = new Dictionary<long, PartBehaviour>();

		public long SelfRoleId;

		public List<Collider> SelfColliders;

		public LayerMask DefaultMask;

		public LayerMask GroundMask;

		public LayerMask StoneMask;

		public GameObject BuildPartParent;

		private List<BuildPart> m_oneTypeParts = new List<BuildPart>();

		protected override void Awake()
		{
			base.Awake();
			Online = true;
			DefaultMask = LayerMask.GetMask("Default");
			GroundMask = LayerMask.GetMask("Ground");
			StoneMask = LayerMask.GetMask("Stone");
			EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildPartFinish));
			SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
			SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Combine(SChangeBuildingName.handler, new SChangeBuildingName.Handler(OnSChangeBuildingName));
			SSitDown.handler = (SSitDown.Handler)Delegate.Combine(SSitDown.handler, new SSitDown.Handler(OnSitDown));
			SGetup.handler = (SGetup.Handler)Delegate.Combine(SGetup.handler, new SGetup.Handler(OnSGetup));
			ViewMgr.Ins.AddOnShowEvent<BattlePanel>(DelayedAttachEvent);
			BuildPartParent = new GameObject("BuildPartParent");
		}

		private void Update()
		{
			DoPartUpdate();
		}

		private void DoPartUpdate()
		{
			foreach (PartBehaviour value in PartDic.Values)
			{
				value.DoUpdate();
			}
		}

		private void OnSGetup(SGetup msg)
		{
			PartBehaviour partByInsID = GetPartByInsID(msg.insId);
			if ((bool)partByInsID)
			{
				partByInsID.StatesMsg = 0;
			}
		}

		private void OnSitDown(SSitDown msg)
		{
			PartBehaviour partByInsID = GetPartByInsID(msg.insId);
			if ((bool)partByInsID)
			{
				partByInsID.StatesMsg = 100;
			}
		}

		private void OnSChangeBuildingName(SChangeBuildingName msg)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.insId);
			if (partByInsID != null)
			{
				partByInsID.MapObjectName = msg.name;
				Utils.TriggerEvent(BattleEvent.OnBuildNameChanged, msg.insId);
			}
		}

		private void OnSBuildingStatusChange(SBuildingStatusChange msg)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.id);
			if (!IsZheDieTizi(partByInsID.Id))
			{
				return;
			}
			TiziController tiziController = partByInsID.gameObject.AddComponent<TiziController>();
			if (msg.status == 6)
			{
				tiziController.Open();
			}
			else
			{
				tiziController.Close();
			}
			if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null && msg.id == SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId)
			{
				if (tiziController.Opening)
				{
					EventHandlers.OnRemoveExtraBtn(107);
					EventHandlers.OnAddExtraBtn(108);
				}
				else
				{
					EventHandlers.OnRemoveExtraBtn(108);
					EventHandlers.OnAddExtraBtn(107);
				}
			}
		}

		public bool IsZheDieTizi(int id)
		{
			if (id == 26005 || id == 26500 || id == 26501 || id == 26502)
			{
				return true;
			}
			return false;
		}

		private void OnBuildPartFinish(long InsId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
		{
			if (IsZheDieTizi(buildPartCfg.id))
			{
				TiziController tiziController = SingletonMono<BuildManager>.Ins.GetPartByInsID(InsId).gameObject.GetComponent<TiziController>();
				if (tiziController == null)
				{
					tiziController = SingletonMono<BuildManager>.Ins.GetPartByInsID(InsId).gameObject.AddComponent<TiziController>();
				}
				if (status == 6)
				{
					tiziController.Open();
					return;
				}
				tiziController.Opening = false;
				tiziController.Close();
			}
		}

		private void DelayedAttachEvent()
		{
			EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimedPart));
			EventHandlers.OnAimedGo = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedGo, new Utils.LongDelegate(OnAimedGo));
			EventHandlers.OnClickExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnClickExtraBtn, new Utils.IntDelegate(ClickBtnCallBack));
			ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", DelayedAttachEvent);
		}

		private void ClickBtnCallBack(int id)
		{
			ClickAboutZhedietizi(id);
			ClickAboutCaiji(id);
		}

		private void ClickAboutZhedietizi(int id)
		{
			switch (id)
			{
			case 107:
			{
				TiziController component2 = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.GetComponent<TiziController>();
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null && component2 != null)
				{
					component2.Open();
					COpenTizi cOpenTizi = new COpenTizi();
					cOpenTizi.instanceId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
					Client2Gs.Ins.Send(cOpenTizi);
				}
				break;
			}
			case 108:
			{
				TiziController component = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.GetComponent<TiziController>();
				if (SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null && component != null)
				{
					component.Close();
					CCloseTizi cCloseTizi = new CCloseTizi();
					cCloseTizi.instanceId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
					Client2Gs.Ins.Send(cCloseTizi);
				}
				break;
			}
			}
		}

		private void ClickAboutCaiji(int id)
		{
			if (id == 110)
			{
				PlantInfo plantInfo = SingletonMono<BuilderBehaviour>.Ins.CurAimMapObject as PlantInfo;
				int num = (int)((float)plantInfo.GrowFinishTime - Time.time);
				if (num <= 0)
				{
					plantInfo.Caiji();
				}
				else
				{
					AlertBox.Show(Utils.GetString(283));
				}
			}
		}

		private void OnAimedPart(long id)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(id);
			if (!(partByInsID != null) || partByInsID.MyCfg == null || !IsZheDieTizi(partByInsID.LV1Id))
			{
				return;
			}
			TiziController component = partByInsID.gameObject.GetComponent<TiziController>();
			if (component != null)
			{
				if (component.Opening)
				{
					EventHandlers.OnRemoveExtraBtn(107);
					EventHandlers.OnAddExtraBtn(108);
				}
				else
				{
					EventHandlers.OnRemoveExtraBtn(108);
					EventHandlers.OnAddExtraBtn(107);
				}
			}
		}

		private void OnAimedGo(long insId)
		{
			AimedPlant(insId);
		}

		private void AimedPlant(long insId)
		{
			PlantInfo value;
			if (Battle.Ins.PlantDic.TryGetValue(insId, out value))
			{
				EventHandlers.OnAddExtraBtn(110);
			}
		}

		private void Start()
		{
		}

		public GameObject GetPartGoByInsID(long insId)
		{
			if (PartDic.ContainsKey(insId))
			{
				return PartDic[insId].gameObject;
			}
			return null;
		}

		public PartBehaviour GetPartByInsID(long insId)
		{
			if (PartDic.ContainsKey(insId))
			{
				return PartDic[insId];
			}
			return null;
		}

		public IEnumerator PreparePartsCollections()
		{
			_003CPreparePartsCollections_003Ec__Iterator0._003CPreparePartsCollections_003Ec__AnonStorey1 _003CPreparePartsCollections_003Ec__AnonStorey = new _003CPreparePartsCollections_003Ec__Iterator0._003CPreparePartsCollections_003Ec__AnonStorey1();
			_003CPreparePartsCollections_003Ec__AnonStorey._003C_003Ef__ref_00240 = this;
			PartCfgList = BuildPart.GetAllList();
			_003CPreparePartsCollections_003Ec__AnonStorey.g = new GameObject("BasePartsCollections");
			_003CPreparePartsCollections_003Ec__AnonStorey.offsetX = -50000;
			_003CPreparePartsCollections_003Ec__AnonStorey.offsetY = 30000;
			using (List<BuildPart>.Enumerator enumerator = PartCfgList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					_003CPreparePartsCollections_003Ec__Iterator0._003CPreparePartsCollections_003Ec__AnonStorey2 _003CPreparePartsCollections_003Ec__AnonStorey2 = new _003CPreparePartsCollections_003Ec__Iterator0._003CPreparePartsCollections_003Ec__AnonStorey2();
					_003CPreparePartsCollections_003Ec__AnonStorey2._003C_003Ef__ref_00240 = this;
					_003CPreparePartsCollections_003Ec__AnonStorey2._003C_003Ef__ref_00241 = _003CPreparePartsCollections_003Ec__AnonStorey;
					_003CPreparePartsCollections_003Ec__AnonStorey2.part = enumerator.Current;
					if (!string.IsNullOrEmpty(_003CPreparePartsCollections_003Ec__AnonStorey2.part.model) && _003CPreparePartsCollections_003Ec__AnonStorey2.part.lv == 1)
					{
						yield return ResMgr.Ins.CreateFromAB(_003CPreparePartsCollections_003Ec__AnonStorey2.part.model, null, _003CPreparePartsCollections_003Ec__AnonStorey2._003C_003Em__0);
					}
				}
			}
			foreach (BuildPart partCfg in PartCfgList)
			{
				if (partCfg.lv == 1)
				{
					BuildPart buildPart = BuildPart.Get(partCfg.id);
					AddLvBuildPartRecursion(buildPart);
				}
			}
		}

		private void AddLvBuildPartRecursion(BuildPart buildPart)
		{
			if (buildPart.nextId == -1 || buildPart.nextId <= 0)
			{
				return;
			}
			if (PartsCollections.ContainsKey(buildPart.id))
			{
				try
				{
					PartsCollections.Add(buildPart.nextId, PartsCollections[buildPart.id]);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Concat(ex, "buildPart.nextId=====", buildPart.nextId));
				}
			}
			else
			{
				Debug.LogError("PartsCollections not exit id::::" + buildPart.id);
			}
			BuildPart buildPart2 = BuildPart.Get(buildPart.nextId);
			if (buildPart2 != null)
			{
				AddLvBuildPartRecursion(buildPart2);
			}
		}

		public List<BuildPart> GetPartByType(int partType)
		{
			m_oneTypeParts.Clear();
			foreach (BuildPart partCfg in PartCfgList)
			{
				if (partCfg.type == partType)
				{
					m_oneTypeParts.Add(partCfg);
				}
			}
			return m_oneTypeParts;
		}

		public List<BuildPart> GetBasicPartByType(int partType)
		{
			m_oneTypeParts.Clear();
			foreach (BuildPart partCfg in PartCfgList)
			{
				if (partCfg.basic && partCfg.lv == 1 && partCfg.type == partType)
				{
					m_oneTypeParts.Add(partCfg);
				}
			}
			return m_oneTypeParts;
		}

		public void AddPart(PartBehaviour part)
		{
			if (!(part == null))
			{
				Parts.Add(part);
			}
		}

		public void RemovePart(PartBehaviour part)
		{
			if (!(part == null))
			{
				Parts.Remove(part);
			}
		}

		public void AddSocket(SocketBehaviour socket)
		{
			if (!(socket == null))
			{
				Sockets.Add(socket);
			}
		}

		public void RemoveSocket(SocketBehaviour socket)
		{
			if (!(socket == null))
			{
				Sockets.Remove(socket);
			}
		}

		public PartBehaviour GetPart(int id)
		{
			if (PartsCollections == null)
			{
				return null;
			}
			return PartsCollections[id];
		}

		public PartBehaviour PlacePrefab(PartBehaviour part, BuildPartInfo buildPartInfo, Transform parent = null, SocketBehaviour socket = null)
		{
			if (Online && PartDic.ContainsKey(buildPartInfo.instanceId))
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(part.gameObject, new Vector3(buildPartInfo.pos.x, buildPartInfo.pos.y, buildPartInfo.pos.z), Quaternion.Euler(new Vector3(0f, buildPartInfo.eulerY, 0f)));
			gameObject.SetActive(true);
			gameObject.transform.parent = BuildPartParent.transform;
			gameObject.transform.localScale = Vector3.one;
			PartBehaviour component = gameObject.GetComponent<PartBehaviour>();
			component.FillMeshAndMaterial();
			if (component.PlacedDisableObjects.Length > 0)
			{
				GameObject[] placedDisableObjects = component.PlacedDisableObjects;
				foreach (GameObject gameObject2 in placedDisableObjects)
				{
					gameObject2.gameObject.SetActiveBetter(false);
				}
			}
			BuildPart buildPart = BuildPart.Get(buildPartInfo.typeId);
			component.Lv = buildPart.lv;
			DestoryXGDCollider(component);
			SingletonMono<BuildManager>.Ins.ChangeBuildEffectByStatus(buildPartInfo.status, component);
			if (buildPart != null)
			{
				component.Init(buildPart);
			}
			component.Oc = buildPartInfo.extraInfo;
			component.Hp = buildPartInfo.hp;
			if (parent != null)
			{
				gameObject.transform.SetParent(parent, true);
			}
			if (Online)
			{
				component.InsId = buildPartInfo.instanceId;
				component.ToolBoxId = buildPartInfo.toolBoxId;
				PartDic.Add(buildPartInfo.instanceId, component);
			}
			EventHandlers.PlacedPart(component, socket);
			component.ChangeState(DefaultState);
			return component;
		}

		public void DestoryXGDCollider(PartBehaviour p)
		{
			BuildCollider component = p.GetComponent<BuildCollider>();
			if (!(component != null))
			{
				return;
			}
			BoxCollider[] boxColliders = component.BoxColliders;
			foreach (BoxCollider boxCollider in boxColliders)
			{
				if (!boxCollider.gameObject.activeSelf)
				{
					UnityEngine.Object.Destroy(boxCollider.gameObject);
				}
			}
		}

		public void ChangePartParent(long parentInsId, long childInsId)
		{
			PartBehaviour partByInsID = GetPartByInsID(parentInsId);
			PartBehaviour partByInsID2 = GetPartByInsID(childInsId);
			if (partByInsID != null)
			{
				partByInsID2.gameObject.transform.SetParent(partByInsID.transform, true);
			}
			else
			{
				partByInsID2.gameObject.transform.SetParent(null, true);
			}
		}

		public void ChangeBuildEffectByStatus(int status, PartBehaviour p)
		{
			if (!(p != null) || !(p.BuildingEffectsScript != null))
			{
				return;
			}
			p.StatesMsg = status;
			p.BuildingEffectsScript.HideAll();
			switch (status)
			{
			case 5:
				if (p.BuildingEffectsScript.UsingEffect[p.Lv - 1] != null)
				{
					p.BuildingEffectsScript.UsingEffect[p.Lv - 1].SetActive(true);
				}
				break;
			case 3:
				if (p.BuildingEffectsScript.UpingLvEffect[p.Lv - 1] != null)
				{
					p.BuildingEffectsScript.UpingLvEffect[p.Lv - 1].SetActive(true);
				}
				break;
			default:
				if (status == 3 && p.BuildingEffectsScript.YouChanchuEffect[p.Lv - 1] != null)
				{
					p.BuildingEffectsScript.YouChanchuEffect[p.Lv - 1].SetActive(true);
				}
				break;
			}
		}
	}
}

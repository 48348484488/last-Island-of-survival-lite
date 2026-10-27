using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Managers.Data;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using EasyBuildSystem.Runtimes.Internal.Socket.Data;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

namespace EasyBuildSystem.Runtimes.Internal.Builder
{
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class BuilderBehaviour : SingletonMono<BuilderBehaviour>
	{
		public enum CantBuildReson
		{
			NoEnoughSupport = 0,
			TooNearToPart = 1,
			TooHigh = 2,
			HaveOtherBuildNear = 3,
			UpPartTooNear = 4,
			Common = 5,
			notallow = 6,
			Null = 7
		}

		public float ActionDistance = 1f;

		public float OutOfRangeDistance;

		public float OverlapAngles = 8f;

		public bool LockRotation = true;

		public int CheckLayers;

		public int AimLayers;

		public int SocketLayers;

		public DetectionType RayDetection = DetectionType.Overlap;

		public RayType CameraType;

		public float RaycastOffset;

		public Transform RaycastOriginTransform;

		public bool UsePlacementMode = true;

		public bool ResetModeAfterPlacement;

		public bool UseDestructionMode = true;

		public bool ResetModeAfterDestruction;

		public bool UseEditionMode;

		public bool ResetModeAfterEdition;

		public bool UsePreviewCamera;

		public Camera PreviewCamera;

		public LayerMask PreviewLayer;

		public MovementType PreviewMovementType;

		public float PreviewGridSize = 1f;

		public float PreviewGridOffset;

		public float PreviewSmoothTime = 5f;

		public AudioSource Source;

		public AudioClip[] PlacementClips;

		public AudioClip[] DestructionClips;

		public InputsCollection InputsCollection;

		[HideInInspector]
		public BuildMode CurrentMode;

		[HideInInspector]
		public PartBehaviour SelectedPrefab;

		[HideInInspector]
		public int SelectedIndex;

		[HideInInspector]
		public PartBehaviour CurrentPreview;

		[HideInInspector]
		public PartBehaviour CurrentEditionPreview;

		[HideInInspector]
		public PartBehaviour CurrentRemovePreview;

		public PartBehaviour CurrentAimPart;

		[HideInInspector]
		public Vector3 CurrentRotationOffset;

		[HideInInspector]
		public bool AllowPlacement = true;

		[HideInInspector]
		public bool AllowDestruction = true;

		[HideInInspector]
		public bool AllowEdition = true;

		[HideInInspector]
		public bool HasSocket;

		[HideInInspector]
		public SocketBehaviour CurrentSocket;

		[HideInInspector]
		public SocketBehaviour LastSocket;

		private Camera BuilderCamera;

		private Vector3 m_BuilderCameraCenterOffset;

		private Ray m_BuilderCameraViewportToWorldPointRay;

		private WaitForSeconds WaitForSeconds02 = new WaitForSeconds(0.2f);

		private PartBehaviour m_LastAimPart;

		private RaycastHit Hit;

		private Collider[] Colliders = new Collider[500];

		private SocketBehaviour[] Sockets = new SocketBehaviour[500];

		private int m_nearSocketNum;

		private bool needFreeMove;

		private List<int> CanOnFreeMoveGoId = new List<int>
		{
			26503, 1, 2, 3, 4, 5, 6, 7, 8, 37,
			38, 39, 40, 41, 42, 43, 44
		};

		private float m_nextCheckNearSocketTime = 0.15f;

		private float m_nextSnapMoveTime = 0.1f;

		public CantBuildReson WhyCantBuild = CantBuildReson.Null;

		private Collider[] tmpColliderArray = new Collider[4];

		public MapObject CurAimMapObject;

		private GameObject m_lastAimGo;

		public Transform GetTransform
		{
			get
			{
				return base.transform;
			}
		}

		private void OnEnable()
		{
			if (!Application.isPlaying)
			{
				InputsCollection inputsCollection = Resources.Load<InputsCollection>("Default Inputs Collection");
				if (InputsCollection == null && inputsCollection != null)
				{
					InputsCollection = inputsCollection;
				}
			}
		}

		private void Start()
		{
			ActionDistance = 12f;
			OverlapAngles = 15f;
			LockRotation = true;
			if (Application.isPlaying)
			{
				CheckLayers = LayerMask.GetMask("Default", "Outline", "Door", "Tree", "Stone", "Building", "Monster");
				AimLayers = LayerMask.GetMask("Default", "Outline", "Window", "Door", "Tree", "Stone", "Building", "Monster", "OtherPlayerCollider", "Grass");
				SocketLayers = LayerMask.NameToLayer("socket");
				if (PreviewCamera != null)
				{
					PreviewCamera.enabled = UsePreviewCamera;
				}
				BuilderCamera = GetComponent<Camera>();
				try
				{
					StartCoroutine(CheckCurPreviewEnterOther());
				}
				catch (Exception)
				{
				}
				if (BuilderCamera == null)
				{
					Debug.Log("<b><color=cyan>[Easy Build System]</color></b> : No camera for the Builder Behaviour component.");
				}
				m_BuilderCameraCenterOffset = new Vector3(0.5f, 0.5f, RaycastOffset);
				BattleEvent.OnExitedBuildState = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnExitedBuildState, new Utils.VoidDelegate(_003CStart_003Em__0));
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				if (m_nextCheckNearSocketTime > 0f)
				{
					m_nextCheckNearSocketTime -= Time.deltaTime;
				}
				if (m_nextSnapMoveTime > 0f)
				{
					m_nextSnapMoveTime -= Time.deltaTime;
				}
				UpdateModes();
			}
		}

		public Ray GetRay()
		{
			if (CameraType == RayType.FirstPerson)
			{
				return new Ray(BuilderCamera.ViewportToWorldPoint(m_BuilderCameraCenterOffset), BuilderCamera.transform.forward);
			}
			if (CameraType == RayType.ThirdPerson && RaycastOriginTransform != null)
			{
				return new Ray(RaycastOriginTransform.position, BuilderCamera.transform.forward);
			}
			return default(Ray);
		}

		public virtual void UpdateModes()
		{
			if (SingletonMono<BuildManager>.Ins == null || SingletonMono<BuildManager>.Ins.PartsCollections == null)
			{
				return;
			}
			UpdateCurAimPart();
			if (CurrentMode == BuildMode.Placement)
			{
				if (SelectedPrefab == null)
				{
					return;
				}
				if (!PreviewExists())
				{
					if (!BuildPart.Get(SelectedPrefab.Id).basic && Singleton<BagMgr>.Ins.GetItemNum(SelectedPrefab.Id) <= 0 && SelectedPrefab.ProtoInsid <= 0)
					{
						SelectedPrefab = null;
						if (Battle.Ins.MyBattlePanel != null && !Battle.Ins.MyBattlePanel.BuildBaseLabelIsShow())
						{
							Utils.TriggerEvent(BattleEvent.OnWantExitBuildState, true);
						}
					}
					else
					{
						CreatePreview(SelectedPrefab.gameObject);
					}
				}
				else
				{
					UpdatePreview();
				}
			}
			else if (CurrentMode == BuildMode.Aim)
			{
				UpdateAimPreview();
			}
			else if (CurrentMode == BuildMode.None)
			{
				ClearPreview();
			}
		}

		private IEnumerator CheckCurPreviewEnterOther()
		{
			while (true)
			{
				if (CurrentPreview != null && CurrentPreview.CurrentState == StateType.Preview)
				{
					CurrentPreview.CheckEnterBoxCollider();
					CurrentPreview.GetInDic();
				}
				yield return WaitForSeconds02;
			}
		}

		private void UpdateAimPreview()
		{
			if (CurrentAimPart != null)
			{
				if (CanShowBuildingBaseOp())
				{
					CurrentAimPart.gameObject.ChangeAllMaterialsOutLineInChildren(CurrentAimPart.CurLvMesh, SingletonMono<BuildManager>.Ins.AimColor, true);
				}
				else if (CanShowBuildingOtherOp() && !CurrentAimPart.MyCfg.basic)
				{
					CurrentAimPart.gameObject.ChangeAllMaterialsOutLineInChildren(CurrentAimPart.CurLvMesh, SingletonMono<BuildManager>.Ins.AimColor, true);
				}
			}
			else if (CurAimMapObject != null && !Battle.Ins.SelfPlayer.InBuildState && CurAimMapObject.Renderers.Count > 0)
			{
				CurAimMapObject.gameObject.ChangeAllMaterialsOutLineInChildren(CurAimMapObject.Renderers[0], SingletonMono<BuildManager>.Ins.AimColor, true);
			}
		}

		public bool CanShowBuildingBaseOp()
		{
			if (Battle.Ins != null && Battle.Ins.SelfPlayer != null && SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null && !SingletonMono<BuilderBehaviour>.Ins.CurrentPreview && Battle.Ins.SelfPlayer.InBuildState)
			{
				return true;
			}
			return false;
		}

		public bool CanShowBuildingOtherOp()
		{
			if (Battle.Ins != null && Battle.Ins.SelfPlayer != null && SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart != null && !SingletonMono<BuilderBehaviour>.Ins.CurrentPreview && !Battle.Ins.SelfPlayer.InBuildState)
			{
				return true;
			}
			return false;
		}

		public void ResetAimPreview()
		{
			ResetBuildAimPreview();
			ResetNormalAim();
		}

		public void ResetBuildAimPreview()
		{
			if (CurrentAimPart != null)
			{
				CurrentAimPart.gameObject.ChangeAllMaterialsOutLineInChildren(CurrentAimPart.CurLvMesh, SingletonMono<BuildManager>.Ins.AimColor, false);
				CurrentAimPart = null;
				m_LastAimPart = null;
			}
		}

		public void ResetNormalAim()
		{
			if (CurAimMapObject != null)
			{
				if (CurAimMapObject.Renderers.Count > 0)
				{
					CurAimMapObject.gameObject.ChangeAllMaterialsOutLineInChildren(CurAimMapObject.Renderers[0], SingletonMono<BuildManager>.Ins.AimColor, false);
				}
				CurAimMapObject = null;
				m_lastAimGo = null;
			}
		}

		public void UpdatePreview()
		{
			if (RayDetection == DetectionType.Overlap)
			{
				needFreeMove = true;
				if (m_nextCheckNearSocketTime <= 0f)
				{
					m_nearSocketNum = Physics.OverlapSphereNonAlloc(GetTransform.position, ActionDistance, Colliders, 1 << SocketLayers, QueryTriggerInteraction.Collide);
					for (int i = 0; i < m_nearSocketNum; i++)
					{
						SocketBehaviour componentInParent = Colliders[i].GetComponentInParent<SocketBehaviour>();
						if (componentInParent != null)
						{
							Sockets[i] = componentInParent;
						}
					}
					m_nextCheckNearSocketTime = 0.15f;
				}
				if (m_nearSocketNum > 0)
				{
					if (m_nextSnapMoveTime <= 0f)
					{
						UpdateSnapsMovement(Sockets, m_nearSocketNum);
						m_nextSnapMoveTime = 0.15f;
					}
				}
				else
				{
					HasSocket = false;
				}
				if (needFreeMove && !HasSocket)
				{
					UpdateFreeMovement();
				}
			}
			else
			{
				SocketBehaviour socketBehaviour = null;
				if (Physics.Raycast(BuilderCamera.ViewportToWorldPoint(m_BuilderCameraCenterOffset), BuilderCamera.transform.forward, out Hit, ActionDistance, 1 << SocketLayers) && Hit.collider.GetComponentInChildren<SocketBehaviour>() != null)
				{
					socketBehaviour = Hit.collider.GetComponentInChildren<SocketBehaviour>();
				}
				if (socketBehaviour != null)
				{
					UpdateSnapsMovement(new SocketBehaviour[1] { socketBehaviour }, 1);
				}
				else
				{
					UpdateFreeMovement();
				}
			}
			UpdatePreviewCollisions();
			CurrentPreview.gameObject.ChangeAllMaterialsOutLineInChildren(CurrentPreview.CurLvMesh, (!AllowPlacement) ? SingletonMono<BuildManager>.Ins.PreviewDeniedColor : SingletonMono<BuildManager>.Ins.PreviewAllowedColor, true);
		}

		public void UpdateFreeMovement()
		{
			if (CurrentPreview == null)
			{
				return;
			}
			float num = ((OutOfRangeDistance != 0f) ? OutOfRangeDistance : ActionDistance);
			CurrentPreview.CanBuildForFreeMove = false;
			RaycastHit hitInfo;
			if (Physics.Raycast(BuilderCamera.ViewportToWorldPoint(m_BuilderCameraCenterOffset), BuilderCamera.transform.forward, out hitInfo, num, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask | (int)SingletonMono<BuildManager>.Ins.StoneMask, QueryTriggerInteraction.Ignore))
			{
				if (PreviewMovementType == MovementType.Normal)
				{
					CurrentPreview.transform.position = hitInfo.point + CurrentPreview.PreviewOffset;
				}
				if (!CurrentPreview.RotateAccordingSlope)
				{
					CurrentPreview.transform.rotation = Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
					if (LockRotation)
					{
						CurrentPreview.transform.rotation *= Quaternion.Euler(new Vector3(0f, GetTransform.localEulerAngles.y + (float)CurrentPreview.SelfRotateAngle, 0f));
					}
				}
				else
				{
					Quaternion identity = Quaternion.identity;
					CurrentPreview.transform.rotation = GetTransform.rotation * SelectedPrefab.transform.localRotation * Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
				}
				return;
			}
			CurrentPreview.transform.rotation = Quaternion.Euler(CurrentPreview.RotationAxis + CurrentRotationOffset);
			Transform transform = ((CurrentPreview.GroundUpperHeight != 0f) ? BuilderCamera.transform : GetTransform);
			transform.position += CurrentPreview.PreviewOffset;
			Vector3 position = transform.position + transform.forward * num;
			if (CurrentPreview.UseGroundUpper)
			{
				position.y = Mathf.Clamp(position.y, GetTransform.position.y - CurrentPreview.GroundUpperHeight, GetTransform.position.y + CurrentPreview.GroundUpperHeight);
			}
			else if (Physics.Raycast(CurrentPreview.transform.position + Vector3.up * 0.3f, Vector3.down, out hitInfo, float.PositiveInfinity, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
			{
				position.y = hitInfo.point.y;
			}
			if (PreviewMovementType == MovementType.Normal)
			{
				CurrentPreview.transform.position = position;
			}
			if (LockRotation)
			{
				CurrentPreview.transform.rotation *= Quaternion.Euler(new Vector3(0f, GetTransform.localEulerAngles.y + (float)CurrentPreview.SelfRotateAngle, 0f));
			}
			if (CurrentPreview.NeedFollowSlopoe)
			{
				CurrentPreview.transform.forward = GetForwardInSlop(CurrentPreview.transform);
			}
			CurrentSocket = null;
			LastSocket = null;
		}

		public void UpdateSnapsMovement(SocketBehaviour[] sockets, int num)
		{
			HasSocket = false;
			if (CurrentPreview == null)
			{
				return;
			}
			float num2 = float.PositiveInfinity;
			float num3 = 2.1474836E+09f;
			CurrentSocket = null;
			for (int i = 0; i < num; i++)
			{
				if (sockets[i] == null || !sockets[i].AllowPart(SelectedPrefab) || !Utils.CanSee(Battle.Ins.MainCamera.transform.position, sockets[i].transform.position))
				{
					continue;
				}
				PartBehaviour attachedPart = sockets[i].AttachedPart;
				if (attachedPart == null || attachedPart.Sockets.Length == 0 || !attachedPart.AllowOp)
				{
					continue;
				}
				SocketBehaviour socketBehaviour = sockets[i];
				if (!(socketBehaviour != null) || !socketBehaviour.gameObject.activeSelf || socketBehaviour.IsDisabled || attachedPart.AvoidAnchoredOnSocket)
				{
					continue;
				}
				if (RayDetection == DetectionType.Overlap)
				{
					float sqrMagnitude = (socketBehaviour.transform.position - GetTransform.position).sqrMagnitude;
					if (!(sqrMagnitude < ActionDistance * ActionDistance))
					{
						continue;
					}
					float num4 = Vector3.Angle(Battle.Ins.MainCamera.SelfTransform.forward, socketBehaviour.transform.position - (Battle.Ins.SelfPlayer.PlayerTransform.position + Vector3.up * 1.8f - Battle.Ins.MainCamera.SelfTransform.forward * 2f));
					if (!(num4 < num2) || !(num4 < OverlapAngles))
					{
						continue;
					}
					num2 = num4;
					if (sqrMagnitude < num3)
					{
						num3 = sqrMagnitude;
						if (CurrentSocket == null)
						{
							CurrentSocket = socketBehaviour;
						}
					}
				}
				else
				{
					CurrentSocket = socketBehaviour;
				}
			}
			if (CurrentSocket != null)
			{
				if (CurrentSocket.CheckPlaced(SelectedPrefab))
				{
					return;
				}
				PartOffset offsetPart = CurrentSocket.GetOffsetPart(SelectedPrefab.Id);
				if (offsetPart != null)
				{
					Vector3 position = CurrentSocket.transform.position + CurrentSocket.transform.TransformVector(offsetPart.Position);
					CurrentPreview.transform.position = position;
					if (CurrentPreview.RotateOnSockets)
					{
						CurrentPreview.transform.rotation = Quaternion.Euler(new Vector3(0f, GetTransform.eulerAngles.y + (float)CurrentPreview.SelfRotateAngle, 0f)) * Quaternion.Euler(CurrentPreview.RotationAxis);
					}
					else
					{
						CurrentPreview.transform.rotation = CurrentSocket.transform.rotation * Quaternion.Euler(offsetPart.Rotation) * Quaternion.Euler(new Vector3(0f, CurrentPreview.SelfRotateAngle, 0f));
					}
					CurrentPreview.transform.localScale = (offsetPart.UseCustomScale ? offsetPart.Scale : ((!(CurrentSocket.AttachedPart != null)) ? CurrentSocket.transform.localScale : CurrentSocket.AttachedPart.transform.localScale));
					LastSocket = CurrentSocket;
					needFreeMove = false;
					HasSocket = true;
					return;
				}
			}
			needFreeMove = false;
			UpdateFreeMovement();
		}

		protected void UpdatePreviewCollisions()
		{
			AllowPlacement = true;
			WhyCantBuild = CantBuildReson.Null;
			if (CurrentPreview.RequireSockets && !HasSocket)
			{
				AllowPlacement = false;
			}
			if (CheckOtherPlayerBuild())
			{
				if (AllowPlacement)
				{
					AllowPlacement = false;
					WhyCantBuild = CantBuildReson.HaveOtherBuildNear;
				}
				return;
			}
			if (CurrentPreview.EnterOther)
			{
				if (HasSocket && CurrentPreview.AvoidClippingOnSocket)
				{
					AllowPlacement = false;
					WhyCantBuild = CantBuildReson.TooNearToPart;
				}
				if (!HasSocket)
				{
					AllowPlacement = false;
					WhyCantBuild = CantBuildReson.TooNearToPart;
				}
			}
			RaycastHit hitInfo;
			if (CurrentPreview.Id == 5 || CurrentPreview.Id == 1)
			{
				CheckDijiTypeBuild();
			}
			else if (!CurrentPreview.RequireSockets && !HasSocket && !CurrentPreview.FreeMove && !Physics.SphereCast(CurrentPreview.transform.position + Vector3.up * 0.5f, 0.1f, Vector3.down, out hitInfo, 0.51f, SingletonMono<BuildManager>.Ins.GroundMask) && AllowPlacement)
			{
				AllowPlacement = false;
				WhyCantBuild = CantBuildReson.TooHigh;
			}
			if (HasSocket && AllowPlacement && CurrentPreview.InDic.Count < CurrentPreview.MyCfg.needSupportValue)
			{
				WhyCantBuild = CantBuildReson.NoEnoughSupport;
				AllowPlacement = false;
			}
			CheckFreeMoveTypeBuild();
		}

		private void CheckFreeMoveTypeBuild()
		{
			if (!CurrentPreview.FreeMove)
			{
				return;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(CurrentPreview.transform.position + Vector3.up, Vector3.down, out hitInfo, 1.1f, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
			{
				PartBehaviour componentInParent = hitInfo.transform.GetComponentInParent<PartBehaviour>();
				if (componentInParent != null)
				{
					if (!CanOnFreeMoveGoId.Contains(componentInParent.MyCfg.id))
					{
						if (AllowPlacement)
						{
							AllowPlacement = false;
						}
					}
					else if (componentInParent.AllowOp)
					{
						CurrentPreview.ParentInfo = componentInParent;
					}
					else if (AllowPlacement)
					{
						WhyCantBuild = CantBuildReson.notallow;
						AllowPlacement = false;
					}
				}
				else
				{
					CurrentPreview.ParentInfo = null;
					if (!CurrentPreview.CanGround && AllowPlacement)
					{
						AllowPlacement = false;
					}
				}
				if (hitInfo.collider.gameObject.layer == 18 && !CurrentPreview.CanGround && AllowPlacement)
				{
					AllowPlacement = false;
				}
			}
			else if (AllowPlacement)
			{
				AllowPlacement = false;
			}
		}

		private void CheckDijiTypeBuild()
		{
			float num = 0.2f;
			float num2 = 0f;
			float num3 = 2f;
			if (CurrentPreview.Id == 1)
			{
				num = 1.3f;
				num2 = 2.011f;
			}
			if (CurrentPreview.Id == 5)
			{
				num = 0.5f;
				num2 = 2.011f;
			}
			RaycastHit hitInfo;
			if (!Physics.SphereCast(CurrentPreview.transform.position + Vector3.up * (num2 + num3 + num), num, Vector3.down, out hitInfo, (!CurrentPreview.UseGroundUpper) ? (0.2f + num3) : (CurrentPreview.GroundUpperHeight + num3), SingletonMono<BuildManager>.Ins.GroundMask) && AllowPlacement)
			{
				AllowPlacement = false;
				WhyCantBuild = CantBuildReson.TooHigh;
			}
			if (Physics.OverlapBoxNonAlloc(CurrentPreview.transform.position + Vector3.up * num2, new Vector3(num, 0.1f, num), tmpColliderArray, CurrentPreview.transform.rotation, -5, QueryTriggerInteraction.Ignore) > 0 && !CurrentPreview.transform.isChild(tmpColliderArray[0].gameObject.transform) && AllowPlacement)
			{
				WhyCantBuild = CantBuildReson.TooNearToPart;
				AllowPlacement = false;
			}
		}

		private bool CheckOtherPlayerBuild()
		{
			if (!BuildPart.Get(CurrentPreview.MyCfg.id).needCheck)
			{
				return false;
			}
			PartBehaviour[] neighborsTypesByBox = PhysicExtension.GetNeighborsTypesByBox<PartBehaviour>(CurrentPreview.transform.position, new Vector3(8f, 8f, 8f), CurrentPreview.transform.rotation, CheckLayers);
			foreach (PartBehaviour partBehaviour in neighborsTypesByBox)
			{
				if (!(partBehaviour == CurrentPreview) && !partBehaviour.AllowOp)
				{
					WhyCantBuild = CantBuildReson.HaveOtherBuildNear;
					return true;
				}
			}
			return false;
		}

		public virtual void PlacePrefab()
		{
			if (AllowPlacement && !(CurrentPreview == null))
			{
				if (CurrentEditionPreview != null)
				{
					UnityEngine.Object.Destroy(CurrentEditionPreview.gameObject);
				}
				if (Source != null && PlacementClips.Length != 0)
				{
					Source.PlayOneShot(PlacementClips[UnityEngine.Random.Range(0, PlacementClips.Length)]);
				}
				CurrentRotationOffset = Vector3.zero;
				CurrentSocket = null;
				LastSocket = null;
				AllowPlacement = false;
				HasSocket = false;
				if (ResetModeAfterPlacement || ResetModeAfterEdition)
				{
					ChangeMode(BuildMode.None);
				}
				if (CurrentPreview != null)
				{
					UnityEngine.Object.Destroy(CurrentPreview.gameObject);
				}
			}
		}

		public bool WantPlacePrefab()
		{
			if (!AllowPlacement)
			{
				return false;
			}
			if (CurrentPreview == null)
			{
				return false;
			}
			CurrentPreview.CheckEnterBoxCollider();
			if (CurrentPreview.EnterOther && CurrentPreview.AvoidClippingOnSocket)
			{
				return false;
			}
			if (CurrentPreview.RequireSockets && CurrentPreview.InDic.Count < CurrentPreview.MyCfg.needSupportValue)
			{
				return false;
			}
			EventHandlers.WantPlacedPart(CurrentPreview, CurrentSocket);
			return true;
		}

		public void PlaceRootPrefabFromMsg(BuildPartInfo buildPartInfo)
		{
			if (SingletonMono<BuildManager>.Ins.PartDic.ContainsKey(buildPartInfo.instanceId))
			{
				return;
			}
			PartBehaviour part = SingletonMono<BuildManager>.Ins.GetPart(buildPartInfo.typeId);
			PartBehaviour partBehaviour = SingletonMono<BuildManager>.Ins.PlacePrefab(part, buildPartInfo);
			partBehaviour.OwnerRoleId = buildPartInfo.roleId;
			if (buildPartInfo.name.Length <= 0)
			{
				partBehaviour.MapObjectName = BuildPart.Get(buildPartInfo.typeId).name;
			}
			else
			{
				partBehaviour.MapObjectName = buildPartInfo.name;
			}
			if (buildPartInfo.roleId == SingletonMono<BuildManager>.Ins.SelfRoleId)
			{
				CurrentRotationOffset = Vector3.zero;
				CurrentSocket = null;
				LastSocket = null;
				AllowPlacement = false;
				HasSocket = false;
				if (ResetModeAfterPlacement)
				{
					ChangeMode(BuildMode.None);
				}
				if (CurrentPreview != null)
				{
					UnityEngine.Object.Destroy(CurrentPreview.gameObject);
				}
			}
			if (EventHandlers.OnBuildPart != null)
			{
				EventHandlers.OnBuildPart(buildPartInfo.instanceId, BuildPart.Get(buildPartInfo.typeId), buildPartInfo.status, buildPartInfo.extraInfo);
			}
		}

		public virtual PartBehaviour CreatePreview(GameObject prefab)
		{
			if (prefab == null)
			{
				return null;
			}
			Vector3 position = Vector3.zero;
			RaycastHit hitInfo;
			if (Physics.Raycast(GetRay(), out hitInfo, float.PositiveInfinity, (int)SingletonMono<BuildManager>.Ins.DefaultMask | (int)SingletonMono<BuildManager>.Ins.GroundMask, QueryTriggerInteraction.Ignore))
			{
				position = hitInfo.point;
			}
			CurrentPreview = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<PartBehaviour>();
			CurrentPreview.gameObject.SetActive(true);
			CurrentPreview.FillMeshAndMaterial();
			Collider[] componentsInChildren = CurrentPreview.transform.GetComponentsInChildren<Collider>(true);
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				if (collider is MeshCollider)
				{
					(collider as MeshCollider).convex = true;
				}
				collider.isTrigger = true;
			}
			BuildCollider component = CurrentPreview.GetComponent<BuildCollider>();
			if (component != null)
			{
				CurrentPreview.BoxColliders = component.BoxColliders;
			}
			CurrentPreview.ChangeState(StateType.Preview);
			SelectedPrefab = prefab.GetComponent<PartBehaviour>();
			if (UsePreviewCamera)
			{
				CurrentPreview.gameObject.SetLayerRecursively(PreviewLayer);
			}
			EventHandlers.PreviewCreated(CurrentPreview);
			CurrentSocket = null;
			LastSocket = null;
			AllowPlacement = false;
			HasSocket = false;
			return CurrentPreview;
		}

		public virtual void ClearPreview()
		{
			if (!(CurrentPreview == null))
			{
				PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(CurrentPreview.ProtoInsid);
				if (partByInsID != null)
				{
					partByInsID.gameObject.SetActiveBetter(true);
				}
				EventHandlers.PreviewCanceled(CurrentPreview);
				UnityEngine.Object.Destroy(CurrentPreview.gameObject);
				AllowPlacement = false;
				CurrentPreview = null;
				CurrentSocket = null;
				LastSocket = null;
				HasSocket = false;
			}
		}

		public bool PreviewExists()
		{
			return CurrentPreview;
		}

		public void UpdateCurAimPart()
		{
			if (Physics.SphereCast(BuilderCamera.transform.position, 0.1f, BuilderCamera.transform.forward, out Hit, 14f, AimLayers))
			{
				PartBehaviour componentInParent = Hit.collider.GetComponentInParent<PartBehaviour>();
				if (componentInParent != null)
				{
					if (Battle.Ins.SelfPlayer != null && Vector3.Distance(Battle.Ins.SelfPlayer.Pos, Hit.transform.position) > 6f)
					{
						NotAim();
						return;
					}
					if (m_lastAimGo != null && Hit.collider.gameObject != m_lastAimGo)
					{
						ResetNormalAim();
						Utils.TriggerEvent(EventHandlers.OnAimedGo, -1L);
						m_LastAimPart = null;
					}
					if (componentInParent != m_LastAimPart)
					{
						ResetBuildAimPreview();
					}
					CurrentAimPart = componentInParent;
					if (componentInParent != m_LastAimPart)
					{
						Utils.TriggerEvent(EventHandlers.OnAimedPart, componentInParent.InsId);
					}
					m_LastAimPart = CurrentAimPart;
					m_lastAimGo = Hit.collider.gameObject;
					return;
				}
				if (Battle.Ins.SelfPlayer != null && Vector3.Distance(Battle.Ins.SelfPlayer.Pos, Hit.transform.position) > 2.5f)
				{
					NotAim();
					return;
				}
				ResetBuildAimPreview();
				if (!(Hit.collider.gameObject != m_lastAimGo))
				{
					return;
				}
				ResetNormalAim();
				MapObject componentInParent2 = Hit.collider.GetComponentInParent<MapObject>();
				if (!(componentInParent2 == null))
				{
					CurAimMapObject = componentInParent2;
					if (CurAimMapObject != null)
					{
						Utils.TriggerEvent(EventHandlers.OnAimedGo, componentInParent2.InsId);
					}
					m_lastAimGo = Hit.collider.gameObject;
				}
			}
			else
			{
				NotAim();
			}
		}

		private void NotAim()
		{
			if (m_LastAimPart != null)
			{
				Utils.TriggerEvent(EventHandlers.OnAimedPart, -1L);
			}
			if (m_lastAimGo != null)
			{
				Utils.TriggerEvent(EventHandlers.OnAimedGo, -1L);
			}
			ResetAimPreview();
		}

		public void WantRemovePrefab()
		{
			if (!(CurrentAimPart == null))
			{
				if (CurrentAimPart.Lv > 3)
				{
					MessageBoxPanel.Show(Utils.GetString(340), _003CWantRemovePrefab_003Em__1);
				}
				else
				{
					EventHandlers.WantDestoryBuilding(CurrentAimPart.InsId);
				}
			}
		}

		public virtual void RemovePrefab()
		{
			if (!(CurrentAimPart == null) && AllowDestruction)
			{
				UnityEngine.Object.Destroy(CurrentAimPart.gameObject);
				if (Source != null && DestructionClips.Length != 0)
				{
					Source.PlayOneShot(DestructionClips[UnityEngine.Random.Range(0, DestructionClips.Length)]);
				}
				CurrentSocket = null;
				LastSocket = null;
				AllowDestruction = false;
				HasSocket = false;
				if (ResetModeAfterDestruction)
				{
					ChangeMode(BuildMode.None);
				}
			}
		}

		public virtual void RemovePrefabFromMsg(long InsId)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(InsId);
			if (partByInsID == null)
			{
				return;
			}
			SingletonMono<BuildManager>.Ins.PartDic.Remove(InsId);
			SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(partByInsID.MyCfg.destoryEffectId).path, partByInsID.Center);
			SingletonMono<AudioManager>.Ins.Play(partByInsID.MyCfg.destorySoundId, partByInsID.transform.position);
			UnityEngine.Object.Destroy(partByInsID.gameObject);
			if (partByInsID.IsSelf)
			{
				CurrentSocket = null;
				LastSocket = null;
				AllowDestruction = false;
				HasSocket = false;
				if (ResetModeAfterDestruction)
				{
					ChangeMode(BuildMode.None);
				}
			}
		}

		public void HitBuilding()
		{
		}

		public Vector3 GetForwardInSlop(Transform transform)
		{
			if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out Hit, 5f, SingletonMono<BuildManager>.Ins.GroundMask))
			{
				Vector3 normal = Hit.normal;
				Vector3 lhs = Vector3.Cross(normal, transform.forward);
				Vector3 vector2 = (transform.forward = Vector3.Cross(lhs, normal));
			}
			return transform.forward;
		}

		public void ChangeMode(BuildMode mode)
		{
			if (CurrentMode != mode)
			{
				if (CurrentMode == BuildMode.Placement)
				{
					ClearPreview();
				}
				if (CurrentMode == BuildMode.Aim)
				{
				}
				if (mode == BuildMode.None)
				{
					ClearPreview();
				}
				CurrentMode = mode;
				EventHandlers.BuildModeChanged(CurrentMode);
			}
		}

		public void SelectPrefab(PartBehaviour prefab)
		{
			if (!(prefab == null))
			{
				SelectedPrefab = SingletonMono<BuildManager>.Ins.GetPart(prefab.Id);
			}
		}

		public void SelectPrefab(int id)
		{
			SelectedPrefab = SingletonMono<BuildManager>.Ins.GetPart(id);
		}

		private void ChangePrefab(PartBehaviour part)
		{
			ChangeMode(BuildMode.None);
			ChangeMode(BuildMode.Placement);
			SelectPrefab(part);
		}

		public void ChangePrefab(int id, int insId = -1, long protoInsid = -1L)
		{
			ChangeMode(BuildMode.None);
			ChangeMode(BuildMode.Placement);
			SelectPrefab(id);
			SelectedPrefab.InsId = insId;
			SelectedPrefab.ProtoInsid = protoInsid;
		}

		public void RotatePreview(Vector3 rotateAxis)
		{
			if (!(CurrentPreview == null))
			{
				CurrentRotationOffset += rotateAxis;
			}
		}

		[CompilerGenerated]
		private void _003CStart_003Em__0()
		{
			ResetBuildAimPreview();
			ResetNormalAim();
		}

		[CompilerGenerated]
		private void _003CWantRemovePrefab_003Em__1()
		{
			EventHandlers.WantDestoryBuilding(CurrentAimPart.InsId);
		}
	}
}

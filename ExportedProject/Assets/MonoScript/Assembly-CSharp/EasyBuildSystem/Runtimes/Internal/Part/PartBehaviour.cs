using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part.Data;
using EasyBuildSystem.Runtimes.Internal.Socket;
using Share;
using UnityEngine;
using cfg;

namespace EasyBuildSystem.Runtimes.Internal.Part
{
	public class PartBehaviour : MapObject
	{
		[CompilerGenerated]
		private sealed class _003CFillMeshAndMaterial_003Ec__AnonStorey0
		{
			internal MeshFilter filter;

			internal string abPath;

			internal PartBehaviour _0024this;

			internal void _003C_003Em__0(Object mesh)
			{
				if (!_0024this.m_PartData || !(filter != null) || !mesh)
				{
					return;
				}
				filter.sharedMesh = mesh as Mesh;
				PartBehaviour part = SingletonMono<BuildManager>.Ins.GetPart(_0024this.Id);
				if (!part.m_PartData)
				{
					return;
				}
				foreach (MeshFilter meshFililter in part.m_PartData.MeshFililters)
				{
					if (meshFililter.sharedMesh == null && meshFililter.name == filter.name)
					{
						meshFililter.sharedMesh = filter.sharedMesh;
						ResMgr.Ins.AddRef(part.gameObject, abPath);
					}
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillMeshAndMaterial_003Ec__AnonStorey1
		{
			internal Renderer r;

			internal string abPath;

			internal PartBehaviour _0024this;

			internal void _003C_003Em__0(Object material)
			{
				if (!_0024this.m_PartData || !(r != null) || !material)
				{
					return;
				}
				r.enabled = true;
				r.sharedMaterial = material as Material;
				PartBehaviour part = SingletonMono<BuildManager>.Ins.GetPart(_0024this.Id);
				if (!part.m_PartData)
				{
					return;
				}
				foreach (Renderer render in part.m_PartData.Renders)
				{
					if (render.sharedMaterial == null && render.name == r.name)
					{
						render.sharedMaterial = r.sharedMaterial;
						ResMgr.Ins.AddRef(part.gameObject, abPath);
					}
				}
			}
		}

		public Octets Oc;

		public short SelfRotateAngle;

		public int StatesMsg;

		public PartData m_PartData;

		public Transform Lvptrans;

		public bool EnterOther;

		public Dictionary<long, byte> InDic = new Dictionary<long, byte>();

		public bool AdvancedFeatures;

		public short Id;

		public short LV1Id;

		public long ProtoInsid;

		public long OwnerRoleId;

		public int Lv = 1;

		public long ToolBoxId;

		public string Name = "New Part";

		public int Type = 2;

		public PartBehaviour[] OccupancyParts;

		public bool AvoidClipping = true;

		public bool AvoidClippingOnSocket;

		public bool AvoidAnchoredOnSocket;

		public bool RequireSockets;

		public bool CanGround;

		public BuildPart MyCfg;

		public BuildPart MyNextLvCfg;

		public bool NeedFollowSlopoe;

		public List<GameObject> LvGos = new List<GameObject>();

		public bool UseGroundUpper;

		public float GroundUpperHeight = 1f;

		public bool RotateOnSockets;

		public bool RotateAccordingSlope;

		public Vector3 RotationAxis = Vector3.up * 90f;

		public Vector3 PreviewOffset = new Vector3(0f, 0.03f, 0f);

		public GameObject[] PreviewDisableObjects;

		public GameObject[] PlacedDisableObjects;

		public MonoBehaviour[] PreviewDisableBehaviours;

		public Collider[] PreviewDisableColliders;

		public bool PreviewUseColorLerpTime;

		public float PreviewColorLerpTime = 15f;

		public Material PreviewMaterial;

		public BuildingEffects BuildingEffectsScript;

		public bool FreeMove = true;

		public PartBehaviour ParentInfo;

		public bool CanBuildForFreeMove;

		public Bounds MeshBounds;

		public bool UseConditionalPhysics;

		public LayerMask PhysicsLayers;

		public float PhysicsLifeTime = 5f;

		public bool PhysicsConvexOnAffected = true;

		public bool PhysicsOnlyStablePlacement;

		public string[] PhysicsIgnoreTags;

		public Detection[] CustomDetections;

		public bool UseTerrainPrevention;

		public Bounds TerrainBounds;

		[HideInInspector]
		public int EntityInstanceId;

		[HideInInspector]
		public StateType CurrentState = StateType.Placed;

		[HideInInspector]
		public StateType LastState;

		[HideInInspector]
		public bool AffectedByPhysics;

		[HideInInspector]
		public List<PartBehaviour> LinkedParts;

		[HideInInspector]
		public Dictionary<Renderer, Material[]> InitialsRenders = new Dictionary<Renderer, Material[]>();

		[HideInInspector]
		public List<Collider> Colliders = new List<Collider>();

		public BoxCollider[] BoxColliders;

		[HideInInspector]
		public SocketBehaviour[] Sockets;

		private bool Quitting;

		private int InitAppearanceIndex;

		public bool AllowOp
		{
			get
			{
				return Singleton<FriendPermitMgr>.Ins.HavePermit(ToolBoxId) || NoOwner;
			}
		}

		public bool HavePermit
		{
			get
			{
				return Singleton<FriendPermitMgr>.Ins.HavePermit(ToolBoxId);
			}
		}

		public Renderer CurLvMesh
		{
			get
			{
				if (LvGos.Count == 0)
				{
					Debug.LogError("xgd sb!!!   " + Id + "name   " + Name);
				}
				return LvGos[Lv - 1].GetComponentInChildren<Renderer>(true);
			}
		}

		public bool IsSelf
		{
			get
			{
				return OwnerRoleId == SingletonMono<BuildManager>.Ins.SelfRoleId;
			}
		}

		public bool NoOwner
		{
			get
			{
				return ToolBoxId == 0;
			}
		}

		public Vector3 Center
		{
			get
			{
				return GetWorldPartMeshBounds().center;
			}
		}

		public bool IsBasePart
		{
			get
			{
				return MyCfg.basic;
			}
		}

		private void OnDrawGizmos()
		{
		}

		public bool CanUpdate()
		{
			if (MyNextLvCfg != null)
			{
				if (Singleton<BagMgr>.Ins.GetItemNum(MyNextLvCfg.materialInfo.itemId) < MyNextLvCfg.materialInfo.needNum)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public void Init(BuildPart buildPart)
		{
			MyCfg = buildPart;
			MyNextLvCfg = BuildPart.Get(MyCfg.nextId);
			if (Id != MyCfg.id && buildPart.lv == 1)
			{
				Debug.LogError(MyCfg.name + " Id Not Same!!!!!!!!!!!!!!!!");
			}
			FreeMove = MyCfg.type == 10;
			if (FreeMove)
			{
				RequireSockets = false;
				MyCfg.needSupportValue = 0;
			}
			Id = (short)MyCfg.id;
			Name = MyCfg.name;
			MapObjectName = Name;
			Type = buildPart.type;
			SetHp(MyCfg.materialInfo.hp);
			MaxHp = MyCfg.materialInfo.hp;
			Lv = MyCfg.lv;
			UpdateMeshByLv(MyCfg.lv);
		}

		public void SetHp(int hp)
		{
			Hp = hp;
			if (!(Hp < 10000f))
			{
			}
		}

		public void UpdateMeshByLv(int lv)
		{
			for (int i = 0; i < LvGos.Count; i++)
			{
				if (i == lv - 1)
				{
					LvGos[i].SetActiveBetter(true);
				}
				else
				{
					LvGos[i].SetActiveBetter(false);
				}
			}
		}

		public GameObject GetMeshGoByLv(int lv)
		{
			return LvGos[lv - 1];
		}

		public void SetLvpAngle(float angle)
		{
			Lvptrans.SetEulerAnglesY(angle);
		}

		protected override void Awake()
		{
			base.Awake();
			Lvptrans = base.transform.Find("lvp");
			if (Lvptrans == null)
			{
				Debug.LogError(Id + "lvp is null!!!");
			}
			UseConditionalPhysics = false;
			MyCfg = BuildPart.Get(Id);
			m_PartData = GetComponent<PartData>();
			BuildingEffectsScript = GetComponent<BuildingEffects>();
			Colliders = GetComponentsInChildren<Collider>(true).vToList();
			Sockets = GetComponentsInChildren<SocketBehaviour>(true);
			for (int i = 0; i < Sockets.Length; i++)
			{
				Sockets[i].Index = (byte)i;
			}
			if (!AdvancedFeatures)
			{
				MeshBounds = base.gameObject.GetChildsBounds();
			}
		}

		public void FillMeshAndMaterial()
		{
			for (int i = 0; i < m_PartData.MeshFililters.Count; i++)
			{
				_003CFillMeshAndMaterial_003Ec__AnonStorey0 _003CFillMeshAndMaterial_003Ec__AnonStorey = new _003CFillMeshAndMaterial_003Ec__AnonStorey0();
				_003CFillMeshAndMaterial_003Ec__AnonStorey._0024this = this;
				_003CFillMeshAndMaterial_003Ec__AnonStorey.filter = m_PartData.MeshFililters[i];
				if (!(_003CFillMeshAndMaterial_003Ec__AnonStorey.filter == null) && !(_003CFillMeshAndMaterial_003Ec__AnonStorey.filter.sharedMesh != null))
				{
					string resName;
					if (m_PartData.MeshNames[i].Contains("!"))
					{
						_003CFillMeshAndMaterial_003Ec__AnonStorey.abPath = "partinfo_mesh/" + m_PartData.MeshNames[i].Substring(0, m_PartData.MeshNames[i].LastIndexOf("!")) + ".ab";
						resName = m_PartData.MeshNames[i].Substring(m_PartData.MeshNames[i].LastIndexOf("!") + 1);
					}
					else
					{
						_003CFillMeshAndMaterial_003Ec__AnonStorey.abPath = "partinfo_mesh/" + m_PartData.MeshNames[i] + ".ab";
						resName = m_PartData.MeshNames[i];
					}
					ResMgr.Ins.LoadAssetFromAB<Mesh>(_003CFillMeshAndMaterial_003Ec__AnonStorey.abPath, resName, _003CFillMeshAndMaterial_003Ec__AnonStorey.filter.gameObject, _003CFillMeshAndMaterial_003Ec__AnonStorey._003C_003Em__0);
				}
			}
			for (int j = 0; j < m_PartData.Renders.Count; j++)
			{
				_003CFillMeshAndMaterial_003Ec__AnonStorey1 _003CFillMeshAndMaterial_003Ec__AnonStorey2 = new _003CFillMeshAndMaterial_003Ec__AnonStorey1();
				_003CFillMeshAndMaterial_003Ec__AnonStorey2._0024this = this;
				_003CFillMeshAndMaterial_003Ec__AnonStorey2.r = m_PartData.Renders[j];
				if (!(_003CFillMeshAndMaterial_003Ec__AnonStorey2.r == null) && !(_003CFillMeshAndMaterial_003Ec__AnonStorey2.r.sharedMaterial != null))
				{
					_003CFillMeshAndMaterial_003Ec__AnonStorey2.r.enabled = false;
					_003CFillMeshAndMaterial_003Ec__AnonStorey2.abPath = "partinfo_material/" + m_PartData.MaterialNames[j] + ".ab";
					ResMgr.Ins.LoadAssetFromAB<Material>(_003CFillMeshAndMaterial_003Ec__AnonStorey2.abPath, m_PartData.MaterialNames[j], _003CFillMeshAndMaterial_003Ec__AnonStorey2.r.gameObject, _003CFillMeshAndMaterial_003Ec__AnonStorey2._003C_003Em__0);
				}
			}
		}

		public bool CheckEnterBoxCollider()
		{
			BoxCollider[] boxColliders = BoxColliders;
			foreach (BoxCollider boxCollider in boxColliders)
			{
				if (Physics.CheckBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, SingletonMono<BuilderBehaviour>.Ins.CheckLayers, QueryTriggerInteraction.Ignore))
				{
					EnterOther = true;
					return true;
				}
			}
			EnterOther = false;
			return false;
		}

		private void Start()
		{
			if (CurrentState != StateType.Preview)
			{
				SingletonMono<BuildManager>.Ins.AddPart(this);
				ChangeAreaState(State.Busy);
			}
		}

		private void CheckBusySpaceGoIsNull()
		{
			SocketBehaviour[] sockets = Sockets;
			foreach (SocketBehaviour socketBehaviour in sockets)
			{
				for (int num = socketBehaviour.BusySpaces.Count - 1; num >= 0; num--)
				{
					if (socketBehaviour.BusySpaces[num] == null)
					{
						socketBehaviour.BusySpaces.Remove(socketBehaviour.BusySpaces[num]);
					}
				}
			}
		}

		public void DoUpdate()
		{
			if (OwnerRoleId != SingletonMono<BuildManager>.Ins.SelfRoleId)
			{
				return;
			}
			CheckBusySpaceGoIsNull();
			bool flag = CurrentState == StateType.Placed;
			if (!flag)
			{
				GameObject[] previewDisableObjects = PreviewDisableObjects;
				foreach (GameObject gameObject in previewDisableObjects)
				{
					if ((bool)gameObject)
					{
						gameObject.SetActiveBetter(flag);
					}
				}
				MonoBehaviour[] previewDisableBehaviours = PreviewDisableBehaviours;
				foreach (MonoBehaviour monoBehaviour in previewDisableBehaviours)
				{
					if ((bool)monoBehaviour)
					{
						monoBehaviour.enabled = flag;
					}
				}
				Collider[] previewDisableColliders = PreviewDisableColliders;
				foreach (Collider collider in previewDisableColliders)
				{
					if ((bool)collider)
					{
						collider.enabled = flag;
					}
				}
				return;
			}
			GameObject[] placedDisableObjects = PlacedDisableObjects;
			foreach (GameObject gameObject2 in placedDisableObjects)
			{
				if ((bool)gameObject2)
				{
					gameObject2.SetActiveBetter(false);
				}
			}
		}

		private void OnDestroy()
		{
			EventHandlers.DestroyedPart(GetComponent<PartBehaviour>());
			SingletonMono<BuildManager>.Ins.RemovePart(this);
		}

		private void OnApplicationQuit()
		{
			Quitting = true;
		}

		public void ChangeState(StateType state)
		{
			if (SingletonMono<BuilderBehaviour>.Ins == null || (state != StateType.Preview && !AllowOp) || CurrentState == state)
			{
				return;
			}
			LastState = CurrentState;
			switch (state)
			{
			case StateType.Preview:
			{
				base.gameObject.ChangeAllMaterialsOutLineInChildren(CurLvMesh, (!SingletonMono<BuilderBehaviour>.Ins.AllowPlacement) ? SingletonMono<BuildManager>.Ins.PreviewDeniedColor : SingletonMono<BuildManager>.Ins.PreviewAllowedColor, true);
				GameObject[] previewDisableObjects2 = PreviewDisableObjects;
				foreach (GameObject gameObject3 in previewDisableObjects2)
				{
					if ((bool)gameObject3)
					{
						gameObject3.SetActive(false);
					}
				}
				MonoBehaviour[] previewDisableBehaviours2 = PreviewDisableBehaviours;
				foreach (MonoBehaviour monoBehaviour2 in previewDisableBehaviours2)
				{
					if ((bool)monoBehaviour2)
					{
						monoBehaviour2.enabled = false;
					}
				}
				Collider[] previewDisableColliders2 = PreviewDisableColliders;
				foreach (Collider collider2 in previewDisableColliders2)
				{
					if ((bool)collider2)
					{
						collider2.enabled = false;
					}
				}
				SocketBehaviour[] sockets2 = Sockets;
				foreach (SocketBehaviour socketBehaviour2 in sockets2)
				{
					socketBehaviour2.DisableCollider();
					socketBehaviour2.gameObject.SetActive(false);
				}
				break;
			}
			case StateType.Placed:
			{
				base.gameObject.ChangeAllMaterialsOutLineInChildren(CurLvMesh, SingletonMono<BuildManager>.Ins.AimColor, false);
				GameObject[] previewDisableObjects = PreviewDisableObjects;
				foreach (GameObject gameObject in previewDisableObjects)
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(true);
					}
				}
				GameObject[] placedDisableObjects = PlacedDisableObjects;
				foreach (GameObject gameObject2 in placedDisableObjects)
				{
					if ((bool)gameObject2)
					{
						gameObject2.SetActive(false);
					}
				}
				MonoBehaviour[] previewDisableBehaviours = PreviewDisableBehaviours;
				foreach (MonoBehaviour monoBehaviour in previewDisableBehaviours)
				{
					if ((bool)monoBehaviour)
					{
						monoBehaviour.enabled = false;
					}
				}
				EnableAllColliders();
				Collider[] previewDisableColliders = PreviewDisableColliders;
				foreach (Collider collider in previewDisableColliders)
				{
					if ((bool)collider)
					{
						collider.enabled = true;
					}
				}
				SocketBehaviour[] sockets = Sockets;
				foreach (SocketBehaviour socketBehaviour in sockets)
				{
					socketBehaviour.EnableCollider();
					socketBehaviour.gameObject.SetActive(true);
				}
				break;
			}
			}
			CurrentState = state;
			EventHandlers.PartStateChanged(this, state);
		}

		public void ChangeAreaState(State type)
		{
			Vector3 extents = GetWorldPartMeshBounds().extents;
			SocketBehaviour[] neighborsTypesByBox = PhysicExtension.GetNeighborsTypesByBox<SocketBehaviour>(GetWorldPartMeshBounds().center, KeepSizeNotToBig(extents), base.transform.rotation, 1 << LayerMask.NameToLayer("socket"), QueryTriggerInteraction.Collide);
			for (int i = 0; i < neighborsTypesByBox.Length; i++)
			{
				if (neighborsTypesByBox[i] != null)
				{
					if (type == State.Busy)
					{
						neighborsTypesByBox[i].CheckSelfToBusy();
					}
					else
					{
						neighborsTypesByBox[i].CheckSelfToFree();
					}
				}
			}
		}

		public void RemoveSelfSocketBusySpace()
		{
			SocketBehaviour[] sockets = Sockets;
			foreach (SocketBehaviour socketBehaviour in sockets)
			{
				socketBehaviour.BusySpaces.Clear();
			}
		}

		private Vector3 KeepSizeNotToBig(Vector3 extents)
		{
			if (extents.x > 5f)
			{
				extents.x = 5f;
			}
			if (extents.y > 5f)
			{
				extents.y = 5f;
			}
			if (extents.z > 5f)
			{
				extents.z = 5f;
			}
			return extents;
		}

		public Dictionary<long, byte> GetInDic()
		{
			InDic.Clear();
			Vector3 extents = GetWorldPartMeshBounds().extents;
			if (extents == Vector3.zero)
			{
				MeshBounds = base.gameObject.GetChildsBounds();
			}
			SocketBehaviour[] neighborsTypesByBox = PhysicExtension.GetNeighborsTypesByBox<SocketBehaviour>(GetWorldPartMeshBounds().center, KeepSizeNotToBig(extents), base.transform.rotation, 1 << LayerMask.NameToLayer("socket"), QueryTriggerInteraction.Collide);
			SocketBehaviour[] array = neighborsTypesByBox;
			foreach (SocketBehaviour socketBehaviour in array)
			{
				if (socketBehaviour != null && socketBehaviour.AttachedPart != null && socketBehaviour.AllowPart(this))
				{
					socketBehaviour.CheckInThisPart(this);
				}
			}
			return InDic;
		}

		public MapList<long, byte> GetOutDic()
		{
			MapList<long, byte> mapList = new MapList<long, byte>();
			SocketBehaviour[] sockets = Sockets;
			foreach (SocketBehaviour socketBehaviour in sockets)
			{
				socketBehaviour.GetOutDic(mapList);
			}
			return mapList;
		}

		public void ActiveAllTriggers()
		{
			foreach (Collider collider in Colliders)
			{
				if (collider != null)
				{
					collider.isTrigger = true;
				}
			}
		}

		public void DisableAllTriggers()
		{
			foreach (Collider collider in Colliders)
			{
				if (collider != null)
				{
					collider.isTrigger = false;
				}
			}
		}

		public void EnableAllColliders()
		{
			foreach (Collider collider in Colliders)
			{
				if (collider != null)
				{
					collider.enabled = true;
				}
			}
		}

		public void EnableArtlColliders()
		{
			BoxCollider[] boxColliders = BoxColliders;
			foreach (Collider collider in boxColliders)
			{
				if (collider != null)
				{
					collider.enabled = true;
				}
			}
		}

		public void DisableArtColliders()
		{
			BoxCollider[] boxColliders = BoxColliders;
			foreach (Collider collider in boxColliders)
			{
				if (collider != null)
				{
					collider.enabled = false;
				}
			}
		}

		public void DisableAllColliders()
		{
			foreach (Collider collider in Colliders)
			{
				if (collider != null)
				{
					collider.enabled = false;
				}
			}
		}

		public void EnableAllSockets()
		{
			SocketBehaviour[] sockets = Sockets;
			foreach (SocketBehaviour socketBehaviour in sockets)
			{
				socketBehaviour.gameObject.SetActiveBetter(true);
				socketBehaviour.EnableCollider();
			}
		}

		public void DisableAllSockets()
		{
			SocketBehaviour[] sockets = Sockets;
			foreach (SocketBehaviour socketBehaviour in sockets)
			{
				socketBehaviour.gameObject.SetActiveBetter(false);
				socketBehaviour.DisableCollider();
			}
		}

		public bool IsSupport(Collider collider)
		{
			return true;
		}

		public bool HasCollider(Collider collider)
		{
			for (int i = 0; i < Colliders.Count; i++)
			{
				if (Colliders[i] == collider)
				{
					return true;
				}
			}
			return false;
		}

		public Bounds GetWorldPartMeshBounds()
		{
			return base.transform.BoundsToWorld(MeshBounds);
		}

		public Bounds GetWorldPartTerrainBounds()
		{
			return base.transform.BoundsToWorld(TerrainBounds);
		}

		public bool NeedFix()
		{
			if (Hp >= (float)MyCfg.materialInfo.hp)
			{
				return false;
			}
			return true;
		}
	}
}

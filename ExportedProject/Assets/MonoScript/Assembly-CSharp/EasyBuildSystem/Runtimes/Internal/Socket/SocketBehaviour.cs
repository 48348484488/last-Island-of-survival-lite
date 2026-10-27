using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket.Data;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Socket
{
	public class SocketBehaviour : MonoBehaviour
	{
		public SocketType Type;

		public float Radius = 0.5f;

		public bool DisableOnGroundContact;

		public Bounds AttachmentBounds;

		public List<PartOffset> PartOffsets = new List<PartOffset>();

		public List<PartBehaviour> BusySpaces = new List<PartBehaviour>();

		public PartBehaviour AttachedPart;

		public bool IsDisabled;

		public byte Index;

		public List<SocketBehaviour> LinkedSocket;

		private void Awake()
		{
			AttachedPart = GetComponentInParent<PartBehaviour>();
			base.gameObject.layer = LayerMask.NameToLayer("socket");
			if (Type == SocketType.Socket)
			{
				base.gameObject.AddSphereCollider(Radius);
			}
			else
			{
				base.gameObject.AddBoxCollider(AttachmentBounds.extents, AttachmentBounds.center);
			}
		}

		private void Start()
		{
			if (AttachedPart.CurrentState != StateType.Preview)
			{
				SingletonMono<BuildManager>.Ins.AddSocket(this);
				CheckSelfToBusy();
			}
		}

		private void OnDestroy()
		{
			SingletonMono<BuildManager>.Ins.RemoveSocket(this);
		}

		public void CheckSelfToBusy()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, 1 << BattleScMgr.DefaultLayer);
			for (int i = 0; i < array.Length; i++)
			{
				PartBehaviour componentInParent = array[i].GetComponentInParent<PartBehaviour>();
				if (componentInParent != null && componentInParent != AttachedPart && componentInParent.CurrentState != StateType.Preview && AllowPart(componentInParent))
				{
					ChangeState(State.Busy, componentInParent);
				}
			}
		}

		public void CheckSelfToFree()
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, SingletonMono<BuilderBehaviour>.Ins.CheckLayers);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<SocketBehaviour>() == null)
				{
					PartBehaviour componentInParent = array[i].GetComponentInParent<PartBehaviour>();
					if (componentInParent != null && componentInParent != AttachedPart)
					{
						ChangeState(State.Free, componentInParent);
					}
				}
			}
		}

		public void CheckInThisPart(PartBehaviour part)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, SingletonMono<BuilderBehaviour>.Ins.CheckLayers);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.parent == AttachedPart.transform || !(array[i].GetComponent<SocketBehaviour>() == null))
				{
					continue;
				}
				PartBehaviour componentInParent = array[i].GetComponentInParent<PartBehaviour>();
				if (componentInParent != null && componentInParent == part && (part.MyCfg.type == 9 || (AttachedPart.Center.y < part.Center.y + 0.1f && part.MyCfg.type != 9)))
				{
					if (!part.InDic.ContainsKey(AttachedPart.InsId))
					{
						part.InDic.Add(AttachedPart.InsId, Index);
					}
					else
					{
						part.InDic[AttachedPart.InsId] = Index;
					}
				}
			}
		}

		public void GetOutDic(MapList<long, byte> dic)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, SingletonMono<BuilderBehaviour>.Ins.CheckLayers);
			for (int i = 0; i < array.Length; i++)
			{
				PartBehaviour componentInParent = array[i].GetComponentInParent<PartBehaviour>();
				if (componentInParent != null && componentInParent.InsId > 0 && componentInParent != AttachedPart && AllowPart(componentInParent) && AttachedPart.Center.y <= componentInParent.Center.y + 0.1f && !dic.ContainsKey(componentInParent.InsId))
				{
					dic.Add(componentInParent.InsId, Index);
				}
			}
		}

		public void DisableCollider()
		{
			if (!IsDisabled)
			{
				IsDisabled = true;
				if (GetComponent<Collider>() != null)
				{
					GetComponent<Collider>().gameObject.layer = 4;
				}
			}
		}

		public void EnableCollider()
		{
			if (IsDisabled)
			{
				IsDisabled = false;
				if (GetComponent<Collider>() != null)
				{
					GetComponent<Collider>().gameObject.layer = LayerMask.NameToLayer("socket");
				}
			}
		}

		public void EnableColliderByPartType(int type)
		{
			if (PartOffsets.Count > 0)
			{
				for (int i = 0; i < PartOffsets.Count; i++)
				{
					if (PartOffsets[i].Part != null && !CheckPlaced(PartOffsets[i].Part))
					{
						if (PartOffsets[i].Part.Type == type)
						{
							EnableCollider();
						}
					}
					else
					{
						EnableCollider();
					}
				}
			}
			else
			{
				DisableCollider();
			}
		}

		public bool AllowPart(PartBehaviour part)
		{
			PartOffset partOffset = null;
			int i = 0;
			for (int count = PartOffsets.Count; i < count; i++)
			{
				partOffset = PartOffsets[i];
				if (partOffset != null && partOffset.Part != null && partOffset.Part.LV1Id == part.LV1Id && !CheckPlaced(part))
				{
					return true;
				}
			}
			return false;
		}

		public bool AllowPartIgnorePlaced(PartBehaviour part)
		{
			for (int i = 0; i < PartOffsets.Count; i++)
			{
				if (PartOffsets[i] != null && PartOffsets[i].Part != null && PartOffsets[i].Part.LV1Id == part.LV1Id)
				{
					return true;
				}
			}
			return false;
		}

		public State GetState(int layer, PartBehaviour placedPart)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, 1 << layer, QueryTriggerInteraction.Ignore);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (AttachedPart != placedPart)
				{
					return State.Busy;
				}
				if (!AttachedPart.HasCollider(collider) && (collider.GetComponent<SocketBehaviour>() != null || collider.GetComponentInParent<PartBehaviour>() != null))
				{
					return State.Busy;
				}
			}
			return State.Free;
		}

		public bool CheckPlaced(PartBehaviour part)
		{
			for (int i = 0; i < BusySpaces.Count; i++)
			{
				if (BusySpaces[i] == null)
				{
					BusySpaces.Remove(BusySpaces[i]);
				}
				else if (BusySpaces[i].Type == part.Type)
				{
					return true;
				}
			}
			return false;
		}

		public void AddPlaced(PartBehaviour part)
		{
			if (!CheckPlaced(part) && part != AttachedPart)
			{
				BusySpaces.Add(part);
			}
		}

		public void RemovePlaced(PartBehaviour part)
		{
			if (CheckPlaced(part))
			{
				BusySpaces.Remove(part);
			}
		}

		public void ChangeState(State state, PartBehaviour part)
		{
			switch (state)
			{
			case State.Busy:
				if (!CheckPlaced(part))
				{
					AddPlaced(part);
					AddLinkedSocketPlaced(part);
				}
				break;
			case State.Free:
				if (CheckPlaced(part))
				{
					RemovePlaced(part);
					RemoveLinkedSocketPlaced(part);
				}
				break;
			}
		}

		public void AddLinkedSocketPlaced(PartBehaviour part)
		{
			foreach (SocketBehaviour item in LinkedSocket)
			{
				item.ChangeState(State.Busy, part);
			}
		}

		public void RemoveLinkedSocketPlaced(PartBehaviour part)
		{
			foreach (SocketBehaviour item in LinkedSocket)
			{
				item.ChangeState(State.Free, part);
			}
		}

		public PartOffset GetOffsetPart(int id)
		{
			for (int i = 0; i < PartOffsets.Count; i++)
			{
				if (PartOffsets[i].Part != null && PartOffsets[i].Part.LV1Id == id)
				{
					return PartOffsets[i];
				}
			}
			return null;
		}

		private void OnDrawGizmos()
		{
			if (!IsDisabled)
			{
				if (BusySpaces.Count != 0)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawCube(base.transform.position, Vector3.one / 6f);
				}
				else
				{
					Gizmos.color = Color.cyan;
					Gizmos.DrawCube(base.transform.position, Vector3.one / 6f);
				}
				if (Type == SocketType.Socket)
				{
					Gizmos.DrawWireCube(base.transform.position, Vector3.one / 6f);
					Gizmos.DrawWireSphere(base.transform.position, Radius);
				}
				else
				{
					Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
					Gizmos.DrawWireCube(AttachmentBounds.center, Vector3.one / 6f);
					Gizmos.DrawWireCube(AttachmentBounds.center, AttachmentBounds.extents);
				}
			}
		}
	}
}

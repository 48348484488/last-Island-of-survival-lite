using System;
using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Socket.Data
{
	[Serializable]
	public class PartOffset
	{
		public PartBehaviour Part;

		public Vector3 Position;

		public Vector3 Rotation;

		public bool UseCustomScale;

		public Vector3 Scale = Vector3.one;

		public short Id;

		public PartOffset(PartBehaviour part)
		{
			Part = part;
		}
	}
}

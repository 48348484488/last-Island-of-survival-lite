using System;
using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Part.Data
{
	[Serializable]
	public class Detection
	{
		public Vector3 Position;

		public Vector3 Size = Vector3.one;

		public SurfaceType[] RequiredSupports;

		public bool CheckType(int type)
		{
			return RequiredSupports.Contains((SurfaceType)type);
		}
	}
}

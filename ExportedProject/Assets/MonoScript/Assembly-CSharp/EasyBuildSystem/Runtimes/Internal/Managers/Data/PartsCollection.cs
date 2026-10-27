using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Internal.Managers.Data
{
	public class PartsCollection : ScriptableObject
	{
		public List<PartBehaviour> Parts = new List<PartBehaviour>();

		public bool CheckPartsCollection()
		{
			List<PartBehaviour> list = new List<PartBehaviour>();
			foreach (PartBehaviour part in Parts)
			{
				if (part != null)
				{
					list.Add(part);
				}
			}
			foreach (PartBehaviour item in list)
			{
				if (!(item != null))
				{
					continue;
				}
				foreach (PartBehaviour part2 in Parts)
				{
					if (part2 != null && item.Name != part2.Name && item.Id == part2.Id)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

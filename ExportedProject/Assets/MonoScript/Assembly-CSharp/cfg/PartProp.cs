using System;
using System.Collections.Generic;
using Share;

namespace cfg
{
	public class PartProp
	{
		public List<int> gunids = new List<int>();

		public List<int> ids = new List<int>();

		public List<float> values = new List<float>();

		public PartProp(Octets oc)
		{
			string text = oc.pop_string();
			string[] array = text.Split('|');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int item;
				try
				{
					item = int.Parse(array[i]);
				}
				catch (Exception)
				{
					continue;
				}
				gunids.Add(item);
			}
			string text2 = oc.pop_string();
			string[] array2 = text2.Split('|');
			int j = 0;
			for (int num2 = array2.Length; j < num2; j++)
			{
				int item2;
				try
				{
					item2 = int.Parse(array2[j]);
				}
				catch (Exception)
				{
					continue;
				}
				ids.Add(item2);
			}
			string text3 = oc.pop_string();
			string[] array3 = text3.Split('|');
			int k = 0;
			for (int num3 = array3.Length; k < num3; k++)
			{
				float item3;
				try
				{
					item3 = float.Parse(array3[k]);
				}
				catch (Exception)
				{
					continue;
				}
				values.Add(item3);
			}
		}
	}
}

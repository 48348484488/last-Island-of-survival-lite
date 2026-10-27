using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using UnityEngine;

public class DefaultBuilderBehaviour : BuilderBehaviour
{
	public bool CreativeMode = true;

	public AudioSource Audio;

	public override void UpdateModes()
	{
		base.UpdateModes();
	}

	private void UpdatePrefabSelection()
	{
		float axis = Input.GetAxis((!(InputsCollection != null)) ? "Mouse ScrollWheel" : InputsCollection.InputSwitchName);
		if (axis > 0f)
		{
			if (SelectedIndex < SingletonMono<BuildManager>.Ins.PartsCollections.Count - 1)
			{
				SelectedIndex++;
			}
			else
			{
				SelectedIndex = 0;
			}
		}
		else if (axis < 0f)
		{
			if (SelectedIndex > 0)
			{
				SelectedIndex--;
			}
			else
			{
				SelectedIndex = SingletonMono<BuildManager>.Ins.PartsCollections.Count - 1;
			}
		}
	}
}

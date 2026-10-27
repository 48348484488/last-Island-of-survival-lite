using UnityEngine;
using cfg;

public class SoundControll : MonoBehaviour
{
	public int PlaySound(int id)
	{
		SoundCfg soundCfg = SoundCfg.Get(id);
		if (soundCfg == null)
		{
			Debug.LogError("音效id不存在 id===" + id);
			return -1;
		}
		return SingletonMono<AudioManager>.Ins.Play(soundCfg.path, base.transform.position, false, soundCfg.volume, true, 0f, soundCfg.radius);
	}

	public int PlaySound(int id, float volumePercent)
	{
		SoundCfg soundCfg = SoundCfg.Get(id);
		if (soundCfg == null)
		{
			Debug.LogError("音效id不存在 id===" + id);
			return -1;
		}
		return SingletonMono<AudioManager>.Ins.Play(soundCfg.path, base.transform.position, false, (float)soundCfg.volume * volumePercent * 0.01f, true, 0f, soundCfg.radius);
	}
}

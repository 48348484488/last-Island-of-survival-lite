using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using cfg;

public class AudioManager : SingletonMono<AudioManager>
{
	[CompilerGenerated]
	private sealed class _003CStopMusic_003Ec__AnonStorey2
	{
		internal AudioBase audioBase;

		internal float _003C_003Em__0()
		{
			return audioBase.audioSource.volume;
		}

		internal void _003C_003Em__1(float x)
		{
			audioBase.audioSource.volume = x;
		}

		internal void _003C_003Em__2()
		{
			audioBase.audioSource.volume = 0f;
			audioBase.audioSource.Stop();
		}
	}

	[CompilerGenerated]
	private sealed class _003CStop_003Ec__AnonStorey3
	{
		internal AudioBase audioBase;

		internal float _003C_003Em__0()
		{
			return audioBase.audioSource.volume;
		}

		internal void _003C_003Em__1(float x)
		{
			audioBase.audioSource.volume = x;
		}

		internal void _003C_003Em__2()
		{
			audioBase.id = -1;
			audioBase.audioSource.volume = 0f;
			audioBase.audioSource.Stop();
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlayOnTarget_003Ec__AnonStorey4
	{
		internal AudioBase audioBase;

		internal bool loop;

		internal float volume;

		internal float pitch;

		internal AudioManager _0024this;

		internal void _003C_003Em__0(Object o)
		{
			if (audioBase != null && !(audioBase.audioSource == null))
			{
				audioBase.isLoading = false;
				audioBase.audioSource.clip = o as AudioClip;
				audioBase.audioSource.loop = loop;
				audioBase.audioSource.playOnAwake = false;
				audioBase.audioSource.volume = volume * _0024this._effectVolume;
				audioBase.audioSource.pitch = pitch;
				audioBase.audioSource.spatialBlend = 1f;
				audioBase.audioSource.rolloffMode = AudioRolloffMode.Linear;
				audioBase.audioSource.Play();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlay_003Ec__AnonStorey6
	{
		internal string abName;

		internal bool loop;

		internal float maxDis;

		internal bool is3d;

		internal float minDis;

		internal float volume;

		internal AudioManager _0024this;
	}

	[CompilerGenerated]
	private sealed class _003CPlay_003Ec__AnonStorey5
	{
		internal AudioBase audioBase;

		internal _003CPlay_003Ec__AnonStorey6 _003C_003Ef__ref_00246;

		internal void _003C_003Em__0(Object o)
		{
			if (!(audioBase.audioSource == null) && audioBase.id != -1)
			{
				audioBase.isLoading = false;
				audioBase.path = _003C_003Ef__ref_00246.abName;
				audioBase.audioSource.clip = o as AudioClip;
				audioBase.audioSource.loop = _003C_003Ef__ref_00246.loop;
				audioBase.audioSource.playOnAwake = true;
				audioBase.audioSource.maxDistance = _003C_003Ef__ref_00246.maxDis;
				audioBase.audioSource.rolloffMode = AudioRolloffMode.Linear;
				if (_003C_003Ef__ref_00246.is3d)
				{
					audioBase.audioSource.spatialBlend = 1f;
				}
				else
				{
					audioBase.audioSource.spatialBlend = 0f;
				}
				audioBase.audioSource.minDistance = _003C_003Ef__ref_00246.minDis;
				audioBase.audioSource.volume = _003C_003Ef__ref_00246.volume * 0.01f * _003C_003Ef__ref_00246._0024this._effectVolume;
				audioBase.audioSource.Play();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlay_003Ec__AnonStorey7
	{
		internal AudioBase au;

		internal _003CPlay_003Ec__AnonStorey6 _003C_003Ef__ref_00246;

		internal void _003C_003Em__0(Object o)
		{
			if (au != null && !(au.audioSource == null))
			{
				au.isLoading = false;
				au.path = _003C_003Ef__ref_00246.abName;
				au.audioSource.clip = o as AudioClip;
				au.audioSource.loop = _003C_003Ef__ref_00246.loop;
				au.audioSource.playOnAwake = true;
				au.audioSource.rolloffMode = AudioRolloffMode.Linear;
				if (_003C_003Ef__ref_00246.is3d)
				{
					au.audioSource.spatialBlend = 1f;
				}
				else
				{
					au.audioSource.spatialBlend = 0f;
				}
				au.audioSource.maxDistance = _003C_003Ef__ref_00246.maxDis;
				au.audioSource.minDistance = _003C_003Ef__ref_00246.minDis;
				au.audioSource.volume = _003C_003Ef__ref_00246.volume * 0.01f * _003C_003Ef__ref_00246._0024this._effectVolume;
				au.audioSource.rolloffMode = AudioRolloffMode.Custom;
				au.audioSource.Play();
			}
		}
	}

	private List<AudioBase> audioList;

	private int currentId;

	private AudioListener uiAudioListener;

	private float _bgVolume;

	private float _effectVolume = 1f;

	private bool _enableBgAudio = true;

	private bool _enableEffectAudio = true;

	private Dictionary<GameObject, List<AudioBase>> gameObject2AudioList = new Dictionary<GameObject, List<AudioBase>>();

	private AudioSource _bgAudioSource;

	private int audioLengh;

	public bool EnableEffectAudio
	{
		set
		{
			_enableEffectAudio = value;
			if (_enableEffectAudio)
			{
				PlayerPrefsData.bSoundEffect = true;
			}
			else
			{
				PlayerPrefsData.bSoundEffect = false;
			}
		}
	}

	public bool EnableBgAudio
	{
		set
		{
			_enableBgAudio = value;
			if (_enableBgAudio)
			{
				PlayMusicBg("Bgm_loading");
				PlayerPrefsData.bMusic = true;
			}
			else
			{
				StopMusicBg();
				PlayerPrefsData.bMusic = false;
			}
		}
	}

	public AudioSource BgAudioSource
	{
		get
		{
			return _bgAudioSource;
		}
	}

	public float BGVolume
	{
		get
		{
			return _bgVolume;
		}
		set
		{
			_bgVolume = value;
			if ((bool)_bgAudioSource)
			{
				_bgAudioSource.volume = _bgVolume;
			}
		}
	}

	public float EffectVolume
	{
		get
		{
			return _effectVolume;
		}
		set
		{
			OnSettingVolumeChange(value);
			_effectVolume = Mathf.Clamp(value, 1E-05f, 1f);
		}
	}

	public void Start()
	{
		StartCoroutine(ClearAudioRef());
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void PlayMusicBg(string musicName)
	{
		string empty = string.Empty;
		if (_bgAudioSource.clip != null)
		{
			empty = _bgAudioSource.clip.name;
		}
		if (empty != musicName && _enableBgAudio)
		{
			ResMgr.Ins.LoadAssetFromAB<AudioClip>("sound/" + musicName + ".ab", null, _bgAudioSource.gameObject, _003CPlayMusicBg_003Em__0);
		}
	}

	public void StopMusicBg()
	{
		_bgAudioSource.Stop();
		if (_bgAudioSource.clip != null)
		{
			_bgAudioSource.clip = null;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		audioList = new List<AudioBase>();
		uiAudioListener = GetComponent<AudioListener>();
		_bgAudioSource = base.gameObject.AddComponent<AudioSource>();
		_bgAudioSource.playOnAwake = false;
		_bgAudioSource.loop = true;
	}

	public void StopMusic(int id, bool needLerp = false)
	{
		using (List<AudioBase>.Enumerator enumerator = audioList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_003CStopMusic_003Ec__AnonStorey2 _003CStopMusic_003Ec__AnonStorey = new _003CStopMusic_003Ec__AnonStorey2();
				_003CStopMusic_003Ec__AnonStorey.audioBase = enumerator.Current;
				if (_003CStopMusic_003Ec__AnonStorey.audioBase.id == id)
				{
					_003CStopMusic_003Ec__AnonStorey.audioBase.id = -1;
					if (needLerp)
					{
						DOTween.To(_003CStopMusic_003Ec__AnonStorey._003C_003Em__0, _003CStopMusic_003Ec__AnonStorey._003C_003Em__1, 0f, 0.4f).OnComplete(_003CStopMusic_003Ec__AnonStorey._003C_003Em__2);
						continue;
					}
					_003CStopMusic_003Ec__AnonStorey.audioBase.audioSource.volume = 0f;
					_003CStopMusic_003Ec__AnonStorey.audioBase.audioSource.Stop();
				}
			}
		}
	}

	public void Stop(string abPath, bool needLerp = true)
	{
		if (string.IsNullOrEmpty(abPath))
		{
			return;
		}
		using (List<AudioBase>.Enumerator enumerator = audioList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_003CStop_003Ec__AnonStorey3 _003CStop_003Ec__AnonStorey = new _003CStop_003Ec__AnonStorey3();
				_003CStop_003Ec__AnonStorey.audioBase = enumerator.Current;
				if (_003CStop_003Ec__AnonStorey.audioBase.path == abPath)
				{
					if (needLerp)
					{
						DOTween.To(_003CStop_003Ec__AnonStorey._003C_003Em__0, _003CStop_003Ec__AnonStorey._003C_003Em__1, 0f, 1.5f).OnComplete(_003CStop_003Ec__AnonStorey._003C_003Em__2);
						continue;
					}
					_003CStop_003Ec__AnonStorey.audioBase.id = -1;
					_003CStop_003Ec__AnonStorey.audioBase.audioSource.volume = 0f;
					_003CStop_003Ec__AnonStorey.audioBase.audioSource.Stop();
				}
			}
		}
	}

	public void StopMusicBgFadeOut()
	{
		DOTween.To(_003CStopMusicBgFadeOut_003Em__1, _003CStopMusicBgFadeOut_003Em__2, 0f, 1.5f).OnComplete(_003CStopMusicBgFadeOut_003Em__3);
	}

	public int Play(int id)
	{
		if (id <= 0)
		{
			return -1;
		}
		return Play(SoundCfg.Get(id).path, Vector3.zero, false, 100f);
	}

	public int Play(int id, Vector3 point)
	{
		if (id <= 0)
		{
			return -1;
		}
		return Play(SoundCfg.Get(id).path, point, false, 100f);
	}

	public int PlayLoop(int id, Vector3 point)
	{
		if (id <= 0)
		{
			return -1;
		}
		return Play(SoundCfg.Get(id).path, point, true, 100f);
	}

	public int Play(string abName)
	{
		return Play(abName, Vector3.zero, false, 100f);
	}

	private int PlayBgAudio(string abName)
	{
		return Play(abName, Vector3.zero, true, 100f, false);
	}

	public int Play2D(int id)
	{
		if (id <= 0)
		{
			return -1;
		}
		return Play(SoundCfg.Get(id).path, Vector3.zero, false, 100f, false);
	}

	public int Play2D(string abName)
	{
		return Play(abName, Vector3.zero, false, 100f, false);
	}

	public int Play2DLoop(int id)
	{
		if (id <= 0)
		{
			return -1;
		}
		return Play(SoundCfg.Get(id).path, Vector3.zero, true, 100f, false);
	}

	public int Play2DLoop(string abName)
	{
		return Play(abName, Vector3.zero, true, 100f, false);
	}

	public int Play(string abName, Vector3 point, bool loop)
	{
		return Play(abName, point, loop, 100f);
	}

	public int Play(string abName, bool loop)
	{
		return Play(abName, Vector3.zero, loop, 100f, false);
	}

	public int Play(string abName, bool loop, float volume)
	{
		return Play(abName, Vector3.zero, loop, volume, false);
	}

	public int Play(string abName, Vector3 point)
	{
		return Play(abName, point, false, 100f);
	}

	public int Play(string abName, Vector3 point, float value)
	{
		return Play(abName, point, false, value);
	}

	public int Play(string abName, float value)
	{
		return Play(abName, Vector3.zero, false, value);
	}

	public int PlayButtonClick(string abName = "buttonclick")
	{
		return Play(abName, Vector3.zero, false, 100f, false);
	}

	private void PlayBattleBg(Dictionary<int, int> audioDictionary, List<string> audioNames)
	{
		StartCoroutine(WaitAudioLengh(audioDictionary, audioNames));
	}

	private void OnSettingVolumeChange(float newValue)
	{
		foreach (List<AudioBase> value in gameObject2AudioList.Values)
		{
			if (value == null || value.Count <= 0)
			{
				continue;
			}
			foreach (AudioBase item in value)
			{
				if (item != null && item.audioSource != null && (item.isLoading || item.audioSource.isPlaying))
				{
					item.audioSource.volume = item.audioSource.volume * newValue / EffectVolume;
				}
			}
		}
	}

	private IEnumerator WaitAudioLengh(Dictionary<int, int> audioDictionary, List<string> audioNames)
	{
		while (true)
		{
			yield return new WaitForSeconds(audioLengh);
			int random = Random.Range(0, audioDictionary.Count);
			int length = 0;
			if (audioDictionary.TryGetValue(random, out length))
			{
				audioLengh = length;
			}
			switch (random)
			{
			case 0:
				Play2D(audioNames[0]);
				break;
			case 1:
				Play2D(audioNames[1]);
				break;
			case 2:
				Play2D(audioNames[2]);
				break;
			default:
				Play2D(audioNames[0]);
				break;
			}
		}
	}

	private void SetAudioBase(AudioBase audioBase, Vector3 offset, float minDistance = 1f, float maxDistance = 500f, bool loop = false, float volume = 1f)
	{
		audioBase.audioSource.spatialBlend = 1f;
		audioBase.audioSource.minDistance = minDistance;
		audioBase.audioSource.maxDistance = maxDistance;
		audioBase.audioSource.loop = loop;
		audioBase.audioSource.volume = volume;
		audioBase.myTransform.localPosition = offset;
		audioBase.audioSource.rolloffMode = AudioRolloffMode.Linear;
	}

	public void PlayOnTarget(string abName, GameObject attachedGameObject, Vector3 offset, bool playMoreThanOne = false, float volume = 1f, float pitch = 1f, float minDist = 1f, float maxDistance = 500f, bool loop = false)
	{
		_003CPlayOnTarget_003Ec__AnonStorey4 _003CPlayOnTarget_003Ec__AnonStorey = new _003CPlayOnTarget_003Ec__AnonStorey4();
		_003CPlayOnTarget_003Ec__AnonStorey.loop = loop;
		_003CPlayOnTarget_003Ec__AnonStorey.volume = volume;
		_003CPlayOnTarget_003Ec__AnonStorey.pitch = pitch;
		_003CPlayOnTarget_003Ec__AnonStorey._0024this = this;
		if (string.IsNullOrEmpty(abName) || !_enableEffectAudio || attachedGameObject == null)
		{
			return;
		}
		_003CPlayOnTarget_003Ec__AnonStorey.audioBase = null;
		List<AudioBase> value;
		if (gameObject2AudioList.TryGetValue(attachedGameObject, out value))
		{
			foreach (AudioBase item in value)
			{
				if (abName.Equals(item.path))
				{
					if (!item.isLoading)
					{
						item.audioSource.pitch = _003CPlayOnTarget_003Ec__AnonStorey.pitch;
						item.audioSource.volume = _003CPlayOnTarget_003Ec__AnonStorey.volume * _effectVolume;
						if (!item.audioSource.isPlaying)
						{
							item.audioSource.Play();
							return;
						}
					}
					if (!playMoreThanOne)
					{
						return;
					}
				}
				if (!item.isLoading && !item.audioSource.isPlaying)
				{
					_003CPlayOnTarget_003Ec__AnonStorey.audioBase = item;
				}
			}
		}
		else
		{
			value = new List<AudioBase>();
			gameObject2AudioList.Add(attachedGameObject, value);
		}
		if (_003CPlayOnTarget_003Ec__AnonStorey.audioBase == null)
		{
			_003CPlayOnTarget_003Ec__AnonStorey.audioBase = new AudioBase();
			GameObject gameObject = new GameObject("Audio");
			gameObject.transform.SetParent(attachedGameObject.transform);
			_003CPlayOnTarget_003Ec__AnonStorey.audioBase.myTransform = gameObject.transform;
			_003CPlayOnTarget_003Ec__AnonStorey.audioBase.audioSource = gameObject.AddComponent<AudioSource>();
			_003CPlayOnTarget_003Ec__AnonStorey.audioBase.audioSource.spatialBlend = 1f;
			value.Add(_003CPlayOnTarget_003Ec__AnonStorey.audioBase);
		}
		_003CPlayOnTarget_003Ec__AnonStorey.audioBase.isLoading = true;
		_003CPlayOnTarget_003Ec__AnonStorey.audioBase.path = abName;
		SetAudioBase(_003CPlayOnTarget_003Ec__AnonStorey.audioBase, offset, minDist, maxDistance, _003CPlayOnTarget_003Ec__AnonStorey.loop, _003CPlayOnTarget_003Ec__AnonStorey.volume);
		ResMgr.Ins.LoadAssetFromAB<AudioClip>("sound/" + abName + ".ab", null, _003CPlayOnTarget_003Ec__AnonStorey.audioBase.audioSource.gameObject, _003CPlayOnTarget_003Ec__AnonStorey._003C_003Em__0);
	}

	public void StopPlayOnTarget(string abName, GameObject attachedGameObject)
	{
		List<AudioBase> value;
		if (string.IsNullOrEmpty(abName) || attachedGameObject == null || !gameObject2AudioList.TryGetValue(attachedGameObject, out value))
		{
			return;
		}
		foreach (AudioBase item in value)
		{
			if (abName.Equals(item.path) && !item.isLoading)
			{
				item.audioSource.volume = 0f;
				item.audioSource.Stop();
			}
		}
	}

	public bool IsSoundPlaying(string abName, GameObject attachedGameObject)
	{
		if (string.IsNullOrEmpty(abName) || attachedGameObject == null)
		{
			return false;
		}
		List<AudioBase> value;
		if (gameObject2AudioList.TryGetValue(attachedGameObject, out value))
		{
			foreach (AudioBase item in value)
			{
				if (abName.Equals(item.path) && item.audioSource.isPlaying)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void DestroyAttachedSound(GameObject go)
	{
		gameObject2AudioList.Remove(go);
	}

	public int Play(string abName, Vector3 point, bool loop, float volume, bool is3d = true, float minDis = 0f, float maxDis = 100f)
	{
		_003CPlay_003Ec__AnonStorey6 _003CPlay_003Ec__AnonStorey = new _003CPlay_003Ec__AnonStorey6();
		_003CPlay_003Ec__AnonStorey.abName = abName;
		_003CPlay_003Ec__AnonStorey.loop = loop;
		_003CPlay_003Ec__AnonStorey.maxDis = maxDis;
		_003CPlay_003Ec__AnonStorey.is3d = is3d;
		_003CPlay_003Ec__AnonStorey.minDis = minDis;
		_003CPlay_003Ec__AnonStorey.volume = volume;
		_003CPlay_003Ec__AnonStorey._0024this = this;
		if (_003CPlay_003Ec__AnonStorey.abName == null)
		{
			return -1;
		}
		if (!_enableEffectAudio)
		{
			return -1;
		}
		int num = currentId++;
		using (List<AudioBase>.Enumerator enumerator = audioList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_003CPlay_003Ec__AnonStorey5 _003CPlay_003Ec__AnonStorey2 = new _003CPlay_003Ec__AnonStorey5();
				_003CPlay_003Ec__AnonStorey2._003C_003Ef__ref_00246 = _003CPlay_003Ec__AnonStorey;
				_003CPlay_003Ec__AnonStorey2.audioBase = enumerator.Current;
				if (!_003CPlay_003Ec__AnonStorey2.audioBase.audioSource.isPlaying && !_003CPlay_003Ec__AnonStorey2.audioBase.isLoading)
				{
					_003CPlay_003Ec__AnonStorey2.audioBase.isLoading = true;
					_003CPlay_003Ec__AnonStorey2.audioBase.id = num;
					_003CPlay_003Ec__AnonStorey2.audioBase.myTransform.position = point;
					ResMgr.Ins.CleanRef(_003CPlay_003Ec__AnonStorey2.audioBase.audioSource.gameObject);
					ResMgr.Ins.LoadAssetFromAB<AudioClip>("sound/" + _003CPlay_003Ec__AnonStorey.abName + ".ab", null, _003CPlay_003Ec__AnonStorey2.audioBase.audioSource.gameObject, _003CPlay_003Ec__AnonStorey2._003C_003Em__0);
					return num;
				}
			}
		}
		if (audioList.Count < 50)
		{
			_003CPlay_003Ec__AnonStorey7 _003CPlay_003Ec__AnonStorey3 = new _003CPlay_003Ec__AnonStorey7();
			_003CPlay_003Ec__AnonStorey3._003C_003Ef__ref_00246 = _003CPlay_003Ec__AnonStorey;
			_003CPlay_003Ec__AnonStorey3.au = new AudioBase();
			GameObject gameObject = new GameObject("sound");
			gameObject.transform.SetParent(base.gameObject.transform, false);
			_003CPlay_003Ec__AnonStorey3.au.audioSource = gameObject.AddComponent<AudioSource>();
			_003CPlay_003Ec__AnonStorey3.au.myTransform = gameObject.transform;
			audioList.Add(_003CPlay_003Ec__AnonStorey3.au);
			_003CPlay_003Ec__AnonStorey3.au.isLoading = true;
			_003CPlay_003Ec__AnonStorey3.au.id = num;
			_003CPlay_003Ec__AnonStorey3.au.myTransform.position = point;
			ResMgr.Ins.LoadAssetFromAB<AudioClip>("sound/" + _003CPlay_003Ec__AnonStorey.abName + ".ab", null, _003CPlay_003Ec__AnonStorey3.au.audioSource.gameObject, _003CPlay_003Ec__AnonStorey3._003C_003Em__0);
			return num;
		}
		return -1;
	}

	public void DisableUIAudioListener()
	{
		uiAudioListener.enabled = false;
	}

	public void EableUIAudioListener()
	{
		uiAudioListener.enabled = base.enabled;
	}

	public void ClearAudio()
	{
		foreach (AudioBase audio in audioList)
		{
			audio.audioSource.clip = null;
			audio.path = string.Empty;
			audio.id = -1;
			audio.isLoading = false;
			audio.isBgAudio = false;
		}
	}

	private IEnumerator ClearAudioRef()
	{
		while (true)
		{
			yield return new WaitForSeconds(90f);
			for (int i = 0; i < audioList.Count; i++)
			{
				if (!audioList[i].audioSource.isPlaying)
				{
					ResMgr.Ins.CleanRef(audioList[i].myTransform.gameObject);
					yield return null;
				}
			}
		}
	}

	[CompilerGenerated]
	private void _003CPlayMusicBg_003Em__0(Object o)
	{
		_bgAudioSource.clip = o as AudioClip;
		_bgAudioSource.loop = true;
		_bgAudioSource.playOnAwake = false;
		_bgAudioSource.volume = BGVolume;
		_bgAudioSource.Play();
	}

	[CompilerGenerated]
	private float _003CStopMusicBgFadeOut_003Em__1()
	{
		return _bgAudioSource.volume;
	}

	[CompilerGenerated]
	private void _003CStopMusicBgFadeOut_003Em__2(float x)
	{
		_bgAudioSource.volume = x;
	}

	[CompilerGenerated]
	private void _003CStopMusicBgFadeOut_003Em__3()
	{
		_bgAudioSource.volume = 0f;
		_bgAudioSource.Stop();
	}
}

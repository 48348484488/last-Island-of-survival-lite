using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InlineManager : MonoBehaviour
{
	private class SpriteGraphicInfo
	{
		public SpriteGraphic _SpriteGraphic;

		public Mesh _Mesh;
	}

	private class MeshInfo
	{
		public string[] _Tag;

		public Vector3[] _Vertices;

		public Vector2[] _UV;

		public int[] _Triangles;

		public bool Equals(MeshInfo _value)
		{
			if (_Tag.Length != _value._Tag.Length || _Vertices.Length != _value._Vertices.Length)
			{
				return false;
			}
			for (int i = 0; i < _Tag.Length; i++)
			{
				if (_Tag[i] != _value._Tag[i])
				{
					return false;
				}
			}
			for (int j = 0; j < _Vertices.Length; j++)
			{
				if (_Vertices[j] != _value._Vertices[j])
				{
					return false;
				}
			}
			return true;
		}
	}

	public Dictionary<int, Dictionary<string, SpriteInforGroup>> _IndexSpriteInfo = new Dictionary<int, Dictionary<string, SpriteInforGroup>>();

	private Dictionary<int, SpriteGraphicInfo> _IndexSpriteGraphic = new Dictionary<int, SpriteGraphicInfo>();

	private Dictionary<int, Dictionary<InlineText, MeshInfo>> _TextMeshInfo = new Dictionary<int, Dictionary<InlineText, MeshInfo>>();

	[SerializeField]
	private bool _IsStatic;

	[SerializeField]
	[Range(1f, 10f)]
	private float _AnimationSpeed = 5f;

	private Coroutine LoadSprite;

	private float _animationTime;

	private int _AnimationIndex;

	private void Start()
	{
		Initialize();
	}

	private void Update()
	{
	}

	private IEnumerator LoadSpriteGraphic()
	{
		SpriteGraphic[] _spriteGraphic = GetComponentsInChildren<SpriteGraphic>();
		if (_spriteGraphic.Length > 0)
		{
			while (_spriteGraphic[0].m_spriteAsset == null)
			{
				yield return new WaitForSeconds(1f);
				Initialize();
			}
		}
		StopCoroutine(LoadSprite);
	}

	private void Initialize()
	{
		if (!base.gameObject)
		{
			StopCoroutine(LoadSprite);
			return;
		}
		SpriteGraphic[] componentsInChildren = GetComponentsInChildren<SpriteGraphic>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SpriteAsset spriteAsset = componentsInChildren[i].m_spriteAsset;
			if (spriteAsset == null && LoadSprite == null)
			{
				LoadSprite = StartCoroutine(LoadSpriteGraphic());
				break;
			}
			if (spriteAsset == null)
			{
				break;
			}
			if (_IndexSpriteGraphic.ContainsKey(spriteAsset.ID) || _IndexSpriteInfo.ContainsKey(spriteAsset.ID))
			{
				continue;
			}
			SpriteGraphicInfo spriteGraphicInfo = new SpriteGraphicInfo();
			spriteGraphicInfo._SpriteGraphic = componentsInChildren[i];
			spriteGraphicInfo._Mesh = new Mesh();
			SpriteGraphicInfo value = spriteGraphicInfo;
			_IndexSpriteGraphic.Add(spriteAsset.ID, value);
			Dictionary<string, SpriteInforGroup> dictionary = new Dictionary<string, SpriteInforGroup>();
			foreach (SpriteInforGroup item in spriteAsset.listSpriteGroup)
			{
				if (!dictionary.ContainsKey(item.tag) && item.listSpriteInfor != null && item.listSpriteInfor.Count > 0)
				{
					dictionary.Add(item.tag, item);
				}
			}
			_IndexSpriteInfo.Add(spriteAsset.ID, dictionary);
			_TextMeshInfo.Add(spriteAsset.ID, new Dictionary<InlineText, MeshInfo>());
		}
	}

	public void UpdateTextInfo(int _id, InlineText _key, List<SpriteTagInfo> _value)
	{
		if (!_IndexSpriteGraphic.ContainsKey(_id) || !_TextMeshInfo.ContainsKey(_id) || _value.Count <= 0)
		{
			return;
		}
		int count = _value.Count;
		Vector3 position = _key.transform.position;
		Vector3 position2 = _IndexSpriteGraphic[_id]._SpriteGraphic.transform.position;
		Vector3 vector = (position - position2) * (1f / _key.pixelsPerUnit);
		MeshInfo meshInfo = new MeshInfo();
		meshInfo._Tag = new string[count];
		meshInfo._Vertices = new Vector3[count * 4];
		meshInfo._UV = new Vector2[count * 4];
		meshInfo._Triangles = new int[count * 6];
		for (int i = 0; i < _value.Count; i++)
		{
			int num = i * 4;
			meshInfo._Tag[i] = _value[i]._Tag;
			meshInfo._Vertices[num] = _value[i]._Pos[0] + vector;
			meshInfo._Vertices[num + 1] = _value[i]._Pos[1] + vector;
			meshInfo._Vertices[num + 2] = _value[i]._Pos[2] + vector;
			meshInfo._Vertices[num + 3] = _value[i]._Pos[3] + vector;
			meshInfo._UV[num] = _value[i]._UV[0];
			meshInfo._UV[num + 1] = _value[i]._UV[1];
			meshInfo._UV[num + 2] = _value[i]._UV[2];
			meshInfo._UV[num + 3] = _value[i]._UV[3];
		}
		if (_TextMeshInfo[_id].ContainsKey(_key))
		{
			MeshInfo value = _TextMeshInfo[_id][_key];
			if (meshInfo.Equals(value))
			{
				return;
			}
			_TextMeshInfo[_id][_key] = meshInfo;
		}
		else
		{
			_TextMeshInfo[_id].Add(_key, meshInfo);
		}
		DrawSprites(_id);
	}

	public void RemoveTextInfo(int _id, InlineText _key)
	{
		if (_TextMeshInfo.ContainsKey(_id) && !_TextMeshInfo[_id].ContainsKey(_key))
		{
			_TextMeshInfo.Remove(_id);
			DrawSprites(_id);
		}
	}

	private void DrawSpriteAnimation()
	{
		_animationTime += Time.deltaTime * _AnimationSpeed;
		if (!(_animationTime >= 1f))
		{
			return;
		}
		_AnimationIndex++;
		foreach (KeyValuePair<int, SpriteGraphicInfo> item in _IndexSpriteGraphic)
		{
			if (item.Value._SpriteGraphic.m_spriteAsset._IsStatic || !_TextMeshInfo.ContainsKey(item.Key) || _TextMeshInfo[item.Key].Count <= 0)
			{
				continue;
			}
			Dictionary<InlineText, MeshInfo> dictionary = _TextMeshInfo[item.Key];
			foreach (KeyValuePair<InlineText, MeshInfo> item2 in dictionary)
			{
				for (int i = 0; i < item2.Value._Tag.Length; i++)
				{
					List<SpriteInfor> listSpriteInfor = _IndexSpriteInfo[item.Key][item2.Value._Tag[i]].listSpriteInfor;
					if (listSpriteInfor.Count > 1)
					{
						int index = _AnimationIndex % listSpriteInfor.Count;
						int num = i * 4;
						item2.Value._UV[num] = listSpriteInfor[index].uv[0];
						item2.Value._UV[num + 1] = listSpriteInfor[index].uv[1];
						item2.Value._UV[num + 2] = listSpriteInfor[index].uv[2];
						item2.Value._UV[num + 3] = listSpriteInfor[index].uv[3];
					}
				}
			}
			DrawSprites(item.Key);
		}
		_animationTime = 0f;
	}

	private void DrawSprites(int _id)
	{
		if (!_IndexSpriteGraphic.ContainsKey(_id) || !_TextMeshInfo.ContainsKey(_id))
		{
			return;
		}
		SpriteGraphic spriteGraphic = _IndexSpriteGraphic[_id]._SpriteGraphic;
		Mesh mesh = _IndexSpriteGraphic[_id]._Mesh;
		Dictionary<InlineText, MeshInfo> dictionary = _TextMeshInfo[_id];
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		foreach (KeyValuePair<InlineText, MeshInfo> item in dictionary)
		{
			for (int i = 0; i < item.Value._Vertices.Length; i++)
			{
				list.Add(item.Value._Vertices[i]);
				list2.Add(item.Value._UV[i]);
			}
			for (int j = 0; j < item.Value._Triangles.Length; j++)
			{
				list3.Add(item.Value._Triangles[j]);
			}
		}
		for (int k = 0; k < list3.Count; k++)
		{
			if (k % 6 == 0)
			{
				int num = k / 6;
				list3[k] = 4 * num;
				list3[k + 1] = 1 + 4 * num;
				list3[k + 2] = 2 + 4 * num;
				list3[k + 3] = 4 * num;
				list3[k + 4] = 2 + 4 * num;
				list3[k + 5] = 3 + 4 * num;
			}
		}
		mesh.Clear();
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
		spriteGraphic.canvasRenderer.SetMesh(mesh);
		spriteGraphic.UpdateMaterial();
	}
}

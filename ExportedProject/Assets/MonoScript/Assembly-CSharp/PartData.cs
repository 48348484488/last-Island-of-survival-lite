using System.Collections.Generic;
using UnityEngine;

public class PartData : MonoBehaviour
{
	[SerializeField]
	public List<MeshFilter> MeshFililters = new List<MeshFilter>();

	[SerializeField]
	public List<string> MeshNames = new List<string>();

	[SerializeField]
	public List<Renderer> Renders = new List<Renderer>();

	[SerializeField]
	public List<string> MaterialNames = new List<string>();
}

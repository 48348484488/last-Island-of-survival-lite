using UnityEngine;

public sealed class InstancingDrawCall
{
	public int meshInsId;

	public int materialInsId;

	public Matrix4x4[] matrices = new Matrix4x4[1000];

	public int objCount;

	public bool castShadows = true;

	public bool receiveShadows = true;

	public void Reset()
	{
		meshInsId = 0;
		materialInsId = 0;
		objCount = 0;
	}
}

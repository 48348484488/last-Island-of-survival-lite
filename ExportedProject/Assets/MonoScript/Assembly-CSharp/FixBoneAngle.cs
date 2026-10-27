using UnityEngine;

public class FixBoneAngle : MonoBehaviour
{
	public Transform Bone1;

	public Transform Bone2;

	private void FixedUpdate()
	{
		Bone2.rotation = Bone1.rotation;
	}
}

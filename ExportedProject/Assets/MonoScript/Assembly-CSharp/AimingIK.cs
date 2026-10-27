using UnityEngine;

public class AimingIK : MonoBehaviour
{
	public Transform rightUpperArm;

	public Transform rightHand;

	public void Init(Transform rightUpperArm, Transform rightHand)
	{
		this.rightUpperArm = rightUpperArm;
		this.rightHand = rightHand;
	}

	public virtual void SetAimPosition(Vector3 aimPosition, Transform aimReference)
	{
		RotateRightArm(aimPosition, aimReference);
		RotateRightHand(aimPosition, aimReference);
	}

	private void RotateRightArm(Vector3 aimPosition, Transform aimReference)
	{
		Vector3 toDirection = aimPosition - rightUpperArm.position;
		Vector3 forward = aimReference.forward;
		Quaternion rotation = Quaternion.FromToRotation(forward, toDirection) * rightUpperArm.rotation;
		rightUpperArm.rotation = rotation;
	}

	private void RotateRightHand(Vector3 aimPosition, Transform aimReference)
	{
		Vector3 toDirection = aimPosition - aimReference.position;
		Vector3 forward = aimReference.forward;
		Quaternion rotation = Quaternion.FromToRotation(forward, toDirection) * rightHand.rotation;
		rightHand.rotation = rotation;
	}
}

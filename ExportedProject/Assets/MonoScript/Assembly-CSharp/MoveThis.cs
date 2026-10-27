using UnityEngine;

public class MoveThis : MonoBehaviour
{
	public float translationSpeedX;

	public float translationSpeedY = 1f;

	public float translationSpeedZ;

	private void Update()
	{
		base.transform.Translate(new Vector3(translationSpeedX, translationSpeedY, translationSpeedZ) * Time.deltaTime);
	}
}

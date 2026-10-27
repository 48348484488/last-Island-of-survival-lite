using UnityEngine;

public interface ICameraFrustumCull
{
	bool pointInFrustum(Vector3 camPos, Vector2 pos);

	void UpdateVisibleDistance(float visibleDis);

	bool IsPointInCircularSector3(Vector2 pos, ref float distance);
}

using UnityEngine;

[RequireComponent(typeof(InterpolatedTransformUpdater))]
public class InterpolatedTransform : MonoBehaviour
{
	private struct TransformData
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}
	}

	private TransformData[] m_lastTransforms;

	private int m_newTransformIndex;

	private void OnEnable()
	{
		ForgetPreviousTransforms();
	}

	public void ForgetPreviousTransforms()
	{
		m_lastTransforms = new TransformData[2];
		TransformData transformData = new TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
		m_lastTransforms[0] = transformData;
		m_lastTransforms[1] = transformData;
		m_newTransformIndex = 0;
	}

	private void FixedUpdate()
	{
		TransformData transformData = m_lastTransforms[m_newTransformIndex];
		base.transform.localPosition = transformData.position;
		base.transform.localScale = transformData.scale;
		Debug.LogError("FixedUpdate" + base.transform.localPosition);
	}

	private void OnAnimatorMove()
	{
		m_newTransformIndex = OldTransformIndex();
		m_lastTransforms[m_newTransformIndex] = new TransformData(base.transform.localPosition, base.transform.localRotation, base.transform.localScale);
		Debug.LogError("LateFixedUpdate" + base.transform.localPosition);
	}

	public void LateFixedUpdate()
	{
	}

	private void Update()
	{
		TransformData transformData = m_lastTransforms[m_newTransformIndex];
		TransformData transformData2 = m_lastTransforms[OldTransformIndex()];
		base.transform.localPosition = Vector3.Lerp(transformData2.position, transformData.position, InterpolationController.InterpolationFactor);
		base.transform.localScale = Vector3.Lerp(transformData2.scale, transformData.scale, InterpolationController.InterpolationFactor);
		Debug.LogError("Update" + base.transform.localPosition);
	}

	private int OldTransformIndex()
	{
		return (m_newTransformIndex == 0) ? 1 : 0;
	}
}

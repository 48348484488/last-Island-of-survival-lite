using UnityEngine;

public class CameraMoveFree : MonoBehaviour
{
	public float _moveSpeed = 0.5f;

	public float _rotateSpeed = 1f;

	private vThirdPersonCamera _camera;

	private Camera m_Camera;

	private void Start()
	{
		m_Camera = Camera.main;
		_camera = m_Camera.GetComponent<vThirdPersonCamera>();
	}

	public void Open()
	{
		base.transform.position = Battle.Ins.SelfPlayer.transform.position;
		_camera.SetMainTarget(base.transform);
	}

	public void Close()
	{
		_camera.SetMainTarget(Battle.Ins.SelfPlayer.transform);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			base.gameObject.transform.Translate(_camera.transform.forward * _moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			base.gameObject.transform.Translate(-_camera.transform.forward * _moveSpeed * Time.deltaTime, Space.World);
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.gameObject.transform.Translate(-_camera.transform.right * _moveSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D))
		{
			base.gameObject.transform.Translate(_camera.transform.right * _moveSpeed * Time.deltaTime, Space.World);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			base.gameObject.transform.Translate(new Vector3(0f, -1f * _moveSpeed * Time.deltaTime, 0f), base.gameObject.transform);
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.gameObject.transform.Translate(new Vector3(0f, Time.deltaTime * _moveSpeed, 0f), base.gameObject.transform);
		}
		if (Input.GetKey(KeyCode.KeypadMinus))
		{
			_moveSpeed -= 1f;
		}
		if (Input.GetKey(KeyCode.KeypadPlus))
		{
			_moveSpeed += 1f;
		}
		if (Input.GetKey(KeyCode.LeftBracket))
		{
			_rotateSpeed -= 1f;
		}
		if (Input.GetKey(KeyCode.RightBracket))
		{
			_rotateSpeed += 1f;
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.gameObject.transform.Translate(new Vector3(0f, Time.deltaTime * _moveSpeed, 0f), base.gameObject.transform);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Battle.Ins.MainCamera.DragCamera(0f, _rotateSpeed);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Battle.Ins.MainCamera.DragCamera(0f, 0f - _rotateSpeed);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			Battle.Ins.MainCamera.DragCamera(0f - _rotateSpeed, 0f);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			Battle.Ins.MainCamera.DragCamera(_rotateSpeed, 0f);
		}
		if (Input.GetKey(KeyCode.H))
		{
			Battle.Ins.SelfPlayer.gameObject.SetActiveBetter(false);
		}
		if (Input.GetKey(KeyCode.G))
		{
			Battle.Ins.SelfPlayer.gameObject.SetActiveBetter(true);
		}
	}
}

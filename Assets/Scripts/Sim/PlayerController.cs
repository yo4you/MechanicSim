using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Animator _animator;
	private Camera _camera;
	private CameraControl _cameraController;
	[SerializeField]
	private float _moveSpeed = 5f;

	void Start()
	{
		_animator = GetComponent<Animator>();
		_camera = Camera.main;
		_cameraController =FindObjectOfType<CameraControl>();
	}

	void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			_animator.Play("Jump");
		}

		var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		var strafe = Input.GetAxis("Strafe");

		var cameraForward = _camera.transform.forward;
		cameraForward.y = 0;
		cameraForward.Normalize();

		var cameraRight = _camera.transform.right;
		cameraRight.y = 0;
		cameraRight.Normalize();

		var camangle = Vector2.SignedAngle(new Vector2(cameraForward.x, cameraForward.z), Vector2.left);
		bool strafing = strafe != 0 || (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E));

		if (input.magnitude > 0.01f || strafing)
		{
			var inputAngle = Vector2.SignedAngle(input.normalized, Vector2.right);
			
			if (strafing)
			{
				var turnedInput = input.normalized;
				turnedInput.y = Mathf.Abs(turnedInput.y);
				inputAngle = Vector2.SignedAngle(turnedInput, Vector2.right);
			}
			if (input == Vector2.zero)
			{
				inputAngle = -90f;
			}
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, inputAngle + camangle, 0), 50f * Time.deltaTime);
			if (strafing)
			{
				inputAngle = Vector2.SignedAngle(new Vector2(strafe, input.y).normalized, Vector2.right);
			}
			var movedir = Quaternion.Euler(0, inputAngle + 90f, 0) * cameraForward;

			var moveOffset = movedir * Time.deltaTime * _moveSpeed;
			if (!(Mathf.Abs(strafe) < 0.1f && input == Vector2.zero))
			{
				transform.position += moveOffset;
			}
			_animator.SetFloat("y", input.magnitude);
			_animator.SetFloat("x", strafe);
			var newCamPos = _cameraController.TargetPosition;
			newCamPos += Vector3.Project(moveOffset, cameraForward);
			if (strafing)
			{
				newCamPos += Vector3.Project(moveOffset, cameraRight);

				if (input.y < 0)
				{
					_animator.SetFloat("y", -input.magnitude);
				}
			}
			_cameraController.TargetPosition = newCamPos;
		}

	}
}

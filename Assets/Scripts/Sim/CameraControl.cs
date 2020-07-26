using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[SerializeField]
	Transform _player;
	[SerializeField]
	float _distance = 4f;
	[SerializeField]
	float _minDistance = 1f;
	[SerializeField]
	float _maxDistance = 10f;
	Vector3 _rotation = new Vector2();
	Vector3 _lastFrameMousePos;
	[SerializeField]
	private float _rotationSpeed = 1f;
	[SerializeField]
	private float _minYAngle = -85f;
	[SerializeField]
	private float _maxYAngle = 85f;
	private float _scrollSpeed = 10f;
	[SerializeField]
	LayerMask _terrainMask;
	public Vector3 TargetPosition { get; set; }
	private void Start()
	{
		TargetPosition = transform.position;
		UpdateOrbitalPosition();
	}
	void Update()
	{
		

		var scroll = Input.GetAxis("Mouse ScrollWheel");
		_distance = Mathf.Clamp(_distance - scroll * _scrollSpeed, _minDistance, _maxDistance);
		if (Input.GetMouseButtonDown(1))
		{
			_lastFrameMousePos = Input.mousePosition;
			Cursor.visible = false;
		}
		else if (Input.GetMouseButtonUp(1))
		{
			Cursor.visible = true;
		}
		else if (Input.GetMouseButton(1))
		{
			var mouseDelta = _lastFrameMousePos - Input.mousePosition;
			_rotation.x = 0;
			_rotation.y -= mouseDelta.x * _rotationSpeed;
			_rotation.z += mouseDelta.y * _rotationSpeed;
			_rotation.z = Mathf.Clamp(_rotation.z, _minYAngle, _maxYAngle);
			_lastFrameMousePos = Input.mousePosition;
			UpdateOrbitalPosition();
		}
		else
		{
			var targetdist = _player.position - TargetPosition;
			targetdist.y = targetdist.z;
			_rotation.y = Vector2.SignedAngle(targetdist, Vector2.right) + 180f;
		}

		AdjustCameraPosition();

		transform.LookAt(_player);

	}

	private void AdjustCameraPosition()
	{
		var dist = Vector3.Distance(TargetPosition, _player.position) - _distance;
		

		Vector3 direction = TargetPosition - _player.position;
		Ray ray = new Ray(_player.position, direction);
		if (Mathf.Abs(dist) < 0.4f)
		{
			dist = 0f;
		}
		if (Physics.Raycast(ray, out RaycastHit hit, _distance, _terrainMask))
		{
			//transform.position = hit.point + (hit.point - TargetPosition).normalized * 1f;
			transform.position = hit.point + hit.normal * 0.1f;
		}
		else
		{
			TargetPosition += (_player.position - TargetPosition).normalized * Math.Sign(dist) * Time.deltaTime * _scrollSpeed;
			transform.position = TargetPosition;
		}
	}

	private void UpdateOrbitalPosition()
	{
		var offset = Quaternion.Euler(_rotation) * new Vector3(Vector3.Distance(TargetPosition, _player.position), 0, 0);
		TargetPosition = _player.position + offset;
	}
}

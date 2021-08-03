using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
	// Input fields
	private ThirdPersonActionsAsset _playerActionsAsset;
	private InputAction _move;
	private InputAction _look;

	// Movement fields
	private Rigidbody _rb;
	[SerializeField] private float _movementForce = 1f;
	[SerializeField] private float _jumpForce = 5f;
	[SerializeField] private float _maxSpeed = 5f;
	private Vector3 _forceDirection = Vector3.zero;

	[SerializeField] private Camera _playerCamera;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_playerActionsAsset = new ThirdPersonActionsAsset();
	}

	private void FixedUpdate()
	{
		_forceDirection += _move.ReadValue<Vector2>().x * GetCameraRight(_playerCamera) * _movementForce;
		_forceDirection += _move.ReadValue<Vector2>().y * GetCameraForward(_playerCamera) * _movementForce;

		_rb.AddForce(_forceDirection, ForceMode.Impulse);
		_forceDirection = Vector3.zero;

		if (_rb.velocity.y < 0)
			_rb.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

		Vector3 horizontalVel = _rb.velocity;
		horizontalVel.y = 0;

		if (horizontalVel.sqrMagnitude > Math.Pow(_maxSpeed, 2))
			_rb.velocity = horizontalVel.normalized * _maxSpeed + Vector3.up * _rb.velocity.y;

		LookAt();
	}

	private void OnEnable()
	{
		_move = _playerActionsAsset.Player.Move;
		_playerActionsAsset.Player.Jump.started += Jump;
		_playerActionsAsset.Player.Enable();
	}

	private void OnDisable()
	{
		_playerActionsAsset.Player.Jump.started -= Jump;
		_playerActionsAsset.Player.Disable();
	}

	private void Jump(InputAction.CallbackContext ctx)
	{
		if (IsGrounded())
		{
			_forceDirection += Vector3.up * _jumpForce;
		}
	}

	private bool IsGrounded()
	{
		Ray ray = new Ray(transform.position + Vector3.up * .25f, Vector3.down);

		if (Physics.Raycast(ray, out RaycastHit hit, .3f)) return false;
		else return false;
	}

	private Vector3 GetCameraRight(Camera playerCamera)
	{
		Vector3 right = playerCamera.transform.right;
		right.y = 0;
		return right.normalized;
	}

	private Vector3 GetCameraForward(Camera playerCamera)
	{
		Vector3 forward = playerCamera.transform.forward;
		forward.y = 0;
		return forward.normalized;
	}

	private void LookAt()
	{
		Vector3 direction = _rb.velocity;
		direction.y = 0;

		if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
			_rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
		else
			_rb.angularVelocity = Vector3.zero;
	}
}

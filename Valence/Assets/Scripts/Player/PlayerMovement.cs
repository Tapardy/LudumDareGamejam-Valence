using RotatingCircle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Rigidbody2D rb;
		[SerializeField] private float jumpForce;
		[SerializeField] private float minJumpForce = 3f;
		[SerializeField] private float maxJumpForce = 10f;
		[SerializeField] private float jumpForceMultiplier = 1f;
		[SerializeField] private Transform playerSprite;
		[SerializeField] private float maxCompressionScale = 0.5f;

		private bool _charging;
		private bool _jumping;

		private Vector3 _originalScale;
		private SnapToCircle _snapToCircle;

		private bool IsGrounded { get; set; }

		private void Awake()
		{
			if (!TryGetComponent(out rb))
			{
				Debug.LogError("Rigidbody2D component not found!");
				enabled = false;
			}

			_originalScale = playerSprite.localScale;
		}

		private void FixedUpdate()
		{
			IsGrounded = _snapToCircle != null;

			if (_charging)
			{
				jumpForce = Mathf.Clamp(jumpForce + Time.fixedDeltaTime * jumpForceMultiplier, minJumpForce, maxJumpForce);
				UpdateCompression();
			}

			Debug.Log($"Jumping: {_jumping}, Grounded: {IsGrounded}");
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.started && !IsGrounded && !_jumping)
				rb.MovePosition(rb.position + Vector2.up * 0.001f);

			if (context.started && IsGrounded && !_jumping)
				_charging = true;

			if (context.canceled)
			{
				if (_charging && IsGrounded)
					Jump();
				ResetJumpState();
			}
		}

		private void Jump()
		{
			if (_snapToCircle == null) return;

			jumpForce = Mathf.Max(jumpForce, minJumpForce);
			var angle = _snapToCircle.GetPlayerAngle();
			var jumpDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			rb.velocity = jumpDirection * jumpForce;
			_jumping = true;
			jumpForce = 0;

			DetachFromCircle();
			playerSprite.localScale = _originalScale;
		}

		private void DetachFromCircle()
		{
			if (_snapToCircle != null)
			{
				_snapToCircle.DetachPlayer();
				_snapToCircle = null;
			}
		}

		private void UpdateCompression()
		{
			if (playerSprite == null || _snapToCircle == null) return;

			var angle = _snapToCircle.GetPlayerAngle();
			var jumpDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

			var compressionFactor = Mathf.InverseLerp(minJumpForce, maxJumpForce, jumpForce);
			var compressionScale = Mathf.Lerp(1f, maxCompressionScale, compressionFactor);

			playerSprite.localScale = new Vector3(_originalScale.x, compressionScale, _originalScale.z);

			playerSprite.rotation = Quaternion.Euler(0, 0, angle + 90);
		}

		public void SetSnapToCircle(SnapToCircle snapToCircle)
		{
			if (snapToCircle != null)
				_snapToCircle = snapToCircle;
		}

		public bool IsJumping()
		{
			return _jumping;
		}

		public bool IsSnappedToCircle()
		{
			return _snapToCircle != null;
		}

		private void ResetJumpState()
		{
			_charging = false;
			if (IsGrounded)
			{
				_jumping = false;
				playerSprite.localScale = _originalScale;
			}
		}
	}
}
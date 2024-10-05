using RotatingCircle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private bool isGrounded;
		[SerializeField] private Rigidbody2D rb;
		[SerializeField] private float groundCheckRadius = 1f;
		[SerializeField] private CircleCollider2D triggerCollider;
		[SerializeField] private float jumpForce;
		[SerializeField] private float minJumpForce = 3f;
		[SerializeField] private float maxJumpForce = 10f;
		[SerializeField] private float jumpForceMultiplier = 1f;
		private bool _charging;
		private bool _jumping;
		private SnapToCircle _snapToCircle;

		private void Awake()
		{
			if (!TryGetComponent(out rb))
			{
				Debug.LogError("Rigidbody2D component not found!");
				enabled = false;
			}
		}

		private void FixedUpdate()
		{
			isGrounded = Physics2D.OverlapCircle(triggerCollider.bounds.center, groundCheckRadius, groundLayer);
			if (_charging) jumpForce = Mathf.Clamp(jumpForce + Time.fixedDeltaTime * jumpForceMultiplier, minJumpForce, maxJumpForce);

			Debug.Log($"Jumping: {_jumping}, Grounded: {isGrounded}");
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(triggerCollider.bounds.center, groundCheckRadius);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Circle"))
				isGrounded = false;
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.CompareTag("Circle"))
				isGrounded = true;
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.started && !isGrounded && !_jumping)
				rb.MovePosition(rb.position + Vector2.up * 0.001f);

			if (context.started && isGrounded && !_jumping) _charging = true;

			if (context.canceled)
			{
				if (_charging && isGrounded)
					Jump();
				DetachFromCircle();
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
		}

		private void DetachFromCircle()
		{
			if (_snapToCircle != null)
			{
				_snapToCircle.DetachPlayer();
				_snapToCircle = null;
			}
		}

		public void SetSnapToCircle(SnapToCircle snapToCircle)
		{
			if (snapToCircle != null) _snapToCircle = snapToCircle;
		}

		public bool IsJumping()
		{
			return _jumping;
		}

		private void ResetJumpState()
		{
			_charging = false;
			if (isGrounded)
				_jumping = false;
		}
	}
}
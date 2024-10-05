using RotatingCircle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private string groundTag = "Circle";
		[SerializeField] private float minJumpForce = 5f;
		[SerializeField] private float maxJumpForce = 10f;
		[SerializeField] private float jumpForceMultiplier = 1f;
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private bool isGrounded;
		[SerializeField] private Rigidbody2D rb;
		private bool _charging;
		private float _jumpForce;
		private bool _jumping;
		private SnapToCircle _snapToCircle;

		private void Awake()
		{
			if (rb == null) enabled = false;
		}

		private void FixedUpdate()
		{
			isGrounded = !_jumping && Physics2D.OverlapCircle(transform.position, 0.1f, groundLayer);
			if (_charging) _jumpForce = Mathf.Clamp(_jumpForce + jumpForceMultiplier * Time.fixedDeltaTime, minJumpForce, maxJumpForce);
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.started && isGrounded) _charging = true;

			if (context.canceled)
			{
				if (_charging && isGrounded) Jump();
				DetachFromCircle();
				ResetJumpState();
			}
		}

		private void Jump()
		{
			if (_snapToCircle == null) return;

			var angle = _snapToCircle.GetPlayerAngle();
			var jumpDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			rb.velocity = jumpDirection * _jumpForce;
			_jumping = true;
		}

		private void DetachFromCircle()
		{
			_snapToCircle?.DetachPlayer();
			_snapToCircle = null;
		}

		public void SetSnapToCircle(SnapToCircle snapToCircle)
		{
			_snapToCircle = snapToCircle;
		}

		public bool IsJumping()
		{
			return _jumping;
		}

		private void ResetJumpState()
		{
			_charging = false;
			_jumpForce = 0;
			_jumping = false;
		}
	}
}
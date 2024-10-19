using RotatingCircle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Rigidbody2D rb;
		[SerializeField] private float maxFallSpeed = -10f;
		[SerializeField] private float jumpForce;
		[SerializeField] private float minJumpForce = 3f;
		[SerializeField] private float maxJumpForce = 10f;
		[SerializeField] private float jumpForceMultiplier = 1f;
		[SerializeField] private Transform playerSprite;
		[SerializeField] private float maxCompressionScale = 0.5f;
		[SerializeField] private float snapCooldownDuration = 1f;
		[SerializeField] private string electronTag = "Electron";
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Sprite hitSpriteRenderer;
		[SerializeField] private Sprite winSprite;
		[SerializeField] private bool isVictorious;
		private bool _charging;
		private bool _jumping;
		private Vector3 _originalScale;
		private Sprite _originalSprite;
		private float _snapCooldownTimer;
		private SnapToCircle _snapToCircle;

		internal bool IsGrounded { get; set; }
		public bool CanSnap { get; private set; } = true;

		private void Awake()
		{
			_originalSprite = spriteRenderer.sprite;
			if (!TryGetComponent(out rb))
			{
				Debug.LogError("Rigidbody2D component not found!");
				enabled = false;
			}

			_originalScale = playerSprite.localScale;
		}

		private void FixedUpdate()
		{
			if (isVictorious)
			{
				// Allow movement and rotation based on circle snapping
				IsGrounded = _snapToCircle != null;

				if (IsGrounded && _snapToCircle != null)
					UpdateRotation();

				if (rb.velocity.y < maxFallSpeed)
					rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

				return; // Skip the rest of the FixedUpdate when victorious
			}

			IsGrounded = _snapToCircle != null;

			if (_charging)
			{
				jumpForce = Mathf.Clamp(jumpForce + Time.fixedDeltaTime * jumpForceMultiplier, minJumpForce, maxJumpForce);
				UpdateCompression();
			}

			if (!CanSnap)
			{
				_snapCooldownTimer -= Time.fixedDeltaTime;
				if (_snapCooldownTimer <= 0)
				{
					spriteRenderer.sprite = _originalSprite;
					CanSnap = true;
				}
			}

			if (IsGrounded && _snapToCircle != null)
				UpdateRotation();

			if (rb.velocity.y < maxFallSpeed)
				rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag(electronTag))
			{
				DetachFromCircle();
				CanSnap = false;
				spriteRenderer.sprite = hitSpriteRenderer;
				_snapCooldownTimer = snapCooldownDuration;
			}
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (isVictorious) return; // Skip any jump action if victorious

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
			UpdateRotation();
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

			var compressionFactor = Mathf.InverseLerp(minJumpForce, maxJumpForce, jumpForce);
			var compressionScale = Mathf.Lerp(1f, 1f - maxCompressionScale, compressionFactor);

			playerSprite.localScale =
				new Vector3(_originalScale.x, _originalScale.y * compressionScale, _originalScale.z);

			UpdateRotation();
		}

		private void UpdateRotation()
		{
			if (_snapToCircle == null) return;

			var angle = _snapToCircle.GetPlayerAngle();
			playerSprite.rotation = Quaternion.Euler(0, 0, angle - 90);
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

		// I am completely aware this is bad practice, but like. I have no time and this will do :)
		public void TriggerVictory()
		{
			isVictorious = true;
			spriteRenderer.sprite = winSprite;
		}

		public bool IsPlayerVictorious()
		{
			return isVictorious;
		}
	}
}
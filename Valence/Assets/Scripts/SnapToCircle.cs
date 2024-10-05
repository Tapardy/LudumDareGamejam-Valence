using Player;
using UnityEngine;

public class SnapToCircle : MonoBehaviour
{
	[SerializeField] private string playerTag = "Player";
	[SerializeField] private RotateCircle rotateCircle;

	private CircleCollider2D _circleCollider;
	private float _playerAngle;
	private Transform _snappedPlayer;

	private void Awake()
	{
		if (!TryGetComponent(out _circleCollider))
		{
			Debug.LogError("no circle collider found");
			enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (_snappedPlayer != null && !IsPlayerJumping()) RotatePlayer();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(playerTag) && _snappedPlayer == null && other.TryGetComponent(out Rigidbody2D playerRb))
		{
			playerRb.velocity = Vector2.zero;
			playerRb.gravityScale = 0;
			_snappedPlayer = other.transform;

			SnapPlayerToCircle(other.transform);
			other.TryGetComponent(out PlayerMovement playerMovement);
			playerMovement?.SetSnapToCircle(this);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag(playerTag) && other.TryGetComponent(out Rigidbody2D playerRb))
		{
			playerRb.gravityScale = 1;
			_snappedPlayer = null;
		}
	}

	private void SnapPlayerToCircle(Transform playerTransform)
	{
		if (_snappedPlayer == null) return;

		var radius = _circleCollider.radius;
		Vector2 circleCenter = transform.position;

		_playerAngle = Mathf.Atan2(playerTransform.position.y - circleCenter.y,
			playerTransform.position.x - circleCenter.x) * Mathf.Rad2Deg;

		playerTransform.position = circleCenter +
		                           new Vector2(Mathf.Cos(_playerAngle * Mathf.Deg2Rad), Mathf.Sin(_playerAngle * Mathf.Deg2Rad)) * radius;
	}

	private void RotatePlayer()
	{
		if (_snappedPlayer == null || rotateCircle == null) return;

		_playerAngle += rotateCircle.rotationSpeed * Time.deltaTime;
		var radius = _circleCollider.radius;
		Vector2 circleCenter = transform.position;
		_snappedPlayer.position = circleCenter +
		                          new Vector2(Mathf.Cos(_playerAngle * Mathf.Deg2Rad), Mathf.Sin(_playerAngle * Mathf.Deg2Rad)) * radius;
	}

	public void DetachPlayer()
	{
		_snappedPlayer = null;
	}

	public float GetPlayerAngle()
	{
		return _playerAngle;
	}

	private bool IsPlayerJumping()
	{
		return _snappedPlayer.TryGetComponent(out PlayerMovement playerMovement) && playerMovement.IsJumping();
	}
}
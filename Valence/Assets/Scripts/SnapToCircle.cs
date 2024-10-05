using UnityEngine;

public class SnapToCircle : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float snapSpeed = 10f;
    private CircleCollider2D _circleCollider;
    private Transform _snappedPlayer;
    private float _playerAngle;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        if (_circleCollider == null)
        {
            Debug.LogError("CircleCollider2D not found on this object!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (other.TryGetComponent(out Rigidbody2D playerRb))
            {
                playerRb.velocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.bodyType = RigidbodyType2D.Kinematic;
                _snappedPlayer = other.transform;
                CalculatePlayerAngle();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (other.TryGetComponent(out Rigidbody2D playerRb))
            {
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                _snappedPlayer = null;
            }
        }
    }

    private void CalculatePlayerAngle()
    {
        if (_snappedPlayer == null) return;

        Vector2 circleCenter = transform.position;
        Vector2 playerPosition = _snappedPlayer.position;
        Vector2 direction = (playerPosition - circleCenter).normalized;
        _playerAngle = Mathf.Atan2(direction.y, direction.x);
    }

    private void FixedUpdate()
    {
        if (_snappedPlayer != null)
        {
            Vector2 circleCenter = transform.position;
            float circleRotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            float targetAngle = _playerAngle + circleRotation;

            Vector2 targetPosition = circleCenter + new Vector2(
                Mathf.Cos(targetAngle),
                Mathf.Sin(targetAngle)
            ) * _circleCollider.radius;

            _snappedPlayer.position = Vector2.Lerp(_snappedPlayer.position, targetPosition, snapSpeed * Time.fixedDeltaTime);

            // Rotate player to face tangent to the circle
            float tangentAngle = targetAngle + Mathf.PI / 2;
            _snappedPlayer.rotation = Quaternion.Euler(0, 0, tangentAngle * Mathf.Rad2Deg);
        }
    }
}
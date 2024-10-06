using UnityEngine;

namespace Player
{
	public class PlayerCamera : MonoBehaviour
	{
		[SerializeField] private Transform playerTransform;
		[SerializeField] private Vector3 offset = new(0, 0, -10);
		[SerializeField] private float smoothSpeed = 0.125f;

		private void LateUpdate()
		{
			if (playerTransform == null) return;

			var desiredPosition = playerTransform.position + offset;
			var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
			transform.position = smoothedPosition;
		}
	}
}
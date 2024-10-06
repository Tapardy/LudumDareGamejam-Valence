using UnityEngine;

namespace Player
{
	public class PlayerCamera : MonoBehaviour
	{
		[SerializeField] private Transform playerTransform;
		[SerializeField] private float offsetZ = -10f;

		private void LateUpdate()
		{
			if (playerTransform == null) return;

			transform.position = new Vector3(0, playerTransform.position.y, offsetZ);
		}
	}
}
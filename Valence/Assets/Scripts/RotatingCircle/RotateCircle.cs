using UnityEngine;

namespace RotatingCircle
{
	public class RotateCircle : MonoBehaviour
	{
		[SerializeField] private Transform circle;
		[SerializeField] internal float rotationSpeed = 10f;
		[SerializeField] internal bool clockwise;

		private void FixedUpdate()
		{
			var direction = clockwise ? -1f : 1f;

			var rotationAmount = direction * rotationSpeed * Time.fixedDeltaTime;

			circle.Rotate(0, 0, rotationAmount);
		}
	}
}
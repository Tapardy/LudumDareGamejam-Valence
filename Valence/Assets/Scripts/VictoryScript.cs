using Player;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{
	[SerializeField] private Canvas victoryCanvas;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent<PlayerMovement>(out var playerMovement)) playerMovement.TriggerVictory();
		victoryCanvas.gameObject.SetActive(true);
	}
}
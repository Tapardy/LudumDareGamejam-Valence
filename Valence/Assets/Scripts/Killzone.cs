using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour
{
	[SerializeField] private string playerTag = "Player";

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(playerTag)) ReloadScene();
	}

	private void ReloadScene()
	{
		var currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.buildIndex);
	}
}
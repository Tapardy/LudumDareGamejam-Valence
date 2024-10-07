using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private Tutorial tutorial;
	[SerializeField] private GameObject tutorialCanvas;
	[SerializeField] private Button okButton;

	private void Start()
	{
		tutorialCanvas.SetActive(false); // Hide the canvas by default

		if (!tutorial.HasSeenTutorial) ShowTutorialCanvas(); // Show only if the tutorial hasn't been seen
	}

	private void ShowTutorialCanvas()
	{
		tutorialCanvas.SetActive(true); // Show the tutorial canvas
		Time.timeScale = 0f; // Pause the game
		okButton.onClick.AddListener(OnOkButtonPressed); // Add listener for button
	}

	public void OnOkButtonPressed()
	{
		tutorial.CompleteTutorial(); // Mark tutorial as seen
		HideTutorialCanvas(); // Hide the canvas
	}

	private void HideTutorialCanvas()
	{
		tutorialCanvas.SetActive(false); // Hide the tutorial canvas
		Time.timeScale = 1f; // Resume the game
		okButton.onClick.RemoveListener(OnOkButtonPressed); // Remove listener for button
	}
}
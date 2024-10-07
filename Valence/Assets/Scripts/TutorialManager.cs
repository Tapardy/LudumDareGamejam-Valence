using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private Tutorial tutorial;
	[SerializeField] private GameObject tutorialCanvas;
	[SerializeField] private Button okButton;

	private void Start()
	{
		tutorialCanvas.SetActive(false);

		if (!tutorial.HasSeenTutorial) ShowTutorialCanvas();
	}

	private void ShowTutorialCanvas()
	{
		tutorialCanvas.SetActive(true);
		Time.timeScale = 0f;
		okButton.onClick.AddListener(OnOkButtonPressed);
	}

	public void OnOkButtonPressed()
	{
		tutorial.CompleteTutorial();
		HideTutorialCanvas();
	}

	private void HideTutorialCanvas()
	{
		tutorialCanvas.SetActive(false);
		Time.timeScale = 1f;
		okButton.onClick.RemoveListener(OnOkButtonPressed);
	}
}
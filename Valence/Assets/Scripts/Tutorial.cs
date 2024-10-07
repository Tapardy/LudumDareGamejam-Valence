using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Tutorial", menuName = "ScriptableObjects/TutorialScriptableObject", order = 1)]
public class Tutorial : ScriptableObject
{
	[SerializeField] private bool hasSeenTutorial;
	[SerializeField] private UnityEvent onTutorialCompleted;

	public bool HasSeenTutorial => hasSeenTutorial;

	public void StartTutorial()
	{
		if (!hasSeenTutorial) onTutorialCompleted?.Invoke();
	}

	public void CompleteTutorial()
	{
		hasSeenTutorial = true;
	}
}
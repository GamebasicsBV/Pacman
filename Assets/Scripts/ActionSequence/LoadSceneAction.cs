using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAction : IActionSequenceAction
{
	public string LevelToGoTo;

	override public void Evoke() {
		if (LevelToGoTo != null && LevelToGoTo != "") {
			SceneManager.LoadScene(LevelToGoTo);
		}
	}
}

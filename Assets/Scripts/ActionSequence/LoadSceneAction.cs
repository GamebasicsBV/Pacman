using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadSceneAction : IActionSequenceAction
{
	public string LevelToGoTo;

	override public void Invoke() {
		if (LevelToGoTo != null && LevelToGoTo != "") {
			SceneManager.LoadScene(LevelToGoTo);
		}
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}

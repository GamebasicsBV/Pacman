using UnityEngine;
using System;

public class CloseGameAction : IActionSequenceAction {

	override public void Invoke() {
        Debug.Log("Exit the game!");
		Application.Quit();
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}

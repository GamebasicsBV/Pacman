using UnityEngine;
using System;

public class ActivateObjectAction : IActionSequenceAction {

	public bool SetActive = true;
	public GameObject GameObject;

	override public void Invoke() {
		if (GameObject) {
			GameObject.SetActive(SetActive);
		}
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}

using UnityEngine;
using System;
using System.Collections;

public class WaitAction : IActionSequenceAction {
	public float TimeToWait;
	private Action onDone;

	override public void Invoke() {
		// don't call this. it's not meant to be just invoked
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		this.onDone = onDone;
		StartCoroutine(Wait());
	}

	private IEnumerator Wait() {
		yield return new WaitForSeconds(TimeToWait);
		onDone?.Invoke();
	}
}

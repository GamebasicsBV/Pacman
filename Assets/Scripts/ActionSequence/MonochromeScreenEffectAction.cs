using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MonochromeScreenEffectAction : IActionSequenceAction {

	public float Duration = 0.07f;

	override public void Invoke() {
		MonochromeEffect.StartDoingTheMonochrome(Duration);
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}

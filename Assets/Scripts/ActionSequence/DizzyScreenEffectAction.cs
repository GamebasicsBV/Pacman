
using System;

public class DizzyScreenEffectAction : IActionSequenceAction {

	override public void Invoke() {
		DizzyEffect.StartDoingTheDizzy();
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}

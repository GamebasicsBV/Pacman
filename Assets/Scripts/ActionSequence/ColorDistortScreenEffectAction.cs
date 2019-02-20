
using System;

public class ColorDistortScreenEffectAction : IActionSequenceAction {

	public float Duration = 0.07f;

	override public void Invoke() {
		ColorDistortEffect.StartDoingTheColorDistort(Duration);
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		Invoke();
		onDone.Invoke();
	}
}




public class ColorDistortScreenEffectAction : IActionSequenceAction {

	public float Duration = 0.07f;

	override public void Evoke() {
		ColorDistortEffect.StartDoingTheColorDistort(Duration);
	}
}

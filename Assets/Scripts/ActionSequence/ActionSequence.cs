using UnityEngine;

public class ActionSequence : IActionSequenceAction {

	public IActionSequenceAction[] Actions;

	override public void Evoke() {
		for (int i = 0; i < Actions.Length; i++) {
			Actions[i].Evoke();
		}
	}
}

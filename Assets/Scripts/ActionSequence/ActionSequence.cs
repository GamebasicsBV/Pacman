using UnityEngine;
using System;

public class ActionSequence : IActionSequenceAction {

	public IActionSequenceAction[] Actions;

	override public void Invoke() {
		Actions[0].InvokeAndDoOnDone(() => { OnActionDone(0); });
	}

	private void OnActionDone(int actionIndexDone) {
		if (actionIndexDone + 1 < Actions.Length) {
			Actions[actionIndexDone + 1].InvokeAndDoOnDone(() => { OnActionDone(actionIndexDone + 1); });
		}
	}

	override public void InvokeAndDoOnDone(Action onDone) {
		onDone.Invoke();
	}
}

using UnityEngine;
using System;

public class ActionSequence : IActionSequenceAction {

	public bool RunOnStart = false;
	public IActionSequenceAction[] Actions;

	private void Start() {
		if (RunOnStart) {
			Invoke();
		}
	}

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

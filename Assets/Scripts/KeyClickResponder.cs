using UnityEngine;

public class KeyClickResponder : MonoBehaviour
{
	public KeyCode KeyToClick;
	public IActionSequenceAction ActionToPerform;

    void Update() {
		if (Input.GetKeyDown(KeyToClick)) {
			ActionToPerform.Evoke();
		}
    }
}

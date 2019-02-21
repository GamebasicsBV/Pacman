using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanTriggerArea : MonoBehaviour
{

	public IActionSequenceAction Action;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.name == "pacman") {
			if (Action) {
				Action.Invoke();
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if((!GameManager.isAnimatorFlipped && other.name == "pacman") ||
		   (GameManager.isAnimatorFlipped && (
		        other.name == "pinky" ||
		        other.name == "clyde" ||
		        other.name == "blinky" ||
		        other.name == "inky"
            )))
		{
			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            Destroy(gameObject);

		    if (pacdots.Length == 1)
		    {
		        GameObject.FindObjectOfType<GameGUINavigation>().LoadLevel();
		    }
		}
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Pacdot : MonoBehaviour
{
    private GameManager _gm;

    void Start()
    {
        _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
	{
		if((!_gm.isAnimatorFlipped && other.name == "pacman") ||
		   (_gm.isAnimatorFlipped && (
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
		        _gm.LoadNextLevel();
		}
	}
}

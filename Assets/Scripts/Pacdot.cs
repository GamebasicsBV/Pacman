using UnityEngine;
using System.Collections;
using System.Linq;
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
		        other.name == "pinky_black" ||
                other.name == "clyde" ||
		        other.name == "clyde_black" ||
                other.name == "blinky" ||
		        other.name == "blinky_black" ||
                other.name == "inky" ||
	            other.name == "inky_black"
            )))
		{
            _gm.PlaySound(Sound.Chomp);

			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot").Where(x => x.name != "energizer").ToArray();
            Destroy(gameObject);

		    if (pacdots.Length == 1)
		        _gm.LoadNextLevel();
		}
	}
}

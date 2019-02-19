using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {

    private GameManager gm;

    public Sprite[] sprites;

	// Use this for initialization
	void Start ()
	{
	    gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if( gm == null )    Debug.Log("Powerup.cs did not find Game Manager!");

	    GetComponent<SpriteRenderer>().sprite = sprites[new System.Random().Next(0, sprites.Length)];
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "pacman" && !GameManager.isAnimatorFlipped)
        {
            //    if (new System.Random().Next(0, 2) < 1)
            //        gm.ScareGhosts();
            //    else
            //        gm.FlipAnimator();

            Destroy(gameObject);
        }
    }
}

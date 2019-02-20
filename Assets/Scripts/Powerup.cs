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
        if (col.name == "pacman" && !gm.isAnimatorFlipped)
        {
            if (gm.Level == 2)
            {
                gm.InverseControls();
                gm.ToggleMoveableWall();
            }

            if (gm.Level == 3)
                gm.FlipAnimator();

            Destroy(gameObject);
        }
    }
}

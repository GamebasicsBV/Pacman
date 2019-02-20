using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {

    private GameManager gm;

    public Sprite[] sprites;

	public IActionSequenceAction Level2PickupEffect;
	public IActionSequenceAction Level3PickupEffect;

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
            gm.PlaySound(Sound.EatFruit);

            if (gm.Level == 2)
            {
                gm.InverseControls();
                gm.ToggleMoveableWall();
				Level2PickupEffect?.Invoke();
            }

			if (gm.Level == 3) {
				gm.FlipAnimator();
				Level3PickupEffect?.Invoke();
			}

            Destroy(gameObject);
        }
    }
}

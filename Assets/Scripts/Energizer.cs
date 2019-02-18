﻿using UnityEngine;
using System.Collections;

public class Energizer : MonoBehaviour {

    private GameManager gm;

	// Use this for initialization
	void Start ()
	{
	    gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if( gm == null )    Debug.Log("Energizer did not find Game Manager!");
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "pacman" && !GameManager.isAnimatorFlipped)
        {
            if (new System.Random().Next(0, 2) < 1)
                gm.ScareGhosts();
            else
                gm.FlipAnimator();

            Destroy(gameObject);
        }
    }
}

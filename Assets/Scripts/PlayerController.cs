﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool _isInversed = false;
    public float speed = 0.4f;
    Vector2 _dest = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 _nextDir = Vector2.zero;
	public bool NotARealLevel = false;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public PointSprites points;

    public static int killstreak = 0;

    // script handles
    private GameGUINavigation GUINav;
    private GameManager GM;

    private bool _deadPlaying = false;

    // Use this for initialization
    void Start()
    {
		if (!NotARealLevel) {
			GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
			GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
			_dest = transform.position;
		}
		else {
			_dest = transform.position;
		}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!NotARealLevel) {
			switch (GameManager.gameState) {
				case GameManager.GameState.Game:
					ReadInputAndMove();
					Animate();
					break;

				case GameManager.GameState.Dead:
					if (!_deadPlaying)
						StartCoroutine("PlayDeadAnimation");
					break;
			}
		}
		else {
			ReadInputAndMove();
			Animate();
		}
    }

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        GM.PlaySound(Sound.Death);

        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

		GM.ResetScene();
	}

    void Animate()
    {
        Vector2 dir = _dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider.name == "energizer" || hit.collider.name == "pacdot" || hit.collider.name == "powerup" || hit.collider.name == "Powerup(Clone)" || hit.collider.name == "pacmanTriggerArea" || (hit.collider == GetComponent<Collider2D>());
    }

    public void ResetDestination()
    {
		if (GM) {
			if (GM.Level == 4)
				_dest = new Vector2(13f, 30f);
			else
				_dest = new Vector2(15f, 11f);

			// Don't keep walking if you died in level 5.
			if (GM.Level == 5) {
				_dir = Vector2.zero;
				_nextDir = Vector2.zero;
			}
		}

        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

		// get the next direction from keyboard
		if (_dir.x != 0) {
			if (Input.GetAxis("Horizontal") > 0) _nextDir = _isInversed ? -Vector2.right : Vector2.right;
			if (Input.GetAxis("Horizontal") < 0) _nextDir = _isInversed ? Vector2.right : -Vector2.right;
			if (Input.GetAxis("Vertical") > 0) _nextDir = _isInversed ? -Vector2.up : Vector2.up;
			if (Input.GetAxis("Vertical") < 0) _nextDir = _isInversed ? Vector2.up : -Vector2.up;
		}
		else {
			if (Input.GetAxis("Vertical") > 0) _nextDir = _isInversed ? -Vector2.up : Vector2.up;
			if (Input.GetAxis("Vertical") < 0) _nextDir = _isInversed ? Vector2.up : -Vector2.up;
			if (Input.GetAxis("Horizontal") > 0) _nextDir = _isInversed ? -Vector2.right : Vector2.right;
			if (Input.GetAxis("Horizontal") < 0) _nextDir = _isInversed ? Vector2.right : -Vector2.right;
		}

		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && GM) {
			GM.PacmanHasMoved = true;
		}

        // if pacman is in the center of a tile
        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            if (Valid(_nextDir))
            {
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;
            }
            else   // if next direction is not valid
            {
                if (Valid(_dir))  // and the prev. direction is valid
                    _dest = (Vector2)transform.position + _dir;   // continue on that direction

                // otherwise, do nothing
            }
        }
    }

    public Vector2 getDir()
    {
        return _dir;
    }

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);
        GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;

    }
}

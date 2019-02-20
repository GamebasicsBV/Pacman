using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomActionInvoker : MonoBehaviour
{
	public int MinTimeBetweenRandomInvokes = 10;
	public int MaxTimeBetweenRandomInvokes = 20;
	private float timeToNextRandomInvoke;
	public IActionSequenceAction Action;
	
    void Start()
    {
		timeToNextRandomInvoke = MinTimeBetweenRandomInvokes + (new System.Random().Next(MaxTimeBetweenRandomInvokes - MinTimeBetweenRandomInvokes));
    }
	
    void Update()
    {
		timeToNextRandomInvoke -= Time.deltaTime;
		Debug.Log(timeToNextRandomInvoke);
		if (timeToNextRandomInvoke < 0) {
			timeToNextRandomInvoke = MinTimeBetweenRandomInvokes + (new System.Random().Next(MaxTimeBetweenRandomInvokes - MinTimeBetweenRandomInvokes));
			Action.Invoke();
		}
    }
}

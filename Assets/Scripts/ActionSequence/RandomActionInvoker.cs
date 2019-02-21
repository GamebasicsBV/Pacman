using UnityEngine;
using System;

public class RandomActionInvoker : MonoBehaviour
{
	private static System.Random _Random;
	private static System.Random Random {
		get {
			if (_Random == null) {
				_Random = new System.Random();
			}
			return _Random;
		}
	}

	public int MinTimeBetweenRandomInvokes = 10;
	public int MaxTimeBetweenRandomInvokes = 20;
	private float timeToNextRandomInvoke;
	public IActionSequenceAction Action;
	
    void Start()
    {
		timeToNextRandomInvoke = MinTimeBetweenRandomInvokes + Random.Next(MaxTimeBetweenRandomInvokes - MinTimeBetweenRandomInvokes);
    }
	
    void Update()
    {
		timeToNextRandomInvoke -= Time.deltaTime;

		if (timeToNextRandomInvoke < 0) {
			timeToNextRandomInvoke = MinTimeBetweenRandomInvokes + Random.Next(MaxTimeBetweenRandomInvokes - MinTimeBetweenRandomInvokes);
			Action.Invoke();
		}
    }
}

using UnityEngine;
using System;

public abstract class IActionSequenceAction : MonoBehaviour
{
	abstract public void Invoke();
	abstract public void InvokeAndDoOnDone(Action onDone);
}

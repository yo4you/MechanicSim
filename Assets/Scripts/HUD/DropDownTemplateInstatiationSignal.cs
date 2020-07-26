using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// very hacky solution because I can't find a way to trigger an event when canceling dropdowns
public class DropDownTemplateInstatiationSignal : MonoBehaviour
{
	private void Start()
	{
		DropDownCallbackRegistry.PopCallback(true);
	}
	private void OnDestroy()
	{
		DropDownCallbackRegistry.PopCallback(false);
	}
}

public static class DropDownCallbackRegistry
{
	static Queue<Action>[] _callbacks = new Queue<Action>[] { new Queue<Action>(), new Queue<Action>() };
	public static void RegisterCallbacks(Action action, bool open)
	{
		_callbacks[open ? 0 : 1].Enqueue(action);
	}
	public static void PopCallback(bool open)
	{
		var queue = _callbacks[open ? 0 : 1];
		if (queue.Count != 0)
			queue.Dequeue().Invoke();
	}
}

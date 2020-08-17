using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalIconSelector : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _images;

	public enum State
	{
		CHECK,
		TRUE,
		FALSE
	}

	public void SetState(State state)
	{
		for (int i = 0; i < _images.Length; i++)
		{
			_images[i].SetActive((int)state == i);
		}
	}
}
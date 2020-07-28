using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntryPrefabs : MonoBehaviour
{
	public EntryHudScriptableObject _entryHudScriptableObject;

	[SerializeField]
	private GameObject _timeLabel;

	[SerializeField]
	private GameObject _castLabel;

	public GameObject TimeLabel { get => _timeLabel; }
	public GameObject CastLabel { get => _castLabel; }
}
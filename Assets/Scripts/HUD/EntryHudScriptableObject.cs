using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EntryHudScriptableObject", order = 1)]
public class EntryHudScriptableObject : ScriptableObject
{
	public GameObject TimeLabel;
	public GameObject CastLabel;
}
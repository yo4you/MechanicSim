using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeLineBehaviour : EntryCollectionHud<TimeLineEntry>
{
	[SerializeField]
	private GameObject _prefabTimeEntry;

	[SerializeField]
	private GameObject _prefabRandomEntry;

	[SerializeField]
	private GameObject _prefabIfElseEntry;

	[SerializeField]
	private GameObject _prefabDistributeEntry;

	protected override GameObject CreateEntry(List<TimeLineEntry> sortedEntries, int i)
	{
		var go = Instantiate(_entryPrefabs[(int)sortedEntries[i].Type], _contentTransform.transform);
		TimeLineEntryHud hudentry = null;// go.AddComponent<TimeLineEntryHud>();
		switch ((TimeLineEntryType)sortedEntries[i].Type)
		{
			case TimeLineEntryType.Time:
				hudentry = go.AddComponent<TimeLineEntryTime>();
				break;

			case TimeLineEntryType.Random:
				hudentry = go.AddComponent<TimeLineEntryRandom>();
				break;

			case TimeLineEntryType.IfElse:
				hudentry = go.AddComponent<TimeLineEntryIfElse>();
				break;

			case TimeLineEntryType.Distribute:
				hudentry = go.AddComponent<TimeLineEntryDistribute>();
				break;
		}
		hudentry.SetEntryData(sortedEntries[i], this);
		return go;
	}

	protected override void DisplayDropDownMenu(Dropdown dropDown)
	{
		dropDown.AddOptions(Enum.GetNames(typeof(TimeLineEntryType)).ToList());
		dropDown.onValueChanged.AddListener((i) =>
		{
			AddEntryAtCursor((ParameterType)i);
			UnlockCursorLine();
			Destroy(dropDown.gameObject);
		});
	}

	internal override void SetEntries(List<TimeLineEntry> timeLineEntries)
	{
		_entryPrefabs = new GameObject[] {
			_prefabTimeEntry,
			_prefabRandomEntry,
			_prefabIfElseEntry,
			_prefabDistributeEntry };
		base.SetEntries(timeLineEntries);
	}

	public void Rename(string prevName, string newName)
	{
		foreach (TimeLineEntry entry in _entries.Where(e => e.Mechanic == prevName))
		{
			entry.Mechanic = newName;
		}
		Redraw();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ValueEditorBehaviour : EntryCollectionHud<ValueEntry>
{
	[SerializeField]
	private GameObject _prefabEmptyValue;

	protected override GameObject CreateEntry(List<ValueEntry> sortedEntries, int i)
	{
		var go = Instantiate(_entryPrefabs[(int)sortedEntries[i].Type], _contentTransform.transform);
		// 		var hudentry = go.AddComponent<TimeLineEntryHud>();
		// 		hudentry.SetEntryData(sortedEntries[i], this);
		throw new NotImplementedException();
		return go;
	}

	protected override void DisplayDropDownMenu(Dropdown dropDown)
	{
		dropDown.AddOptions(Enum.GetNames(typeof(TimeLineEntryType)).ToList());
		dropDown.onValueChanged.AddListener((i) =>
		{
			AddNewEntryAtCursor(i);
			UnlockCursorLine();
			Destroy(dropDown.gameObject);
		});
	}

	internal override void SetEntries(List<ValueEntry> timeLineEntries)
	{
		_entryPrefabs = new GameObject[] {
			_prefabEmptyValue,};
		base.SetEntries(timeLineEntries);
	}
}
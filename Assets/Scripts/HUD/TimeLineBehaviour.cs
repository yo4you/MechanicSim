using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeLineBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private SVGImage _addSeperator;

	[SerializeField]
	private GameObject _prefabTimeEntry;

	[SerializeField]
	private GameObject _prefabRandomEntry;

	[SerializeField]
	private GameObject _prefabIfElseEntry;

	[SerializeField]
	private GameObject _prefabDistributeEntry;

	private RectTransform _contentTransform;

	[SerializeField]
	private GameObject _overLayTransform;

	[SerializeField]
	private float _entryMargin = 5f;

	[SerializeField]
	private EntryHudScriptableObject _entryHudScriptableObject;

	[SerializeField]
	private GameObject _stretchLine;

	[SerializeField]
	private GameObject _dropdownPrefab;

	private ScrollRect _scrollRect;
	private GameObject[] _entryPrefabs;
	private int _cursorIndex = 0;
	private List<TimeLineEntry> _entries = new List<TimeLineEntry>();
	private List<GameObject> _entryGameObjects = new List<GameObject>();

	private float _entryHeight;
	private float _heightScale;
	private GameObject _button;
	private bool _cursorLocked = false;

	public EntryHudScriptableObject EntryHudScriptableObject { get => _entryHudScriptableObject; }

	private void Update()
	{
		if (!_cursorLocked)
		{
			_cursorIndex = (int)((
				(Input.mousePosition.y - _contentTransform.position.y) * _heightScale - _entryMargin * 2f)
				/ -(_entryHeight + _entryMargin));
			_cursorIndex = Mathf.Clamp(_cursorIndex, 0, _entries.Count);
			var seperatorPos = _addSeperator.rectTransform.anchoredPosition;
			_addSeperator.rectTransform.anchoredPosition =
				new Vector2(seperatorPos.x, -(_entryHeight + _entryMargin) * _cursorIndex);
		}
	}

	internal void SetEntries(List<TimeLineEntry> timeLineEntries)
	{
		_scrollRect = GetComponent<ScrollRect>();
		_addSeperator.enabled = false;
		_entryPrefabs = new GameObject[] {
			_prefabTimeEntry,
			_prefabRandomEntry,
			_prefabIfElseEntry,
			_prefabDistributeEntry };
		_contentTransform = _scrollRect.content;
		_entryHeight = _prefabTimeEntry.GetComponent<RectTransform>().rect.height;
		_heightScale = FindObjectOfType<CanvasScaler>().referenceResolution.y / Screen.height;
		_button = _addSeperator.GetComponentInChildren<Button>().gameObject;

		_entries = timeLineEntries;
		Redraw();
	}

	internal void RemoveEntry(TimeLineEntry entry)
	{
		foreach (var child in GetChildren(entry))
		{
			child.ParentEntry = entry.ParentEntry;
		}
		_entries.Remove(entry);
		_cursorIndex = 0;
		Redraw();
	}

	private IEnumerable<TimeLineEntry> GetChildren(TimeLineEntry entry)
	{
		return _entries.Where(e => e.ParentEntry == entry);
	}

	public void AddNewEntryAtCursor(int entryType)
	{
		AddNewEntryAtCursor((TimeLineEntryType)entryType);
	}

	public void AddNewEntryAtCursor(TimeLineEntryType entryType)
	{
		var entry = new TimeLineEntry()
		{
			Mechanic = null,
			Time = 0,
			Type = entryType,
			ParentEntry = null
		};

		if (_entries.Count != 0 && _cursorIndex - 1 >= 0)
		{
			var prevEntry = _entries[_cursorIndex - 1];
			entry.Time = prevEntry.Time + 1;
			if (entryType != TimeLineEntryType.Time && prevEntry.Type == entryType)
			{
				entry.ParentEntry = prevEntry;
			}

			if (_cursorIndex < _entries.Count)
			{
				var nextEntry = _entries[_cursorIndex];
				if (nextEntry.Type != TimeLineEntryType.Time && nextEntry.Type == entryType)
				{
					nextEntry.ParentEntry = entry;
				}
			}
		}
		_entries.Insert(Mathf.Clamp(_cursorIndex, 0, _entries.Count), entry);
		_cursorIndex++;
		Redraw();
	}

	public void Redraw()
	{
		_entryGameObjects.ForEach(Destroy);
		List<TimeLineEntry> sortedEntries = SortEntries();

		for (int i = 0; i < sortedEntries.Count; i++)
		{
			var go = Instantiate(_entryPrefabs[(int)sortedEntries[i].Type], _contentTransform.transform);
			_entryGameObjects.Add(go);
			var hudentry = go.AddComponent<TimeLineEntryHud>();
			hudentry.SetEntryData(sortedEntries[i], this);
			go.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, (_entryHeight + _entryMargin) * i);
		}
		var reguiredContentHeight = (1 + sortedEntries.Count) * (_entryHeight + _entryMargin);
		if (_scrollRect != null && _scrollRect.content.rect.height < reguiredContentHeight)
		{
			_scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, reguiredContentHeight);
		}
		_overLayTransform.transform.SetAsLastSibling();

		DrawStretchLines(sortedEntries);
	}

	private void DrawStretchLines(List<TimeLineEntry> sortedEntries)
	{
		var topLevelChildren = sortedEntries.Where(i => i.ParentEntry != null).Where(i => GetChildren(i).Count() == 0);
		foreach (var child in topLevelChildren)
		{
			var startIndex = sortedEntries.IndexOf(child);
			var indexStretch = 0;
			TimeLineEntry parent = child;
			while ((parent = parent.ParentEntry) != null)
			{
				indexStretch++;
			}

			if (indexStretch == 0)
				continue;
			var line = Instantiate(_stretchLine, _overLayTransform.transform).GetComponent<RectTransform>();
			line.anchoredPosition -= new Vector2(0, ((_entryHeight + _entryMargin) * startIndex - _entryMargin));
			var verticalLine = line.GetChild(0).GetComponent<RectTransform>();
			var size = (_entryHeight + _entryMargin) * (indexStretch);
			verticalLine.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			verticalLine.anchoredPosition = new Vector2(verticalLine.anchoredPosition.x, size / 2f);
			_entryGameObjects.Add(line.gameObject);
		}
	}

	private List<TimeLineEntry> SortEntries()
	{
		var sortedEntries = new List<TimeLineEntry>();
		var baseLevelEntries = new List<TimeLineEntry>();
		var childEntries = new List<TimeLineEntry>();
		foreach (var entry in _entries)
		{
			if (entry.ParentEntry == null)
			{
				baseLevelEntries.Add(entry);
			}
			else
			{
				childEntries.Add(entry);
			}
		}
		baseLevelEntries.Sort((entry0, entry1) => entry0.Time.CompareTo(entry1.Time));

		for (int i = 0; i < baseLevelEntries.Count; i++)
		{
			sortedEntries.Add(baseLevelEntries[i]);
			TimeLineEntry child;
			while ((child = childEntries.FirstOrDefault(j => j.ParentEntry == sortedEntries.Last())) != default)
			{
				sortedEntries.Add(child);
			}
		}

		return sortedEntries;
	}

	public void UnlockCursorLine()
	{
		_cursorLocked = false;
	}

	public void LockCursorLine()
	{
		_cursorLocked = true;
	}

	public void SpawnDropDown()
	{
		LockCursorLine();
		var dropDown = Instantiate(_dropdownPrefab, _button.transform).GetComponent<Dropdown>();
		dropDown.AddOptions(Enum.GetNames(typeof(TimeLineEntryType)).ToList());
		dropDown.onValueChanged.AddListener((i) =>
		{
			AddNewEntryAtCursor(i);
			UnlockCursorLine();
			Destroy(dropDown.gameObject);
		});
		dropDown.Show();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_addSeperator.enabled = true;
	}

	public void Rename(string prevName, string newName)
	{
		foreach (TimeLineEntry entry in _entries.Where(e => e.Mechanic == prevName))
		{
			entry.Mechanic = newName;
		}
		Redraw();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!_cursorLocked)
			_addSeperator.enabled = false;
	}
}
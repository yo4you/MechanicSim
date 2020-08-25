using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class EntryCollectionHud<EntryType> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler where EntryType : Entry, new()
{
	[SerializeField]
	private SVGImage _addSeperator;

	protected RectTransform _contentTransform;

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

	protected GameObject[] _entryPrefabs;

	private ScrollRect _scrollRect;
	private int _cursorIndex = 0;
	protected List<EntryType> _entries = new List<EntryType>();
	private List<GameObject> _entryGameObjects = new List<GameObject>();

	private float _entryHeight;
	private float _heightScale;
	private GameObject _button;
	private bool _cursorLocked = false;
	private List<GameObject> _dropDownGameObjects = new List<GameObject>();
	private bool _initialized;

	public EntryHudScriptableObject EntryHudScriptableObject { get => _entryHudScriptableObject; }

	private void Update()
	{
		if (!_initialized)
		{
			Debug.LogWarning("call SetEntries before using");
			return;
		}
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
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			UnlockCursorLine();
			_dropDownGameObjects.ForEach(Destroy);
			_dropDownGameObjects = new List<GameObject>();
		}
	}

	internal virtual void SetEntries(List<EntryType> timeLineEntries)
	{
		_initialized = true;
		_scrollRect = GetComponent<ScrollRect>();
		_addSeperator.enabled = false;

		_contentTransform = _scrollRect.content;
		_entryHeight = _entryPrefabs.First().GetComponent<RectTransform>().rect.height;
		_heightScale = FindObjectOfType<CanvasScaler>().referenceResolution.y / Screen.height;
		_button = _addSeperator.GetComponentInChildren<Button>().gameObject;

		_entries = timeLineEntries;
		Redraw();
	}

	internal void RemoveEntry(EntryType entry)
	{
		foreach (var child in GetChildren(entry))
		{
			child.ParentEntry = entry.ParentEntry;
		}
		_entries.Remove(entry);
		_cursorIndex = 0;
		Redraw();
	}

	private IEnumerable<EntryType> GetChildren(EntryType entry)
	{
		return _entries.Where(e => e.ParentEntry == entry);
	}

	public virtual void AddEntryAtCursor(ParameterType entryType)
	{
		var entry = new EntryType()
		{
			Time = 0,
			Type = entryType,
			ParentEntry = null
		};

		if (_entries.Count != 0 && _cursorIndex - 1 >= 0)
		{
			var prevEntry = _entries[_cursorIndex - 1];
			entry.Time = prevEntry.Time + 1;
			if (entry.IsParentingType() && prevEntry.Type.Equality(entryType))
			{
				entry.ParentEntry = prevEntry;
			}

			if (_cursorIndex < _entries.Count)
			{
				var nextEntry = _entries[_cursorIndex];
				if (entry.IsParentingType() && nextEntry.Type.Equality(entryType))
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
		List<EntryType> sortedEntries = SortEntries();

		for (int i = 0; i < sortedEntries.Count; i++)
		{
			GameObject go = CreateEntry(sortedEntries, i);
			_entryGameObjects.Add(go);
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

	protected abstract GameObject CreateEntry(List<EntryType> sortedEntries, int i);

	private void DrawStretchLines(List<EntryType> sortedEntries)
	{
		var topLevelChildren = sortedEntries.Where(i => i.ParentEntry != null).Where(i => GetChildren(i).Count() == 0);
		foreach (var child in topLevelChildren)
		{
			var startIndex = sortedEntries.IndexOf(child);
			var indexStretch = 0;
			Entry parent = child;
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

	private List<EntryType> SortEntries()
	{
		var sortedEntries = new List<EntryType>();
		var baseLevelEntries = new List<EntryType>();
		var childEntries = new List<EntryType>();
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
			EntryType child;
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
		DisplayDropDownMenu(dropDown);
		_dropDownGameObjects.Add(dropDown.gameObject);
		dropDown.Show();
	}

	protected abstract void DisplayDropDownMenu(Dropdown dropDown);

	public void OnPointerEnter(PointerEventData eventData)
	{
		_addSeperator.enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_addSeperator.enabled = false;
	}
}
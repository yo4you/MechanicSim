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

	private ScrollRect _scrollRect;
	private GameObject[] _entryPrefabs;
	private int _cursorIndex = 0;
	private List<TimeLineEntry> _entries = new List<TimeLineEntry>();
	private List<GameObject> _entryGameObjects = new List<GameObject>();

	private float _entryHeight;
	private float _heightScale;
	private Dropdown _dropdown;
	private bool _cursorLocked = false;

	public EntryHudScriptableObject EntryHudScriptableObject { get => _entryHudScriptableObject; }

	private void Start()
	{
		_scrollRect = GetComponent<ScrollRect>();
		_addSeperator.enabled = false;
		_entryPrefabs = new GameObject[] { _prefabTimeEntry, _prefabRandomEntry, _prefabIfElseEntry, _prefabDistributeEntry };
		_contentTransform = _scrollRect.content;
		_entryHeight = _prefabTimeEntry.GetComponent<RectTransform>().rect.height;
		_heightScale = FindObjectOfType<CanvasScaler>().referenceResolution.y / Screen.height;
		_dropdown = GetComponentInChildren<Dropdown>();
		_dropdown.AddOptions(Enum.GetNames(typeof(TimeLineEntryType)).ToList());
		_dropdown.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!_cursorLocked)
		{
			_cursorIndex = (int)(((Input.mousePosition.y - _contentTransform.position.y) * _heightScale - _entryMargin * 2f) / -((_entryHeight + _entryMargin)));
			_cursorIndex = Mathf.Clamp(_cursorIndex, 0, _entries.Count);
			var seperatorPos = _addSeperator.rectTransform.anchoredPosition;
			_addSeperator.rectTransform.anchoredPosition = new Vector2(seperatorPos.x, -(_entryHeight + _entryMargin) * _cursorIndex);
		}
	}

	internal void SetEntries(List<TimeLineEntry> timeLineEntries)
	{
		_entries = timeLineEntries;
		Redraw();
	}

	internal void RemoveEntry(TimeLineEntry entry)
	{
		_entries.Remove(entry);
		_cursorIndex = 0;
		Redraw();
	}

	public void AddNewEntryAtCursor(int entryType)
	{
		AddNewEntryAtCursor((TimeLineEntryType)entryType);
	}

	public void AddNewEntryAtCursor(TimeLineEntryType entryType)
	{
		var time = 0f;
		if (_entries.Count != 0 && _cursorIndex - 1 >= 0)
		{
			time = _entries[_cursorIndex - 1].Time + 1;
		}
		_entries.Insert(Mathf.Clamp(_cursorIndex, 0, _entries.Count), new TimeLineEntry()
		{
			Mechanic = null,
			Time = time,
			Type = entryType
		});
		_cursorIndex++;
		Redraw();
	}

	public void Redraw()
	{
		_entryGameObjects.ForEach(Destroy);
		_entries.Sort((entry0, entry1) => entry0.Time.CompareTo(entry1.Time));
		for (int i = 0; i < _entries.Count; i++)
		{
			var entry = _entries[i];
			var go = Instantiate(_entryPrefabs[(int)entry.Type], _contentTransform.transform);
			_entryGameObjects.Add(go);
			var hudentry = go.AddComponent<TimeLineEntryHud>();
			hudentry.SetEntryData(entry, this);
			go.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, (_entryHeight + _entryMargin) * i);
		}
		var reguiredContentHeight = (1 + _entries.Count) * (_entryHeight + _entryMargin);
		if (_scrollRect != null && _scrollRect.content.rect.height < reguiredContentHeight)
		{
			_scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, reguiredContentHeight);
		}
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
		_dropdown.gameObject.SetActive(true);
		_dropdown.Show();
		DropDownCallbackRegistry.RegisterCallbacks(() =>
		{
			UnlockCursorLine();
			AddNewEntryAtCursor(_dropdown.value);
		}, false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_addSeperator.enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!_cursorLocked)
			_addSeperator.enabled = false;
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubElementBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject _tabPrefab;

	[SerializeField]
	private GameObject _addTabDropDownPrefab;

	[SerializeField]
	private float _tabWidth = 300f;

	[SerializeField]
	private int _maxTabs = 9;

	[SerializeField]
	private GameObject _mechanicEditorWidnowPrefab;

	private MainTimeLineBehaviour _mainTimeLine;
	private List<string> _tabs = new List<string>();
	private List<GameObject> _tabGameObjects = new List<GameObject>();

	internal void Rename(string name, string newName)
	{
		var i = _tabs.IndexOf(name);
		_tabs[_tabs.IndexOf(name)] = newName;
		RedrawTabbar();
	}

	private ToggleGroup _toggleGroup;
	private Button _addButton;
	private int _activeTabIndex = -1;
	private GameObject _windowGameObject;

	private void Start()
	{
		_toggleGroup = GetComponentInChildren<ToggleGroup>();
		_addButton = GetComponentInChildren<Button>();
		_addButton.onClick.AddListener(ShowAddDropDown);
		_mainTimeLine = FindObjectOfType<MainTimeLineBehaviour>();
	}

	private void ShowAddDropDown()
	{
		var dropDown = Instantiate(_addTabDropDownPrefab, _addButton.transform).GetComponent<Dropdown>();
		var options = (
					  from option in _mainTimeLine.CustomMechanics
					  where !_tabs.Contains(option.Key)
					  select option.Key).ToList();

		if (options.Count() == 0)
		{
			Destroy(dropDown.gameObject);
			return;
		}
		dropDown.AddOptions(options.ToList());
		dropDown.onValueChanged.AddListener((i) =>
		{
			AddWindow(options[i]);
			Destroy(dropDown.gameObject);
		});
		dropDown.Show();
	}

	public void AddWindow(string mechanic)
	{
		if (_tabs.Contains(mechanic))
		{
			_tabs.Remove(mechanic);
		}
		if (_tabs.Count >= _maxTabs)
		{
			_tabs.RemoveAt(_tabs.Count - 1);
		}
		_tabs.Insert(0, mechanic);
		SetActiveTab(0);
		RedrawTabbar();
	}

	private void RedrawTabbar()
	{
		_tabGameObjects.ForEach(Destroy);
		_tabGameObjects = new List<GameObject>();
		var toggles = new List<Toggle>();
		for (int i = 0; i < _tabs.Count; i++)
		{
			var tab = Instantiate(_tabPrefab, _toggleGroup.transform);
			var toggle = tab.GetComponent<Toggle>();
			toggle.group = _toggleGroup;
			toggle.GetComponentInChildren<Text>().text = _tabs[i];
			var rect = tab.GetComponent<RectTransform>();
			rect.anchoredPosition += new Vector2(_tabWidth * i, 0);
			_tabGameObjects.Add(tab);
			var j = i;
			toggle.onValueChanged.AddListener((active) =>
			{
				if (active)
					SetActiveTab(j);
			});
			toggles.Add(toggle);
			toggle.GetComponentInChildren<Button>().onClick.AddListener(() => CloseWindow(_tabs[j]));
		}
		if (_activeTabIndex >= 0)
		{
			_activeTabIndex %= toggles.Count;
			toggles[_activeTabIndex].SetIsOnWithoutNotify(true);
		}
	}

	private void SetActiveTab(int i)
	{
		_activeTabIndex = i;
		RedrawContent();
	}

	private void RedrawContent()
	{
		Destroy(_windowGameObject);
		if (_activeTabIndex < 0)
			return;
		_windowGameObject = Instantiate(_mechanicEditorWidnowPrefab, transform);
		_windowGameObject.GetComponent<MechanicEditor>().Load(_tabs[_activeTabIndex]);
	}

	public void CloseWindow(string mechanic)
	{
		if (_activeTabIndex <= _tabs.IndexOf(mechanic))
		{
			_activeTabIndex--;
		}
		_tabs.Remove(mechanic);
		RedrawTabbar();
		SetActiveTab(_activeTabIndex);
	}

	public void SetActiveWindow(string mechanic)
	{
		int index = _tabs.IndexOf(mechanic);
		if (index != -1)
		{
			_activeTabIndex = index;
		}
		RedrawTabbar();
	}
}
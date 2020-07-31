using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class SubElementBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject _tabPrefab;

	private List<string> _tabs = new List<string>();
	private List<GameObject> _tabGameObjects = new List<GameObject>();
	private ToggleGroup _toggleGroup;
	private int _activeWindowIndex = -1;

	[SerializeField]
	private float _tabWidth = 300f;

	private void Start()
	{
		_toggleGroup = GetComponentInChildren<ToggleGroup>();
	}

	public void AddWindow(string mechanic)
	{
		if (_tabs.Contains(mechanic))
		{
			_tabs.Remove(mechanic);
		}
		_tabs.Insert(0, mechanic);
		_activeWindowIndex = 0;

		RedrawTabbar();
	}

	private void RedrawTabbar()
	{
		_tabGameObjects.ForEach(Destroy);
		_tabGameObjects = new List<GameObject>();
		_tabs.Reverse();
		for (int i = _tabs.Count - 1; i >= 0; i--)
		{
			var tab = Instantiate(_tabPrefab, _toggleGroup.transform);
			var toggle = tab.GetComponent<Toggle>();
			toggle.group = _toggleGroup;
			toggle.onValueChanged.AddListener((active) =>
			{
				if (!active)
					SetActiveTab(i);
			});
			toggle.SetIsOnWithoutNotify(_activeWindowIndex == i);
			toggle.GetComponentInChildren<Text>().text = _tabs[i];
			var rect = tab.GetComponent<RectTransform>();
			rect.anchoredPosition += new Vector2(_tabWidth * i, 0);
			//rect.anchoredPosition += new Vector2(_tabWidth * (_tabs.Count - 1 - i), 0);
			_tabGameObjects.Add(tab);
		}
		_tabs.Reverse();
	}

	private void SetActiveTab(int i)
	{
		_activeWindowIndex = i;
		RedrawContent();
	}

	private void RedrawContent()
	{
	}

	public void CloseWindow(string mechanic)
	{
		_tabs.Remove(mechanic);
		RedrawTabbar();
	}

	public void SetActiveWindow(string mechanic)
	{
		int index = _tabs.IndexOf(mechanic);
		if (index != -1)
		{
			_activeWindowIndex = index;
		}
		RedrawTabbar();
	}
}
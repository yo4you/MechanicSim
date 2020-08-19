using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ValueEditorBehaviour : EntryCollectionHud<ValueEntry>
{
	[SerializeField]
	private GameObject _prefabEmptyValue;

	[SerializeField]
	private GameObject _prefabChildValue;

	private MainTimeLineBehaviour _mainTimeline;

	protected override GameObject CreateEntry(List<ValueEntry> sortedEntries, int i)
	{
		ValueEntry entry = sortedEntries[i];
		var entryName = entry.Label.Clone();
		var hasParent = entry.ParentEntry != null;
		var go = Instantiate(hasParent ? _prefabChildValue : _prefabEmptyValue, _contentTransform.transform);
		go.GetComponentInChildren<Button>().onClick.AddListener(() =>
		{
			RemoveEntry(entry);
			_mainTimeline.Redraw();
		});
		var input = go.GetComponentInChildren<InputField>();
		if (input != null)
		{
			input.SetTextWithoutNotify(entry.Label);
			input.onEndEdit.AddListener((s) => Rename(entry, s));
		}
		var type = (ParameterType)((int)entry.Type % ((int)ParameterType.POS + 1));
		GameObject field = null;
		switch (type)
		{
			case ParameterType.NUM:
				field = NewNumField(entry, go);
				break;

			case ParameterType.STRING:
				field = NewStringField(entry, go);
				break;

			case ParameterType.POS:
				field = NewVec2Field(entry, go);
				break;

			default:
				break;
		}
		var rect = field.GetComponent<RectTransform>();
		rect.anchorMin = new Vector2(.5f, rect.anchorMin.y);
		rect.anchorMax = new Vector2(.9f, rect.anchorMax.y);
		return go;
	}

	public override void AddEntryAtCursor(ParameterType entryType)
	{
		base.AddEntryAtCursor(entryType);
		_mainTimeline.Redraw();
	}

	private GameObject NewVec2Field(ValueEntry entry, GameObject go)
	{
		GameObject field = Instantiate(EntryHudScriptableObject.Vec2Field, go.transform);
		var inputs = field.GetComponentsInChildren<InputField>();
		Vector2 pos = new Vector2();
		Vector2 getpos()
		{
			for (int i = 0; i < 2; i++)
			{
				if (float.TryParse(inputs[i].text, out float result))
				{
					pos[i] = result;
				}
			}
			return pos;
		}
		for (int i = 0; i < inputs.Length; i++)
		{
			InputField input = inputs[i];
			if (entry.Value is Vector2 entryValue)
			{
				input.text = entryValue[i].ToString();
			}
			input.onEndEdit.AddListener((s) =>
			{
				entry.Value = getpos();
				Redraw();
			});
		}

		return field;
	}

	private GameObject NewStringField(ValueEntry entry, GameObject go)
	{
		GameObject field = Instantiate(EntryHudScriptableObject.StringField, go.transform);
		var input = field.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify((entry.Value ?? "").ToString());
		input.onEndEdit.AddListener((s) =>
		{
			entry.Value = s;
			Redraw();
		});
		return field;
	}

	private GameObject NewNumField(ValueEntry entry, GameObject go)
	{
		GameObject field = Instantiate(EntryHudScriptableObject.NumField, go.transform);
		var input = field.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify((entry.Value ?? 0f).ToString());
		input.onEndEdit.AddListener((s) =>
		{
			if (float.TryParse(s, out float result))
			{
				entry.Value = result;
				Redraw();
			}
		});
		return field;
	}

	private void Rename(ValueEntry entry, string s)
	{
		entry.Label = s;
		_mainTimeline.Redraw();
		Redraw();
	}

	protected override void DisplayDropDownMenu(Dropdown dropDown)
	{
		var options = Enum.GetNames(typeof(ParameterType)).ToList();
		options.RemoveAt(options.Count - 1);
		dropDown.AddOptions(options);

		dropDown.onValueChanged.AddListener((i) =>
		{
			AddEntryAtCursor((ParameterType)i);
			UnlockCursorLine();
			Destroy(dropDown.gameObject);
		});
	}

	internal override void SetEntries(List<ValueEntry> timeLineEntries)
	{
		_mainTimeline = FindObjectOfType<MainTimeLineBehaviour>();
		_entryPrefabs = new GameObject[] {
			_prefabEmptyValue,
			_prefabChildValue,
		};
		base.SetEntries(timeLineEntries);
	}
}
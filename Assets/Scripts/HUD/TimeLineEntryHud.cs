using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class TimeLineEntryHud : MonoBehaviour
{
	private const string _newMechanicPrefix = "NewMech_";
	private SubElementBehaviour _subElementBehaviour;
	private TimeLineEntry _entry;
	private TimeLineBehaviour _timeLine;

	private GameObject _timeLabel;
	private GameObject _castMenu;
	private MainTimeLineBehaviour _mainTimeLine;

	public delegate void ChangeHandler();

	private delegate GameObject CreateEntryWindowHandler(ParameterData parameter);

	private Dictionary<Type, CreateEntryWindowHandler> _createEntryWindowForType = new Dictionary<Type, CreateEntryWindowHandler>();

	private Vector2 _margin = new Vector2(0.15f, 0.04f);
	private Vector2 _marginWithIcon = new Vector2(0.20f, 0.04f);

	internal void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		_subElementBehaviour = FindObjectOfType<SubElementBehaviour>();
		_entry = entry;
		_timeLine = timeLineBehaviour;
		GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => timeLineBehaviour.RemoveEntry(entry));
		if (entry.IsParentingType())
		{
			_margin = _marginWithIcon;
		}
		switch ((TimeLineEntryType)entry.Type)
		{
			case TimeLineEntryType.Random:
			case TimeLineEntryType.Time:
				ConstructEmptyEntry(entry);
				break;

			case TimeLineEntryType.IfElse:
				break;

			case TimeLineEntryType.Distribute:
				break;

			default:
				break;
		}
	}

	private void ConstructEmptyEntry(TimeLineEntry entry)
	{
		_createEntryWindowForType = new Dictionary<Type, CreateEntryWindowHandler>
		{
			{typeof(float), EntryWindowNum  },
			{typeof(Vector2), EntryWindowVec2  },
			{typeof(string), EntryWindowString  },

			{typeof(RefrenceType<float>), EntryWindowNum},
			{typeof(RefrenceType<Vector2>), EntryWindowVec2},
			{typeof(RefrenceType<string>), EntryWindowString}
		};

		_mainTimeLine = FindObjectOfType<MainTimeLineBehaviour>();

		if (entry.ParentEntry == null)
		{
			_timeLabel = Instantiate(_timeLine.EntryHudScriptableObject.TimeLabel, transform);
			var inputField = _timeLabel.GetComponent<InputField>();
			inputField.SetTextWithoutNotify(TimeStampUtil.ToTimeStamp(entry.Time));
			inputField.onEndEdit.AddListener((inputString) => SubmitTime(inputString));
		}

		_castMenu = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform);

		var castRect = _castMenu.GetComponent<RectTransform>();
		castRect.anchorMin = new Vector2(_margin.x, castRect.anchorMin.y);
		castRect.anchorMax = new Vector2(1f - _margin.y, castRect.anchorMax.y);

		var dropdown = _castMenu.GetComponent<Dropdown>();
		var options = _mainTimeLine.Mechanics.Keys.ToList();
		var defaultOption = options.IndexOf(entry.Mechanic);
		dropdown.AddOptions(new List<string>() { "New Mechanic" });
		dropdown.AddOptions(options);
		if (defaultOption != -1)
		{
			dropdown.SetValueWithoutNotify(defaultOption + 1);
			SetMechanic(entry.Mechanic);
		}
		dropdown.onValueChanged.AddListener((optionIndex) =>
		{
			_entry.Parameters = new Dictionary<string, ParameterData>();
			if (optionIndex == 0)
			{
				_entry.Parameters = null;
				_entry.Mechanic = CreateNewMechanic();
				_timeLine.Redraw();
			}
			else
			{
				SetMechanic(options[optionIndex - 1]);
				_timeLine.Redraw();
			}
		});
	}

	private string CreateNewMechanic()
	{
		var nameIndex = 0;
		while (_mainTimeLine.Mechanics.ContainsKey(_newMechanicPrefix + (++nameIndex).ToString())) ;
		var name = _newMechanicPrefix + nameIndex.ToString();
		_mainTimeLine.AddMechanic(new Mechanic()
		{
			Name = name,
			ParameterTypes = new Dictionary<string, Type>(),
			TimeLine = new MechanicTimeLine(),
			FuncCall = null
		});

		_subElementBehaviour.AddWindow(name);
		_subElementBehaviour.SetActiveWindow(name);
		return name;
	}

	private void SetMechanic(string mechanicName)
	{
		if (mechanicName == null)
		{
			return;
		}
		_entry.Mechanic = mechanicName;
		var mechanic = _mainTimeLine.Mechanics[mechanicName];
		var parameterWindowsToAllign = new List<GameObject>();
		foreach (var parameterSignature in mechanic.ParameterTypes)
		{
			if (!_entry.Parameters.TryGetValue(parameterSignature.Key, out ParameterData existingEntry))
			{
				existingEntry = new ParameterData();
				_entry.Parameters[parameterSignature.Key] = existingEntry;
			}
			var isRefrenceOnlyParameter = false;
			if (parameterSignature.Value.IsGenericType)
			{
				existingEntry.IsRefrenceValue = true;
				isRefrenceOnlyParameter = true;
			}

			var window = _createEntryWindowForType[parameterSignature.Value].Invoke(existingEntry);
			window.GetComponentInChildren<Text>().text = parameterSignature.Key;
			if (!isRefrenceOnlyParameter)
			{
				var button = Instantiate(_timeLine.EntryHudScriptableObject.SwitchToValueParamButton, window.transform).GetComponent<Button>();
				button.onClick.AddListener(() => SwapParameterType(existingEntry));
			}
			parameterWindowsToAllign.Add(window);
		}

		AlignParameterWidnows(parameterWindowsToAllign);
	}

	private void SwapParameterType(ParameterData parameter)
	{
		parameter.IsRefrenceValue = !parameter.IsRefrenceValue;
		_timeLine.Redraw();
	}

	private void AlignParameterWidnows(List<GameObject> windows)
	{
		if (windows.Count == 0)
		{
			return;
		}
		windows.Insert(0, _castMenu);
		int count = windows.Count;

		float inverseCount = (1f / count) * (1f - _margin.x - _margin.y);
		for (int i = 0; i < count; i++)
		{
			var rect = windows[i].GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(_margin.x + i * inverseCount, 0f);
			rect.anchorMax = new Vector2(_margin.x + (i + 1) * inverseCount, 1f);
		}
	}

	private void SubmitTime(string inputString)
	{
		var time = TimeStampUtil.FromTimeStamp(inputString);
		_entry.Time = time;
		_timeLine.Redraw();
	}

	public GameObject EntryWindowVec2(ParameterData parameter)
	{
		if (parameter.IsRefrenceValue)
		{
			return InstantiateRefrenceValuePicker(parameter, ParameterType.POS);
		}
		var readval = (Vector2)(parameter.Value ?? Vector2.zero);
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.Vec2Field, transform);
		var inputs = window.GetComponentsInChildren<InputField>();
		for (int i = 0; i < inputs.Length; i++)
		{
			InputField input = inputs[i];
			input.SetTextWithoutNotify(readval[i].ToString());
			input.onEndEdit.AddListener((_) =>
			{
				Vector2 pos = new Vector2(0, 0);

				for (int j = 0; j < 2; j++)
				{
					if (float.TryParse(inputs[j].text, out float res))
					{
						pos[j] = res;
					}
					else
					{
						pos = Vector2.zero;
						break;
					}
				}
				parameter.Value = pos;
			});
		}
		return window;
	}

	private GameObject EntryWindowString(ParameterData parameter)
	{
		if (parameter.IsRefrenceValue)
		{
			return InstantiateRefrenceValuePicker(parameter, ParameterType.STRING);
		}

		var readval = (string)(parameter.Value ?? "");
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.StringField, transform);
		var input = window.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify(readval);
		input.onEndEdit.AddListener((newVal) => parameter.Value = newVal);
		return window;
	}

	private GameObject InstantiateRefrenceValuePicker(ParameterData parameter, ParameterType type)
	{
		List<string> options = (
						from val in _mainTimeLine.RefrenceValues
						where val.Type == type
						select val.Label).ToList();

		var valuePicker = Instantiate(_timeLine.EntryHudScriptableObject.ValuePicker, transform);
		var dropDown = valuePicker.GetComponentInChildren<Dropdown>();
		dropDown.SetValueWithoutNotify(-1);
		options.Insert(0, "[New Value]");
		dropDown.AddOptions(options);
		if (parameter.RefrenceValue != null)
			dropDown.SetValueWithoutNotify(options.IndexOf(parameter.RefrenceValue.Label ?? ""));
		dropDown.onValueChanged.AddListener(
			(optionIndex) =>
			{
				if (optionIndex == 0)
				{
					parameter.RefrenceValue = CreateNewValue(type);
					_mainTimeLine.Redraw();
				}
				else
					parameter.RefrenceValue = _mainTimeLine.RefrenceValues.FirstOrDefault((value) => value.Label == options[optionIndex - 1]);
			});
		return valuePicker;
	}

	private ValueEntry CreateNewValue(ParameterType type)
	{
		var newVal = new ValueEntry()
		{
			Label = $"New {type} Value",
			Time = 0,
			ParentEntry = null,
			Type = type,
			Value = null
		};
		_mainTimeLine.RefrenceValues.Add(newVal);
		_subElementBehaviour.SetActiveWindow(SubElementBehaviour.SpecialWindows.ValueEditor);
		return newVal;
	}

	private GameObject EntryWindowNum(ParameterData parameter)
	{
		if (parameter.IsRefrenceValue)
		{
			return InstantiateRefrenceValuePicker(parameter, ParameterType.NUM);
		}
		var readval = (float)(parameter.Value ?? 0f);
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.NumField, transform);
		window.GetComponentInChildren<Text>().text = name;
		var input = window.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify(readval.ToString());
		input.onEndEdit.AddListener((newVal) =>
		{
			if (float.TryParse(newVal, out float result))
			{
				parameter.Value = result;
			}
		});
		return window;
	}
}
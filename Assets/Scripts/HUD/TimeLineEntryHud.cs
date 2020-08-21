// @TODO split this into several classes

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public abstract class TimeLineEntryHud : MonoBehaviour
{
	private const string _newMechanicPrefix = "NewMech_";
	private SubElementBehaviour _subElementBehaviour;
	protected TimeLineEntry _entry;
	protected TimeLineBehaviour _timeLine;
	private GameObject _timeLabel;
	protected MainTimeLineBehaviour _mainTimeLine;

	public delegate void ChangeHandler();

	private delegate GameObject CreateEntryWindowHandler(ParameterData parameter);

	private Dictionary<Type, CreateEntryWindowHandler> _createEntryWindowForType = new Dictionary<Type, CreateEntryWindowHandler>();

	private Vector2 _margin = new Vector2(0.15f, 0.04f);
	private Vector2 _marginWithIcon = new Vector2(0.20f, 0.04f);

	internal virtual void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
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

		_subElementBehaviour = FindObjectOfType<SubElementBehaviour>();
		_entry = entry;
		_timeLine = timeLineBehaviour;

		GetComponentInChildren<Button>().onClick.AddListener(() => timeLineBehaviour.RemoveEntry(entry));
		if (entry.IsParentingType())
		{
			_margin = _marginWithIcon;
		}
	}

	protected void ConstructEmptyEntry(TimeLineEntry entry)
	{
		if (entry.ParentEntry == null)
		{
			AddTimeLabel(entry);
		}

		var castMenu = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform);

		var castRect = castMenu.GetComponent<RectTransform>();
		castRect.anchorMin = new Vector2(_margin.x, castRect.anchorMin.y);
		castRect.anchorMax = new Vector2(1f - _margin.y, castRect.anchorMax.y);

		var dropdown = castMenu.GetComponent<Dropdown>();
		var options = _mainTimeLine.Mechanics.Keys.ToList();
		var defaultOption = options.IndexOf(entry.Mechanic);
		dropdown.AddOptions(new List<string>() { "New Mechanic" });
		dropdown.AddOptions(options);
		if (defaultOption != -1)
		{
			dropdown.SetValueWithoutNotify(defaultOption + 1);
			var windows = SetMechanic(entry.Mechanic);
			windows.Insert(0, castMenu);
			AlignParameterWidnows(windows);
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
				var windows = SetMechanic(options[optionIndex - 1]);
				windows.Insert(0, castMenu);
				AlignParameterWidnows(windows);
				_timeLine.Redraw();
			}
		});
	}

	protected void AddTimeLabel(TimeLineEntry entry)
	{
		_timeLabel = Instantiate(_timeLine.EntryHudScriptableObject.TimeLabel, transform);
		var inputField = _timeLabel.GetComponent<InputField>();
		inputField.SetTextWithoutNotify(TimeStampUtil.ToTimeStamp(entry.Time));
		inputField.onEndEdit.AddListener((inputString) => SubmitTime(inputString));
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

	private List<GameObject> SetMechanic(string mechanicName)
	{
		if (mechanicName == null)
		{
			return new List<GameObject>();
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

		return parameterWindowsToAllign;
	}

	private void SwapParameterType(ParameterData parameter)
	{
		parameter.IsRefrenceValue = !parameter.IsRefrenceValue;
		_timeLine.Redraw();
	}

	protected void AlignParameterWidnows(List<GameObject> windows)
	{
		if (windows.Count == 0)
		{
			return;
		}
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

	protected GameObject InstantiateRefrenceValuePicker(ParameterData parameter, ParameterType type)
	{
		List<ValueEntry> refrenceValues = new List<ValueEntry>();
		refrenceValues.AddRange(_mainTimeLine.CustomRefrenceValues);
		refrenceValues.AddRange(_mainTimeLine.GetScriptedRefrenceValues(false));
		List<string> options = (
						from val in refrenceValues
						where val.Type.Equality(type) && val.ParentEntry == null
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
					parameter.RefrenceValue = refrenceValues.FirstOrDefault((value) => value.Label == options[optionIndex]);
			});
		return valuePicker;
	}

	private ValueEntry CreateNewValue(ParameterType type)
	{
		if (type == ParameterType.ANY)
			type = ParameterType.NUM;
		var newVal = new ValueEntry()
		{
			Label = $"{type} Value #{_mainTimeLine.CustomRefrenceValues.Count}",
			Time = 0,
			ParentEntry = null,
			Type = type,
			Value = null
		};
		_mainTimeLine.CustomRefrenceValues.Add(newVal);
		_subElementBehaviour.SetActiveWindow(SubElementBehaviour.SpecialWindows.ValueEditor);
		return newVal;
	}
}
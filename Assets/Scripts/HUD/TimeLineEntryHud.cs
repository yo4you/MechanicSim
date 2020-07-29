using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class TimeLineEntryHud : MonoBehaviour
{
	private TimeLineEntry _entry;
	private TimeLineBehaviour _timeLine;

	private GameObject _timeLabel;
	private GameObject _castMenu;
	private MainTimeLineBehaviour _mainTimeLine;

	public delegate void ChangeHandler();

	private delegate Func<object> EntryWindowInOut(object input, string name);

	public event ChangeHandler OnChange;

	private Dictionary<Type, EntryWindowInOut> _entryWindowDefinitions = default;

	public void Start()
	{
	}

	internal void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		_entry = entry;
		_timeLine = timeLineBehaviour;
		GetComponentInChildren<Button>().onClick.AddListener(() => timeLineBehaviour.RemoveEntry(entry));

		switch (entry.Type)
		{
			case TimeLineEntryType.Time:
				ConstructEmptyEntry(entry);
				break;

			case TimeLineEntryType.Random:
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
		_mainTimeLine = FindObjectOfType<MainTimeLineBehaviour>();
		_entryWindowDefinitions = new Dictionary<Type, EntryWindowInOut>()
		{
			{ typeof(Vector2), EntryWindowVec2 },
			{ typeof(float), EntryWindowNum },
			{ typeof(string), EntryWindowString },
		};

		_timeLabel = Instantiate(_timeLine.EntryHudScriptableObject.TimeLabel, transform);
		var inputField = _timeLabel.GetComponent<InputField>();
		inputField.SetTextWithoutNotify(ToTimeStamp(entry.Time));
		inputField.onEndEdit.AddListener((inputString) => SubmitTime(inputString));
		_castMenu = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform);
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
			if (optionIndex == 0)
			{
				NewMechanic();
			}
			else
			{
				SetMechanic(options[optionIndex - 1]);
			}
		});
	}

	private void NewMechanic()
	{
	}

	private void SetMechanic(string mechanicName)
	{
		if (mechanicName == null)
		{
			return;
		}
		_entry.Mechanic = mechanicName;
		var parameters = _mainTimeLine.Mechanics[mechanicName].Parameters;
		foreach (var parameter in parameters)
		{
			_entry.Parameters.TryGetValue(parameter.Key, out object existingEntry);

			var func = _entryWindowDefinitions[parameter.Value].Invoke(existingEntry, parameter.Key);
			OnChange += () => _entry.Parameters[parameter.Key] = func.Invoke();
		}

		//_timeLine.Redraw();
	}

	private void SubmitTime(string inputString)
	{
		var time = FromTimeStamp(inputString);
		_entry.Time = time;
		_timeLine.Redraw();
	}

	public static float FromTimeStamp(string time)
	{
		if (float.TryParse(time, out float res))
		{
			return res;
		}
		var groups = Regex.Match(time, @"(\d*):(\d*)").Groups;
		if (groups.Count == 3)
		{
			return (int.Parse(groups[1].ToString()) * 60) + int.Parse(groups[2].ToString());
		}

		return 0f;
	}

	public static string ToTimeStamp(float time)
	{
		string ToTwoPlaces(int digit)
		{
			var outp = "";
			outp += digit / 10;
			outp += digit % 10;
			return outp;
		}
		return ToTwoPlaces((int)(time / 60f)) + ":" + ToTwoPlaces((int)(time % 60f));
	}

	public Func<object> EntryWindowVec2(object vec2, string name)
	{
		var readval = (Vector2)(vec2 ?? Vector2.zero);
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.Vec2Field, transform);
		FitParamaters(window, name);
		var inputs = window.GetComponentsInChildren<InputField>();
		for (int i = 0; i < inputs.Length; i++)
		{
			InputField input = inputs[i];
			input.SetTextWithoutNotify(readval[i].ToString());
			input.onEndEdit.AddListener((_) => ValueChanged());
		}

		return () =>
		{
			Vector2 pos = new Vector2(0, 0);

			for (int i = 0; i < 2; i++)
			{
				if (float.TryParse(inputs[0].text, out float res))
				{
					pos[i] = res;
				}
				else
				{
					pos = Vector2.zero;
					break;
				}
			}
			return pos;
		};
	}

	private Func<object> EntryWindowString(object read, string name)
	{
		var readval = (string)(read ?? "");
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.StringField, transform);
		FitParamaters(window, name);
		var input = window.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify(readval);
		input.onEndEdit.AddListener((_) => ValueChanged());

		return () =>
		{
			if (float.TryParse(input.text, out float result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		};
	}

	private Func<object> EntryWindowNum(object read, string name)
	{
		var readval = (float)(read ?? 0f);
		GameObject window = Instantiate(_timeLine.EntryHudScriptableObject.NumField, transform);
		window.GetComponentInChildren<Text>().text = name;
		FitParamaters(window, name);
		var input = window.GetComponentInChildren<InputField>();
		input.SetTextWithoutNotify(readval.ToString());
		input.onEndEdit.AddListener((_) => ValueChanged());

		return () =>
		{
			return input.text;
		};
	}

	private void ValueChanged()
	{
		OnChange?.Invoke();
	}

	private void FitParamaters(GameObject window, string name)
	{
		window.GetComponentInChildren<Text>().text = name;
		_parametereWidnows.Add(window);
	}
}
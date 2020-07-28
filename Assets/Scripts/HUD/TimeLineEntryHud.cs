using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineEntryHud : MonoBehaviour
{
	private TimeLineEntry _entry;
	private TimeLineBehaviour _timeLine;
	private EntryPrefabs _prefabs;

	private GameObject _timeLabel;
	private GameObject _castMenu;
	private MainTimeLineBehaviour _mainTimeLine;

	internal void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		_entry = entry;
		_timeLine = timeLineBehaviour;
		GetComponentInChildren<Button>().onClick.AddListener(() => timeLineBehaviour.RemoveEntry(entry));
		_prefabs = GetComponent<EntryPrefabs>();

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
		_timeLabel = Instantiate(_prefabs.TimeLabel, transform);
		var inputField = _timeLabel.GetComponent<InputField>();
		inputField.SetTextWithoutNotify(ToTimeStamp(entry.Time));
		inputField.onEndEdit.AddListener((inputString) => SubmitTime(inputString));

		_castMenu = Instantiate(_prefabs.CastLabel, transform);
		var dropdown = _castMenu.GetComponent<Dropdown>();
		dropdown.AddOptions(new List<string>() { "New Mechanic" });
		_mainTimeLine = FindObjectOfType<MainTimeLineBehaviour>();
		var options = _mainTimeLine.Mechanics.Keys.ToList();
		dropdown.AddOptions(options);
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
		throw new NotImplementedException();
	}

	private void SetMechanic(string mechanicName)
	{
		var parameters = _mainTimeLine.Mechanics[mechanicName].Parameters;
		foreach (var parameter in parameters)
		{
		}
		_timeLine.Redraw();
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

	public Func<Vector2> EntryWindowVec2()
	{
		// TODO prefab
		GameObject window = Instantiate(gameObject, transform);

		return () =>
		{
			Vector2 pos = new Vector2(0, 0);
			var inputs = window.GetComponentsInChildren<InputField>();
			for (int i = 0; i < 2; i++)
			{
				if (int.TryParse(inputs[0].text, out int res))
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
}
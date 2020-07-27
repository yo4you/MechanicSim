using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineEntryHud : MonoBehaviour
{
	private TimeLineEntry _entry;
	private TimeLineBehaviour _timeLine;
	private EntryPrefabs _prefabs;

	private GameObject _timeLabel;

	internal void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		_entry = entry;
		_timeLine = timeLineBehaviour;
		GetComponentInChildren<Button>().onClick.AddListener(() => timeLineBehaviour.RemoveEntry(entry));
		_prefabs = GetComponent<EntryPrefabs>();

		switch (entry.Type)
		{
			case TimeLineEntryType.Time:
				_timeLabel = Instantiate(_prefabs.TimeLabel, transform);
				var inputField = _timeLabel.GetComponent<InputField>();
				inputField.SetTextWithoutNotify(ToTimeStamp(entry.Time));
				inputField.onEndEdit.AddListener((inputString) => SubmitTime(inputString));
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
}
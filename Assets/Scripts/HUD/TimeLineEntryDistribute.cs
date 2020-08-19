using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineEntryDistribute : TimeLineEntryHud
{
	private string _distParamString = "distType";

	internal override void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		base.SetEntryData(entry, timeLineBehaviour);

		if (entry.ParentEntry == null)
		{
			ConstructEmptyEntry(entry);
			return;
		}

		if (entry.Parameters.TryGetValue(_distParamString, out ParameterData parameterData))
		{
			AlignParameterWidnows(new List<GameObject>() {
				InstantiateRefrenceValuePicker(parameterData, ParameterType.ANY)
		});
		}
		else
		{
			ConstructEmptyEntry(entry);
		}

		var button = Instantiate(_timeLine.EntryHudScriptableObject.SwitchToValueParamButton, transform).GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			if (entry.Parameters.ContainsKey(_distParamString))
			{
				entry.Parameters.Remove(_distParamString);
			}
			else
			{
				entry.Parameters.Add(_distParamString, new ParameterData());
			}
			_timeLine.Redraw();
		});
	}
}
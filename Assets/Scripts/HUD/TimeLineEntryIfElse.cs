using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineEntryIfElse : TimeLineEntryHud
{
	private readonly string _lefthandString = "LHand";
	private readonly string _righthandString = "RHand";
	private readonly string _comparatorString = "Comp";
	private Dropdown _leftHand;
	private Dropdown _comparator;
	private Dropdown _rightHand;

	internal override void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		base.SetEntryData(entry, timeLineBehaviour);
		var iconSelector = GetComponentInChildren<ConditionalIconSelector>();
		if (entry.ParentEntry == null)
		{
			iconSelector.SetState(ConditionalIconSelector.State.CHECK);
			ConstructComparisonEntry(entry);
		}
		else if (entry.ParentEntry.ParentEntry == null)
		{
			iconSelector.SetState(ConditionalIconSelector.State.TRUE);
			ConstructEmptyEntry(entry);
		}
		else
		{
			iconSelector.SetState(ConditionalIconSelector.State.FALSE);
			ConstructEmptyEntry(entry);
		}
	}

	private void ConstructComparisonEntry(TimeLineEntry entry)
	{
		_entry.Parameters.TryGetValue(_lefthandString, out ParameterData lh);
		_entry.Parameters.TryGetValue(_righthandString, out ParameterData rh);
		_entry.Parameters.TryGetValue(_comparatorString, out ParameterData c);

		AddTimeLabel(entry);
		_leftHand = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform).GetComponent<Dropdown>();

		var leftHandOptions =
			from val in _mainTimeLine.CustomRefrenceValues
			where val.Type <= ParameterType.POS
			orderby val.Type
			select val;
		List<string> lhOptionStrings = AsStringList(leftHandOptions);
		_leftHand.AddOptions(lhOptionStrings);
		if (_entry.Parameters.TryGetValue(_lefthandString, out ParameterData parameterDataLh))
		{
			_leftHand.SetValueWithoutNotify(lhOptionStrings.IndexOf(parameterDataLh.RefrenceValue.Label));
			DrawRightHand(parameterDataLh.RefrenceValue.Type);
		}
		else
		{
			DrawRightHand((ParameterType)256);
		}

		_leftHand.onValueChanged.AddListener((l) =>
		{
			var leftHandOption = leftHandOptions.ElementAt(l);
			StoreParameter(leftHandOption, _lefthandString, new ParameterData()
			{
				IsRefrenceValue = true,
				RefrenceValue = leftHandOption,
				Value = null
			});

			DrawRightHand(leftHandOption.Type);
		});
	}

	private void StoreParameter(ValueEntry refrenceVal, string keyString, ParameterData newParamData)
	{
		if (!_entry.Parameters.ContainsKey(keyString))
		{
			_entry.Parameters[keyString] = newParamData;
		}
		else
		{
			_entry.Parameters[keyString].RefrenceValue = refrenceVal;
		}
	}

	private void DrawRightHand(ParameterType type)
	{
		Destroy(_rightHand);
		Destroy(_comparator);
		_comparator = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform).GetComponent<Dropdown>();
		_rightHand = Instantiate(_timeLine.EntryHudScriptableObject.CastLabel, transform).GetComponent<Dropdown>();

		var rhOptions = from val in _mainTimeLine.CustomRefrenceValues where val.Type.Equality(type) select val;
		_rightHand.ClearOptions();
		List<string> rhOptionsAsString = AsStringList(rhOptions);
		_rightHand.AddOptions(rhOptionsAsString);
		if (_entry.Parameters.TryGetValue(_righthandString, out ParameterData parameterDataRh))
		{
			_rightHand.SetValueWithoutNotify(rhOptionsAsString.IndexOf(parameterDataRh.RefrenceValue.Label));
		}
		else if (rhOptions.Count() != 0)
		{
			_rightHand.SetValueWithoutNotify(0);

			_entry.Parameters[_righthandString] = new ParameterData()
			{
				IsRefrenceValue = true,
				RefrenceValue = rhOptions.First()
			};
		}
		_rightHand.onValueChanged.AddListener(r =>
					{
						if (rhOptions.Count() == 0)
							return;
						var rhOption = rhOptions.ElementAt(r);
						StoreParameter(rhOption, _righthandString,
									new ParameterData()
									{
										IsRefrenceValue = true,
										RefrenceValue = rhOption,
										Value = null
									});
					});

		var comparatorOptions = new List<string>() { "==" };
		if (type == ParameterType.NUM)
		{
			comparatorOptions.Add("<");
			comparatorOptions.Add(">");
		}
		_comparator.ClearOptions();
		_comparator.AddOptions(comparatorOptions);

		if (_entry.Parameters.TryGetValue(_comparatorString, out ParameterData parameterDataComp))
		{
			_comparator.SetValueWithoutNotify(comparatorOptions.IndexOf(parameterDataComp.Value.ToString()));
		}
		else
		{
			_entry.Parameters[_comparatorString] = new ParameterData { Value = comparatorOptions[0] };
			_comparator.SetValueWithoutNotify(0);
		}

		_comparator.onValueChanged.AddListener(c =>
		{
			_entry.Parameters[_comparatorString] = new ParameterData()
			{
				Value = comparatorOptions[c]
			};
		});

		AlignParameterWidnows(new List<GameObject>{
			_leftHand.gameObject,
			_comparator.gameObject,
			_rightHand.gameObject
		});
	}

	private static List<string> AsStringList(IEnumerable<ValueEntry> options) => options.Select(i => i.Label).ToList();
}